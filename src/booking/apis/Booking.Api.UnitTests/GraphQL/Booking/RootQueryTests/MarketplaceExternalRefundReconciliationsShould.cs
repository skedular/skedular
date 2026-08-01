using Booking.Api.GraphQL.Booking;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Pagination;
using ReconciliationModel = Booking.Shared.Models.MarketplaceExternalRefundReconciliationModel;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceExternalRefundReconciliationsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Expose_Organization_Scoped_Filtered_Connection(
        [Frozen] IMarketplaceRefundOperationsService operationsService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        CancellationToken cancellationToken)
    {
        var organization = new OrganizationEntity { Id = "org-1" };
        var reconciliation = new ReconciliationModel
        {
            Id = "reconciliation-1",
            OrganizationId = organization.Id,
            Provider = MarketplaceExternalRefundReconciliationProvider.StripePayout,
            ExternalRefundId = "po_1",
            Status = MarketplaceExternalRefundReconciliationStatus.Open,
            FirstSeenAt = TimeProvider.System.GetUtcNow(),
            LastSeenAt = TimeProvider.System.GetUtcNow()
        };
        var pagination = new PaginationInputParam("after-1", 10, null, null);
        var providerEdge = (reconciliation, "cursor-1");

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(null, "example", cancellationToken))
            .Returns(organization);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(
            organization.Id, "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => operationsService.GetExternalRefundsAsync(
                organization.Id,
                "STRIPE_PAYOUT",
                "Open",
                A<PaginationInputParam>.That.Matches(value =>
                    value.After == pagination.After && value.First == pagination.First &&
                    value.Before == pagination.Before && value.Last == pagination.Last),
                cancellationToken))
            .Returns((new PaginatedInfo(true, true, "cursor-1", "cursor-1"),
                new[] { providerEdge },
                1));

        var result = await new MarketplaceRefundRootQuery().MarketplaceExternalRefundReconciliationsAsync(
            "example",
            pagination.After,
            pagination.First,
            pagination.Before,
            pagination.Last,
            "STRIPE_PAYOUT",
            "Open",
            operationsService,
            organizationAuthorizationService,
            cachedOrganizationService,
            cachedCustomerService,
            cancellationToken);

        result.TotalCount.ShouldBe(1);
        result.PageInfo.HasNextPage.ShouldBeTrue();
        result.Edges.Single().Node.ExternalRefundId.ShouldBe("po_1");
        result.Edges.Single().Node.Provider.ShouldBe("STRIPE_PAYOUT");
        A.CallTo(() => operationsService.GetExternalRefundsAsync(
                organization.Id,
                "STRIPE_PAYOUT",
                "Open",
                A<PaginationInputParam>.That.Matches(value =>
                    value.After == pagination.After && value.First == pagination.First &&
                    value.Before == pagination.Before && value.Last == pagination.Last),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
