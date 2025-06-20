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
    IDepartmentServiceProxy _departmentServiceProxy,
    IAppointmentServiceProxyInternal _appointmentServiceProxyInternal,
    IAuthServiceProxy _authServiceProxy,
    IMemoryCache _memoryCache,
    IMapper _mapper) : IDoctorService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
    public async Task<Response<DoctorDto>> CreateAsync(CreateDoctorDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            return Response<DoctorDto>.Fail("Permission denied", 403);

        var department = await _departmentServiceProxy.GetDepartmentAsync(dto.DeptId, cancellationToken);
        if (department == null)
            return Response<DoctorDto>.Fail($"Department with ID {dto.DeptId} not found", 404);

        if (dto.UserId.HasValue)
        {
            try
            {
                if (!await _authServiceProxy.CheckUserExistsAsync(dto.UserId.Value, cancellationToken))
                {
                    return Response<DoctorDto>.Fail("User not found", 404);
                }
            }
            catch
            {
                return Response<DoctorDto>.Fail("User not found", 404);
            }

            // Check if the user is already associated with another doctor
            var existingDoctorByUser = await _unitOfWork.DoctorRepository.GetByUserIdAsync(dto.UserId.Value, cancellationToken);
            if (existingDoctorByUser != null)
                return Response<DoctorDto>.Fail("User is already associated with another doctor", 409);
        }




        var existingDoctor = await _unitOfWork.DoctorRepository.GetByLicenseNumberAsync(dto.LicenseNumber, cancellationToken);
        if (existingDoctor != null)
            return Response<DoctorDto>.Fail("Doctor with this license number already exists", 409);

        var existingDoctorByEmail = await _unitOfWork.DoctorRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingDoctorByEmail != null)
            return Response<DoctorDto>.Fail("Doctor with this email already exists", 409);

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
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<DoctorDto>.Ok(doctorDto);
    }

    public async Task<Response<DoctorDto>> UpdateAsync(int id, int userId, UpdateDoctorDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            return Response<DoctorDto>.Fail("Permission denied", 403);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            return Response<DoctorDto>.Fail($"Doctor {id} not found", 404);

        if (dto.IsLicenseNumberSet && !string.IsNullOrWhiteSpace(dto.LicenseNumber))
        {
            var existingDoctor = await _unitOfWork.DoctorRepository.GetByLicenseNumberAsync(dto.LicenseNumber, cancellationToken);
            if (existingDoctor != null && existingDoctor.DoctorId != id)
                return Response<DoctorDto>.Fail("Another doctor with this license number exists", 409);

            doctor.LicenseNumber = dto.LicenseNumber;
        }

        if (dto.IsDeptIdSet && dto.DeptId.HasValue && dto.DeptId.Value > 0)
        {
            var department = await _departmentServiceProxy.GetDepartmentAsync(dto.DeptId.Value, cancellationToken);
            if (department == null)
                return Response<DoctorDto>.Fail($"Department with ID {dto.DeptId} not found", 404);

            doctor.DeptId = dto.DeptId.Value;
        }

        if (dto.IsUserIdSet && dto.UserId.HasValue)
        {
            try
            {
                if (!await _authServiceProxy.CheckUserExistsAsync(dto.UserId.Value, cancellationToken))
                {
                    return Response<DoctorDto>.Fail("User not found", 404);
                }
            }
            catch
            {
                return Response<DoctorDto>.Fail("User not found", 404);
            }

            // Check if the user is already associated with another doctor
            var existingDoctorByUser = await _unitOfWork.DoctorRepository.GetByUserIdAsync(dto.UserId.Value, cancellationToken);
            if (existingDoctorByUser != null)
                return Response<DoctorDto>.Fail("User is already associated with another doctor", 409);

            doctor.UserId = dto.UserId.Value;
        }

        if (dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingDoctorByEmail = await _unitOfWork.DoctorRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (existingDoctorByEmail != null && existingDoctorByEmail.DoctorId != id)
                return Response<DoctorDto>.Fail("Another doctor with this email exists", 409);

            doctor.Email = dto.Email;
        }

        if (dto.IsPhoneSet && !string.IsNullOrWhiteSpace(dto.Phone))
        {
            var existingDoctorByPhone = await _unitOfWork.DoctorRepository.GetByPhoneAsync(dto.Phone, cancellationToken);
            if (existingDoctorByPhone != null && existingDoctorByPhone.DoctorId != id)
                return Response<DoctorDto>.Fail("Another doctor with this phone number exists", 409);

            doctor.Phone = dto.Phone;
        }


        if (dto.IsNameSet && !string.IsNullOrWhiteSpace(dto.Name))
            doctor.Name = dto.Name;
        if (dto.IsSpecializationSet && !string.IsNullOrWhiteSpace(dto.Specialization))
            doctor.Specialization = dto.Specialization;

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
            var data = await _appointmentServiceProxyInternal.GetAppointmentsAsync(doctorId, cancellationToken);
            _memoryCache.Set(cacheKey, data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });
            return Response<IEnumerable<AppointmentDto>>.Ok(data);
        }
        catch (Exception ex)
        {
            return Response<IEnumerable<AppointmentDto>>.Fail($"Failed to retrieve appointments: {ex.Message}", 500);
        }
    }

    public async Task<bool> CheckDepartmentAsignedAsync(int deptId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new UnauthorizedAccessException("No access permission");

        var doctor = await _unitOfWork.DoctorRepository.GetByDepartmentIdAsync(deptId, cancellationToken);
        return doctor != null && doctor.Any();

    }
}
