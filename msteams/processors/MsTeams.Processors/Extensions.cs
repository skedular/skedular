using MsTeams.Processors.Jobs;
using MsTeams.Processors.Mappers;
using MsTeams.Processors.Services;

namespace MsTeams.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IGraphService, GraphService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<RefreshRefreshAzureTenantTeamsAndChannelsJob>();
}
