using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllJobs;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program : WebHostServiceBase<Program>
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

        await EnvironmentHelper.LoadEnvFileAsync(
            Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", ".env"),
            cancellationToken);

        await Task.WhenAll(
            Billing.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Marketplace.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Jobs.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken)
        );
    }
}
