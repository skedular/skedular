using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using MarketplaceBookingModificationActorKind = Booking.Shared.Models.MarketplaceBookingModificationActorKind;
using MarketplaceBookingModificationErrorCode = Booking.Shared.Models.MarketplaceBookingModificationErrorCode;
using MarketplaceBookingModificationRequest = Booking.Shared.Models.MarketplaceBookingModificationRequest;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using IdentityEntity = Booking.Shared.Database.Entities.Identity;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using EntitlementEntity = Booking.Shared.Database.Entities.Entitlement;
using CreditLedgerEntryEntity = Booking.Shared.Database.Entities.CreditLedgerEntry;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class MarketplaceBookingModificationPersistenceShould(
    IRepositoryFactory repositoryFactory,
    IServiceScopeFactory scopeFactory,
    IHttpContextAccessor httpContextAccessor,
    IContext context)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Modification_Record_After_Successful_Change(
        string bookingId,
        string customerId,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = TimeProvider.System.GetUtcNow().AddDays(1);
        var until = from.AddHours(1);
        var newFrom = from.AddDays(1);
        var newUntil = until.AddDays(1);

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, true, cancellationToken);
        customer.Identities.Add(new IdentityEntity
        {
            Id = customerId,
            Customer = customer,
        });
        SetCustomerContext(customerId);
        var productVersion = await CreateProductVersionAsync(bookingId, cancellationToken);
        var booking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = 1,
            From = from,
            Until = until,
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = bookingId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductVersion = productVersion,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 0,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            },
            InvolvedCustomers = [customer],
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Act
        await using var scope = scopeFactory.CreateAsyncScope();
        var scopedRepositories = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var marketplaceBookingService = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingService>();

        var request = new MarketplaceBookingModificationRequest(
            bookingId,
            booking.EntityFrameworkVersion,
            newFrom,
            newUntil,
            null,
            "Customer requested change",
            customerId,
            MarketplaceBookingModificationActorKind.Customer);

        var result = await marketplaceBookingService.ModifyAsync(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue(result.Error?.Message ?? "Modification failed without an error.");

        var modification = (await scopedRepositories.MarketplaceBookingModificationRepository
            .GetByBookingIdAsync(bookingId, cancellationToken)).Single();
        modification.ShouldNotBeNull();
        modification.BookingId.ShouldBe(bookingId);
        modification.ActorCustomerId.ShouldBe(customerId);
        modification.ActorKind.ToMarketplaceBookingModificationActorKind().ShouldBe(MarketplaceBookingModificationActorKind.Customer);
        modification.Reason.ShouldBe("Customer requested change");
        modification.OriginalFrom.ShouldBe(from);
        modification.OriginalUntil.ShouldBe(until);
        modification.ResultFrom.ShouldBe(newFrom);
        modification.ResultUntil.ShouldBe(newUntil);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Stale_Version_In_Concurrent_Modification_Scenario(
        string bookingId,
        string customerId,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = TimeProvider.System.GetUtcNow().AddDays(1);
        var until = from.AddHours(1);

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, true, cancellationToken);
        customer.Identities.Add(new IdentityEntity
        {
            Id = customerId,
            Customer = customer,
        });
        SetCustomerContext(customerId);
        var productVersion = await CreateProductVersionAsync(bookingId, cancellationToken);
        var booking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = 1,
            From = from,
            Until = until,
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = bookingId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductVersion = productVersion,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 0,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            },
            InvolvedCustomers = [customer],
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Act - First modification succeeds
        await using var firstScope = scopeFactory.CreateAsyncScope();
        var firstMarketplaceBookingService = firstScope.ServiceProvider.GetRequiredService<IMarketplaceBookingService>();

        var firstRequest = new MarketplaceBookingModificationRequest(
            bookingId,
            booking.EntityFrameworkVersion, // Correct version
            from.AddDays(1),
            until.AddDays(1),
            null,
            "First change",
            customerId,
            MarketplaceBookingModificationActorKind.Customer);

        var firstResult = await firstMarketplaceBookingService.ModifyAsync(firstRequest, cancellationToken);
        firstResult.Succeeded.ShouldBeTrue(firstResult.Error?.Message ?? "Modification failed without an error.");

        // Second modification with stale version fails
        await using var secondScope = scopeFactory.CreateAsyncScope();
        var secondMarketplaceBookingService = secondScope.ServiceProvider.GetRequiredService<IMarketplaceBookingService>();

        var secondRequest = new MarketplaceBookingModificationRequest(
            bookingId,
            booking.EntityFrameworkVersion, // Stale version after the first change
            from.AddDays(2),
            until.AddDays(2),
            null,
            "Second change",
            customerId,
            MarketplaceBookingModificationActorKind.Customer);

        var secondResult = await secondMarketplaceBookingService.ModifyAsync(secondRequest, cancellationToken);

        // Assert
        secondResult.Succeeded.ShouldBeFalse();
        secondResult.Error.ShouldNotBeNull();
        secondResult.Error.Code.ShouldBe(MarketplaceBookingModificationErrorCode.StaleVersion);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Commercial_State_During_Modification(
        string bookingId,
        string customerId,
        CancellationToken cancellationToken)
    {
        // Arrange
        var from = TimeProvider.System.GetUtcNow().AddDays(1);
        var until = from.AddHours(1);
        var originalPrice = 100m;

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, true, cancellationToken);
        customer.Identities.Add(new IdentityEntity
        {
            Id = customerId,
            Customer = customer,
        });
        SetCustomerContext(customerId);
        var productVersion = await CreateProductVersionAsync(bookingId, cancellationToken);
        var booking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = 1,
            From = from,
            Until = until,
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = bookingId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductVersion = productVersion,
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 0,
                    Price = originalPrice,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
                TotalAmount = originalPrice,
            },
            InvolvedCustomers = [customer],
        });

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        // Act
        await using var scope = scopeFactory.CreateAsyncScope();
        var marketplaceBookingService = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingService>();

        var request = new MarketplaceBookingModificationRequest(
            bookingId,
            booking.EntityFrameworkVersion,
            from.AddDays(1),
            until.AddDays(1),
            null,
            "Time change only",
            customerId,
            MarketplaceBookingModificationActorKind.Customer);

        var result = await marketplaceBookingService.ModifyAsync(request, cancellationToken);

        // Assert
        result.Succeeded.ShouldBeTrue(result.Error?.Message ?? "Modification failed without an error.");

        var updatedBooking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        updatedBooking.ShouldNotBeNull();
        updatedBooking.MarketplaceBooking.ShouldNotBeNull();
        updatedBooking.MarketplaceBooking.TotalAmount.ShouldBe(originalPrice);
        updatedBooking.MarketplaceBooking.ProductPricing.Price.ShouldBe(originalPrice);
        updatedBooking.MarketplaceBooking.Quantity.ShouldBe(1);
        updatedBooking.MarketplaceBooking.PaymentStatus.ShouldBe(PaymentStatusConstants.Confirmed);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_Token_Modification_And_Restore_Credit_On_Cancellation(
        string bookingId,
        string customerId,
        string entitlementId,
        CancellationToken cancellationToken)
    {
        var from = TimeProvider.System.GetUtcNow().AddDays(1);
        var until = from.AddHours(1);
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, true, cancellationToken);
        customer.Identities.Add(new IdentityEntity
        {
            Id = customerId,
            Customer = customer,
        });
        SetCustomerContext(customerId);
        var productVersion = await CreateProductVersionAsync($"token-{bookingId}", cancellationToken);
        var entitlement = repositoryFactory.EntitlementRepository.Add(new EntitlementEntity
        {
            Id = entitlementId,
            CustomerId = customerId,
            OrganizationId = productVersion.Product.Organization.Id,
            PurchaseReference = $"purchase-{entitlementId}",
            PricingId = "token-pricing",
            GrantedQuantity = 5,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(30),
            Status = EntitlementStatus.Active,
            Currency = CurrencyConstants.Nzd,
        });
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntryEntity
        {
            Id = $"grant-{entitlementId}",
            EntitlementId = entitlement.Id,
            Quantity = 5,
            TransactionType = CreditLedgerTransactionType.Granted.ToPersistedValue(),
            ReferenceKey = $"grant-{entitlementId}",
            CreatedAt = TimeProvider.System.GetUtcNow().AddDays(-1),
        });
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntryEntity
        {
            Id = $"consume-{bookingId}",
            EntitlementId = entitlement.Id,
            BookingId = bookingId,
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
            ReferenceKey = $"booking:{bookingId}",
            CreatedAt = TimeProvider.System.GetUtcNow(),
        });
        var booking = repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            EntityFrameworkVersion = 1,
            From = from,
            Until = until,
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                Id = bookingId,
                EntitlementId = entitlementId,
                PaymentStatus = PaymentStatusConstants.Confirmed,
                ProductVersion = productVersion,
                ProductPricing = ProductPricing.Empty("token-pricing") with
                {
                    FulfillmentType = ProductPricingFulfillmentType.Entitlement,
                    EntitlementCreditQuantity = 1,
                    PurchaseCadence = ProductPricingCadence.Daily,
                    NumberOfResourcesToBook = 0,
                },
                Quantity = 1,
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.Upfront.ToProductPricingBillingMode(),
            },
            InvolvedCustomers = [customer],
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        await using var scope = scopeFactory.CreateAsyncScope();
        var marketplaceBookingService = scope.ServiceProvider.GetRequiredService<IMarketplaceBookingService>();
        var cancellationService = scope.ServiceProvider.GetRequiredService<IEntitlementCancellationService>();
        var request = new MarketplaceBookingModificationRequest(
            bookingId,
            booking.EntityFrameworkVersion,
            from.AddDays(1),
            until.AddDays(1),
            null,
            "Token booking time change",
            customerId,
            MarketplaceBookingModificationActorKind.Customer);

        var modification = await marketplaceBookingService.ModifyAsync(request, cancellationToken);
        modification.Succeeded.ShouldBeTrue(modification.Error?.Message ?? "Token modification failed without an error.");

        var restored = await cancellationService.CancelBookingAsync(bookingId, true, "Customer cancelled token booking", cancellationToken);
        restored.ShouldNotBeNull();
        restored!.TransactionType.ShouldBe(CreditLedgerTransactionType.Released);
        (await scope.ServiceProvider.GetRequiredService<IRepositoryFactory>().MarketplaceBookingModificationRepository
            .GetByBookingIdAsync(bookingId, cancellationToken)).ShouldHaveSingleItem();
    }

    private async Task<ProductVersionEntity> CreateProductVersionAsync(string key, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync($"organization-{key}", cancellationToken);
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync($"product-{key}", organization, cancellationToken);
        var productVersion = await repositoryFactory.ProductVersionRepository.UpsertNakedAsync($"product-version-{key}", product, cancellationToken);
        productVersion.Currency = CurrencyConstants.Nzd;
        var productTag = await repositoryFactory.OrganizationTagRepository.UpsertNakedAsync($"product-tag-{key}", organization, cancellationToken);
        productTag.Type = OrganizationTagTypeConstants.Product;
        productVersion.OrganizationTags.Add(productTag);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return productVersion;
    }

    private void SetCustomerContext(string customerId)
    {
        httpContextAccessor.HttpContext = new DefaultHttpContext();
        context.SetVerifiableToken(customerId);
    }
}
