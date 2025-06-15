using appointment.api.V1.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container..
builder.Configuration.AddAzureAppConfigurationWithSecrets();
builder.Services.AddServiceCollection(builder.Configuration);
var app = builder.Build();
app.UseApplicationMiddlewares();
app.Run();
