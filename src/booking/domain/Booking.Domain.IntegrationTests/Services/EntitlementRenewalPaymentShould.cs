using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using EntitlementPurchase = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public sealed class EntitlementRenewalPaymentShould(IRepositoryFactory repositoryFactory, IEntitlementRenewalService renewalService)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task DoesNotCreateRenewalWhenCancellationIsScheduledAtPeriodEnd(CancellationToken cancellationToken)
    {
        var key = Guid.NewGuid().ToString("N");
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync($"customer-{key}", false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync($"organization-{key}", cancellationToken);
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync($"product-{key}", organization, cancellationToken);
        var productVersion = await repositoryFactory.ProductVersionRepository.UpsertNakedAsync($"product-version-{key}", product, cancellationToken);
        var pricing = ProductPricing.Empty($"pricing-{key}") with
        {
            FulfillmentType = ProductPricingFulfillmentType.Entitlement,
            EntitlementCreditQuantity = 3,
            EntitlementValidityDays = 30,
            SupportsSubscriptionAutoRenewal = true,
        };
        productVersion.PricingOptions = [pricing];
        var sourcePurchase = repositoryFactory.EntitlementPurchaseRepository.Add(new EntitlementPurchase
        {
            Id = $"purchase-{key}",
            CustomerId = customer.Id,
            OrganizationId = organization.Id,
            ProductVersionId = productVersion.Id,
            ProductVersion = productVersion,
            PaymentStatus = PaymentStatusConstants.Confirmed,
            PaymentMethod = PaymentMethodConstants.Card,
            PaymentExpiry = TimeProvider.System.GetUtcNow().AddDays(1),
            Amount = pricing.Price,
            Currency = "NZD",
            ProductPricing = pricing,
        });
        repositoryFactory.EntitlementRepository.Add(new Entitlement
        {
            Id = $"entitlement-{key}",
            CustomerId = customer.Id,
            OrganizationId = organization.Id,
            PurchaseReference = sourcePurchase.Id,
            PricingId = pricing.Id,
            GrantedQuantity = pricing.EntitlementCreditQuantity!.Value,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-29),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(1),
            Status = EntitlementStatus.Active,
            Currency = "NZD",
            CancelAtPeriodEnd = true,
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var renewal = await renewalService.CreatePendingRenewalAsync($"entitlement-{key}", TimeProvider.System.GetUtcNow().AddHours(2),
            cancellationToken);

        renewal.ShouldBeNull();
        (await repositoryFactory.EntitlementPurchaseRepository.GetForCustomerAsync(customer.Id, cancellationToken))
            .ShouldHaveSingleItem();
    }
}
