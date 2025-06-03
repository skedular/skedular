using Core.Api.Mappers;
using Core.Api.Services;

namespace Core.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IFileUploaderService, FileUploaderServiceService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
