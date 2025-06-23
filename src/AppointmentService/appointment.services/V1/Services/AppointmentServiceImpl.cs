using appointment.models.V1.Db;
using appointment.models.V1.Dtos;
using appointment.repositories.V1.Contracts;
using appointment.services.V1.Contracts;
using appointment.services.V1.Exceptions;
using AutoMapper;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using shared.V1.Events;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Hubs;
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
    IHubContext<AppointmentHub> _hubContext,
    ILogger<AppointmentServiceImpl> _logger
    ) : IAppointmentService
{

    private readonly EventHubProducerClient _eventHubAppointmentScheduledClient = _eventHubClientProvider.GetClient(EventNames.AppointmentScheduled);
    private readonly EventHubProducerClient _eventHubAppointmentCancelledClient = _eventHubClientProvider.GetClient(EventNames.AppointmentCancelled);
    private const int _maxAppointmentsPerDay = 10;

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

        var slot = await _unitOfWork.AppointmentRepository.GetDoctorSlotAvailabilityAsync(doctorId, appointmentDateTime.Date, cancellationToken);
        if (slot != null && slot.SlotStatus == "Full")
            throw new AppointmentConflictException($"Doctor has reached maximum appointments for {appointmentDateTime.Date:yyyy-MM-dd}");

        if (!await _unitOfWork.AppointmentRepository.IsDoctorAvailableAsync(doctorId, appointmentDateTime, excludeAppointmentId, cancellationToken))
            throw new AppointmentConflictException("Doctor not available at this time");
    }

    private async Task UpdateDoctorSlotAvailabilityAsync(int doctorId, DateTime slotDate, bool isCancellation, byte[]? originalRowVersion, CancellationToken cancellationToken)
    {
        var slot = await _unitOfWork.AppointmentRepository.GetDoctorSlotAvailabilityAsync(doctorId, slotDate, cancellationToken);
        var newSlot = slot ?? new DoctorSlotAvailability
        {
            DoctorId = doctorId,
            SlotDate = slotDate.Date,
            AppointmentCount = 0,
            SlotStatus = "Available"
        };

        newSlot.AppointmentCount = isCancellation ? newSlot.AppointmentCount - 1 : newSlot.AppointmentCount + 1;
        if (newSlot.AppointmentCount < 0)
            newSlot.AppointmentCount = 0;

        var previousStatus = newSlot.SlotStatus;
        newSlot.SlotStatus = newSlot.AppointmentCount >= _maxAppointmentsPerDay ? "Full" : "Available";
        newSlot.RowVersion = originalRowVersion ?? newSlot.RowVersion;

        await _unitOfWork.AppointmentRepository.AddOrUpdateDoctorSlotAvailabilityAsync(newSlot, cancellationToken);

        // Send notification to UI to update slot status

        if (newSlot.SlotStatus != previousStatus)
        {
            await _hubContext.Clients.All.SendAsync("NotifySlotStatus", newSlot.DoctorId, newSlot.SlotDate, newSlot.SlotStatus, cancellationToken);
            _logger.LogInformation("Notified UI of slot status change for DoctorId {DoctorId} on {SlotDate} to {SlotStatus}", newSlot.DoctorId, newSlot.SlotDate, newSlot.SlotStatus);
        }
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

        await _unitOfWork.AppointmentRepository.AddAsync(appointment, cancellationToken);
        await UpdateDoctorSlotAvailabilityAsync(dto.DoctorId, dto.AppointmentDateTime.Date, false, null, cancellationToken);
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

        _cache.Remove($"doctor-appointments:{dto.DoctorId}:{dto.AppointmentDateTime:yyyyMMdd}");

        return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
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
        if (appointment == null)
            throw new RecordNotFoundException($"Appointment {id} not found");

        return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
    }

    public async Task<bool> CheckAppointmentExistsAsync(
        int id,
        int userId,
        CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid appointment ID");

        var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id, cancellationToken);
        return appointment != null;
    }

    public async Task<bool> CheckDoctorHasAppointmentsAsync(int doctorId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new AppointmentAccessPermissionException("Permission denied");

        if (doctorId <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var appointments = await _unitOfWork.AppointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
        return appointments != null && appointments.Any();
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

        appointment.Status = "Cancelled";
        var slot = await _unitOfWork.AppointmentRepository.GetDoctorSlotAvailabilityAsync(appointment.DoctorId, appointment.AppointmentDateTime.Date, cancellationToken);
        await UpdateDoctorSlotAvailabilityAsync(appointment.DoctorId, appointment.AppointmentDateTime.Date, true, slot?.RowVersion, cancellationToken);
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
        DateTime originalSlotDate = appointment.AppointmentDateTime.Date;

        if (dto.IsPatientIdSet)
            await ValidatePatientAsync(dto.PatientId!.Value, cancellationToken);

        if (dto.IsDoctorIdSet && dto.IsAppointmentDateTimeSet)
            await ValidateDoctorAndAvailabilityAsync(dto.DoctorId!.Value, dto.AppointmentDateTime!.Value, id, cancellationToken);
        else if (dto.IsDoctorIdSet || dto.IsAppointmentDateTimeSet)
            throw new ArgumentException("Both doctor ID and appointment date/time must be updated together");

        if (dto.IsPatientIdSet) appointment.PatientId = dto.PatientId!.Value;
        if (dto.IsDoctorIdSet) appointment.DoctorId = dto.DoctorId!.Value;
        if (dto.IsAppointmentDateTimeSet) appointment.AppointmentDateTime = dto.AppointmentDateTime!.Value;
        if (dto.IsNotesSet) appointment.Notes = dto.Notes;

        if (dto.IsDoctorIdSet || dto.IsAppointmentDateTimeSet)
        {
            var newSlotDate = dto.AppointmentDateTime!.Value.Date;
            var newDoctorId = dto.DoctorId!.Value;
            if (newDoctorId != originalDoctorId || newSlotDate != originalSlotDate)
            {
                var originalSlot = await _unitOfWork.AppointmentRepository.GetDoctorSlotAvailabilityAsync(originalDoctorId, originalSlotDate, cancellationToken);
                await UpdateDoctorSlotAvailabilityAsync(originalDoctorId, originalSlotDate, true, originalSlot?.RowVersion, cancellationToken);
                await UpdateDoctorSlotAvailabilityAsync(newDoctorId, newSlotDate, false, null, cancellationToken);
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        _cache.Remove($"doctor-appointments:{originalDoctorId}:{originalSlotDate:yyyyMMdd}");
        if (dto.IsDoctorIdSet && dto.DoctorId != originalDoctorId)
            _cache.Remove($"doctor-appointments:{dto.DoctorId}:{dto.AppointmentDateTime:yyyyMMdd}");

        _logger.LogInformation("Updated appointment {AppointmentId}", id);

        return Response<AppointmentResponseDto>.Ok(_mapper.Map<AppointmentResponseDto>(appointment));
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

        var slot = await _unitOfWork.AppointmentRepository.GetDoctorSlotAvailabilityAsync(appointment.DoctorId, appointment.AppointmentDateTime.Date, cancellationToken);
        _unitOfWork.AppointmentRepository.Remove(appointment);
        await UpdateDoctorSlotAvailabilityAsync(appointment.DoctorId, appointment.AppointmentDateTime.Date, true, slot?.RowVersion, cancellationToken);
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
}
