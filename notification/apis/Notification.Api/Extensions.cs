using Api.Shared.Services.Configurations.Grpc;
using Notification.Api.Mappers;
using Notification.Api.Services;

namespace Notification.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerService, CustomerService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var notificationConfiguration = configuration.GetSection(NotificationConfiguration.Key).Get<NotificationConfiguration>();
        ArgumentNullException.ThrowIfNull(notificationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationConfiguration.ApiKey);

        return services
            .AddSingleton(notificationConfiguration);
    }
}
