using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Cdn;

public static class Extensions
{
    public static IServiceCollection AddCdn(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudflare = configuration.GetSection(Cloudflare.Key).Get<Cloudflare>();
        ArgumentNullException.ThrowIfNull(cloudflare);

        return services
            .AddSingleton(cloudflare)
            .AddSingleton<ICdnService, CdnService>();
    }
}
