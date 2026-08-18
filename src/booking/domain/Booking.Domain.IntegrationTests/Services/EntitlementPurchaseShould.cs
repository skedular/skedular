using Api.Shared.Services.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class EntitlementPurchaseShould(
    IEntitlementPurchaseService entitlementPurchaseService,
    IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task GrantOnceForConfirmedPurchase(
        string customerId,
        string organizationId,
        string purchaseReference,
        CancellationToken cancellationToken)
    {
        await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);

        var pricing = ProductPricing.Empty("entitlement-pricing") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 4,
            EntitlementValidityDays = 30,
        };

        var first = await entitlementPurchaseService.CompleteAsync(
            purchaseReference, customerId, organizationId, pricing, PaymentStatus.Confirmed,
            TimeProvider.System.GetUtcNow(), "NZD", cancellationToken);
        var second = await entitlementPurchaseService.CompleteAsync(
            purchaseReference, customerId, organizationId, pricing, PaymentStatus.Confirmed,
            TimeProvider.System.GetUtcNow(), "NZD", cancellationToken);

        first.ShouldNotBeNull();
        second.ShouldNotBeNull();
        second.Id.ShouldBe(first.Id);
        var persisted = await repositoryFactory.EntitlementRepository.GetByPurchaseReferenceAsync(purchaseReference, cancellationToken);
        persisted.ShouldNotBeNull();
        persisted.LedgerEntries.Count(item => item.TransactionType == CreditLedgerTransactionType.Granted.ToPersistedValue()).ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task RejectReservationPricingWithoutCreatingPurchaseOrBooking(CancellationToken cancellationToken)
    {
        var customerId = $"customer-{Guid.NewGuid()}";
        var organizationId = $"organization-{Guid.NewGuid()}";
        var productVersionId = $"version-{Guid.NewGuid()}";
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync(
            $"product-{Guid.NewGuid()}",
            await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken),
            cancellationToken);
        await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(productVersionId, product, cancellationToken);
        await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var pricing = ProductPricing.Empty($"reservation-pricing-{Guid.NewGuid()}") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Reservation,
            Price = 100,
        };

        await Should.ThrowAsync<InvalidOperationException>(() => entitlementPurchaseService.CreatePendingAsync(
            customerId,
            organizationId,
            productVersionId,
            pricing,
            "NZD",
            PaymentMethod.BankTransfer,
            TimeProvider.System.GetUtcNow().AddMinutes(30),
            null,
            [],
            cancellationToken));

        (await repositoryFactory.EntitlementPurchaseRepository.GetForCustomerAsync(customerId, cancellationToken)).ShouldBeEmpty();
        (await repositoryFactory.BookingRepository.GetByIdAsync($"booking-for-{productVersionId}", cancellationToken)).ShouldBeNull();
    }
}
