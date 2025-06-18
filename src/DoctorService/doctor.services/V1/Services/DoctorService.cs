using AutoMapper;
using doctor.models.V1.Db;
using doctor.models.V1.Dto;
using doctor.repositories.V1.Contracts;
using doctor.services.V1.Contracts;
using Microsoft.Extensions.Caching.Memory;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;
using shared.Models;

namespace doctor.services.V1.Services;

public class DoctorService(IUnitOfWork _unitOfWork,
    IAppointmentServiceProxy _appointmentServiceProxy,
    IMemoryCache _memoryCache,
    IAuthServiceProxy _authServiceProxy,
    IMapper _mapper) : IDoctorService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    public async Task<Response<DoctorDto>> CreateAsync(CreateDoctorDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            return Response<DoctorDto>.Fail("Permission denied", 403);

        var existingDoctor = await _unitOfWork.DoctorRepository.GetByLicenseNumberAsync(dto.LicenseNumber, cancellationToken);
        if (existingDoctor != null)
            return Response<DoctorDto>.Fail("Doctor with this license number already exists", 409);

        var doctor = _mapper.Map<Doctor>(dto);
        await _unitOfWork.DoctorRepository.AddAsync(doctor, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this doctor if it exists
        _memoryCache.Remove($"Doctor_{doctor.DoctorId}");

        return Response<DoctorDto>.Ok(_mapper.Map<DoctorDto>(doctor));
    }

    public async Task<Response<DoctorDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadDoctor, cancellationToken))
            return Response<DoctorDto>.Fail("Permission denied", 403);

        var cacheKey = $"Doctor_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out DoctorDto? cachedDoctor))
            return Response<DoctorDto>.Ok(cachedDoctor!);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            return Response<DoctorDto>.Fail($"Doctor {id} not found", 404);

        var doctorDto = _mapper.Map<DoctorDto>(doctor);
        _memoryCache.Set(cacheKey, doctorDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        });

        return Response<DoctorDto>.Ok(doctorDto);
    }

    public async Task<Response<DoctorDto>> UpdateAsync(int id, string licenseNumber, int userId, UpdateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            return Response<DoctorDto>.Fail("Permission denied", 403);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            return Response<DoctorDto>.Fail($"Doctor {id} not found", 404);

        var existingDoctor = await _unitOfWork.DoctorRepository.GetByLicenseNumberAsync(licenseNumber, cancellationToken);
        if (existingDoctor != null && existingDoctor.DoctorId != id)
            return Response<DoctorDto>.Fail("Another doctor with this license number exists", 409);

        if (dto.IsFirstNameSet && !string.IsNullOrWhiteSpace(dto.FirstName))
            doctor.FirstName = dto.FirstName;
        if (dto.IsLastNameSet && !string.IsNullOrWhiteSpace(dto.LastName))
            doctor.LastName = dto.LastName;
        if (dto.IsSpecializationSet && !string.IsNullOrWhiteSpace(dto.Specialization))
            doctor.Specialization = dto.Specialization;
        if (dto.IsContactNumberSet && !string.IsNullOrWhiteSpace(dto.ContactNumber))
            doctor.ContactNumber = dto.ContactNumber;
        if (dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email))
            doctor.Email = dto.Email;
        if (dto.IsHospitalAffiliationSet && !string.IsNullOrWhiteSpace(dto.HospitalAffiliation))
            doctor.HospitalAffiliation = dto.HospitalAffiliation;

        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this doctor
        _memoryCache.Remove($"Doctor_{id}");

        return Response<DoctorDto>.Ok(_mapper.Map<DoctorDto>(doctor));
    }

    public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            return Response<bool>.Fail("Permission denied", 403);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            return Response<bool>.Fail($"Doctor {id} not found", 404);

        await _unitOfWork.DoctorRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this doctor
        _memoryCache.Remove($"Doctor_{id}");

        return Response<bool>.Ok(true);
    }

    public async Task<Response<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(int doctorId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            return Response<IEnumerable<AppointmentDto>>.Fail("Permission denied", 403);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor == null)
            return Response<IEnumerable<AppointmentDto>>.Fail("Doctor not found", 404);

        var cacheKey = $"DoctorAppointments_{doctorId}";
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppointmentDto>? cachedAppointments))
            return Response<IEnumerable<AppointmentDto>>.Ok(cachedAppointments!);

        try
        {
            var data = await _appointmentServiceProxy.GetAppointmentsAsync(doctorId, cancellationToken);
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
