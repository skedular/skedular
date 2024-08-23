using MsTeams.Api.Jobs;
using MsTeams.Api.Mappers;
using MsTeams.Api.Services;

namespace MsTeams.Api;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IAzureTenantOnboardingService, AzureTenantOnboardingService>()
            .AddScoped<IAzureTenantService, AzureTenantService>();

    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<ConnectionKeepAliveJob>();
}
