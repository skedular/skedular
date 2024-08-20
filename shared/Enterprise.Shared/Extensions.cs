using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Time;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared;

public static class Extensions
{
    public static IServiceCollection AddSecurity(this IServiceCollection services) =>
        services
            .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>()
            .AddSingleton<ICognitoTokenService, CognitoTokenService>()
            .AddSingleton<IGoogleTokenService, GoogleTokenService>()
            .AddSingleton<IMsTeamsTokenServiceTokenService, MsTeamsTokenServiceTokenService>()
            .AddSingleton<IEnumerable<ITokenService>>(sp =>
            {
                var applicationConfiguration = sp.GetRequiredService<ApplicationConfiguration>();
                ArgumentNullException.ThrowIfNull(applicationConfiguration);

                var tokenServices = new List<ITokenService>();

                if (applicationConfiguration.IdentityProviders.Cognito is not null &&
                    applicationConfiguration.IdentityProviders.Cognito.JwksUri is not null &&
                    !string.IsNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Cognito.Issuer) &&
                    !string.IsNullOrWhiteSpace(applicationConfiguration.IdentityProviders.Cognito.Audiences))
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
                    tokenServices.Add(sp.GetRequiredService<IMsTeamsTokenServiceTokenService>());
                }

                return tokenServices;
            });

    public static IServiceCollection AddContext(this IServiceCollection services) =>
        services.AddScoped<IContext, Context.Context>();

    public static IServiceCollection AddTimeHelper(this IServiceCollection services) =>
        services
            .AddSingleton<ITimeHelper, TimeHelper>();

    public static IServiceCollection AddRandomHelper(this IServiceCollection services) =>
        services
            .AddSingleton(new System.Random())
            .AddSingleton<IRandomHelper, RandomHelper>();
}
