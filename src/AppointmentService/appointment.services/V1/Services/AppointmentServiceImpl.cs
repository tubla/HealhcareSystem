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
        int userId
    )
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, "WriteAppointment"))
            return Response<AppointmentDto>.Fail("Permission denied");

        if (
            !await _unitOfWork.Appointments.IsDoctorAvailableAsync(
                dto.DoctorID,
                dto.AppointmentDateTime
            )
        )
            return Response<AppointmentDto>.Fail("Doctor not available at this time");

        var appointment = _mapper.Map<Appointment>(dto);
        appointment.Status = "Scheduled";
        await _unitOfWork.Appointments.AddAsync(appointment);
        await _unitOfWork.CompleteAsync();

        // Publish event to Event Hub via Dapr
        var @event = new AppointmentScheduledEvent
        {
            AppointmentId = appointment.AppointmentID,
            PatientId = appointment.PatientID,
            DoctorId = appointment.DoctorID,
            AppointmentDateTime = appointment.AppointmentDateTime,
        };
        try
        {
            var eventData = new EventData(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event))
            );
            await _eventHubClient.SendAsync(new[] { eventData });
            _logger.LogInformation("Published event to Event Hub: AppointmentId {AppointmentId}", @event.AppointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event to Event Hub for AppointmentId {AppointmentId}", @event.AppointmentId);
            // Optionally handle error (e.g., retry or log only)
        }

        // Invalidate cache
        _cache.Remove($"doctor-appointments:{dto.DoctorID}");

        return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    public async Task<Response<AppointmentDto>> GetAppointmentAsync(int id, int userId)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, "ReadAppointment"))
            return Response<AppointmentDto>.Fail("Permission denied");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Response<AppointmentDto>.Fail($"Appointment {id} not found");

        return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
    }

    public async Task<Response<IEnumerable<AppointmentDto>>> GetDoctorAppointmentsAsync(
        int doctorId,
        int userId
    )
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, "ReadAppointment"))
            return Response<IEnumerable<AppointmentDto>>.Fail("Permission denied");

        var cacheKey = $"doctor-appointments:{doctorId}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AppointmentDto> cachedAppointments))
            return Response<IEnumerable<AppointmentDto>>.Ok(cachedAppointments);

        var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctorId);
        var dtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);

        _cache.Set(cacheKey, dtos, TimeSpan.FromMinutes(5));

        return Response<IEnumerable<AppointmentDto>>.Ok(dtos);
    }

    public async Task<Response<AppointmentDto>> CancelAppointmentAsync(int id, int userId)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, "CancelAppointment"))
            return Response<AppointmentDto>.Fail("Permission denied");

        var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
        if (appointment == null)
            return Response<AppointmentDto>.Fail($"Appointment {id} not found");

        if (appointment.Status == "Cancelled")
            return Response<AppointmentDto>.Fail("Appointment already cancelled");

        appointment.Status = "Cancelled";
        await _unitOfWork.CompleteAsync();

        // Invalidate cache
        _cache.Remove($"doctor-appointments:{appointment.DoctorID}");

        return Response<AppointmentDto>.Ok(_mapper.Map<AppointmentDto>(appointment));
    }
}
