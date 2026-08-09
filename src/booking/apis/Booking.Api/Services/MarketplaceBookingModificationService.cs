using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Context;
using Resource = Booking.Shared.Database.Entities.Resource;
using Location = Booking.Shared.Database.Entities.Location;
using SharedMarketplaceBookingService = Booking.Shared.Services.IMarketplaceBookingService;

namespace Booking.Api.Services;

/// <summary>
///     API boundary for a marketplace booking modification. The implementation derives the actor and
///     authorization context from the authenticated request before issuing the Booking-owned command.
/// </summary>
public interface IMarketplaceBookingModificationService
{
    Task<MarketplaceBookingModificationResult> ModifyAsync(
        MarketplaceBookingModificationCommand command,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<MarketplaceBookingModificationSummary>> GetHistoryAsync(
        string bookingId,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingResourceSelection> GetResourceSelectionAsync(
        string bookingId,
        DateTimeOffset? from,
        DateTimeOffset? until,
        string? locationId,
        CancellationToken cancellationToken);
}

public sealed record MarketplaceBookingModificationCommand(
    string BookingId,
    uint ExpectedVersion,
    DateTimeOffset From,
    DateTimeOffset Until,
    IReadOnlyCollection<string>? ResourceIds,
    string? Reason,
    MarketplaceBookingModificationActorKind ActorKind);

/// <summary>
///     Read model for an explicit marketplace resource replacement. The list is constrained to the
///     booked product's resource tags; the command still revalidates availability for the proposed window.
///     transactionally at submission time.
/// </summary>
public sealed record MarketplaceBookingResourceSelection(
    bool CanSelectResources,
    int MaximumResourceCount,
    IReadOnlyList<Resource> EligibleResources,
    IReadOnlySet<string> AvailableResourceIds,
    IReadOnlyList<Location> EligibleLocations);

public sealed class MarketplaceBookingModificationService(
    IRepositoryFactory repositoryFactory,
    IContext context,
    IOrganizationAuthorizationService organizationAuthorizationService,
    SharedMarketplaceBookingService marketplaceBookingService) : IMarketplaceBookingModificationService
{
    public async Task<MarketplaceBookingModificationResult> ModifyAsync(
        MarketplaceBookingModificationCommand command,
        CancellationToken cancellationToken)
    {
        var (customerId, actorKind) = await ResolveActorAsync(command.BookingId, command.ActorKind, cancellationToken);
        return await marketplaceBookingService.ModifyAsync(
            new MarketplaceBookingModificationRequest(command.BookingId, command.ExpectedVersion, command.From, command.Until,
                command.ResourceIds, command.Reason, customerId, actorKind), cancellationToken);
    }

    public async Task<IReadOnlyList<MarketplaceBookingModificationSummary>> GetHistoryAsync(
        string bookingId,
        CancellationToken cancellationToken)
    {
        await ResolveActorAsync(bookingId, null, cancellationToken);
        var modifications = await repositoryFactory.MarketplaceBookingModificationRepository.GetByBookingIdAsync(bookingId, cancellationToken);
        var resourceIds = modifications.SelectMany(modification => modification.OriginalResourceIds.Concat(modification.ResultResourceIds)).Distinct()
            .ToList();
        var resources = await repositoryFactory.ResourceRepository.GetByIdsAsync(resourceIds, false, cancellationToken);
        var resourceNames = resources.ToDictionary(resource => resource.Id, resource => resource.Name);
        return
        [
            .. modifications.Select(modification => new MarketplaceBookingModificationSummary(
                modification.Id, modification.BookingId, modification.OccurredAt,
                modification.ActorKind.ToMarketplaceBookingModificationActorKind(), modification.Reason,
                modification.OriginalFrom, modification.OriginalUntil, modification.ResultFrom, modification.ResultUntil,
                [.. modification.OriginalResourceIds], [.. modification.ResultResourceIds],
                [.. modification.OriginalResourceIds.Select(id => resourceNames.GetValueOrDefault(id) ?? $"Resource unavailable ({id})")],
                [.. modification.ResultResourceIds.Select(id => resourceNames.GetValueOrDefault(id) ?? $"Resource unavailable ({id})")],
                modification.SubscriptionOccurrenceOverride)),
        ];
    }

    public async Task<MarketplaceBookingResourceSelection> GetResourceSelectionAsync(
        string bookingId,
        DateTimeOffset? from,
        DateTimeOffset? until,
        string? locationId,
        CancellationToken cancellationToken)
    {
        await ResolveActorAsync(bookingId, null, cancellationToken);
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken) ?? throw new BookingNotFound();
        var marketplaceBooking = booking.MarketplaceBooking;
        if (marketplaceBooking is null)
        {
            return new MarketplaceBookingResourceSelection(false, 0, [], new HashSet<string>(), []);
        }

        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(marketplaceBooking.ProductVersion.Id, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (productVersion.Type == ProductTypeConstants.Event)
        {
            return new MarketplaceBookingResourceSelection(false, 0, [], new HashSet<string>(), []);
        }

        var maximumResourceCount = marketplaceBooking.Quantity * marketplaceBooking.ProductPricing.NumberOfResourcesToBook;
        var productTagIds = productVersion.OrganizationTags
            .Where(tag => tag.Type == OrganizationTagTypeConstants.Product)
            .Select(tag => tag.Id)
            .ToList();
        if (productTagIds.Count == 0)
        {
            return new MarketplaceBookingResourceSelection(false, 0, [], new HashSet<string>(), []);
        }

        var organizationId = productVersion.Product.Organization.Id;

        var proposedFrom = from ?? booking.From;
        var proposedUntil = until ?? booking.Until;
        var allEligibleResources = await repositoryFactory.ResourceRepository.GetResourcesByOrganizationAndTagIdsAsync(
            organizationId, productTagIds, null, cancellationToken);
        var eligibleLocations = allEligibleResources
            .Where(resource => resource.Location is not null)
            .Select(resource => resource.Location!)
            .DistinctBy(location => location.Id)
            .OrderBy(location => location.Name)
            .ToList();
        if (locationId is not null && eligibleLocations.All(location => location.Id != locationId))
        {
            return new MarketplaceBookingResourceSelection(true, maximumResourceCount, [], new HashSet<string>(), eligibleLocations);
        }

        var eligibleResources = locationId is null
            ? allEligibleResources
            : [.. allEligibleResources.Where(resource => resource.Location?.Id == locationId)];

        var availableResources = await repositoryFactory.ResourceRepository.GetAvailableResourcesAsync(
            organizationId,
            locationId,
            proposedFrom,
            proposedUntil,
            [],
            productTagIds,
            [],
            [.. booking.InvolvedResources.Select(resource => resource.Id)],
            cancellationToken);
        var availableResourceIds = availableResources.Select(resource => resource.Id).ToHashSet();

        return new MarketplaceBookingResourceSelection(
            true,
            maximumResourceCount,
            eligibleResources,
            availableResourceIds,
            eligibleLocations);
    }

    private async Task<(string CustomerId, MarketplaceBookingModificationActorKind ActorKind)> ResolveActorAsync(
        string bookingId,
        MarketplaceBookingModificationActorKind? requestedActorKind,
        CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        ArgumentException.ThrowIfNullOrWhiteSpace(verifiableToken);
        var customer = await repositoryFactory.CustomerRepository.GetByVerifiableTokenAsync(verifiableToken, true, cancellationToken) ??
                       throw new CustomerNotFound();
        var booking = await repositoryFactory.BookingRepository.GetByIdAsync(bookingId, cancellationToken) ?? throw new BookingNotFound();
        if (requestedActorKind == MarketplaceBookingModificationActorKind.Customer ||
            (requestedActorKind is null && booking.InvolvedCustomers.Any(item => item.Id == customer.Id)))
        {
            return booking.InvolvedCustomers.All(item => item.Id != customer.Id)
                ? throw new UnauthorizedAccessException()
                : (customer.Id, MarketplaceBookingModificationActorKind.Customer);
        }

        var productVersionId = booking.MarketplaceBooking?.ProductVersion.Id;
        ArgumentException.ThrowIfNullOrWhiteSpace(productVersionId);
        var productVersion = await repositoryFactory.ProductVersionRepository.GetByIdAsync(productVersionId, cancellationToken) ??
                             throw new ProductVersionNotFound();
        if (!await organizationAuthorizationService.CanOverrideCancellationPolicyAsync(
                productVersion.Product.Organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return (customer.Id, MarketplaceBookingModificationActorKind.OrganizationOperator);
    }
}
