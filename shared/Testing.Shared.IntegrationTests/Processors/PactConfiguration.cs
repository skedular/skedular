using Microsoft.Extensions.DependencyInjection;

namespace Testing.Shared.IntegrationTests.Processors;

internal static class PactConfiguration
{
    private static void SetPactUriOnInstance<TConfig>(IServiceCollection collection, int port, Action<TConfig, Uri> setter) where TConfig : class
    {
        var serviceDescriptors = collection.Where(descriptor => descriptor.ServiceType == typeof(TConfig));

        foreach (var serviceDescriptor in serviceDescriptors)
        {
            var config = serviceDescriptor.ImplementationInstance as TConfig;

            ArgumentNullException.ThrowIfNull(config);

            setter(config, new Uri($"http://localhost:{port}"));
        }
    }

    /// <param name="collection"></param>
    extension(IServiceCollection collection)
    {
        /// <summary>
        ///     Updates settings where pact will be used instead of the external service
        /// </summary>
        /// <param name="port"></param>
        public void UpdateConfigsToUsePactHost(int port)
        {
        }
    }
}
