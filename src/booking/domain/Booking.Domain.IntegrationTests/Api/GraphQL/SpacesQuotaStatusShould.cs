using Api.Shared.Services.Models;
using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;
using Booking.Shared.Repositories;
using OfferingModel = Api.Shared.Services.Models.Offering;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class SpacesQuotaStatusShould(
    ISpacesQuotaStatusQuery spacesQuotaStatusQuery,
    IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Current_Usage_And_Upgrade_Plans(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var periodStart = new DateTimeOffset(2026, 6, 1, 0, 0, 0, TimeSpan.Zero);
        var periodEnd = periodStart.AddMonths(1);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
            organizationId,
            cancellationToken);
        organization.Type = OrganizationTypeConstants.Marketplace;
        organization.Offering = new OfferingModel
        {
            SpacesPlanCode = 1,
            SpacesQuotaLimit = 100,
            SpacesPeriodStart = periodStart,
            SpacesPeriodEnd = periodEnd,
        };

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await spacesQuotaStatusQuery.ExecuteAsync(organizationId, cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var status = result.Data.ShouldNotBeNull().BookingSpacesQuotaStatus;
        status.OrganizationId.ShouldBe(organizationId);
        status.PlanCode.ShouldBe(1);
        status.CurrentUsage.ShouldBe(0);
        status.QuotaLimit.ShouldBe(100);
        status.TotalAttemptedInstanceCount.ShouldBe(0);
        status.RemainingQuota.ShouldBe(100);
        status.QuotaExceeded.ShouldBeFalse();
        status.ReasonCode.ShouldNotBeNull();
        status.ReasonCode.Type.ShouldBe(SpacesQuotaReasonCode.WithinQuota);
        status.UpgradePlans.ShouldBeEmpty();
    }
}
