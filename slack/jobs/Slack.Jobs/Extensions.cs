using Slack.Jobs.Jobs;

namespace Slack.Jobs;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<LocationDailyUpdateJob>()
            .AddHostedService<RefreshWorkspaceChannelsJob>()
            .AddHostedService<RefreshWorkspaceJob>()
            .AddHostedService<RefreshWorkspaceMembersJob>()
            .AddHostedService<TeamDailyUpdateJob>()
            .AddHostedService<UpdateWorkspaceMemberProfileStatusJob>();
}
