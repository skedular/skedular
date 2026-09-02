using System.Data;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.UnitTests.Services.Entitlements.EntitlementBookingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ConsumeAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task LinkBookingToConsumedLedgerAndEntitlement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IEntitlementEligibilityService entitlementEligibilityService,
        [Frozen]
        ICreditLedgerService creditLedgerService,
        [Frozen]
        IDbTransactionBuilder dbTransactionBuilder,
        [Frozen]
        IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
        [Frozen]
        IEntitlementRepository entitlementRepository,
        [Frozen]
        IBookingRepository bookingRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction dbContextTransaction,
        [Frozen]
        ILogger<EntitlementBookingService> logger,
        EntitlementBookingService sut,
        CancellationToken cancellationToken)
    {
        var entitlement = new Entitlement
        {
            Id = "entitlement-1",
            Organization = new Organization
            {
                CustomDomain = "test",
            },
            CustomerId = "customer-1",
            GrantedQuantity = 1,
            Status = EntitlementStatus.Active,
            ActivatesAt = TimeProvider.System.GetUtcNow().AddHours(-1),
            ExpiresAt = TimeProvider.System.GetUtcNow().AddHours(1),
        };
        var booking = new Database.Entities.Booking
        {
            Id = "booking-1",
            MarketplaceBooking = new MarketplaceBooking(),
            InvolvedCustomers =
            [
                new Customer
                {
                    Id = "customer-1",
                },
            ],
        };
        var pricing = ProductPricing.Empty("entitlement") with
        {
            RequiredDaysPerWeek = 1,
        };
        var entry = new CreditLedgerEntry
        {
            Id = "entry-1",
            EntitlementId = entitlement.Id,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
        };
        A.CallTo(() => marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(A<ProductPricing>._, A<DateOnly>._)).Returns(true);
        A.CallTo(() => repositoryFactory.EntitlementRepository).Returns(entitlementRepository);
        A.CallTo(() => repositoryFactory.BookingRepository).Returns(bookingRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => bookingRepository.GetByIdAsync("booking-1", cancellationToken)).Returns(booking);
        A.CallTo(() => entitlementEligibilityService.SelectAsync("customer-1", A<string>._, A<DateTimeOffset>._, cancellationToken))
            .Returns(new EntitlementModel
            {
                Id = entitlement.Id,
                CustomerId = entitlement.CustomerId,
                ProductPricing = pricing,
                ActivatesAt = entitlement.ActivatesAt,
                ExpiresAt = entitlement.ExpiresAt,
                Status = entitlement.Status,
            });
        A.CallTo(() => entitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)).Returns(entitlement);
        A.CallTo(() => entitlementRepository.CountSuccessfulRedemptionsAsync(
                entitlement.Id, A<DateTimeOffset>._, A<DateTimeOffset>._, cancellationToken))
            .Returns(0);
        A.CallTo(() => creditLedgerService.GetAvailableCredits(entitlement)).Returns(1);
        A.CallTo(() => creditLedgerService.AddConsumption(entitlement, "booking-1", "key-1", A<DateTimeOffset>._)).Returns(entry);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => dbTransactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken))
            .Returns(dbContextTransaction);

        await sut.ConsumeAsync("customer-1", "booking-1", "key-1", TimeProvider.System.GetUtcNow(), cancellationToken);

        Assert.Equal(entry.Id, booking.ConsumingCreditLedgerEntryId);
        Assert.Equal(entitlement.Id, booking.MarketplaceBooking!.EntitlementId);
        A.CallTo(() => bookingRepository.Update(booking)).MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call => call.Method.Name == nameof(ILogger.Log) && call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
