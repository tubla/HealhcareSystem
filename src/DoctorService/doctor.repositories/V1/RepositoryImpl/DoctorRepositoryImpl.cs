﻿using doctor.models.V1.Db;
using doctor.repositories.V1.Context;
using doctor.repositories.V1.Contracts;
using Microsoft.EntityFrameworkCore;

namespace doctor.repositories.V1.RepositoryImpl;

public class DoctorRepositoryImpl(DoctorDbContext _context) : IDoctorRepository
{
    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.DoctorId == id, cancellationToken);
    }

    public async Task<IEnumerable<Doctor>?> GetByDepartmentIdAsync(int deptId, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .Where(d => d.DeptId == deptId).ToListAsync(cancellationToken);
    }

    public async Task<Doctor?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId, cancellationToken);
    }

    public async Task<Doctor?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.Email == email, cancellationToken);
    }

    public async Task<Doctor?> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.Phone == phone, cancellationToken);
    }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.LicenseNumber == licenseNumber, cancellationToken);
    }

    public async Task AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        await _context.Doctors.AddAsync(doctor, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var doctor = await GetByIdAsync(id, cancellationToken);
        if (doctor != null)
        {
            _context.Doctors.Remove(doctor);
        }
    }
}
