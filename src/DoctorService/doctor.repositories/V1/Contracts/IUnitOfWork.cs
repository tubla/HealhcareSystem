﻿namespace doctor.repositories.V1.Contracts;

public interface IUnitOfWork
{
    IDoctorRepository DoctorRepository { get; }
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
