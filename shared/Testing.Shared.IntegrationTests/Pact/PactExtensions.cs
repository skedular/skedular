using Enterprise.Shared.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Testing.Shared.IntegrationTests.Processors;

namespace Testing.Shared.IntegrationTests.Pact;

public static class PactExtensions
{
    /// <summary>
    ///     Use Pact to verify calls to services
    /// </summary>
    /// <remarks>Place this at the end of the services configuration</remarks>
    /// <param name="services"></param>
    /// <param name="consumerName"></param>
    /// <param name="providerName"></param>
    /// <param name="port">
    ///     if port is null, then create a dynamic port number, otherwise will directly use the port as service
    ///     port number
    /// </param>
    /// <returns></returns>
    public static IServiceCollection UsePact(
        this IServiceCollection services,
        string consumerName,
        string providerName,
        int? port = null)
    {
        var pactPort = port ?? PortFinder.FindFreePort();
        services.UpdateConfigsToUsePactHost(pactPort);

        services.AddSingleton(new PactSettings(Path.Join(Path.GetFullPath("../../../pact/")))
        {
            ConsumerName = consumerName, ProviderName = providerName, PactDirectory = GetPactDirectory(), Port = pactPort
        });

        services.AddScoped<IPactAccessor, PactAccessor>();

        return services;
    }

    private static string GetPactDirectory()
    {
        var pactDirectory = Environment.GetEnvironmentVariable("PACT_DIRECTORY");

        return string.IsNullOrWhiteSpace(pactDirectory) ? "../../../pact" : pactDirectory;
    }
}
