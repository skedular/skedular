using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllApis;

// ReSharper disable once ClassNeverInstantiated.Global
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

        await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", ".env"), cancellationToken);
        await EnvironmentHelper.LoadEnvFileAsync(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"), cancellationToken);

        await Task.WhenAll(
            Booking.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Core.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Gateway.Program.CreateHostBuilder(args).RunAsync(cancellationToken)
        );
    }
}
