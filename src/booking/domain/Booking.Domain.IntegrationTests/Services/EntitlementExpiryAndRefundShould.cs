using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Entitlements;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using EntitlementPurchase = Booking.Shared.Database.Entities.EntitlementPurchase;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class EntitlementExpiryAndRefundShould(IRepositoryFactory repositoryFactory, IEntitlementExpiryService expiryService)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task CreateProviderSpecificSettlementStateForConfirmedUnusedCredits(CancellationToken cancellationToken)
    {
        foreach (var (paymentMethod, expectedRefundStatus) in new[] { ("CARD", "Requested"), ("BANK_TRANSFER", "UnderReview") })
        {
            var customerId = $"customer-{Guid.NewGuid()}";
            var organizationId = $"organization-{Guid.NewGuid()}";
            var entitlementId = $"entitlement-{Guid.NewGuid()}";
            var bookingId = $"booking-{Guid.NewGuid()}";
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
            var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
            var product = await repositoryFactory.ProductRepository.UpsertNakedAsync($"product-{bookingId}", organization, cancellationToken);
            var productVersion =
                await repositoryFactory.ProductVersionRepository.UpsertNakedAsync($"version-{bookingId}", product, cancellationToken);
            var pricing = ProductPricing.Empty($"pricing-{entitlementId}") with
            {
                FulfillmentType = ProductPricingFulfillmentType.Entitlement,
                EntitlementCreditQuantity = 2,
                EntitlementValidityDays = 10,
                Price = 100m,
                IsTaxInclusive = true,
                CancellationPolicyType = ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
                CancellationRefundRules = [new ProductPricingCancellationRefundRule(0, 100)],
            };
            repositoryFactory.EntitlementPurchaseRepository.Add(new EntitlementPurchase
            {
                Id = $"purchase-{entitlementId}",
                CustomerId = customer.Id,
                OrganizationId = organization.Id,
                ProductVersionId = productVersion.Id,
                ProductVersion = productVersion,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                PaymentMethod = paymentMethod,
                PaymentExpiry = TimeProvider.System.GetUtcNow().AddDays(1),
                Amount = 100m,
                Currency = "NZD",
                ProductPricing = pricing,
            });
            repositoryFactory.BookingRepository.Add(new BookingEntity
            {
                Id = bookingId,
                Channel = BookingChannelConstants.Marketplace,
                From = TimeProvider.System.GetUtcNow().AddDays(1),
                Until = TimeProvider.System.GetUtcNow().AddDays(1).AddHours(1),
                Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
                Schedules = [],
                InvolvedCustomers = [customer],
                InvolvedOrganizations = [organization],
                MarketplaceBooking = new MarketplaceBookingEntity
                {
                    Id = bookingId,
                    PaymentStatus = PaymentStatusConstants.Confirmed,
                    PaymentMethod = paymentMethod,
                    ProductVersion = productVersion,
                    ProductPricing = pricing,
                    Quantity = 1,
                    BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                    TotalAmount = 100m,
                    TotalAmountExcludeTax = 100m,
                    Currency = "NZD",
                },
            });
            repositoryFactory.EntitlementRepository.Add(new Entitlement
            {
                Id = entitlementId,
                CustomerId = customer.Id,
                OrganizationId = organization.Id,
                PurchaseReference = $"purchase-{entitlementId}",
                PricingId = pricing.Id,
                GrantedQuantity = 2,
                ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-10),
                ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(-1),
                Status = EntitlementStatus.Active,
                NetPurchaseAmount = 100m,
                Currency = "NZD",
            });
            repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
            {
                Id = $"grant-{entitlementId}",
                EntitlementId = entitlementId,
                Quantity = 2,
                TransactionType = CreditLedgerTransactionType.Granted.ToPersistedValue(),
                ReferenceKey = $"grant-{entitlementId}",
                CreatedAt = TimeProvider.System.GetUtcNow().AddDays(-10),
            });
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

            (await expiryService.ExpireAsync(entitlementId, cancellationToken)).ShouldBeTrue();

            var persisted = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
            var refund = persisted!.RefundLinks.Single().MarketplaceRefund!;
            refund.Status.ShouldBe(expectedRefundStatus);
            refund.RefundAmount.ShouldBe(100m);
        }
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task ExpireOnceAndRemainIdempotent(
        string customerId,
        string organizationId,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        var entitlement = repositoryFactory.EntitlementRepository.Add(new Entitlement
        {
            Id = entitlementId,
            CustomerId = customer.Id,
            OrganizationId = organization.Id,
            PurchaseReference = $"expiry-{entitlementId}",
            PricingId = "expiry-pricing",
            GrantedQuantity = 2,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-10),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            Status = EntitlementStatus.Active,
            Currency = "NZD",
        });
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
        {
            Id = $"grant-{entitlementId}",
            EntitlementId = entitlement.Id,
            Quantity = entitlement.GrantedQuantity,
            TransactionType = CreditLedgerTransactionType.Granted.ToPersistedValue(),
            ReferenceKey = $"grant-{entitlementId}",
            CreatedAt = TimeProvider.System.GetUtcNow().AddDays(-10),
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        (await expiryService.ExpireAsync(entitlementId, cancellationToken)).ShouldBeTrue();
        (await expiryService.ExpireAsync(entitlementId, cancellationToken)).ShouldBeFalse();

        var persisted = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        persisted!.Status.ShouldBe(EntitlementStatus.Expired);
        persisted.LedgerEntries.Count(item => item.TransactionType == CreditLedgerTransactionType.Forfeited.ToPersistedValue()).ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DoNotCreateRefundWithoutConfirmedPayment(
        string customerId,
        string organizationId,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        repositoryFactory.EntitlementRepository.Add(new Entitlement
        {
            Id = entitlementId,
            CustomerId = customer.Id,
            OrganizationId = organization.Id,
            PurchaseReference = $"pending-{entitlementId}",
            PricingId = "pending-pricing",
            GrantedQuantity = 1,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-10),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            Status = EntitlementStatus.Active,
            Currency = "NZD",
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        (await expiryService.ExpireAsync(entitlementId, cancellationToken)).ShouldBeTrue();
        var persisted = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        persisted!.RefundLinks.ShouldBeEmpty();
    }
}
