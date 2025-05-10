using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkOS;

namespace Enterprise.Shared;

public static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);

        if (applicationConfiguration.IdentityProviders.WorkOS is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.IdentityProviders.WorkOS.ApiKey);
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.IdentityProviders.WorkOS.Issuer);
            
            services
                .AddSingleton(new WorkOSClient(new WorkOSOptions { ApiKey = applicationConfiguration.IdentityProviders.WorkOS.ApiKey }))
                .AddSingleton<IWorkOSTokenService, WorkOSTokenService>();
        }

        if (applicationConfiguration.IdentityProviders.Cognito is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Cognito.Issuer);
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Cognito.Audiences);
            
            services
                .AddSingleton<ICognitoTokenService, CognitoTokenService>();
        }

        if (applicationConfiguration.IdentityProviders.Google is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Google.Issuer);

            services
                .AddSingleton<IGoogleTokenService, GoogleTokenService>();
        }

        return services
            .AddSingleton<ICookieHelper, CookieHelper>()
            .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>()
            .AddSingleton<IAzureEntraTokenService, AzureEntraTokenService>()
            .AddSingleton<IEnumerable<ITokenService>>(sp =>
            {
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
    }

    public static IServiceCollection AddContext(this IServiceCollection services) => services.AddSingleton<IContext, Context.Context>();

    public static IServiceCollection AddRandomHelper(this IServiceCollection services) =>
        services
            .AddSingleton(new System.Random())
            .AddSingleton<IRandomHelper, RandomHelper>();

    public static string ToFullName(this Type type) => type.FullName ?? type.Name;
}
