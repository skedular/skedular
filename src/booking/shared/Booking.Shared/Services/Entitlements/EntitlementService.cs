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
    public async Task<EntitlementModel?> GetByIdAsync(string entitlementId, CancellationToken cancellationToken)
    {
        var entitlement = await repositoryFactory.EntitlementRepository.GetByIdAsync(entitlementId, cancellationToken);
        return entitlement is null ? null : await MapWithWeeklyAllowanceAsync(entitlement, cancellationToken);
    }

    public async Task<IReadOnlyList<EntitlementModel>> GetForCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        await MapWithWeeklyAllowanceAsync(await repositoryFactory.EntitlementRepository.GetForCustomerAsync(customerId, cancellationToken),
            cancellationToken);

    public async Task<IReadOnlyList<EntitlementModel>> GetForOrganizationAsync(string organizationId, CancellationToken cancellationToken) =>
        await MapWithWeeklyAllowanceAsync(await repositoryFactory.EntitlementRepository.GetForOrganizationAsync(organizationId, cancellationToken),
            cancellationToken);


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

    private async Task<EntitlementModel> MapWithWeeklyAllowanceAsync(Entitlement entitlement, CancellationToken cancellationToken)
    {
        var model = entitlementModelMapper.Map(entitlement);
        if (entitlement.Status == EntitlementStatus.Active && model.ProductPricing?.RequiredDaysPerWeek is { } limit && entitlement.ActivatesAt <= timeProvider.GetUtcNow() &&
            entitlement.ExpiresAt > timeProvider.GetUtcNow())
        {
            var now = timeProvider.GetUtcNow();
            var weekStart = UtcCalendarWeek.Start(now);
            var weekEnd = weekStart.AddDays(7);
            if (UtcCalendarWeek.IsComplete(weekStart, entitlement.ActivatesAt, entitlement.ExpiresAt))
            {
                model.RemainingWeeklyRedemptions = Math.Max(0,
                    limit - await repositoryFactory.EntitlementRepository.CountSuccessfulRedemptionsAsync(entitlement.Id, weekStart, weekEnd,
                        cancellationToken));
            }
        }

        return model;
    }

    private async Task<IReadOnlyList<EntitlementModel>> MapWithWeeklyAllowanceAsync(IReadOnlyList<Entitlement> entitlements,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var weekStart = UtcCalendarWeek.Start(now);
        var weekEnd = weekStart.AddDays(7);
        var mapped = entitlements.Select(item => (Entity: item, Model: entitlementModelMapper.Map(item))).ToList();
        var limitedEntitlements = mapped.Where(item => item.Entity.Status == EntitlementStatus.Active &&
                                                       item.Model.ProductPricing?.RequiredDaysPerWeek is not null &&
                                                       item.Entity.ActivatesAt <= now && item.Entity.ExpiresAt > now &&
                                                       UtcCalendarWeek.IsComplete(weekStart, item.Entity.ActivatesAt, item.Entity.ExpiresAt)).ToList();
        var counts = limitedEntitlements.Count == 0
            ? new Dictionary<string, int>()
            : await repositoryFactory.EntitlementRepository.CountSuccessfulRedemptionsAsync(
                limitedEntitlements.Select(item => item.Entity.Id).ToArray(), weekStart, weekEnd, cancellationToken);
        var result = new List<EntitlementModel>(entitlements.Count);
        foreach (var item in mapped)
        {
            var model = item.Model;
            if (counts.TryGetValue(item.Entity.Id, out var count) && model.ProductPricing?.RequiredDaysPerWeek is { } limit)
            {
                model.RemainingWeeklyRedemptions = Math.Max(0, limit - count);
            }
            result.Add(model);
        }

        return result;
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
