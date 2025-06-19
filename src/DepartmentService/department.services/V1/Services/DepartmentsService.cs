using AutoMapper;
using department.models.V1.Db;
using department.models.V1.Dto;
using department.repositories.V1.Contracts;
using department.services.V1.Contracts;
using Microsoft.Extensions.Caching.Memory;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;
using shared.Models;

namespace department.services.V1.Services;

public class DepartmentsService(IUnitOfWork _unitOfWork,
    IMapper _mapper,
    IDoctorServiceProxy _doctorServiceProxy,
    IAuthServiceProxy _authServiceProxy,
    IMemoryCache _memoryCache) : IDepartmentsService
{
    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Response<DepartmentDto>> CreateAsync(CreateDepartmentDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDepartment, cancellationToken))
            return Response<DepartmentDto>.Fail("Permission denied", 403);

        var existingDepartment = await _unitOfWork.DepartmentRepository.GetByNameAsync(dto.Name, cancellationToken);
        if (existingDepartment != null)
            return Response<DepartmentDto>.Fail("Department with this name already exists", 409);

        var department = _mapper.Map<Department>(dto);
        await _unitOfWork.DepartmentRepository.AddAsync(department, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{department.DeptId}");

        return Response<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department));
    }

    public async Task<Response<DepartmentDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPrescription, cancellationToken))
            return Response<DepartmentDto>.Fail("Permission denied", 403);

        var cacheKey = $"Department_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out DepartmentDto? cachedDepartment))
            return Response<DepartmentDto>.Ok(cachedDepartment!);

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            return Response<DepartmentDto>.Fail($"Department {id} not found", 404);

        var departmentDto = _mapper.Map<DepartmentDto>(department);
        _memoryCache.Set(cacheKey, departmentDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<DepartmentDto>.Ok(departmentDto);
    }

    public async Task<Response<DepartmentDto>> UpdateAsync(int id, UpdateDepartmentDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WriteDepartment, cancellationToken))
            return Response<DepartmentDto>.Fail("Permission denied", 403);

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            return Response<DepartmentDto>.Fail($"Department {id} not found", 404);

        if (dto.IsNameSet && !string.IsNullOrEmpty(dto.Name))
        {
            var existingDepartment = await _unitOfWork.DepartmentRepository.GetByNameAsync(dto.Name, cancellationToken);
            if (existingDepartment != null && existingDepartment.DeptId != id)
                return Response<DepartmentDto>.Fail("Another department with this name exists", 409);

            department.Name = dto.Name.Trim();
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{id}");

        return Response<DepartmentDto>.Ok(_mapper.Map<DepartmentDto>(department));
    }

    public async Task<Response<bool>> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, "Department.Delete", cancellationToken))
            return Response<bool>.Fail("Permission denied", 403);

        var department = await _unitOfWork.DepartmentRepository.GetByIdAsync(id, cancellationToken);
        if (department == null)
            return Response<bool>.Fail($"Department {id} not found", 404);

        try
        {
            if (await _doctorServiceProxy.CheckDoctorAssigned(id, cancellationToken))
                return Response<bool>.Fail("Cannot delete department with assigned doctors", 400);
        }
        catch
        {
            return Response<bool>.Fail($"Department {id} cannot be deleted.", 404);
        }

        await _unitOfWork.DepartmentRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _memoryCache.Remove($"Department_{id}");

        return Response<bool>.Ok(true);
    }
}
