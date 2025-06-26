using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllApisJobs;

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
            Booking.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Core.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Core.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Gateway.Program.CreateHostBuilder(args).RunAsync(cancellationToken)
        );
    }
}
