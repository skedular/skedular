using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using MsTeams.Shared.Configurations;

namespace MsTeams.Api.Jobs;

public class ConnectionKeepAliveJob(
    IServiceProvider serviceProvider,
    ILogger<ConnectionKeepAliveJob> logger,
    ITimeHelper timeHelper) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            await timeHelper.RandomSleepWhileStartingUpAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        do
        {
            try
            {
                await using var scope = serviceProvider.CreateAsyncScope();
                var customerConfiguration = scope.ServiceProvider.GetRequiredService<CustomerConfiguration>();
                var customerServiceClient =
                    scope.ServiceProvider.GetRequiredService<CustomerService.CustomerServiceClient>();

                await customerServiceClient.GetVersionAsync(
                    new VersionInput(),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to run job: {job}", nameof(ConnectionKeepAliveJob));
            }
        } while (true);
    }
}
