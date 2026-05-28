using System.Text;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Enterprise.Shared.HealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Testing.Shared.IntegrationTests.Aspire;

public static class Extensions
{
    private const int RetryCount = 60;
    private static readonly TimeSpan s_defaultTimeout = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan s_retryDelay = TimeSpan.FromMilliseconds(500);

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

    extension(HttpClient httpClient)
    {
        public async Task WaitForReadinessAsync(CancellationToken cancellationToken) =>
            await HttpClient.WaitUntilAsync(
                async token =>
                {
                    using var response = await httpClient.GetAsync(Constants.ReadinessPath, token);
                    return response.IsSuccessStatusCode;
                },
                $"Resource did not become ready at '{Constants.ReadinessPath}' within {RetryCount * s_retryDelay.TotalMilliseconds}ms",
                cancellationToken);

        public async Task WaitForGraphQlAsync(CancellationToken cancellationToken) =>
            await HttpClient.WaitUntilAsync(
                async token =>
                {
                    using var request = new HttpRequestMessage(HttpMethod.Post, "/v1/graphql");
                    request.Content = new StringContent("{\"query\":\"query{__typename}\"}", Encoding.UTF8, "application/json");

                    using var response = await httpClient.SendAsync(request, token);
                    if (!response.IsSuccessStatusCode)
                    {
                        return false;
                    }

                    var responseBody = await response.Content.ReadAsStringAsync(token);
                    return !string.IsNullOrWhiteSpace(responseBody) &&
                           responseBody.Contains("__typename", StringComparison.Ordinal);
                },
                $"Resource did not serve GraphQL within {RetryCount * s_retryDelay.TotalMilliseconds}ms",
                cancellationToken);

        private static async Task WaitUntilAsync(
            Func<CancellationToken, Task<bool>> isReady,
            string timeoutMessage,
            CancellationToken cancellationToken)
        {
            try
            {
                await Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(RetryCount - 1, _ => s_retryDelay)
                    .ExecuteAsync(async token =>
                    {
                        if (!await isReady(token))
                        {
                            throw new InvalidOperationException("Resource not ready.");
                        }
                    }, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                throw new TimeoutException(timeoutMessage, exception);
            }
        }
    }
}
