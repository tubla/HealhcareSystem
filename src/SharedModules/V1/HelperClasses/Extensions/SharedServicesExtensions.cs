using Microsoft.Extensions.DependencyInjection;
using shared.V1.HelperClasses.BlobStorage;
using shared.V1.HelperClasses.Contracts;
using shared.V1.HelperClasses.EventHubs;
using shared.V1.HelperClasses.ServiceProxies;

namespace shared.V1.HelperClasses.Extensions;

public static class SharedServicesExtensions
{
    public static void AddSharedServices(this IServiceCollection services)
    {
        //services.AddScoped<ISecretClient, HealthcareSecretClient>();
        services.AddScoped<IAppointmentServiceProxy, AppointmentServiceProxy>();
        services.AddScoped<IAuthServiceProxy, AuthServiceProxy>();
        services.AddScoped<IDoctorServiceProxy, DoctorServiceProxy>();
        services.AddScoped<IHttpClientService, HttpClientService>();
        services.AddScoped<IInsuranceServiceProxy, InsuranceServiceProxy>();
        services.AddScoped<IMedicationServiceProxy, MedicationServiceProxy>();
        services.AddScoped<IPatientServiceProxy, PatientServiceProxy>();

        // Register IEventHubClientFactory with IConfiguration
        services.AddSingleton<IEventHubClientFactory, EventHubClientFactory>();

        // Register EventHubWarmupService as both IEventHubClientProvider and IHostedService
        services.AddSingleton<EventHubWarmupService>();
        services.AddSingleton<IEventHubClientProvider>(sp => sp.GetRequiredService<EventHubWarmupService>());
        services.AddHostedService(sp => sp.GetRequiredService<EventHubWarmupService>());

        // Register BlobServiceWarmupService as both IBlobServiceClientProvider and IHostedService
        services.AddSingleton<BlobServiceWarmupService>();
        services.AddSingleton<IBlobServiceClientProvider>(sp => sp.GetRequiredService<BlobServiceWarmupService>());
        services.AddHostedService(sp => sp.GetRequiredService<BlobServiceWarmupService>());

        services.AddHttpContextAccessor();
    }
}
