using System.Data;
using Api.Shared.Services;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementBookingService
{
    Task<CreditLedgerEntryModel> ConsumeAsync(
        string customerId,
        string bookingId,
        string idempotencyKey,
        DateTimeOffset bookingAt,
        CancellationToken cancellationToken);

    Task<CreditLedgerEntryModel> ConsumeAsync(
        string customerId,
        string bookingId,
        string? entitlementId,
        string idempotencyKey,
        DateTimeOffset bookingAt, bool useExistingTransaction, CancellationToken cancellationToken);
}

public sealed class EntitlementBookingService(
    IEntitlementEligibilityService eligibilityService,
    ICreditLedgerService creditLedgerService,
    IRepositoryFactory repositoryFactory,
    IEntitlementModelMapper entitlementModelMapper,
    TimeProvider timeProvider,
    ILogger<EntitlementBookingService> logger,
    IDbTransactionBuilder transactionBuilder,
    IMarketplaceBookingAvailableDaysService marketplaceBookingAvailableDaysService,
    IGraphQlTopicEventSender graphQlTopicEventSender) : IEntitlementBookingService
{
    public async Task<CreditLedgerEntryModel> ConsumeAsync(
        string customerId,
        string bookingId,
        string idempotencyKey,
        DateTimeOffset bookingAt,
        CancellationToken cancellationToken)
        => await ConsumeAsync(customerId, bookingId, null, idempotencyKey, bookingAt, false, cancellationToken);

    public async Task<CreditLedgerEntryModel> ConsumeAsync(
        string customerId,
        string bookingId,
        string? entitlementId,
        string idempotencyKey,
        DateTimeOffset bookingAt,
        bool useExistingTransaction,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var transaction = useExistingTransaction
                ? null
                : await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
            var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken)
                          ?? throw new BookingNotFound();
            if (booking.InvolvedCustomers.All(customer => customer.Id != customerId))
            {
                logger.LogWarning(
                    "Credit booking rejected because the booking is not owned by the requesting customer. CustomerId={CustomerId}, BookingId={BookingId}",
                    customerId, bookingId);
                throw new UnauthorizedAccessException();
            }

            var entitlement = entitlementId is null
                ? await eligibilityService.SelectAsync(
                    customerId,
                    booking.MarketplaceBooking?.ProductPricing?.Id ?? string.Empty,
                    bookingAt,
                    cancellationToken)
                : entitlementModelMapper.Map(await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken)
                                             ?? throw new EntitlementCreditUnavailable());
            if (entitlement is null)
            {
                logger.LogInformation(
                    "No eligible entitlement found for credit booking. CustomerId={CustomerId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}",
                    customerId, bookingId, idempotencyKey);
                throw new EntitlementCreditUnavailable();
            }

            var entitlementEntity = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken)
                                    ?? throw new EntitlementCreditUnavailable();
            if (entitlementEntity.CustomerId != customerId || entitlementEntity.Status != EntitlementStatus.Active ||
                entitlementEntity.ActivatesAt > bookingAt || entitlementEntity.ExpiresAt <= bookingAt ||
                (entitlementId is not null &&
                 booking.MarketplaceBooking?.ProductPricing.Id != entitlementEntity.PricingId) ||
                creditLedgerService.GetAvailableCredits(entitlementEntity) <= 0)
            {
                throw new EntitlementCreditUnavailable();
            }

            if (booking.MarketplaceBooking is null ||
                !marketplaceBookingAvailableDaysService.IsAvailableOnBookingDate(
                    booking.MarketplaceBooking.ProductPricing,
                    DateOnly.FromDateTime(booking.From.Date)))
            {
                logger.LogWarning(
                    "Entitlement credit booking rejected because the requested weekday is not allowed. CustomerId={CustomerId}, BookingId={BookingId}, EntitlementId={EntitlementId}, BookingDate={BookingDate}",
                    customerId,
                    bookingId,
                    entitlementEntity.Id,
                    DateOnly.FromDateTime(booking.From.Date));
                throw new EntitlementCreditUnavailable();
            }

            var entry = creditLedgerService.AddConsumption(entitlementEntity, bookingId, idempotencyKey, timeProvider.GetUtcNow());
            repositoryFactory.EntitlementRepository.AddLedgerEntry(entry);
            booking.ConsumingCreditLedgerEntryId = entry.Id;
            if (booking.MarketplaceBooking is not null)
            {
                booking.MarketplaceBooking.EntitlementId = entitlementEntity.Id;
            }

            repositoryFactory.BookingRepository.Update(booking);
            await repositoryFactory.MarketplacePurchaseHistoryRepository.RefreshForEntitlementPurchaseAsync(
                entitlementEntity.PurchaseReference,
                cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.EntitlementPurchaseTopicName,
                entitlementEntity.PurchaseReference, cancellationToken);

            logger.LogInformation(
                "Consumed entitlement credit. EntitlementId={EntitlementId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}",
                entitlementEntity.Id,
                bookingId, idempotencyKey);
            return entitlementModelMapper.Map(entry);
        }
        catch (EntitlementCreditUnavailable exception)
        {
            logger.LogWarning(exception,
                "Entitlement credit unavailable or rejected. CustomerId={CustomerId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}",
                customerId, bookingId, idempotencyKey);
            throw;
        }
        catch (DbUpdateException exception)
        {
            logger.LogWarning(exception,
                "Entitlement credit claim lost a database concurrency race. CustomerId={CustomerId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}",
                customerId, bookingId, idempotencyKey);
            throw;
        }
    }
}
