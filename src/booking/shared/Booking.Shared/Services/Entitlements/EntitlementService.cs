using System.Data;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Models.Entitlements;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services.Entitlements;

public interface IEntitlementService
{
    Task<EntitlementModel?> GetByIdAsync(string entitlementId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementModel>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken);
    Task<IReadOnlyList<EntitlementModel>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken);
    Task<EntitlementModel> SetRenewalPolicyAsync(string entitlementId, bool autoRenew, bool cancelAtPeriodEnd, CancellationToken cancellationToken);

    Task<EntitlementModel> GrantAsync(
        string purchaseReference,
        string customerId,
        string organizationId,
        ProductPricing pricing,
        DateTimeOffset activatesAt,
        string currency,
        CancellationToken cancellationToken);

    Task<EntitlementModel> GrantAsync(
        string purchaseReference, string customerId, string organizationId, ProductPricing pricing,
        DateTimeOffset activatesAt, string currency, bool autoRenew, CancellationToken cancellationToken);
}

public sealed class EntitlementService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IEntitlementModelMapper entitlementModelMapper,
    TimeProvider timeProvider,
    ILogger<EntitlementService> logger,
    IDbTransactionBuilder transactionBuilder) : IEntitlementService
{
    public async Task<EntitlementModel?> GetByIdAsync(string entitlementId, CancellationToken cancellationToken) =>
        await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken) is { } entitlement
            ? entitlementModelMapper.Map(entitlement)
            : null;

    public async Task<IReadOnlyList<EntitlementModel>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        [.. (await repositoryFactory.EntitlementRepository.GetForCustomerAsync(customerId, cancellationToken)).Select(entitlementModelMapper.Map)];

    public async Task<IReadOnlyList<EntitlementModel>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
    [
        .. (await repositoryFactory.EntitlementRepository.GetForOrganizationAsync(organizationId, cancellationToken)).Select(entitlementModelMapper
            .Map),
    ];

    public async Task<EntitlementModel> SetRenewalPolicyAsync(string entitlementId, bool autoRenew, bool cancelAtPeriodEnd,
        CancellationToken cancellationToken)
    {
        if (autoRenew || cancelAtPeriodEnd)
        {
            throw new InvalidOperationException("Credit entitlements don't support auto-renewal.");
        }

        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken)
                          ?? throw new KeyNotFoundException("The entitlement was not found.");
        if (entitlement.Status != EntitlementStatus.Active)
        {
            throw new InvalidOperationException("Only active entitlements can change renewal policy.");
        }

        entitlement.AutoRenew = false;
        entitlement.CancelAtPeriodEnd = false;
        entitlement.RenewalFailureReason = null;
        entitlement.NextRenewalAt = entitlement.AutoRenew ? entitlement.ExpiresAt : null;
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        return entitlementModelMapper.Map(entitlement);
    }

    public Task<EntitlementModel> GrantAsync(
        string purchaseReference, string customerId, string organizationId, ProductPricing pricing,
        DateTimeOffset activatesAt, string currency, CancellationToken cancellationToken) =>
        GrantAsync(purchaseReference, customerId, organizationId, pricing, activatesAt, currency,
            false, cancellationToken);

    public async Task<EntitlementModel> GrantAsync(
        string purchaseReference,
        string customerId,
        string organizationId,
        ProductPricing pricing,
        DateTimeOffset activatesAt,
        string currency,
        bool autoRenew,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, IsolationLevel.Serializable, cancellationToken);
        ArgumentException.ThrowIfNullOrWhiteSpace(purchaseReference);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        if (pricing.FulfillmentType != ProductPricingFulfillmentType.Entitlement)
        {
            throw new InvalidOperationException("Only entitlement pricing can grant an entitlement.");
        }

        if (pricing.EntitlementCreditQuantity is not > 0 ||
            pricing.EntitlementValidityDays is not > 0)
        {
            throw new EntitlementPricingConfigurationInvalid();
        }

        var existing = await repositoryFactory.EntitlementRepository.GetByPurchaseReferenceAsync(purchaseReference, cancellationToken);
        if (existing is not null)
        {
            await LinkPurchaseAsync(purchaseReference, existing.Id, cancellationToken);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Entitlement grant already exists for purchase {PurchaseReference}; returning {EntitlementId}", purchaseReference,
                existing.Id);
            return entitlementModelMapper.Map(existing);
        }

        var now = timeProvider.GetUtcNow();
        var entitlement = new Entitlement
        {
            Id = randomHelper.Generate(),
            CustomerId = customerId,
            OrganizationId = organizationId,
            PurchaseReference = purchaseReference,
            PricingId = pricing.Id,
            GrantedQuantity = pricing.EntitlementCreditQuantity.Value,
            ActivatesAt = activatesAt,
            ExpiresAt = activatesAt.AddDays(pricing.EntitlementValidityDays.Value),
            Status = EntitlementStatus.Active,
            AutoRenew = false,
            NextRenewalAt = null,
            NetPurchaseAmount = pricing.Price,
            Currency = currency,
            CreatedAt = now,
        };

        repositoryFactory.EntitlementRepository.Add(entitlement);
        await LinkPurchaseAsync(purchaseReference, entitlement.Id, cancellationToken);
        repositoryFactory.EntitlementRepository.AddLedgerEntry(new CreditLedgerEntry
        {
            Id = randomHelper.Generate(),
            EntitlementId = entitlement.Id,
            Quantity = entitlement.GrantedQuantity,
            TransactionType = CreditLedgerTransactionType.Granted.ToPersistedValue(),
            ReferenceKey = $"{purchaseReference}:grant",
            ActorOrSource = "purchase",
            CreatedAt = now,
        });
        try
        {
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(cancellationToken);

            var concurrentGrant = await repositoryFactory.EntitlementRepository.GetByPurchaseReferenceAsync(purchaseReference, cancellationToken);
            if (concurrentGrant is null)
            {
                throw;
            }

            logger.LogInformation(
                "Concurrent entitlement grant resolved to existing purchase {PurchaseReference}; returning {EntitlementId}",
                purchaseReference, concurrentGrant.Id);

            return entitlementModelMapper.Map(concurrentGrant);
        }

        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Granted entitlement {EntitlementId} for purchase {PurchaseReference} with {CreditQuantity} credits", entitlement.Id,
            purchaseReference, entitlement.GrantedQuantity);
        // The newly-created entity does not have navigation properties populated.
        // Reload it through the repository before mapping so required relationships
        // such as Organization are available to the model mapper.
        var persistedEntitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlement.Id, cancellationToken) ??
                                   throw new InvalidOperationException("The granted entitlement could not be reloaded.");
        return entitlementModelMapper.Map(persistedEntitlement);
    }

    private async Task LinkPurchaseAsync(string purchaseReference, string entitlementId, CancellationToken cancellationToken)
    {
        var purchase = await repositoryFactory.EntitlementPurchaseRepository.GetByIdAsync(purchaseReference, cancellationToken);
        if (purchase is not null && purchase.EntitlementId != entitlementId)
        {
            purchase.EntitlementId = entitlementId;
        }
    }
}
