using Location.Jobs.Jobs;

namespace Location.Jobs;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<LocationDailyDeskCountRecorderJob>()
            .AddHostedService<LocationDailyRoomCountRecorderJob>();
//            .AddHostedService<DeskRoomToResourceSyncJob>();
}
