using Enterprise.Shared.Configurations;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Security.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security;

public static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>()
            .AddSingleton<IEnumerable<ITokenService>>(sp =>
            {
                var applicationConfiguration = sp.GetRequiredService<ApplicationConfiguration>();
                var tokenServices = new List<ITokenService>();

                if (applicationConfiguration.IdentityProviders.WorkOS is not null)
                {
                    tokenServices.Add(sp.GetRequiredService<IWorkOSTokenService>());
                }

                if (applicationConfiguration.IdentityProviders.Cognito?.JwksUri != null)
                {
                    tokenServices.Add(sp.GetRequiredService<ICognitoTokenService>());
                }

                if (applicationConfiguration.IdentityProviders.Google is not null &&
                    !string.IsNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Google.Issuer))
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
