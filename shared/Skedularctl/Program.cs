using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skedularctl.Services;
using Skedularctl.Services.Sharedspaces;

namespace Skedularctl;

public static class Program
{
    public static async Task Main(string[] args)
    {
        args = ["crawl-sharedspace"];

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
            .ParseArguments<ProtobufEventMetadataGenerateOptions, CrawlOptions>(args)
            .MapResult(
                async (ProtobufEventMetadataGenerateOptions options) =>
                {
                    var handler = serviceProvider.GetRequiredService<IProtobufEventMetadataGenerateService>();
                    await handler.HandleAsync(options, cancellationToken);
                },
                async (CrawlOptions options) =>
                {
                    var handler = serviceProvider.GetRequiredService<ICrawlerService>();
                    await handler.HandleAsync(options, cancellationToken);
                },
                _ => Task.CompletedTask
            );
    }

    private static IHost CreateHost(string[] args) =>
        Host.CreateDefaultBuilder(args).ConfigureServices((_, services) =>
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);

                builder.Services
                    .AddSingleton<IProtobufEventMetadataGenerateService, ProtobufEventMetadataGenerateService>()
                    .AddSingleton<IPlaywrightProvider, PlaywrightProvider>()
                    .AddSingleton<ICrawlerService, CrawlerService>()
                    .AddSingleton<ILocationsCrawler, LocationsCrawler>();
            });
        }).Build();
}
