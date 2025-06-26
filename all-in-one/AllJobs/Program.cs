using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllJobs;

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
            Booking.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Core.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken)
        );
    }
}
