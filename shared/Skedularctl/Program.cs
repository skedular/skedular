using CommandLine;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Skedularctl.Services;

namespace Skedularctl;

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
            .ParseArguments<ProtobufEventMetadataGenerateOptions>(args)
            .MapResult(
                async options =>
                {
                    var handler = serviceProvider.GetRequiredService<IProtobufEventMetadataGenerateService>();
                    await handler.HandleAsync(options, cancellationToken);
                },
                _ => Task.CompletedTask
            );
    }

    private static IHost CreateHost(string[] args) =>
        Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, configuration) => configuration.BuildConfig<Program>(_.HostingEnvironment.EnvironmentName, args))
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);

                    builder.Services
                        .AddSingleton<IProtobufEventMetadataGenerateService, ProtobufEventMetadataGenerateService>();
                });
            }).Build();
}
