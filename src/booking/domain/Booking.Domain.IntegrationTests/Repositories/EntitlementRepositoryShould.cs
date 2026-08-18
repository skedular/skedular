using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using BookingEntity = Booking.Shared.Database.Entities.Booking;

namespace Booking.Domain.IntegrationTests.Repositories;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class EntitlementRepositoryShould(IRepositoryFactory repositoryFactory)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Grant_By_Unique_Purchase_Reference(
        string customerId,
        string organizationId,
        string entitlementId,
        string purchaseReference,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        repositoryFactory.EntitlementRepository.Add(new Entitlement
        {
            Id = entitlementId,
            CustomerId = customer.Id,
            Customer = customer,
            OrganizationId = organization.Id,
            Organization = organization,
            PurchaseReference = purchaseReference,
            PricingId = $"pricing-{entitlementId}",
            GrantedQuantity = 3,
            Currency = "NZD",
            ActivatesAt = TimeProvider.System.GetUtcNow().AddDays(-1),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddDays(30),
            Status = EntitlementStatus.Active,
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.EntitlementRepository.GetByPurchaseReferenceAsync(purchaseReference, cancellationToken);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entitlementId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Active_Entitlements_In_Earliest_Expiry_Order(
        string customerId,
        string organizationId,
        string firstId,
        string secondId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        var now = TimeProvider.System.GetUtcNow();
        repositoryFactory.EntitlementRepository.Add(Create(customer, organization, firstId, now.AddDays(2)));
        repositoryFactory.EntitlementRepository.Add(Create(customer, organization, secondId, now.AddDays(5)));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.EntitlementRepository.GetActiveForCustomerAsync(customerId, now, cancellationToken);

        result.Select(item => item.Id).ShouldBe([firstId, secondId]);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Only_Expired_Active_Entitlements(
        string customerId,
        string organizationId,
        string expiredId,
        string futureId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        var now = TimeProvider.System.GetUtcNow();
        repositoryFactory.EntitlementRepository.Add(Create(customer, organization, expiredId, now.AddMinutes(-1)));
        repositoryFactory.EntitlementRepository.Add(Create(customer, organization, futureId, now.AddDays(1)));
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.EntitlementRepository.GetExpiredActiveAsync(now, cancellationToken);

        result.Select(item => item.Id).ShouldContain(expiredId);
        result.Select(item => item.Id).ShouldNotContain(futureId);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_Consumed_Credit_By_Booking(
        string customerId,
        string organizationId,
        string entitlementId,
        string bookingId,
        CancellationToken cancellationToken)
    {
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(customerId, false, cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(organizationId, cancellationToken);
        repositoryFactory.EntitlementRepository.Add(Create(customer, organization, entitlementId, TimeProvider.System.GetUtcNow().AddDays(1)));
        repositoryFactory.BookingRepository.Add(new BookingEntity
        {
            Id = bookingId,
            Channel = BookingChannelConstants.Marketplace,
            From = TimeProvider.System.GetUtcNow().AddDays(1),
            Until = TimeProvider.System.GetUtcNow().AddDays(1).AddHours(1),
            Category = BookingCategory.WorkingFromCoworkingSpace.ToBookingCategory(),
            Schedules = [],
        });
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
        {
            Id = $"consumed-{bookingId}",
            EntitlementId = entitlementId,
            BookingId = bookingId,
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
            ReferenceKey = $"booking:{bookingId}",
            CreatedAt = TimeProvider.System.GetUtcNow(),
        });
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);

        var result = await repositoryFactory.EntitlementRepository.GetConsumedByBookingIdAsync(bookingId, cancellationToken);

        result.ShouldNotBeNull();
        result!.EntitlementId.ShouldBe(entitlementId);
        result.Entitlement.LedgerEntries.ShouldContain(item => item.ReferenceKey == $"booking:{bookingId}");
    }

    private static Entitlement Create(Customer customer, Organization organization, string id, DateTimeOffset expiresAt) => new()
    {
        Id = id,
        CustomerId = customer.Id,
        Customer = customer,
        OrganizationId = organization.Id,
        Organization = organization,
        PurchaseReference = $"purchase-{id}",
        PricingId = $"pricing-{id}",
        GrantedQuantity = 1,
        Currency = "NZD",
        ActivatesAt = expiresAt.AddDays(-10),
        ExpiresAt = expiresAt,
        Status = EntitlementStatus.Active,
    };
}
