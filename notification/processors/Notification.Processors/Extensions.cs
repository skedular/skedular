using Notification.Processors.Mappers;
using Notification.Processors.Services;

namespace Notification.Processors;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();
    
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddSingleton<IEmailService, EmailService>();

}
