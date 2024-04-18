using Billing.Processors.Jobs;
using Billing.Processors.Mappers;

namespace Billing.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services.AddHostedService<OrganizationBillingGenerationJob>();
}
