using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllProcessors;

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
            Booking.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Payment.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Core.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken)
        );
    }
}
