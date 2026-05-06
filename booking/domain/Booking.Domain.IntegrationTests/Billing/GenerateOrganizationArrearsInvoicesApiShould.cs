using Api.Shared.Clients.OpenApi.Skedular.BookingWorkaround.V1;
using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Booking.Domain.IntegrationTests.Fixtures;
using Booking.Shared.Repositories;
using Testing.Shared.IntegrationTests;

namespace Booking.Domain.IntegrationTests.Billing;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class GenerateOrganizationArrearsInvoicesApiShould(
    IBookingWorkaroundClient bookingWorkaroundClient,
    IRepositoryFactory repositoryFactory,
    InfrastructureTestService.InfrastructureTestServiceClient infrastructureTestClient,
    IEventually eventually)
{
    [Theory]
    [AutoFakeItEasyData([typeof(UpfrontArrearsTriggerScenarioFixtureCustomizer)])]
    public async Task Skip_Upfront_Bookings_When_Manual_Arrears_Run_Is_Triggered(
        UpfrontArrearsTriggerScenario scenario,
        CancellationToken cancellationToken)
    {
        await infrastructureTestClient.ResetAsync(new ResetInput(), cancellationToken: cancellationToken);
        await BillingScenarioSeeder.SeedAsync(repositoryFactory, scenario, cancellationToken);

        await bookingWorkaroundClient.GenerateOrganizationArrearsInvoicesAsync(scenario.Organization.Id, cancellationToken);

        await eventually.ConsistentlyAsync(
            async ct =>
            {
                var invoices = await repositoryFactory.OrganizationArrearsInvoiceRepository.GetByBookingIdUntrackedAsync(
                    scenario.Booking.Id,
                    ct);
                invoices.ShouldBeEmpty();
            },
            cancellationToken);
    }
}
