using Organization.Processors.Jobs;
using Organization.Processors.Mappers;
using Organization.Processors.Services;

namespace Organization.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<OrganizationDailyMemberCountRecorderJob>()
            .AddHostedService<OrganizationOfferingRenewalJob>()
            .AddHostedService<RefreshAzureTenantMembersJob>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationMemberService, OrganizationMemberService>()
            .AddScoped<IGraphService, GraphService>();
}
