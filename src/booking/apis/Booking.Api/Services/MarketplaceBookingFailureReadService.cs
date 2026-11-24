using Booking.Shared.Models;
using Booking.Shared.Repositories;
using BookingEntity = Booking.Shared.Database.Entities.MarketplaceBookingFailure;
using FailureModel = Booking.Shared.Models.MarketplaceBookingFailureSummary;

namespace Booking.Api.Services;

public interface IMarketplaceBookingFailureReadService
{
    Task<FailureModel?> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<FailureModel?> GetByRecurringBookingIdAsync(string recurringBookingId, CancellationToken cancellationToken);
    Task<FailureModel?> GetBySubscriptionIdAsync(string subscriptionId, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, FailureModel>> GetByBookingIdsAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, FailureModel>> GetByMarketplaceBookingIdsAsync(
        IReadOnlyCollection<string> marketplaceBookingIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, FailureModel>> GetByRecurringBookingIdsAsync(
        IReadOnlyCollection<string> recurringBookingIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<string, FailureModel>> GetBySubscriptionIdsAsync(
        IReadOnlyCollection<string> subscriptionIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FailureModel>> GetVisibleToCustomerAsync(string customerId, CancellationToken cancellationToken);
}

public sealed class MarketplaceBookingFailureReadService(IRepositoryFactory repositoryFactory) : IMarketplaceBookingFailureReadService
{
    public async Task<FailureModel?> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken) =>
        Map(await repositoryFactory.MarketplaceBookingFailureRepository.GetByBookingIdAsync(bookingId, cancellationToken));

    public async Task<FailureModel?> GetByRecurringBookingIdAsync(string recurringBookingId, CancellationToken cancellationToken) =>
        Map(await repositoryFactory.MarketplaceBookingFailureRepository.GetByRecurringBookingIdAsync(recurringBookingId, cancellationToken));

    public async Task<FailureModel?> GetBySubscriptionIdAsync(string subscriptionId, CancellationToken cancellationToken) =>
        Map(await repositoryFactory.MarketplaceBookingFailureRepository
            .GetByMarketplaceBookingSubscriptionIdAsync(subscriptionId, cancellationToken));

    public async Task<IReadOnlyDictionary<string, FailureModel>> GetByBookingIdsAsync(
        IReadOnlyCollection<string> bookingIds,
        CancellationToken cancellationToken) =>
        MapLatest(
            await repositoryFactory.MarketplaceBookingFailureRepository.GetByBookingIdsAsync(bookingIds, cancellationToken),
            failure => failure.BookingId!);

    public async Task<IReadOnlyDictionary<string, FailureModel>> GetByMarketplaceBookingIdsAsync(
        IReadOnlyCollection<string> marketplaceBookingIds,
        CancellationToken cancellationToken)
    {
        var bookingIdsByMarketplaceBookingId = await repositoryFactory.MarketplaceBookingRepository
            .GetBookingIdsByIdsAsync(marketplaceBookingIds, cancellationToken);
        var failuresByBookingId = await GetByBookingIdsAsync(
            bookingIdsByMarketplaceBookingId.Values.Distinct().ToList(),
            cancellationToken);
        return bookingIdsByMarketplaceBookingId
            .Where(pair => failuresByBookingId.ContainsKey(pair.Value))
            .ToDictionary(pair => pair.Key, pair => failuresByBookingId[pair.Value]);
    }

    public async Task<IReadOnlyDictionary<string, FailureModel>> GetByRecurringBookingIdsAsync(
        IReadOnlyCollection<string> recurringBookingIds,
        CancellationToken cancellationToken) =>
        MapLatest(
            await repositoryFactory.MarketplaceBookingFailureRepository.GetByRecurringBookingIdsAsync(recurringBookingIds, cancellationToken),
            failure => failure.RecurringBookingId!);

    public async Task<IReadOnlyDictionary<string, FailureModel>> GetBySubscriptionIdsAsync(
        IReadOnlyCollection<string> subscriptionIds,
        CancellationToken cancellationToken) =>
        MapLatest(
            await repositoryFactory.MarketplaceBookingFailureRepository
                .GetByMarketplaceBookingSubscriptionIdsAsync(subscriptionIds, cancellationToken),
            failure => failure.MarketplaceBookingSubscriptionId!);

    public async Task<IReadOnlyList<FailureModel>> GetVisibleToCustomerAsync(string customerId, CancellationToken cancellationToken) =>
    [
        .. (await repositoryFactory.MarketplaceBookingFailureRepository.GetVisibleToCustomerAsync(customerId, cancellationToken))
        .Select(item => Map(item)!),
    ];

    private static IReadOnlyDictionary<string, FailureModel> MapLatest(
        IReadOnlyList<BookingEntity> failures,
        Func<BookingEntity, string> keySelector) =>
        failures
            .GroupBy(keySelector)
            .ToDictionary(group => group.Key, group => Map(group.First())!);

    private static FailureModel? Map(BookingEntity? entity) => entity is null
        ? null
        : new FailureModel(entity.Id, entity.Category, entity.Scope, entity.FinalizedAt, entity.RequestedFrom, entity.RequestedUntil,
            entity.CustomerAction ?? string.Empty,
            entity.ResourceReleaseStatus.ToResourceReleaseStatus(),
            entity.AccountingCleanupStatus.ToAccountingCleanupStatus());
}
