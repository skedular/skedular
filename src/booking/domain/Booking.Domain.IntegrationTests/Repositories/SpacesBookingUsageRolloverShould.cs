using Api.Shared.Services.Models;
using Booking.Shared.Repositories;
using OfferingModel = Api.Shared.Services.Models.Offering;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class SpacesBookingUsageRolloverShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Offering_State_When_Usage_Is_Count_Based(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var oldPeriodStart = new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero);
        var currentPeriodStart = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            organizationId,
            cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        organization.Offering = new OfferingModel
        {
            SpacesPlanCode = 5,
            SpacesQuotaLimit = 500,
            SpacesPeriodStart = oldPeriodStart,
            SpacesPeriodEnd = currentPeriodStart,
        };

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var updated = await repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(
            organizationId,
            cancellationToken);
        updated.ShouldNotBeNull();
        updated.Offering.ShouldNotBeNull();
        updated.Offering.SpacesPeriodStart.ShouldBe(oldPeriodStart);
        updated.Offering.SpacesPeriodEnd.ShouldBe(currentPeriodStart);
    }
}
