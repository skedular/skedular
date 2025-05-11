using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllInOne;

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
            Billing.Infrastructure.Program.MigrateAsync(Billing.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Booking.Infrastructure.Program.MigrateAsync(Booking.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Customer.Infrastructure.Program.MigrateAsync(Customer.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Location.Infrastructure.Program.MigrateAsync(Location.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Marketplace.Infrastructure.Program.MigrateAsync(Marketplace.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            MsTeams.Infrastructure.Program.MigrateAsync(MsTeams.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Notification.Infrastructure.Program.MigrateAsync(Notification.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Organization.Infrastructure.Program.MigrateAsync(Organization.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Payment.Infrastructure.Program.MigrateAsync(Payment.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Slack.Infrastructure.Program.MigrateAsync(Slack.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Team.Infrastructure.Program.MigrateAsync(Team.Infrastructure.Program.CreateHostBuilder(args), cancellationToken)
        );

        await Task.WhenAll(
            Billing.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Billing.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Billing.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Booking.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Booking.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Booking.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Customer.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Location.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Marketplace.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            MsTeams.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Notification.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Organization.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Payment.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Payment.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Payment.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Slack.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Api.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Processors.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Team.Jobs.Program.CreateHostBuilder(args).RunAsync(cancellationToken),
            Gateway.Program.CreateHostBuilder(args).RunAsync(cancellationToken)
        );
    }
}
