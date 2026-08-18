using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models.Entitlements;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services.Entitlements;

public interface ICreditLedgerService
{
    int GetAvailableCredits(Entitlement entitlement);
    CreditLedgerEntry AddConsumption(Entitlement entitlement, string bookingId, string idempotencyKey, DateTimeOffset occurredAt);
}

public sealed class CreditLedgerService(IRandomHelper randomHelper, ILogger<CreditLedgerService> logger) : ICreditLedgerService
{
    public int GetAvailableCredits(Entitlement entitlement) =>
        entitlement.GrantedQuantity
        + entitlement.LedgerEntries
            .Where(item => item.TransactionType == CreditLedgerTransactionType.Released.ToPersistedValue() ||
                           item.TransactionType == CreditLedgerTransactionType.Adjusted.ToPersistedValue())
            .Sum(item => item.Quantity)
        - entitlement.LedgerEntries
            .Where(item => item.TransactionType == CreditLedgerTransactionType.Consumed.ToPersistedValue() ||
                           item.TransactionType == CreditLedgerTransactionType.Forfeited.ToPersistedValue() ||
                           item.TransactionType == CreditLedgerTransactionType.Expired.ToPersistedValue()).Sum(item => item.Quantity);

    public CreditLedgerEntry AddConsumption(Entitlement entitlement, string bookingId, string idempotencyKey, DateTimeOffset occurredAt)
    {
        var existing = entitlement.LedgerEntries.SingleOrDefault(item => item.ReferenceKey == idempotencyKey);
        if (existing is not null)
        {
            logger.LogInformation(
                "Credit consumption is idempotent. EntitlementId={EntitlementId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}",
                entitlement.Id, bookingId, idempotencyKey);
            return existing;
        }

        if (GetAvailableCredits(entitlement) <= 0)
        {
            throw new EntitlementCreditUnavailable();
        }

        var entry = new CreditLedgerEntry
        {
            Id = randomHelper.Generate(),
            EntitlementId = entitlement.Id,
            BookingId = bookingId,
            Quantity = 1,
            TransactionType = CreditLedgerTransactionType.Consumed.ToPersistedValue(),
            ReferenceKey = idempotencyKey,
            ActorOrSource = "booking",
            CreatedAt = occurredAt,
        };
        entitlement.LedgerEntries.Add(entry);
        logger.LogInformation(
            "Prepared entitlement credit consumption. EntitlementId={EntitlementId}, BookingId={BookingId}, IdempotencyKey={IdempotencyKey}, Quantity={Quantity}",
            entitlement.Id, bookingId, idempotencyKey, entry.Quantity);
        return entry;
    }
}
