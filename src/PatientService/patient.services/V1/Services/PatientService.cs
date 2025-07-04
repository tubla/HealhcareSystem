﻿using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using patient.models.V1.Db;
using patient.models.V1.Dto;
using patient.repositories.V1.Contracts;
using patient.services.V1.Contracts;
using patient.services.V1.Exceptions;
using shared.V1.HelperClasses;
using shared.V1.HelperClasses.Contracts;
using shared.V1.Models;

namespace patient.services.V1.Services;

public class PatientService(
            IUnitOfWork _unitOfWork,
            IMapper _mapper,
            IAppointmentServiceProxyInternal _appointmentServiceProxyInternal,
            IInsuranceServiceProxy _insuranceServiceProxy,
            IAuthServiceProxy _authServiceProxy,
            IMemoryCache _memoryCache) : IPatientService
{

    private static readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Response<PatientResponseDto>> CreateAsync(CreatePatientRequestDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePatient, cancellationToken))
            return Response<PatientResponseDto>.Fail("Permission denied", 403);

        if (dto.InsuranceProviderId.HasValue)
        {
            try
            {
                if (!await _insuranceServiceProxy.CheckInsuranceProviderAsync(dto.InsuranceProviderId.Value, cancellationToken))
                    return Response<PatientResponseDto>.Fail("Insurance provider not found", 404);
            }
            catch
            {
                return Response<PatientResponseDto>.Fail("Insurance provider not found", 404);
            }
        }

        if (dto.UserId.HasValue)
        {
            try
            {
                if (!await _authServiceProxy.CheckUserExistsAsync(dto.UserId.Value, cancellationToken))
                {
                    return Response<PatientResponseDto>.Fail("User not found", 404);
                }
            }
            catch
            {
                return Response<PatientResponseDto>.Fail("User not found", 404);
            }

            // Check if the user is already associated with another patient
            var existingDoctorByUser = await _unitOfWork.PatientRepository.GetByUserIdAsync(dto.UserId.Value, cancellationToken);
            if (existingDoctorByUser != null)
                return Response<PatientResponseDto>.Fail("User is already associated with another patient", 409);
        }

        var existingPatient = await _unitOfWork.PatientRepository.GetByEmailAsync(dto.Email, cancellationToken);
        if (existingPatient != null)
            return Response<PatientResponseDto>.Fail("Patient with this email already exists", 409);

        if (dto.Dob > DateTime.Today || dto.Dob < DateTime.Today.AddYears(-120))
            return Response<PatientResponseDto>.Fail("Invalid date of birth", 400);

        if (!new[] { "M", "F", "O" }.Contains(dto.Gender))
            return Response<PatientResponseDto>.Fail("Invalid gender. Must be 'M', 'F', or 'O'", 400);

        var patient = _mapper.Map<Patient>(dto);
        await _unitOfWork.PatientRepository.AddAsync(patient, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this patient if it exists
        _memoryCache.Remove($"Patient_{patient.PatientId}");

        return Response<PatientResponseDto>.Ok(_mapper.Map<PatientResponseDto>(patient));
    }

    public async Task<Response<PatientResponseDto>> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPatient, cancellationToken))
            return Response<PatientResponseDto>.Fail("Permission denied", 403);

        var cacheKey = $"Patient_{id}";
        if (_memoryCache.TryGetValue(cacheKey, out PatientResponseDto? cachedPatient))
            return Response<PatientResponseDto>.Ok(cachedPatient!);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        if (patient == null)
            return Response<PatientResponseDto>.Fail($"Patient {id} not found", 404);

        var patientDto = _mapper.Map<PatientResponseDto>(patient);
        _memoryCache.Set(cacheKey, patientDto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        });

        return Response<PatientResponseDto>.Ok(patientDto);
    }

    public async Task<bool> CheckPatientExistsAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadPatient, cancellationToken))
            throw new PatientAccessPermissionException("Permission denied");

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        return patient != null;
    }

    public async Task<Response<PatientResponseDto>> UpdateAsync(int id, UpdatePatientRequestDto dto, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.WritePatient, cancellationToken))
            return Response<PatientResponseDto>.Fail("Permission denied", 403);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(id, cancellationToken);
        if (patient == null)
            return Response<PatientResponseDto>.Fail($"Patient {id} not found", 404);

        if (dto.IsInsuranceProviderIdSet && dto.InsuranceProviderId.HasValue && dto.InsuranceProviderId > 0)
        {
            try
            {
                if (!await _insuranceServiceProxy.CheckInsuranceProviderAsync(dto.InsuranceProviderId.Value, cancellationToken))
                    return Response<PatientResponseDto>.Fail("Insurance provider not found", 404);
            }
            catch
            {
                return Response<PatientResponseDto>.Fail("Insurance provider not found", 404);
            }

            patient.InsuranceProviderId = dto.InsuranceProviderId.Value;
        }

        if (dto.IsUserIdSet && dto.UserId.HasValue && dto.UserId > 0)
        {
            try
            {
                if (!await _authServiceProxy.CheckUserExistsAsync(dto.UserId.Value, cancellationToken))
                {
                    return Response<PatientResponseDto>.Fail("User not found", 404);
                }
            }
            catch
            {
                return Response<PatientResponseDto>.Fail("User not found", 404);
            }

            // Check if the user is already associated with another patient
            var existingDoctorByUser = await _unitOfWork.PatientRepository.GetByUserIdAsync(dto.UserId.Value, cancellationToken);
            if (existingDoctorByUser != null)
                return Response<PatientResponseDto>.Fail("User is already associated with another patient", 409);

            patient.UserId = dto.UserId.Value;
        }

        if (dto.IsEmailSet && !string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingPatient = await _unitOfWork.PatientRepository.GetByEmailAsync(dto.Email, cancellationToken);
            if (existingPatient != null && existingPatient.PatientId != id)
                return Response<PatientResponseDto>.Fail("Patient with this email already exists", 409);
        }

        if (dto.IsDobSet)
        {
            if (dto.Dob > DateTime.Today || dto.Dob < DateTime.Today.AddYears(-120))
                return Response<PatientResponseDto>.Fail("Invalid date of birth", 400);

            patient.Dob = dto.Dob;
        }

        if (dto.IsGenderSet)
        {
            if (!new[] { "M", "F", "O" }.Contains(dto.Gender))
                return Response<PatientResponseDto>.Fail("Invalid gender. Must be 'M', 'F', or 'O'", 400);

            patient.Gender = dto.Gender;
        }

        if (dto.IsPhoneSet && !string.IsNullOrWhiteSpace(dto.Phone))
        {
            var existingPaientByPhone = await _unitOfWork.PatientRepository.GetByPhoneAsync(dto.Phone, cancellationToken);
            if (existingPaientByPhone != null && existingPaientByPhone.PatientId != id)
                return Response<PatientResponseDto>.Fail("Another patient with this phone number exists", 409);

            patient.Phone = dto.Phone;
        }

        if (dto.IsNameSet && !string.IsNullOrWhiteSpace(dto.Name))
            patient.Name = dto.Name;
        if (dto.IsAddressSet && !string.IsNullOrWhiteSpace(dto.Address))
            patient.Address = dto.Address;

        await _unitOfWork.CompleteAsync(cancellationToken);

        // Invalidate cache for this patient
        _memoryCache.Remove($"Patient_{id}");

        return Response<PatientResponseDto>.Ok(_mapper.Map<PatientResponseDto>(patient));
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

    public async Task<Response<IEnumerable<AppointmentResponseDto>>> GetAppointmentsAsync(int patientId, int userId, CancellationToken cancellationToken = default)
    {
        if (!await _authServiceProxy.CheckPermissionAsync(userId, RbacPermissions.ReadAppointment, cancellationToken))
            return Response<IEnumerable<AppointmentResponseDto>>.Fail("Permission denied", 403);

        var patient = await _unitOfWork.PatientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
            return Response<IEnumerable<AppointmentResponseDto>>.Fail($"Patient {patientId} not found", 404);

        var cacheKey = $"PatientAppointments_{patientId}";
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<AppointmentResponseDto>? cachedAppointments))
            return Response<IEnumerable<AppointmentResponseDto>>.Ok(cachedAppointments!);

        try
        {
            var data = await _appointmentServiceProxyInternal.GetAppointmentsAsync(patientId, cancellationToken);

            _memoryCache.Set(cacheKey, data, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheDuration
            });

            return Response<IEnumerable<AppointmentResponseDto>>.Ok(data);
        }
        catch (Exception ex)
        {
            return Response<IEnumerable<AppointmentResponseDto>>.Fail($"Failed to retrieve appointments: {ex.Message}", 500);
        }
    }
}
