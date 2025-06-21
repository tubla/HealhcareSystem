using AutoMapper;
using doctor.models.V1.Db;
using doctor.models.V1.Dto;
using doctor.repositories.V1.Contracts;
using doctor.services.V1.Contracts;
using doctor.services.V1.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;

namespace doctor.services.V1.Services;

public class DoctorService(IUnitOfWork _unitOfWork,
    IDepartmentServiceProxy _departmentServiceProxy,
    IAppointmentServiceProxy _appointmentServiceProxy,
    IAppointmentServiceProxyInternal _appointmentServiceProxyInternal,
    IAuthServiceProxy _authServiceProxy,
    IMemoryCache _memoryCache,
    IMapper _mapper) : IDoctorService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    private async Task ValidateUserIdAsync(int? userId, int? excludeDoctorId, CancellationToken cancellationToken)
    {
        if (!userId.HasValue) return;

        if (userId.Value <= 0)
            throw new ArgumentException("Invalid user ID");

        if (!await _authServiceProxy.CheckUserExistsAsync(userId.Value, cancellationToken))
            throw new RecordNotFoundException($"User {userId.Value} not found");

        var existingDoctor = await _unitOfWork.DoctorRepository.GetByUserIdAsync(userId.Value, cancellationToken);
        if (existingDoctor != null && (!excludeDoctorId.HasValue || existingDoctor.DoctorId != excludeDoctorId.Value))
            throw new InvalidOperationException("User is already associated with another doctor");
    }

    private async Task ValidateDoctorUniquenessAsync(string licenseNumber, string email, string? phone, int? excludeDoctorId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new ArgumentException("License number is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        var existingDoctorByLicense = await _unitOfWork.DoctorRepository.GetByLicenseNumberAsync(licenseNumber, cancellationToken);
        if (existingDoctorByLicense != null && (!excludeDoctorId.HasValue || existingDoctorByLicense.DoctorId != excludeDoctorId.Value))
            throw new InvalidOperationException("Doctor with this license number already exists");

        var existingDoctorByEmail = await _unitOfWork.DoctorRepository.GetByEmailAsync(email, cancellationToken);
        if (existingDoctorByEmail != null && (!excludeDoctorId.HasValue || existingDoctorByEmail.DoctorId != excludeDoctorId.Value))
            throw new InvalidOperationException("Doctor with this email already exists");

        if (!string.IsNullOrWhiteSpace(phone))
        {
            var existingDoctorByPhone = await _unitOfWork.DoctorRepository.GetByPhoneAsync(phone, cancellationToken);
            if (existingDoctorByPhone != null && (!excludeDoctorId.HasValue || existingDoctorByPhone.DoctorId != excludeDoctorId.Value))
                throw new InvalidOperationException("Doctor with this phone number already exists");
        }
    }

    public async Task<Response<DoctorResponseDto>> CreateAsync(CreateDoctorRequestDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (dto.DeptId <= 0)
            throw new ArgumentException("Invalid department ID");

        var department = await _departmentServiceProxy.GetDepartmentAsync(dto.DeptId, cancellationToken);
        if (department == null)
            throw new RecordNotFoundException($"Department with ID {dto.DeptId} not found");

        await ValidateUserIdAsync(dto.UserId, null, cancellationToken);
        await ValidateDoctorUniquenessAsync(dto.LicenseNumber, dto.Email, dto.Phone, null, cancellationToken);

        var doctor = _mapper.Map<Doctor>(dto);
        await _unitOfWork.DoctorRepository.AddAsync(doctor, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Doctor_{doctor.DoctorId}");

        return Response<DoctorResponseDto>.Ok(_mapper.Map<DoctorResponseDto>(doctor));
    }

    public async Task<Response<DoctorResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadDoctor, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var cacheKey = $"Doctor_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out DoctorResponseDto? cachedDoctor))
            return Response<DoctorResponseDto>.Ok(cachedDoctor!);

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            throw new RecordNotFoundException($"Doctor {id} not found");

        var doctorDto = _mapper.Map<DoctorResponseDto>(doctor);
        _memoryCache.Set(cacheKey, doctorDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<DoctorResponseDto>.Ok(doctorDto);
    }

    public async Task<Response<DoctorResponseDto>> UpdateAsync(int id, int userId, UpdateDoctorRequestDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            throw new RecordNotFoundException($"Doctor {id} not found");

        if (dto.IsDeptIdSet && dto.DeptId.HasValue)
        {
            if (dto.DeptId.Value <= 0)
                throw new ArgumentException("Invalid department ID");

            var department = await _departmentServiceProxy.GetDepartmentAsync(dto.DeptId.Value, cancellationToken);
            if (department == null)
                throw new RecordNotFoundException($"Department with ID {dto.DeptId} not found");

            doctor.DeptId = dto.DeptId.Value;
        }

        if (dto.IsUserIdSet)
            await ValidateUserIdAsync(dto.UserId, id, cancellationToken);

        if (dto.IsLicenseNumberSet && !string.IsNullOrWhiteSpace(dto.LicenseNumber) ||
            dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email) ||
            dto.IsPhoneSet && !string.IsNullOrWhiteSpace(dto.Phone))
        {
            await ValidateDoctorUniquenessAsync(
                dto.IsLicenseNumberSet ? dto.LicenseNumber! : doctor.LicenseNumber!,
                dto.IsEmailSet ? dto.Email! : doctor.Email!,
                dto.IsPhoneSet ? dto.Phone : doctor.Phone,
                id,
                cancellationToken);
        }

        if (dto.IsLicenseNumberSet && !string.IsNullOrWhiteSpace(dto.LicenseNumber))
            doctor.LicenseNumber = dto.LicenseNumber;

        if (dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email))
            doctor.Email = dto.Email;

        if (dto.IsPhoneSet && !string.IsNullOrWhiteSpace(dto.Phone))
            doctor.Phone = dto.Phone;

        if (dto.IsNameSet && !string.IsNullOrWhiteSpace(dto.Name))
            doctor.Name = dto.Name;

        if (dto.IsSpecializationSet && !string.IsNullOrWhiteSpace(dto.Specialization))
            doctor.Specialization = dto.Specialization;

        if (dto.IsUserIdSet)
            doctor.UserId = dto.UserId;

        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Doctor_{id}");

        return Response<DoctorResponseDto>.Ok(_mapper.Map<DoctorResponseDto>(doctor));
    }

    public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDoctor, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
            throw new RecordNotFoundException($"Doctor {id} not found");

        if (await _appointmentServiceProxy.CheckDoctorHasAppointmentsAsync(id, cancellationToken))
            throw new InvalidOperationException("Cannot delete doctor with active appointments");

        await _unitOfWork.DoctorRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Doctor_{id}");

        return Response<bool>.Ok(true);
    }

    public async Task<Response<IEnumerable<AppointmentResponseDto>>> GetAppointmentsAsync(int doctorId, DateTime date, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (doctorId <= 0)
            throw new ArgumentException("Invalid doctor ID");

        var doctor = await _unitOfWork.DoctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor == null)
            throw new RecordNotFoundException($"Doctor {doctorId} not found");

        var cacheKey = $"DoctorAppointments_{doctorId}_{date:yyyyMMdd}";
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppointmentResponseDto>? cachedAppointments))
            return Response<IEnumerable<AppointmentResponseDto>>.Ok(cachedAppointments!);

        var appointments = await _appointmentServiceProxyInternal.GetAppointmentsAsync(doctorId, date, cancellationToken);
        _memoryCache.Set(cacheKey, appointments, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<IEnumerable<AppointmentResponseDto>>.Ok(appointments);
    }

    public async Task<bool> CheckDepartmentAsignedAsync(int deptId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadDepartment, cancellationToken))
            throw new DoctorAccessPermissionException("Permission denied");

        if (deptId <= 0)
            throw new ArgumentException("Invalid department ID");

        var doctors = await _unitOfWork.DoctorRepository.GetByDepartmentIdAsync(deptId, cancellationToken);
        return doctors != null && doctors.Any();

    }
}
