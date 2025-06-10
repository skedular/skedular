using Enterprise.Shared.Outbox;
using Organization.Jobs.Jobs;
using Organization.Jobs.Services;

namespace Organization.Jobs;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ITemporalOutboxExecutor, TemporalOutboxExecutorService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<OrganizationDailyMemberCountRecorderJob>()
            .AddHostedService<OrganizationOfferingRenewalJob>()
            .AddHostedService<RefreshAzureTenantMembersJob>()
            .AddHostedService<BuiltInTagsSyncJob>();
}
