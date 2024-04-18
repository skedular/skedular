using Location.Processors.Jobs;
using Location.Processors.Mappers;

namespace Location.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<LocationDailyDeskCountRecorderJob>();
}
