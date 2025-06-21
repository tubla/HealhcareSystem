using AutoMapper;
using department.models.V1.Db;
using department.models.V1.Dto;
using department.repositories.V1.Contracts;
using department.services.V1.Contracts;
using department.services.V1.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;

namespace department.services.V1.Services;

public class DepartmentsService(IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IDoctorServiceProxy _doctorServiceProxy,
    IAuthServiceProxy _authServiceProxy,
    IMemoryCache _memoryCache) : IDepartmentsService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    private async Task ValidateDepartmentNameAsync(string name, int? excludeDeptId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Department name is required");

        var existingDepartment = await _unitOfWork.DepartmentRepository.GetByNameAsync(name.Trim(), cancellationToken);
        if (existingDepartment != null && (!excludeDeptId.HasValue || existingDepartment.DeptId != excludeDeptId.Value))
            throw new InvalidOperationException("Department with this name already exists");
    }

    public async Task<Response<DepartmentResponseDto>> CreateAsync(CreateDepartmentRequestDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDepartment, cancellationToken))
            throw new DepartmentAccessPermissionException("Permission denied");

        await ValidateDepartmentNameAsync(dto.Name, null, cancellationToken);

        var department = _mapper.Map<Department>(dto);
        await _unitOfWork.DepartmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{department.DeptId}");

        return Response<DepartmentResponseDto>.Ok(_mapper.Map<DepartmentResponseDto>(department));

    }

    public async Task<Response<DepartmentResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadDepartment, cancellationToken))
            throw new DepartmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid department ID");

        var cacheKey = $"Department_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out DepartmentResponseDto? cachedDepartment))
            return Response<DepartmentResponseDto>.Ok(cachedDepartment!);

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            throw new RecordNotFoundException($"Department {id} not found");

        var departmentDto = _mapper.Map<DepartmentResponseDto>(department);
        _memoryCache.Set(cacheKey, departmentDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<DepartmentResponseDto>.Ok(departmentDto);
    }

    public async Task<Response<DepartmentResponseDto>> UpdateAsync(int id, UpdateDepartmentRequestDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDepartment, cancellationToken))
            throw new DepartmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid department ID");

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            throw new RecordNotFoundException($"Department {id} not found");

        if (dto.IsNameSet && !string.IsNullOrWhiteSpace(dto.Name))
            await ValidateDepartmentNameAsync(dto.Name, id, cancellationToken);

        if (dto.IsNameSet && !string.IsNullOrWhiteSpace(dto.Name))
            department.Name = dto.Name.Trim();

        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{id}");

        return Response<DepartmentResponseDto>.Ok(_mapper.Map<DepartmentResponseDto>(department));
    }

    public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDepartment, cancellationToken))
            throw new DepartmentAccessPermissionException("Permission denied");

        if (id <= 0)
            throw new ArgumentException("Invalid department ID");

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            throw new RecordNotFoundException($"Department {id} not found");

        if (await _doctorServiceProxy.CheckDoctorAssigned(id, cancellationToken))
            throw new InvalidOperationException("Cannot delete department with assigned doctors");

        await _unitOfWork.DepartmentRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{id}");

        return Response<bool>.Ok(true);
    }
}
