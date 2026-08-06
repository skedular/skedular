using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Time;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Customer = Booking.Shared.Database.Entities.Customer;
using RecurringBooking = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.Repositories;

public interface IRecurringBookingRepository : IRepository<RecurringBooking>
{
    Task<RecurringBooking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<RecurringBooking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyList<RecurringBooking>> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken);

    RecurringBooking Add(RecurringBooking recurringBooking);
    RecurringBooking Update(RecurringBooking recurringBooking);
    RecurringBooking Remove(RecurringBooking recurringBooking);

    Task<(PaginatedInfo, IReadOnlyList<Edge<RecurringBooking>>, int)> GetPaginatedRecurringBookingsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        RecurringBookingSearchCriteria searchCriteria,
        IReadOnlyList<RecurringBookingOrder> orderByFields,
        RecurringBookingAccessScope? accessScope,
        CancellationToken cancellationToken);
}

public static class RecurringBookingExtensions
{
    extension(IQueryable<RecurringBooking> originalQuery)
    {
        public IIncludableQueryable<RecurringBooking, Customer?> AddDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.MarketplaceBookingSubscription)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.ProductVersion)
            .ThenInclude(query => query.Product)
            .ThenInclude(query => query.Organization)
            .Include(query => query.InvolvedCustomers)
            .ThenInclude(query => query.Identities)
            .Include(query => query.InvolvedOrganizations)
            .ThenInclude(query => query.OrganizationMembers.Where(organizationMember => !organizationMember.DeletedAt.HasValue))
            .Include(query => query.InvolvedTeams)
            .Include(query => query.RequestedResources)
            .Include(query => query.CreatedByCustomer)
            .Include(query => query.LastModifiedByCustomer)
            .Include(query => query.DeletedByCustomer);

        public IQueryable<RecurringBooking> AddSearchCriteria(
            RecurringBookingSearchCriteria searchCriteria,
            TimeProvider timeProvider,
            RecurringBookingAccessScope? accessScope)
        {
            originalQuery = originalQuery.Where(item => !item.DeletedAt.HasValue);

            if (searchCriteria.IncludeFutureBookingsOnly is not null && searchCriteria.IncludeFutureBookingsOnly.Value)
            {
                originalQuery = originalQuery.Where(item => item.From >= timeProvider.GetUtcNow().StartOfDay());
            }

            if (searchCriteria.FromGt is not null)
            {
                originalQuery = originalQuery.Where(item => item.From > searchCriteria.FromGt);
            }

            if (searchCriteria.FromGte is not null)
            {
                originalQuery = originalQuery.Where(item => item.From >= searchCriteria.FromGte);
            }

            if (searchCriteria.FromLt is not null)
            {
                originalQuery = originalQuery.Where(item => item.From < searchCriteria.FromLt);
            }

            if (searchCriteria.FromLte is not null)
            {
                originalQuery = originalQuery.Where(item => item.From <= searchCriteria.FromLte);
            }

            if (searchCriteria.ToGt is not null)
            {
                originalQuery = originalQuery.Where(item => item.Until > searchCriteria.ToGt);
            }

            if (searchCriteria.ToGte is not null)
            {
                originalQuery = originalQuery.Where(item => item.Until >= searchCriteria.ToGte);
            }

            if (searchCriteria.ToLt is not null)
            {
                originalQuery = originalQuery.Where(item => item.Until < searchCriteria.ToLt);
            }

            if (searchCriteria.ToLte is not null)
            {
                originalQuery = originalQuery.Where(item => item.Until <= searchCriteria.ToLte);
            }

            if (searchCriteria.CustomerIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item => item.InvolvedCustomers.Any(customer =>
                    !customer.DeletedAt.HasValue && searchCriteria.CustomerIds.Contains(customer.Id)));
            }

            if (searchCriteria.Category is not null)
            {
                originalQuery = originalQuery.Where(item => item.Category == searchCriteria.Category.Value.ToBookingCategory());
            }

            if (searchCriteria.Channel is not null)
            {
                originalQuery = originalQuery.Where(item => item.Channel == searchCriteria.Channel.Value.ToBookingChannel());
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

public class RecurringBookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, RecurringBooking>(dbContext, timeProvider), IRecurringBookingRepository
{
    public async Task<RecurringBooking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<RecurringBooking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<IReadOnlyList<RecurringBooking>> GetByMarketplaceBookingSubscriptionIdAsync(
        string marketplaceBookingSubscriptionId,
        CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddDependentObjects(true)
            .Where(query => query.MarketplaceBookingSubscription != null &&
                            query.MarketplaceBookingSubscription.Id == marketplaceBookingSubscriptionId)
            .OrderByDescending(query => query.StartDate)
            .ToListAsync(cancellationToken);

    public RecurringBooking Add(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.CreatedAt = now;
        return DbContext.RecurringBooking.Add(recurringBooking).Entity;
    }

    public RecurringBooking Update(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.ModifiedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }

    public RecurringBooking Remove(RecurringBooking recurringBooking)
    {
        var now = TimeProvider.GetUtcNow();
        recurringBooking.DeletedAt = now;
        return DbContext.RecurringBooking.Update(recurringBooking).Entity;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<RecurringBooking>>, int)> GetPaginatedRecurringBookingsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        RecurringBookingSearchCriteria searchCriteria,
        IReadOnlyList<RecurringBookingOrder> orderByFields,
        RecurringBookingAccessScope? accessScope,
        CancellationToken cancellationToken) =>
        await DbContext.RecurringBooking
            .AddSearchCriteria(searchCriteria, TimeProvider, accessScope)
            .AddDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<RecurringBooking>> GetPaginationFields(IReadOnlyList<RecurringBookingOrder> orderByFields)
    {
        if (!orderByFields.Any())
        {
            return
            [
                KeysetPaginationField<RecurringBooking>.Create(
                    nameof(RecurringBooking.From),
                    query => query.From,
                    OrderDirection.Ascending),
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                RecurringBookingOrderField.From => KeysetPaginationField<RecurringBooking>.Create(
                    nameof(RecurringBooking.From),
                    query => query.From,
                    orderField.Direction),
                RecurringBookingOrderField.To => KeysetPaginationField<RecurringBooking>.Create(
                    nameof(RecurringBooking.Until),
                    query => query.Until,
                    orderField.Direction),
                RecurringBookingOrderField.Category => KeysetPaginationField<RecurringBooking>.Create(
                    nameof(RecurringBooking.Category),
                    query => query.Category,
                    orderField.Direction),
                RecurringBookingOrderField.Channel => KeysetPaginationField<RecurringBooking>.Create(
                    nameof(RecurringBooking.Channel),
                    query => query.Channel,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            })
            .ToList();
    }
}
