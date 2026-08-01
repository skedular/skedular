using Booking.Api.GraphQL.Booking;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;

namespace Booking.Api.UnitTests.GraphQL.Booking.RootMutationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ResolveMarketplaceExternalRefundReconciliationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_An_Actionable_External_Reconciliation(
        [Frozen] IMarketplaceRefundOperationsService operationsService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IContext context,
        ResolveMarketplaceExternalRefundReconciliationInput input,
        MarketplaceRefundRootMutation sut,
        CancellationToken cancellationToken)
    {
        input.OrganizationId = "org-1";
        input.Provider = "STRIPE_PAYOUT";
        input.ExternalRefundId = "po_1";
        input.Status = "Resolved";
        input.Reason = "Matched to payout transaction.";
        input.ClientMutationId = "client-mutation-1";

        var reconciliation = new MarketplaceExternalRefundReconciliationModel
        {
            Id = "reconciliation-1",
            OrganizationId = input.OrganizationId,
            Provider = MarketplaceExternalRefundReconciliationProvider.StripePayout,
            ExternalRefundId = input.ExternalRefundId,
            Status = MarketplaceExternalRefundReconciliationStatus.Resolved,
            ResolutionReason = input.Reason,
            ResolutionActorCustomerId = "customer-1",
            ResolutionCorrelationId = "correlation-1"
        };

        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationAuthorizationService.CanModifyPaymentMethodAsync(
            input.OrganizationId, "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => context.GetCorrelationId()).Returns("correlation-1");
        A.CallTo(() => operationsService.ResolveExternalRefundAsync(
            input.Provider,
            input.ExternalRefundId,
            input.Status,
            input.Reason,
            input.OrganizationId,
            "customer-1",
            "correlation-1",
            cancellationToken)).Returns(reconciliation);

        var result = await sut.ResolveMarketplaceExternalRefundReconciliationAsync(
            input,
            operationsService,
            organizationAuthorizationService,
            cachedCustomerService,
            context,
            cancellationToken);

        result.ClientMutationId.ShouldBe(input.ClientMutationId);
        result.Reconciliation.Status.ShouldBe("Resolved");
        result.Reconciliation.ResolutionReason.ShouldBe(input.Reason);
        result.Reconciliation.ResolutionActorCustomerId.ShouldBe("customer-1");
        result.Reconciliation.ResolutionCorrelationId.ShouldBe("correlation-1");
        A.CallTo(() => operationsService.ResolveExternalRefundAsync(
            input.Provider,
            input.ExternalRefundId,
            input.Status,
            input.Reason,
            input.OrganizationId,
            "customer-1",
            "correlation-1",
            cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
