using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Security.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security;

public static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services) =>
        services
            .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>()
            .AddSingleton<IEnumerable<ITokenService>>(sp =>
            {
                var identityProvidersConfiguration = sp.GetService<IdentityProvidersConfiguration>();
                if (identityProvidersConfiguration is null)
                {
                    return [];
                }

                var tokenServices = new List<ITokenService>();

                if (identityProvidersConfiguration.WorkOS is not null)
                {
                    tokenServices.Add(sp.GetRequiredService<IWorkOSTokenService>());
                }

                if (identityProvidersConfiguration.Cognito?.JwksUri != null)
                {
                    tokenServices.Add(sp.GetRequiredService<ICognitoTokenService>());
                }

                if (identityProvidersConfiguration.Google is not null && !string.IsNullOrWhiteSpace(identityProvidersConfiguration.Google.Issuer))
                {
                    tokenServices.Add(sp.GetRequiredService<IGoogleTokenService>());
                }

                var azureEntraConfiguration = sp.GetService<AzureEntraConfiguration>();
                if (azureEntraConfiguration is not null)
                {
                    tokenServices.Add(sp.GetRequiredService<IAzureEntraTokenService>());
                }

                return tokenServices;
            });

    public static WebApplication UseSecurity(this WebApplication app)
    {
        app.UseMiddleware<SecurityContextEnricherMiddleware>();

        return app;
    }
}
