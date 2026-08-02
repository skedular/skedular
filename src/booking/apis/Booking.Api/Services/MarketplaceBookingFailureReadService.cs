using Booking.Shared.Repositories;
using FailureModel = Booking.Shared.Models.MarketplaceBookingFailureSummary;
using BookingEntity = Booking.Shared.Database.Entities.MarketplaceBookingFailure;

namespace Booking.Api.Services;

public interface IMarketplaceBookingFailureReadService
{
    Task<FailureModel?> GetByBookingIdAsync(string bookingId, CancellationToken cancellationToken);
    Task<FailureModel?> GetByRecurringBookingIdAsync(string recurringBookingId, CancellationToken cancellationToken);
    Task<FailureModel?> GetBySubscriptionIdAsync(string subscriptionId, CancellationToken cancellationToken);
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

    public async Task<IReadOnlyList<FailureModel>> GetVisibleToCustomerAsync(string customerId, CancellationToken cancellationToken) =>
        (await repositoryFactory.MarketplaceBookingFailureRepository.GetVisibleToCustomerAsync(customerId, cancellationToken))
        .Select(item => Map(item)!)
        .ToList();

    private static FailureModel? Map(BookingEntity? entity) => entity is null
        ? null
        : new FailureModel(entity.Id, entity.Category, entity.Scope, entity.FinalizedAt, entity.RequestedFrom, entity.RequestedUntil,
            entity.CustomerAction ?? string.Empty);
}
