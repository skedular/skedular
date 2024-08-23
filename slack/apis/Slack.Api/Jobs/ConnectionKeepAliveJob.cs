using Api.Shared.Services.Grpc.UnityHub.Billing.V1;
using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Grpc.UnityHub.Notification.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Api.Shared.Services.Grpc.UnityHub.Payment.V1;
using Api.Shared.Services.Grpc.UnityHub.Team.V1;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using Slack.Shared.Configurations;
using VersionInput = Api.Shared.Services.Grpc.UnityHub.Billing.V1.VersionInput;

namespace Slack.Api.Jobs;

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
                var billingConfiguration = scope.ServiceProvider.GetRequiredService<BillingConfiguration>();
                var billingServiceClient =
                    scope.ServiceProvider.GetRequiredService<BillingService.BillingServiceClient>();

                var bookingConfiguration = scope.ServiceProvider.GetRequiredService<BookingConfiguration>();
                var bookingServiceClient =
                    scope.ServiceProvider.GetRequiredService<BookingService.BookingServiceClient>();

                var customerConfiguration = scope.ServiceProvider.GetRequiredService<CustomerConfiguration>();
                var customerServiceClient =
                    scope.ServiceProvider.GetRequiredService<CustomerService.CustomerServiceClient>();

                var locationConfiguration = scope.ServiceProvider.GetRequiredService<LocationConfiguration>();
                var locationServiceClient =
                    scope.ServiceProvider.GetRequiredService<LocationService.LocationServiceClient>();

                var notificationConfiguration = scope.ServiceProvider.GetRequiredService<NotificationConfiguration>();
                var notificationServiceClient =
                    scope.ServiceProvider.GetRequiredService<NotificationService.NotificationServiceClient>();

                var organizationConfiguration = scope.ServiceProvider.GetRequiredService<OrganizationConfiguration>();
                var organizationServiceClient =
                    scope.ServiceProvider.GetRequiredService<OrganizationService.OrganizationServiceClient>();

                var paymentConfiguration = scope.ServiceProvider.GetRequiredService<PaymentConfiguration>();
                var paymentServiceClient =
                    scope.ServiceProvider.GetRequiredService<PaymentService.PaymentServiceClient>();

                var teamConfiguration = scope.ServiceProvider.GetRequiredService<TeamConfiguration>();
                var teamServiceClient = scope.ServiceProvider.GetRequiredService<TeamService.TeamServiceClient>();

                await billingServiceClient.GetVersionAsync(
                    new VersionInput(),
                    billingConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await bookingServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.VersionInput(),
                    bookingConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await customerServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Customer.V1.VersionInput(),
                    customerConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await locationServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Location.V1.VersionInput(),
                    locationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await notificationServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Notification.V1.VersionInput(),
                    notificationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await organizationServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.VersionInput(),
                    organizationConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await paymentServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Payment.V1.VersionInput(),
                    paymentConfiguration.ApiKey.CreateMetadata(),
                    cancellationToken: cancellationToken);

                await teamServiceClient.GetVersionAsync(
                    new global::Api.Shared.Services.Grpc.UnityHub.Team.V1.VersionInput(),
                    teamConfiguration.ApiKey.CreateMetadata(),
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
