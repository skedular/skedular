using System.Data;
using Api.Shared.Services;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Random;
using Microsoft.Extensions.Logging;
using Constants = Booking.Shared.GraphQL.Constants;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementAdjustmentService
{
    Task<CreditLedgerEntryModel> AdjustAsync(string entitlementId, int quantity, string actorCustomerId, string reason,
        CancellationToken cancellationToken);

    Task<CreditLedgerEntryModel> AdjustAsync(string entitlementId, int quantity, string actorCustomerId, string reason,
        string idempotencyKey, CancellationToken cancellationToken);
}

public sealed class EntitlementAdjustmentService(
    IEntitlementModelMapper entitlementModelMapper,
    IRepositoryFactory repositoryFactory,
    ICreditLedgerService creditLedgerService,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IDbTransactionBuilder transactionBuilder,
    IGraphQlTopicEventSender graphQlTopicEventSender,
    ILogger<EntitlementAdjustmentService> logger) : IEntitlementAdjustmentService
{
    public async Task<CreditLedgerEntryModel> AdjustAsync(
        string entitlementId,
        int quantity,
        string actorCustomerId,
        string reason,
        CancellationToken cancellationToken)
        => await AdjustAsync(entitlementId, quantity, actorCustomerId, reason, $"legacy:{entitlementId}:{actorCustomerId}:{reason}",
            cancellationToken);

    public async Task<CreditLedgerEntryModel> AdjustAsync(
        string entitlementId,
        int quantity,
        string actorCustomerId,
        string reason,
        string idempotencyKey,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);

        ArgumentException.ThrowIfNullOrWhiteSpace(entitlementId);
        ArgumentException.ThrowIfNullOrWhiteSpace(actorCustomerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        ArgumentException.ThrowIfNullOrWhiteSpace(idempotencyKey);
        ArgumentOutOfRangeException.ThrowIfZero(quantity);

        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken)
                          ?? throw new EntitlementCreditUnavailable();
        if (quantity < 0 && creditLedgerService.GetAvailableCredits(entitlement) + quantity < 0)
        {
            throw new EntitlementCreditUnavailable();
        }

        var existing = entitlement.LedgerEntries.SingleOrDefault(item => item.ReferenceKey == idempotencyKey);
        if (existing is not null)
        {
            logger.LogInformation("Entitlement adjustment is idempotent. EntitlementId={EntitlementId}, IdempotencyKey={IdempotencyKey}",
                entitlementId,
                idempotencyKey);
            return entitlementModelMapper.Map(existing);
        }

        var entry = new CreditLedgerEntry
        {
            Id = randomHelper.Generate(),
            EntitlementId = entitlement.Id,
            Quantity = quantity,
            TransactionType = CreditLedgerTransactionType.Adjusted.ToPersistedValue(),
            ReferenceKey = idempotencyKey,
            ActorOrSource = actorCustomerId,
            Metadata = new CreditLedgerEntryMetadata
            {
                ActorCustomerId = actorCustomerId,
                Reason = reason,
            },
            CreatedAt = timeProvider.GetUtcNow(),
        };
        repositoryFactory.EntitlementRepository.AddLedgerEntry(entry);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await graphQlTopicEventSender.RaiseGraphqlChangeAsync(Constants.EntitlementPurchaseTopicName,
            entitlement.PurchaseReference, cancellationToken);

        logger.LogInformation(
            "Recorded entitlement adjustment. EntitlementId={EntitlementId}, Quantity={Quantity}, ActorCustomerId={ActorCustomerId}, IdempotencyKey={IdempotencyKey}",
            entitlementId, quantity, actorCustomerId, idempotencyKey);
        return entitlementModelMapper.Map(entry);
    }
}
