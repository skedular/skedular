using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllProcessors;

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

        await Task.WhenAll(Billing.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Processors.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken));
    }
}
