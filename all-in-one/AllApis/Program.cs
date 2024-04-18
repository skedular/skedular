using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Configurations;
using Microsoft.Extensions.Hosting;

namespace AllApis;

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

        await Task.WhenAll([
            Gateway.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Billing.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Booking.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Customer.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Location.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            MsTeams.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Organization.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Notification.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Payment.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Slack.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken),
            Team.Api.Program.CreateHostBuilder(args).Build().RunAsync(cancellationToken)
        ]);
    }
}
