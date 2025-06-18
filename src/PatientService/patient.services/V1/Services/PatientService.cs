using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using patient.models.V1.Db;
using patient.models.V1.Dto;
using patient.repositories.V1.Contracts;
using patient.services.V1.Contracts;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;
using shared.Models;

namespace patient.services.V1.Services;

public class PatientService(
            IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IAppointmentServiceProxy _appointmentServiceProxy,
            IAuthServiceProxy _authServiceProxy,
            IMemoryCache _memoryCache) : IPatientService
{

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Response<PatientDto>> CreateAsync(CreatePatientDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePatient, cancellationToken))
            return Response<PatientDto>.Fail("Permission denied", 403);

        var existingPatient = await _unitOfWork.PatientRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingPatient != null)
            return Response<PatientDto>.Fail("Patient with this email already exists", 409);

        var patient = _mapper.Map<Patient>(dto);
        await _unitOfWork.PatientRepository.AddAsync(patient, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this patient if it exists
        _memoryCache.Remove($"Patient_{patient.PatientId}");

        return Response<PatientDto>.Ok(_mapper.Map<PatientDto>(patient));
    }

    public async Task<Response<PatientDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPatient, cancellationToken))
            return Response<PatientDto>.Fail("Permission denied", 403);

        var cacheKey = $"Patient_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out PatientDto? cachedPatient))
            return Response<PatientDto>.Ok(cachedPatient!);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        if (patient == null)
            return Response<PatientDto>.Fail($"Patient {id} not found", 404);

        var patientDto = _mapper.Map<PatientDto>(patient);
        _memoryCache.Set(cacheKey, patientDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        });

        return Response<PatientDto>.Ok(patientDto);
    }

    public async Task<Response<PatientDto>> UpdateAsync(int id, UpdatePatientDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePatient, cancellationToken))
            return Response<PatientDto>.Fail("Permission denied", 403);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        if (patient == null)
            return Response<PatientDto>.Fail($"Patient {id} not found", 404);

        if (dto.IsFirstNameSet && !string.IsNullOrWhiteSpace(dto.FirstName))
            patient.FirstName = dto.FirstName;
        if (dto.IsLastNameSet && !string.IsNullOrWhiteSpace(dto.LastName))
            patient.LastName = dto.LastName;
        if (dto.IsDateOfBirthSet && dto.DateOfBirth.HasValue)
            patient.DateOfBirth = dto.DateOfBirth.Value;
        if (dto.IsGenderSet && !string.IsNullOrWhiteSpace(dto.Gender))
            patient.Gender = dto.Gender;
        if (dto.IsContactNumberSet && !string.IsNullOrWhiteSpace(dto.ContactNumber))
            patient.ContactNumber = dto.ContactNumber;
        if (dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email))
            patient.Email = dto.Email;
        if (dto.IsAddressSet && !string.IsNullOrWhiteSpace(dto.Address))
            patient.Address = dto.Address;

        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this patient
        _memoryCache.Remove($"Patient_{id}");

        return Response<PatientDto>.Ok(_mapper.Map<PatientDto>(patient));
    }

    public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePatient, cancellationToken))
            return Response<bool>.Fail("Permission denied", 403);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        if (patient == null)
            return Response<bool>.Fail($"Patient {id} not found", 404);

        await _unitOfWork.PatientRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this patient
        _memoryCache.Remove($"Patient_{id}");

        return Response<bool>.Ok(true);
    }

    public async Task<Response<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int patientId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            return Response<IEnumerable<AppointmentDto>>.Fail("Permission denied", 403);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            return Response<IEnumerable<AppointmentDto>>.Fail($"Patient {patientId} not found", 404);

        var cacheKey = $"PatientAppointments_{patientId}";
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppointmentDto>? cachedAppointments))
            return Response<IEnumerable<AppointmentDto>>.Ok(cachedAppointments!);

        try
        {
            var data = await _appointmentServiceProxy.GetAppointmentsAsync(patientId, cancellationToken);

            _memoryCache.Set(cacheKey, data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheDuration
            });

            return Response<IEnumerable<AppointmentDto>>.Ok(data);
        }
        catch (Exception ex)
        {
            return Response<IEnumerable<AppointmentDto>>.Fail($"Failed to retrieve appointments: {ex.Message}", 500);
        }
    }
}
