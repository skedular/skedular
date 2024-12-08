using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security.Sso;

public static class Extensions
{
    public static IServiceCollection AddSso(this IServiceCollection services) =>
        services
            .AddSingleton<ISamlAssertionConsumerService, SamlAssertionConsumerService>()
            .AddSingleton<ISamlLoginRequestFactory, SamlLoginRequestFactory>();
}
