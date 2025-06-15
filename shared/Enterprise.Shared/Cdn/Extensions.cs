using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Path = System.IO.Path;

namespace Enterprise.Shared.Cdn;

public static class Extensions
{
    public static IServiceCollection AddCdn(this IServiceCollection services, IConfiguration configuration)
    {
        var cdnConfiguration = configuration.GetSection(CdnConfiguration.Key).Get<CdnConfiguration>();
        ArgumentNullException.ThrowIfNull(cdnConfiguration);

        services = services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = cdnConfiguration.MaxFileSize;
            options.ValueLengthLimit = int.MaxValue;
            options.MultipartHeadersLengthLimit = int.MaxValue;
        });

        if (cdnConfiguration.UseLocal)
        {
            if (string.IsNullOrWhiteSpace(cdnConfiguration.LocalCdnPath))
            {
                cdnConfiguration.LocalCdnPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cdn");
            }

            Directory.CreateDirectory(cdnConfiguration.LocalCdnPath);

            return services
                .AddSingleton(cdnConfiguration)
                .AddSingleton<ICdnService, LocalCdnService>();
        }

        var cloudflare = configuration.GetSection(CloudflareConfiguration.Key).Get<CloudflareConfiguration>();
        ArgumentNullException.ThrowIfNull(cloudflare);

        return services
            .AddSingleton(cloudflare)
            .AddSingleton<ICdnService, CloudflareCdnService>();
    }
}
