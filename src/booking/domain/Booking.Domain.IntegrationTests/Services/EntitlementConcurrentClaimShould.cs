using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Services;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class EntitlementConcurrentClaimShould(
    IRepositoryFactory repositoryFactory,
    IServiceScopeFactory scopeFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task LeaveBookingUnlinkedWhenCreditClaimFails(
        string customerId,
        string bookingId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var productVersionId = await AddProductVersionAsync($"product-{bookingId}", $"organization-{bookingId}", cancellationToken);
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
        AddBooking(bookingId, customer, productVersion!);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        (await TryConsumeAsync(bookingId, customerId, cancellationToken)).ShouldBeFalse();

        var persisted = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken);
        persisted.ShouldNotBeNull();
        persisted.ConsumingCreditLedgerEntryId.ShouldBeNull();
        persisted.MarketplaceBooking?.EntitlementId.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task AllowOnlyOneClaimForTheFinalCredit(
        string customerId,
        string organizationId,
        string entitlementId,
        string firstBookingId,
        string secondBookingId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        var productVersionId = await AddProductVersionAsync($"product-{entitlementId}", organizationId, cancellationToken);
        repositoryFactory.EntitlementRepository.Add(new Entitlement
        {
            Id = entitlementId,
            CustomerId = customer.Id,
            OrganizationId = organization.Id,
            PurchaseReference = $"concurrency-{entitlementId}",
            PricingId = "concurrency-pricing",
            GrantedQuantity = 1,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(30),
            Status = EntitlementStatus.Active,
            Currency = "NZD",
        });
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
        {
            Id = $"grant-{entitlementId}",
            EntitlementId = entitlementId,
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Granted.ToPersistedValue(),
            ReferenceKey = $"grant-{entitlementId}",
            CreatedAt = TimeProvider.System.GetUtcNow().AddDays(-1),
        });
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersionId, cancellationToken);
        AddBooking(firstBookingId, customer, productVersion!);
        AddBooking(secondBookingId, customer, productVersion!);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var results = await Task.WhenAll(
            TryConsumeAsync(firstBookingId, customerId, cancellationToken),
            TryConsumeAsync(secondBookingId, customerId, cancellationToken));

        results.Count(result => result).ShouldBe(1);
        results.Count(result => !result).ShouldBe(1);
        var persisted = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        persisted!.LedgerEntries.Count(item => item.TransactionType == CreditLedgerTransactionType.Consumed.ToPersistedValue()).ShouldBe(1);
    }

    private async Task<string> AddProductVersionAsync(string productId, string organizationId, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        var product = await repositoryFactory.ProductRepository.UpsertNakedAsync(productId, organization, cancellationToken);
        var productVersionId = $"version-{productId}";
        await repositoryFactory.ProductVersionRepository.UpsertNakedAsync(productVersionId, product, cancellationToken);
        return productVersionId;
    }

    private void AddBooking(string bookingId, Customer customer, ProductVersion productVersion) => repositoryFactory.BookingRepository.Add(
        new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            From = TimeProvider.System.GetUtcNow().AddDays(1),
            Until = TimeProvider.System.GetUtcNow().AddDays(1).AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
            InvolvedCustomers = [customer],
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = $"{bookingId}-marketplace",
                PaymentMethod = PaymentMethod.Card.ToPaymentMethod(),
                BillingMode = ProductPricingBillingMode.NotSet.ToProductPricingBillingMode(),
                ProductVersion = productVersion,
                ProductPricing = new ProductPricing(
                    "concurrency-pricing",
                    0,
                    ListingMetadata.Empty,
                    ProductPricingCadence.NotSet,
                    0,
                    false,
                    false,
                    [],
                    ProductPricingBillingMode.NotSet,
                    null,
                    null,
                    0,
                    0,
                    1,
                    ProductPricingCancellationPolicyType.NotSet,
                    []),
            },
        });

    private async Task<bool> TryConsumeAsync(string bookingId, string customerId, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var scopedRepositories = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
        var ledger = new CreditLedgerService(
            scope.ServiceProvider.GetRequiredService<IRandomHelper>(),
            scope.ServiceProvider.GetRequiredService<ILogger<CreditLedgerService>>());
        var mapper = scope.ServiceProvider.GetRequiredService<IEntitlementModelMapper>();
        var eligibility = new EntitlementEligibilityService(scopedRepositories, ledger, mapper);
        var service = new EntitlementBookingService(
            eligibility,
            ledger,
            scopedRepositories,
            mapper,
            scope.ServiceProvider.GetRequiredService<TimeProvider>(),
            scope.ServiceProvider.GetRequiredService<ILogger<EntitlementBookingService>>(),
            scope.ServiceProvider.GetRequiredService<IDbTransactionBuilder>(),
            scope.ServiceProvider.GetRequiredService<IMarketplaceBookingAvailableDaysService>(),
            scope.ServiceProvider.GetRequiredService<IGraphQlTopicEventSender>());
        try
        {
            await service.ConsumeAsync(customerId, bookingId, $"concurrent:{bookingId}", TimeProvider.System.GetUtcNow().AddDays(1),
                cancellationToken);
            return true;
        }
        catch (EntitlementCreditUnavailable)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
        catch (InvalidOperationException exception) when (exception.InnerException is DbUpdateException)
        {
            // Npgsql's execution strategy wraps PostgreSQL serialization failures
            // raised by the serializable transaction in InvalidOperationException.
            return false;
        }
        catch (Exception exception) when (exception.Message.Contains("40001", StringComparison.Ordinal))
        {
            return false;
        }
    }
}
