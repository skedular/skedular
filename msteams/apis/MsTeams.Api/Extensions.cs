using MsTeams.Api.Mappers;
using MsTeams.Api.Services;

namespace MsTeams.Api;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services.AddScoped<IMsTeamsService, MsTeamsService>();

    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();
}
