using Enterprise.Shared.Accounting.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Accounting;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddXeroServices(IConfiguration configuration)
        {
            var xeroConfiguration = configuration.GetSection(XeroConfiguration.Key).Get<XeroConfiguration>();
            ArgumentNullException.ThrowIfNull(xeroConfiguration);

            return services
                .AddSingleton(xeroConfiguration)
                .AddSingleton<IXeroSdkClientFactory, XeroSdkClientFactory>()
                .AddSingleton<IXeroTokenEncryptionService, XeroTokenEncryptionService>();
        }
    }
}
