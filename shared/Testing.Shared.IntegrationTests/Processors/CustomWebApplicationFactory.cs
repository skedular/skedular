using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Testing.Shared.IntegrationTests.Pact;
using Xunit.DependencyInjection;

namespace Testing.Shared.IntegrationTests.Processors;

public class CustomWebApplicationFactory<TStartup>(
    ITestOutputHelperAccessor testOutputHelper,
    IServiceProvider serviceProvider)
    : WebApplicationFactory<TStartup>
    where TStartup : class
{
    private readonly IPactAccessor? _accessor = serviceProvider.GetService<IPactAccessor>();

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(collection =>
        {
            collection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.Services.AddSingleton(testOutputHelper)
                    .AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();
            });

            if (_accessor is not null)
            {
                // Update the local configs to use pact
                collection.UpdateConfigsToUsePactHost(_accessor.PactPort);
            }
        });
}
