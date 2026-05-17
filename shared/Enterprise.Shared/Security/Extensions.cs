using System.Net;
using System.Text.Json;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Security.Configurations;
using Enterprise.Shared.Security.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Enterprise.Shared.Security;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecurity() =>
            services
                .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>();

        public IServiceCollection AddOpenApiAuthentication(IConfiguration configuration)
        {
            var authenticationConfiguration = configuration.GetSection(AuthenticationConfiguration.Key).Get<AuthenticationConfiguration>();
            if (authenticationConfiguration is null)
            {
                throw new InvalidOperationException("Authentication requires Authentication configuration.");
            }

            if (string.IsNullOrWhiteSpace(authenticationConfiguration.Jwt.Issuer))
            {
                throw new InvalidOperationException("Authentication requires Authentication:Jwt:Issuer configuration.");
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var isDevelopmentEnvironment = string.Equals(
                        configuration["ASPNETCORE_ENVIRONMENT"],
                        Environments.Development,
                        StringComparison.InvariantCultureIgnoreCase);
                    var isHttpsIssuer = Uri.TryCreate(authenticationConfiguration.Jwt.Issuer, UriKind.Absolute, out var authorityUri)
                                        && string.Equals(authorityUri.Scheme, Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase);

                    options.RequireHttpsMetadata = !isDevelopmentEnvironment || isHttpsIssuer;
                    options.Authority = authenticationConfiguration.Jwt.Issuer;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authenticationConfiguration.Jwt.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            if (!context.Response.Headers.ContainsKey("WWW-Authenticate"))
                            {
                                context.Response.Headers.Append("WWW-Authenticate", "Bearer");
                            }

                            context.HandleResponse();

                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";

                            var payload = new
                            {
                                type = "about:blank",
                                title = "Unauthorized",
                                status = (int)HttpStatusCode.Unauthorized,
                                detail = "A valid bearer token is required.",
                                instance = context.Request.Path.Value
                            };

                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        }
                    };
                });

            services.AddAuthorizationBuilder();

            return services;
        }
    }

    extension(WebApplication app)
    {
        public WebApplication UseSecurity()
        {
            app.UseMiddleware<SecurityContextEnricherMiddleware>();

            return app;
        }
    }
}
