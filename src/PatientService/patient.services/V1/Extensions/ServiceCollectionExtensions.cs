using doctor.services.V1.Services;
using Microsoft.Extensions.DependencyInjection;
using patient.repositories.V1.Contracts;
using patient.repositories.V1.RepositoryImpl;
using patient.services.V1.Contracts;
using patient.services.V1.Services;
using shared.HelperClasses;
using shared.HelperClasses.Contracts;

namespace patient.services.V1.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPatientServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPatientRepository, PatientRepositoryImpl>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IAppointmentServiceProxy, AppointmentServiceProxy>();
        services.AddScoped<IInsuranceServiceProxy, InsuranceServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddMemoryCache();
    }
}
