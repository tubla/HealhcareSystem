using doctor.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;
using patient.repositories.V1.Contracts;
using patient.repositories.V1.RepositoryImpl;
using patient.services.V1.Contracts;
using patient.services.V1.Exceptions;
using patient.services.V1.Services;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.Extensions;

namespace patient.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPatientServices(this IServiceCollection services)
    {
        services.AddSharedServices();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPatientRepository, PatientRepositoryImpl>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentServiceProxyInternal, AppointmentServiceProxyInternal>();
        services.AddScoped<IExceptionHandlerStrategy, PatientExceptionStrategy>();
    }
}
