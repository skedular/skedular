using Enterprise.Shared.Grpc;
using Enterprise.Shared.Security.Token;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSecurity() =>
            services
                .AddScoped<IGrpcAuthenticator, GrpcAuthenticator>();
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
