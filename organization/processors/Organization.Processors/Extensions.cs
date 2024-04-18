using Organization.Processors.Jobs;
using Organization.Processors.Mappers;

namespace Organization.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<OrganizationDailyMemberCountRecorderJob>()
            .AddHostedService<OrganizationOfferingRenewalJob>();
}
