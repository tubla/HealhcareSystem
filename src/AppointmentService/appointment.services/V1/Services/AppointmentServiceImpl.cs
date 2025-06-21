using appointment.models.V1.Db;
using appointment.models.V1.Dtos;
using appointment.repositories.V1.Contracts;
using appointment.services.V1.Contracts;
using appointment.services.V1.Exceptions;
using AutoMapper;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using shared.V1.Events;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;
using System.Text;
using System.Text.Json;

namespace appointment.services.V1.Services;

public class AppointmentServiceImpl(
    IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IMemoryCache _cache,
    IAuthServiceProxy _authServiceProxy,
    IPatientServiceProxy _patientServiceProxy,
    IEventHubClientProvider _eventHubClientProvider,
    ILogger<AppointmentServiceImpl> _logger
    ) : IAppointmentService
{

    private readonly EventHubProducerClient _eventHubAppointmentScheduledClient = _eventHubClientProvider.GetClient(EventNames.AppointmentScheduled);
    private readonly EventHubProducerClient _eventHubAppointmentCancelledClient = _eventHubClientProvider.GetClient(EventNames.AppointmentCancelled);
    private const int _appointmentSlotDurationMinutes = 30;

    private async Task ValidatePatientAsync(int patientId, CancellationToken cancellationToken)
    {
        if (patientId <= 0)
            throw new ArgumentException("Invalid patient ID");
        if (!await _patientServiceProxy.CheckPatientExistsAsync(patientId, cancellationToken))
            throw new RecordNotFoundException($"Patient {patientId} not found");
    }

    private async Task ValidateDoctorAndAvailabilityAsync(int doctorId, DateTime appointmentDateTime, int? excludeAppointmentId, CancellationToken cancellationToken)
    {
        if (doctorId <= 0)
            throw new ArgumentException("Invalid doctor ID");
        if (appointmentDateTime < DateTime.UtcNow)
            throw new ArgumentException("Cannot schedule appointments in the past");
        if (!await _unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(doctorId, appointmentDateTime, excludeAppointmentId, cancellationToken))
            throw new AppointmentConflictException("Doctor not available at this time");
    }

    public async Task<Response<AppointmentResponseDto>> CreateAppointmentAsync(
        CreateAppointmentRequestDto dto,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Invalid appointment data");

        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        await ValidatePatientAsync(dto.PatientId, cancellationToken);
        await ValidateDoctorAndAvailabilityAsync(dto.DoctorId, dto.AppointmentDateTime, null, cancellationToken);

        var appointment = _mapper.Map<Appointment>(dto);
        appointment.Status = "Scheduled";

        try
        {
            await _unitOfWork.AppointmentRepository.AddAsync(appointment, cancellationToken);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var @event = new AppointmentScheduledEvent
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime
            };

            await _eventHubAppointmentScheduledClient.SendAsync(
                new[] { new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event))) },
                cancellationToken);
            _logger.LogInformation("Published AppointmentScheduledEvent for AppointmentId {AppointmentId}", appointment.AppointmentId);

            _cache.Remove($"doctor-appointments:{dto.DoctorId}");

            return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create appointment for DoctorId {DoctorId}, PatientId {PatientId}", dto.DoctorId, dto.PatientId);
            throw;
        }
    }

    public async Task<Response<AppointmentResponseDto>> GetAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid appointment ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id, cancellationToken);
        return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
    }

    public async Task<Response<AppointmentResponseDto>> GetDoctorAppointmentAsync(
        int doctorId,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (doctorId <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
        return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
    }

    public async Task<Response<IEnumerable<AppointmentResponseDto>>> GetDoctorAppointmentsByDateAsync(
        int doctorId,
        DateTime date,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (doctorId <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var cacheKey = $"doctor-appointments:{doctorId}:{date:yyyyMMdd}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<AppointmentResponseDto>? cachedAppointments))
        {
            _logger.LogInformation("Cache hit for doctor appointments: {CacheKey}", cacheKey);
            return Response<IEnumerable<AppointmentResponseDto>>.Ok(cachedAppointments!);
        }

        try
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetByDoctorIdAndDateAsync(doctorId, date, cancellationToken);
            var dtos = _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                SlidingExpiration = TimeSpan.FromMinutes(2)
            };
            _cache.Set(cacheKey, dtos, cacheOptions);
            _logger.LogInformation("Cached doctor appointments: {CacheKey}", cacheKey);

            return Response<IEnumerable<AppointmentResponseDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve appointments for DoctorId {DoctorId} on {Date}", doctorId, date);
            throw;
        }
    }

    public async Task<Response<AppointmentResponseDto>> CancelAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid appointment ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new RecordNotFoundException($"Appointment {id} not found");

        if (appointment.Status == "Cancelled")
            throw new InvalidOperationException("Appointment already cancelled");

        try
        {
            appointment.Status = "Cancelled";
            await _unitOfWork.CompleteAsync(cancellationToken);

            var @event = new AppointmentCancelledEvent
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime
            };

            await _eventHubAppointmentCancelledClient.SendAsync(
                new[] { new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event))) },
                cancellationToken);
            _logger.LogInformation("Published AppointmentCancelledEvent for AppointmentId {AppointmentId}", appointment.AppointmentId);

            _cache.Remove($"doctor-appointments:{appointment.DoctorId}:{appointment.AppointmentDateTime:yyyyMMdd}");

            return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel appointment {AppointmentId}", id);
            throw;
        }
    }

    public async Task<Response<AppointmentResponseDto>> UpdateAppointmentAsync(
        int id,
        int userId,
        UpdateAppointmentRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Invalid appointment data");

        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid appointment ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new RecordNotFoundException($"Appointment {id} not found");

        int originalDoctorId = appointment.DoctorId;

        if (dto.IsPatientIdSet)
            await ValidatePatientAsync(dto.PatientId!.Value, cancellationToken);

        if (dto.IsDoctorIdSet && dto.IsAppointmentDateTimeSet)
            await ValidateDoctorAndAvailabilityAsync(dto.DoctorId!.Value, dto.AppointmentDateTime!.Value, id, cancellationToken);
        else if (dto.IsDoctorIdSet || dto.IsAppointmentDateTimeSet)
            throw new ArgumentException("Both doctor ID and appointment date/time must be updated together");

        try
        {
            if (dto.IsPatientIdSet) appointment.PatientId = dto.PatientId!.Value;
            if (dto.IsDoctorIdSet) appointment.DoctorId = dto.DoctorId!.Value;
            if (dto.IsAppointmentDateTimeSet) appointment.AppointmentDateTime = dto.AppointmentDateTime!.Value;
            if (dto.IsNotesSet) appointment.Notes = dto.Notes;

            await _unitOfWork.CompleteAsync(cancellationToken);

            _cache.Remove($"doctor-appointments:{originalDoctorId}:{appointment.AppointmentDateTime:yyyyMMdd}");
            if (dto.IsDoctorIdSet && dto.DoctorId != originalDoctorId)
                _cache.Remove($"doctor-appointments:{dto.DoctorId}:{dto.AppointmentDateTime:yyyyMMdd}");

            _logger.LogInformation("Updated appointment {AppointmentId}", id);

            return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update appointment {AppointmentId}", id);
            throw;
        }
    }

    public async Task<Response<bool>> DeleteAppointmentAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid appointment ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment == null)
            throw new RecordNotFoundException($"Appointment {id} not found");

        try
        {
            _unitOfWork.AppointmentRepository.Remove(appointment);
            await _unitOfWork.CompleteAsync(cancellationToken);

            var @event = new AppointmentCancelledEvent
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                DoctorId = appointment.DoctorId,
                AppointmentDateTime = appointment.AppointmentDateTime
            };

            await _eventHubAppointmentCancelledClient.SendAsync(
                new[] { new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event))) },
                cancellationToken);
            _logger.LogInformation("Published AppointmentCancelledEvent for AppointmentId {AppointmentId}", appointment.AppointmentId);

            _cache.Remove($"doctor-appointments:{appointment.DoctorId}:{appointment.AppointmentDateTime:yyyyMMdd}");

            return Response<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete appointment {AppointmentId}", id);
            throw;
        }
    }
}
