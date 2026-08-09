using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Interceptors;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Time;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Customer = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingSubscription = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Repositories;

public interface IMarketplaceBookingSubscriptionRepository : IRepository<MarketplaceBookingSubscription>
{
    Task<(PaginatedInfo, IReadOnlyList<Edge<RecurringBookingEntity>>, int)> GetPaginatedBookingInstancesUntrackedAsync(
        string subscriptionId,
        PaginationInputParam paginationInputParam,
        DateTimeOffset? from,
        DateTimeOffset? until,
        CancellationToken cancellationToken);

    Task<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceBookingSubscription?> GetByIdForUpdateAsync(string id, CancellationToken cancellationToken);
    Task<MarketplaceBookingSubscription?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);
    MarketplaceBookingSubscription Add(MarketplaceBookingSubscription recurringBooking);
    MarketplaceBookingSubscription Update(MarketplaceBookingSubscription recurringBooking);
    MarketplaceBookingSubscription Remove(MarketplaceBookingSubscription recurringBooking);

    Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceBookingSubscription>>, int)> GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
        IReadOnlyList<MarketplaceBookingSubscriptionOrder> orderByFields,
        MarketplaceBookingSubscriptionAccessScope? accessScope,
        CancellationToken cancellationToken);
}

public static class MarketplaceBookingSubscriptionExtensions
{
    extension(IQueryable<MarketplaceBookingSubscription> originalQuery)
    {
        public IIncludableQueryable<MarketplaceBookingSubscription, Customer?> AddDependentObjects(bool isTracked, TimeProvider timeProvider)
        {
            var activeRecurringWindowStart = timeProvider.GetUtcNow().StartOfDay();

            return
                (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
                .Include(query => query.MarketplaceBooking)
                .ThenInclude(query => query.ProductVersion)
                .ThenInclude(query => query.Product)
                .ThenInclude(query => query.Organization)
                .Include(query => query.MarketplaceBooking)
                .ThenInclude(query => query.ProductVersion)
                .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query =>
                    query.RecurringBookings.Where(recurringBooking =>
                        !recurringBooking.DeletedAt.HasValue &&
                        recurringBooking.MarketplaceBooking != null &&
                        (!recurringBooking.EndDate.HasValue || recurringBooking.EndDate.Value >= activeRecurringWindowStart)))
                .ThenInclude(query => query.MarketplaceBooking)
                .ThenInclude(query => query!.StripeCheckoutSession)
                .ThenInclude(query => query!.StripeCustomer)
                .Include(query =>
                    query.RecurringBookings.Where(recurringBooking =>
                        !recurringBooking.DeletedAt.HasValue &&
                        recurringBooking.MarketplaceBooking != null &&
                        (!recurringBooking.EndDate.HasValue || recurringBooking.EndDate.Value >= activeRecurringWindowStart)))
                .ThenInclude(query => query.InvolvedCustomers)
                .ThenInclude(query => query.Identities)
                .Include(query =>
                    query.RecurringBookings.Where(recurringBooking =>
                        !recurringBooking.DeletedAt.HasValue &&
                        recurringBooking.MarketplaceBooking != null &&
                        (!recurringBooking.EndDate.HasValue || recurringBooking.EndDate.Value >= activeRecurringWindowStart)))
                .ThenInclude(query => query.MarketplaceBooking)
                .ThenInclude(query => query!.ProductVersion)
                .ThenInclude(query => query.Product)
                .ThenInclude(query => query.Organization)
                .Include(query =>
                    query.RecurringBookings.Where(recurringBooking =>
                        !recurringBooking.DeletedAt.HasValue &&
                        recurringBooking.MarketplaceBooking != null &&
                        (!recurringBooking.EndDate.HasValue || recurringBooking.EndDate.Value >= activeRecurringWindowStart)))
                .ThenInclude(query => query.MarketplaceBooking)
                .ThenInclude(query => query!.ProductVersion)
                .Include(query =>
                    query.RecurringBookings.Where(recurringBooking =>
                        !recurringBooking.DeletedAt.HasValue &&
                        recurringBooking.MarketplaceBooking != null &&
                        (!recurringBooking.EndDate.HasValue || recurringBooking.EndDate.Value >= activeRecurringWindowStart)))
                .ThenInclude(query => query.MarketplaceBooking)
                .ThenInclude(query => query!.StripeCheckoutSession)
                .Include(query => query.ProductVersion)
                .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
                .Include(query => query.InvolvedCustomers)
                .ThenInclude(query => query.Identities)
                .Include(query => query.InvolvedOrganizations)
                .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
                .Include(query => query.InvolvedTeams)
                .Include(query => query.RequestedResources)
                .ThenInclude(query => query.Location)
                .Include(query => query.CreatedByCustomer)
                .Include(query => query.LastModifiedByCustomer)
                .Include(query => query.DeletedByCustomer);
        }

        public IQueryable<MarketplaceBookingSubscription> AddSearchCriteria(
            MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
            MarketplaceBookingSubscriptionAccessScope? accessScope)
        {
            // A subscription is only displayable once its root marketplace booking exists.
            // Historical/incomplete projections without that booking cannot satisfy the
            // non-null GraphQL marketplaceBooking contract.
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue && item.MarketplaceBooking != null);

            if (searchCriteria.StartedAtGt is not null)
            {
                originalQuery = originalQuery.Where(item => item.StartedAt > searchCriteria.StartedAtGt);
            }

            if (searchCriteria.StartedAtGte is not null)
            {
                originalQuery = originalQuery.Where(item => item.StartedAt >= searchCriteria.StartedAtGte);
            }

            if (searchCriteria.StartedAtLt is not null)
            {
                originalQuery = originalQuery.Where(item => item.StartedAt < searchCriteria.StartedAtLt);
            }

            if (searchCriteria.StartedAtLte is not null)
            {
                originalQuery = originalQuery.Where(item => item.StartedAt <= searchCriteria.StartedAtLte);
            }

            if (searchCriteria.CancelledAtGt is not null)
            {
                originalQuery = originalQuery.Where(item => item.CancelledAt > searchCriteria.CancelledAtGt);
            }

            if (searchCriteria.CancelledAtGte is not null)
            {
                originalQuery = originalQuery.Where(item => item.CancelledAt >= searchCriteria.CancelledAtGte);
            }

            if (searchCriteria.CancelledAtLt is not null)
            {
                originalQuery = originalQuery.Where(item => item.CancelledAt < searchCriteria.CancelledAtLt);
            }

            if (searchCriteria.CancelledAtLte is not null)
            {
                originalQuery = originalQuery.Where(item => item.CancelledAt <= searchCriteria.CancelledAtLte);
            }

            if (searchCriteria.NextRenewalAtGt is not null)
            {
                originalQuery = originalQuery.Where(item => item.NextRenewalAt > searchCriteria.NextRenewalAtGt);
            }

            if (searchCriteria.NextRenewalAtGte is not null)
            {
                originalQuery = originalQuery.Where(item => item.NextRenewalAt >= searchCriteria.NextRenewalAtGte);
            }

            if (searchCriteria.NextRenewalAtLt is not null)
            {
                originalQuery = originalQuery.Where(item => item.NextRenewalAt < searchCriteria.NextRenewalAtLt);
            }

            if (searchCriteria.NextRenewalAtLte is not null)
            {
                originalQuery = originalQuery.Where(item => item.NextRenewalAt <= searchCriteria.NextRenewalAtLte);
            }

            if (searchCriteria.CustomerIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item => item.InvolvedCustomers.Any(customer =>
                    !customer.DeletedAt.HasValue && searchCriteria.CustomerIds.Contains(customer.Id)));
            }

            if ((searchCriteria.Statuses?.Count ?? 0) != 0)
            {
                var statusStrings = searchCriteria.Statuses!.Select(s => s.ToMarketplaceBookingSubscriptionStatus()).ToList();
                originalQuery = originalQuery.Where(item => statusStrings.Contains(item.Status));
            }
            else if (searchCriteria.Status is not null)
            {
                originalQuery = originalQuery.Where(item => item.Status == searchCriteria.Status.Value.ToMarketplaceBookingSubscriptionStatus());
            }

            if ((searchCriteria.PaymentStatuses?.Count ?? 0) != 0)
            {
                var paymentStatusStrings = searchCriteria.PaymentStatuses!.Select(s => s.ToPaymentStatus()).ToList();
                originalQuery = originalQuery.Where(item => paymentStatusStrings.Contains(item.MarketplaceBooking.PaymentStatus));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
            {
                originalQuery = originalQuery.Where(item => item.InvolvedOrganizations.Any(organization =>
                    !organization.DeletedAt.HasValue && organization.Id == searchCriteria.OrganizationId));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
            {
                originalQuery = originalQuery.Where(item => item.InvolvedOrganizations.Any(organization =>
                    !organization.DeletedAt.HasValue &&
                    organization.CustomDomain != null &&
                    organization.CustomDomain == searchCriteria.OrganizationCustomDomain));
            }

            if (searchCriteria.TeamIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && searchCriteria.TeamIds.Contains(team.Id)));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NameContains))
            {
                originalQuery = originalQuery.Where(item =>
                    item.InvolvedCustomers.Any(customer => (customer.Name != null &&
                                                            EF.Functions.ILike(customer.Name, $"%{searchCriteria.NameContains}%")) ||
                                                           (customer.GivenName != null &&
                                                            EF.Functions.ILike(customer.GivenName, $"%{searchCriteria.NameContains}%")) ||
                                                           (customer.MiddleName != null &&
                                                            EF.Functions.ILike(customer.MiddleName, $"%{searchCriteria.NameContains}%")) ||
                                                           (customer.FamilyName != null &&
                                                            EF.Functions.ILike(customer.FamilyName, $"%{searchCriteria.NameContains}%"))));
            }

            if (accessScope is not null && (accessScope.OrganizationIds.Count != 0 || accessScope.TeamIds.Count != 0))
            {
                originalQuery = originalQuery.Where(item =>
                    (accessScope.OrganizationIds.Count != 0 &&
                     item.InvolvedOrganizations.Any(organization =>
                         !organization.DeletedAt.HasValue && accessScope.OrganizationIds.Contains(organization.Id))) ||
                    (accessScope.TeamIds.Count != 0 &&
                     item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && accessScope.TeamIds.Contains(team.Id))));
            }

            return originalQuery;
        }
    }
}

public class MarketplaceBookingSubscriptionRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, MarketplaceBookingSubscription>(dbContext, timeProvider), IMarketplaceBookingSubscriptionRepository
{
    public async Task<(PaginatedInfo, IReadOnlyList<Edge<RecurringBookingEntity>>, int)> GetPaginatedBookingInstancesUntrackedAsync(
        string subscriptionId,
        PaginationInputParam paginationInputParam,
        DateTimeOffset? from,
        DateTimeOffset? until,
        CancellationToken cancellationToken)
    {
        var query = DbContext.RecurringBooking
            .Where(item => item.MarketplaceBookingSubscription != null &&
                           item.MarketplaceBookingSubscription.Id == subscriptionId);
        if (from.HasValue)
        {
            query = query.Where(item => item.Until >= from.Value);
        }

        if (until.HasValue)
        {
            query = query.Where(item => item.From <= until.Value);
        }

        return await query
            .AddDependentObjects(false)
            .ToPaginatedAsync<RecurringBookingEntity>(
                paginationInputParam,
                [
                    KeysetPaginationField<RecurringBookingEntity>.Create(nameof(RecurringBookingEntity.From), item => item.From,
                        OrderDirection.Ascending),
                ],
                cancellationToken);
    }

    public async Task<MarketplaceBookingSubscription?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingSubscription
            .AddDependentObjects(true, TimeProvider)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<MarketplaceBookingSubscription?> GetByIdForUpdateAsync(string id, CancellationToken cancellationToken)
    {
        // PostgreSQL cannot apply FOR UPDATE to the nullable side of the outer joins that load
        // the aggregate's dependent graph. Lock the root row first, then load its graph within
        // the same transaction.
        var lockedSubscription = await DbContext.MarketplaceBookingSubscription
            .TagWith(EntityFrameworkInterceptorTags.ForUpdate)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
        if (lockedSubscription is null)
        {
            return null;
        }

        return await DbContext.MarketplaceBookingSubscription
            .AddDependentObjects(true, TimeProvider)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);
    }

    public async Task<MarketplaceBookingSubscription?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingSubscription
            .AddDependentObjects(false, TimeProvider)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public MarketplaceBookingSubscription Add(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.CreatedAt = now;
        return DbContext.MarketplaceBookingSubscription.Add(recurringBooking).Entity;
    }

    public MarketplaceBookingSubscription Update(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.ModifiedAt = now;
        return DbContext.MarketplaceBookingSubscription.Update(recurringBooking).Entity;
    }

    public MarketplaceBookingSubscription Remove(MarketplaceBookingSubscription recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.DeletedAt = now;
        return DbContext.MarketplaceBookingSubscription.Update(recurringBooking).Entity;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<MarketplaceBookingSubscription>>, int)>
        GetPaginatedMarketplaceBookingSubscriptionsUntrackedAsync(
            PaginationInputParam paginationInputParam,
            MarketplaceBookingSubscriptionSearchCriteria searchCriteria,
            IReadOnlyList<MarketplaceBookingSubscriptionOrder> orderByFields,
            MarketplaceBookingSubscriptionAccessScope? accessScope,
            CancellationToken cancellationToken) =>
        await DbContext.MarketplaceBookingSubscription
            .AddSearchCriteria(searchCriteria, accessScope)
            .AddDependentObjects(false, TimeProvider)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<MarketplaceBookingSubscription>> GetPaginationFields(
        IReadOnlyList<MarketplaceBookingSubscriptionOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<MarketplaceBookingSubscription>.Create(
                    nameof(MarketplaceBookingSubscription.StartedAt),
                    query => query.StartedAt,
                    OrderDirection.Ascending),
            ];
        }

        return
        [
            .. orderByFields.Select(orderField => orderField.Field switch
            {
                MarketplaceBookingSubscriptionOrderField.StartedAt => KeysetPaginationField<MarketplaceBookingSubscription>.Create(
                    nameof(MarketplaceBookingSubscription.StartedAt),
                    query => query.StartedAt,
                    orderField.Direction),
                MarketplaceBookingSubscriptionOrderField.CancelledAt => KeysetPaginationField<MarketplaceBookingSubscription>.Create(
                    nameof(MarketplaceBookingSubscription.CancelledAt),
                    query => query.CancelledAt,
                    orderField.Direction),
                MarketplaceBookingSubscriptionOrderField.NextRenewalAt => KeysetPaginationField<MarketplaceBookingSubscription>.Create(
                    nameof(MarketplaceBookingSubscription.NextRenewalAt),
                    query => query.NextRenewalAt,
                    orderField.Direction),
                MarketplaceBookingSubscriptionOrderField.Status => KeysetPaginationField<MarketplaceBookingSubscription>.Create(
                    nameof(MarketplaceBookingSubscription.Status),
                    query => query.Status,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            }),
        ];
    }
}
