using MsTeams.Api.Mappers;

namespace MsTeams.Api;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();
}
