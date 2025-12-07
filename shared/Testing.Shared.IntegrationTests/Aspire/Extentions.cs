using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Testing.Shared.IntegrationTests.Aspire;

public static class Extensions
{
    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromMinutes(1);

    extension(IDistributedApplicationTestingBuilder builder)
    {
        public IDistributedApplicationTestingBuilder AddDefaultServices()
        {
            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Debug);

                // Override the logging filters from the app's configuration
                logging.AddFilter(builder.Environment.ApplicationName, LogLevel.Debug);
                logging.AddFilter("Aspire.", LogLevel.Debug);
            });
            builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
            {
                clientBuilder.AddStandardResilienceHandler();
            });

            return builder;
        }

        public async Task<DistributedApplication> StartAsync(CancellationToken cancellationToken)
        {
            var distributedApplication = await builder.BuildAsync(cancellationToken).WaitAsync(s_defaultTimeout, cancellationToken);

            await distributedApplication.StartAsync(cancellationToken).WaitAsync(s_defaultTimeout, cancellationToken);

            return distributedApplication;
        }
    }
}
