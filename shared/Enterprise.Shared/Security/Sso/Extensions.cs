using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Security.Sso;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddSso() =>
            services
                .AddSingleton<ISamlAssertionConsumerService, SamlAssertionConsumerService>()
                .AddSingleton<ISamlLoginRequestFactory, SamlLoginRequestFactory>();
    }

    extension(WebApplication app)
    {
        public WebApplication UseSso()
        {
            app.UseMiddleware<SsoContextEnricherMiddleware>();

            return app;
        }
    }
}
