using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security.Sso;

public static class Extensions
{
    public static IServiceCollection AddSso(this IServiceCollection services) =>
        services
            .AddSingleton<ISamlAssertionConsumerService, SamlAssertionConsumerService>()
            .AddSingleton<ISamlLoginRequestFactory, SamlLoginRequestFactory>();

    public static WebApplication UseSso(this WebApplication app)
    {
        app.UseMiddleware<SsoContextEnricherMiddleware>();

        return app;
    }
}
