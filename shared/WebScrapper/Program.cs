using CommandLine;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Random;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebScrapper.Services;
using WebScrapper.Sharedspaces;

namespace WebScrapper;

public class Program
{
    public static async Task Main(string[] args)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        Console.CancelKeyPress += (_, eventArgs) =>
        {
            cancellationTokenSource.Cancel();
            eventArgs.Cancel = true;
        };

        using var host = CreateHost(args);
        var serviceProvider = host.Services;

        await Parser.Default
            .ParseArguments<ImportOptions, CrawlSharedspacesOptions>(args)
            .MapResult(
                async (ImportOptions options) =>
                {
                    var handler = serviceProvider.GetRequiredService<ILocationService>();
                    await handler.HandleAsync(options, cancellationToken);
                },
                async (CrawlSharedspacesOptions options) =>
                {
                    var handler = serviceProvider.GetRequiredService<ICrawlerService>();
                    await handler.HandleAsync(options, cancellationToken);
                },
                _ => Task.CompletedTask
            );
    }

    private static IHost CreateHost(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostBuilderContext, configuration) =>
                configuration.BuildConfig<Program>(hostBuilderContext.HostingEnvironment.EnvironmentName, args))
            .ConfigureServices((hostBuilderContext, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);

                    builder.Services
                        .AddSingleton(new SharedSpacesConfiguration())
                        .AddSingleton<IRandomHelper, RandomHelper>()
                        .AddSingleton<IPlaywrightProvider, PlaywrightProvider>()
                        .AddSingleton<ICrawlerService, CrawlerService>()
                        .AddSingleton<ILocationsCrawlerService, LocationsCrawlerServiceService>()
                        .AddSingleton<ILocationCrawlerService, LocationCrawlerServiceService>()
                        .AddSingleton<IContentEnricherService, ContentEnricherService>()
                        .AddSingleton<ILocationService, LocationService>()
                        .AddSingleton<ICsvLocationFileReaderService, CsvLocationFileReaderService>()
                        .AddDomainSharedGrpcClients(hostBuilderContext.Configuration);
                });
            }).Build();
}
