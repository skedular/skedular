using Enterprise.Shared.Accounting.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Accounting;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers Xero OAuth2 SDK services: <see cref="IXeroSdkClientFactory" /> and
        ///     <see cref="IXeroTokenEncryptionService" />.
        ///     Reads configuration from the <c>Xero</c> section.
        ///     Token encryption uses a dedicated key (<c>Xero:EncryptionKey</c>) separate from
        ///     cookie-encryption keys.
        /// </summary>
        /// <param name="configuration">Application configuration (reads the <c>Xero</c> section).</param>
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
