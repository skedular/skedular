using Enterprise.Shared.Configurations;

namespace AllInfra;

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
            Booking.Infrastructure.Program.MigrateAsync(Booking.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Customer.Infrastructure.Program.MigrateAsync(Customer.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Location.Infrastructure.Program.MigrateAsync(Location.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Marketplace.Infrastructure.Program.MigrateAsync(Marketplace.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            MsTeams.Infrastructure.Program.MigrateAsync(MsTeams.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Notification.Infrastructure.Program.MigrateAsync(Notification.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Organization.Infrastructure.Program.MigrateAsync(Organization.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Slack.Infrastructure.Program.MigrateAsync(Slack.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Team.Infrastructure.Program.MigrateAsync(Team.Infrastructure.Program.CreateHostBuilder(args), cancellationToken),
            Core.Infrastructure.Program.MigrateAsync(Core.Infrastructure.Program.CreateHostBuilder(args), cancellationToken)
        );
    }
}
