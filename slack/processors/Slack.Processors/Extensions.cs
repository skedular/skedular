using Slack.Processors.Mappers;

namespace Slack.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
