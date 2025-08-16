using Organization.Processors.Mappers;

namespace Organization.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services;
}
