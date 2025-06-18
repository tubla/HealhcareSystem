using appointment.models.V1.Db;
using appointment.models.V1.Dtos;
using appointment.repositories.V1.Contracts;
using appointment.services.V1.Contracts;
using AutoMapper;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using shared.Events;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;
using shared.Models;
using System.Text;
using System.Text.Json;

namespace appointment.services.V1.Services;

public class AppointmentServiceImpl(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    EventHubProducerClient _eventHubClient,
    IMemoryCache _cache,
    IAuthServiceProxy _authServiceProxy,
    ILogger<AppointmentServiceImpl> _logger
    ) : IAppointmentService
{
    public async Task<Response<AppointmentDto>> CreateAppointmentAsync(
            AppointmentDto dto,
            int userId,
            CancellationToken cancellationToken = default)
    {
        if (dto == null)
            return Response<AppointmentDto>.Fail("Invalid appointment data");

        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            return Response<AppointmentDto>.Fail("Permission denied", 403);

        if (dto.AppointmentDateTime < DateTime.UtcNow)
            return Response<AppointmentDto>.Fail("Cannot schedule appointments in the past");

        if (!await _unitOfWork.Appointments.IsDoctorAvailableAsync(dto.DoctorId, dto.AppointmentDateTime, cancellationToken))
            return Response<AppointmentDto>.Fail("Doctor not available at this time", 409);

        var appointment = _mapper.Map<Appointment>(dto);
        appointment.Status = "Scheduled";

        try
        {
            await _unitOfWork.Appointments.AddAsync(appointment, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Publish event to Event Hub
            var @event = new AppointmentScheduledEvent
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime
            };

            var eventData = new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)));
            await _eventHubClient.SendAsync(new[] { eventData }, cancellationToken);
            _logger.LogInformation("Published AppointmentScheduledEvent for AppointmentId {AppointmentId}", appointment.AppointmentId);

            // Invalidate cache
            _cache.Remove($"doctor-appointments:{dto.DoctorId}");

            return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create appointment for DoctorId {DoctorId}, PatientId {PatientId}", dto.DoctorId, dto.PatientId);
            return Response<AppointmentDto>.Fail("Failed to create appointment");
        }
    }

    public async Task<Response<AppointmentDto>> GetAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            return Response<AppointmentDto>.Fail("Permission denied", 403);

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Response<AppointmentDto>.Fail($"Appointment {id} not found", 404);

        return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    public async Task<Response<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(
        int doctorId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            return Response<IEnumerable<AppointmentDto>>.Fail("Permission denied", 403);

        var cacheKey = $"doctor-appointments:{doctorId}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AppointmentDto>? cachedAppointments))
        {
            _logger.LogInformation("Cache hit for doctor appointments: {CacheKey}", cacheKey);
            return Response<IEnumerable<AppointmentDto>>.Ok(cachedAppointments!);
        }

        try
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _cache.Set(cacheKey, dtos, cacheOptions);
            _logger.LogInformation("Cached doctor appointments: {CacheKey}", cacheKey);

            return Response<IEnumerable<AppointmentDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve appointments for DoctorId {DoctorId}", doctorId);
            return Response<IEnumerable<AppointmentDto>>.Fail("Failed to retrieve appointments");
        }
    }

    public async Task<Response<AppointmentDto>> CancelAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            return Response<AppointmentDto>.Fail("Permission denied", 403);

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            return Response<AppointmentDto>.Fail($"Appointment {id} not found", 404);

        if (appointment.Status == "Cancelled")
            return Response<AppointmentDto>.Fail("Appointment already cancelled", 400);

        try
        {
            appointment.Status = "Cancelled";
            await _unitOfWork.CompleteAsync(cancellationToken);

            // Publish cancellation event
            var @event = new AppointmentCancelledEvent
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime
            };

            var eventData = new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)));
            await _eventHubClient.SendAsync(new[] { eventData }, cancellationToken);
            _logger.LogInformation("Published AppointmentCancelledEvent for AppointmentId {AppointmentId}", appointment.AppointmentId);

            // Invalidate cache
            _cache.Remove($"doctor-appointments:{appointment.DoctorId}");

            return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel appointment {AppointmentId}", id);
            return Response<AppointmentDto>.Fail("Failed to cancel appointment");
        }
    }
}
