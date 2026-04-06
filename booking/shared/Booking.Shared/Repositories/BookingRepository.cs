using Api.Shared.Services.Models;
using Booking.Shared.Database;
using Booking.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Time;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using StripeCheckoutSession = Booking.Shared.Database.Entities.StripeCheckoutSession;

namespace Booking.Shared.Repositories;

public interface IBookingRepository : IRepository<Database.Entities.Booking>
{
    Task<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Booking>> GetByIdsMinimalAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Database.Entities.Booking>> GetByIdsWithValidMarketplaceAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<Database.Entities.Booking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Booking>> GetByRecurringBookingIdAsync(
        string recurringBookingId,
        DateTimeOffset from,
        DateTimeOffset? until,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Booking>> GetByRecurringBookingIdUntrackedAsync(
        string recurringBookingId,
        DateTimeOffset from,
        DateTimeOffset? until,
        CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Booking>> GetAllUntrackedAsync(CancellationToken cancellationToken);

    Task<ICollection<Database.Entities.Booking>> GetInArrearsByOrganizationBeforeAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken);

    Database.Entities.Booking Add(Database.Entities.Booking booking);
    Database.Entities.Booking Update(Database.Entities.Booking booking);
    Database.Entities.Booking Remove(Database.Entities.Booking booking);

    Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Booking>>, int)> GetPaginatedBookingsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        BookingAccessScope? accessScope,
        CancellationToken cancellationToken);
}

internal static class BookingExtensions
{
    extension(IQueryable<Database.Entities.Booking> originalQuery)
    {
        internal IIncludableQueryable<Database.Entities.Booking, StripeCheckoutSession?> AddSingleBookingMinimumDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.InvolvedCustomers)
            .Include(query => query.InvolvedOrganizations)
            .Include(query => query.InvolvedLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.InvolvedTeams)
            .Include(query => query.InvolvedResources)
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.CreatedByCustomer)
            .Include(query => query.LastModifiedByCustomer)
            .Include(query => query.DeletedByCustomer)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByCustomer)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByOrganization)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.ProductVersion)
            .ThenInclude(query => query!.Product)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.StripeCheckoutSession)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByCustomer)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByOrganization)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.ProductVersion)
            .ThenInclude(query => query!.Product)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.StripeCheckoutSession);

        internal IIncludableQueryable<Database.Entities.Booking, StripeCheckoutSession?> AddSingleBookingDependentObjects(bool isTracked) =>
            (isTracked ? originalQuery.AsTracking() : originalQuery.AsNoTrackingWithIdentityResolution())
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Customers)
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Resource)
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.ResourceBookingSlots.Where(resourceBookingSlot => !resourceBookingSlot.Resource.DeletedAt.HasValue))
            .ThenInclude(query => query.Resource)
            .ThenInclude(query => query.Location)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.InvolvedCustomers)
            .ThenInclude(query => query.Identities)
            .Include(query => query.InvolvedOrganizations)
            .Include(query => query.InvolvedLocations)
            .ThenInclude(query => query.Organization)
            .Include(query => query.InvolvedTeams)
            .Include(query => query.InvolvedResources)
            .ThenInclude(query => query.OrganizationTags.Where(tag => !tag.DeletedAt.HasValue))
            .Include(query => query.CreatedByCustomer)
            .Include(query => query.LastModifiedByCustomer)
            .Include(query => query.DeletedByCustomer)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByCustomer)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByOrganization)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.ProductVersion)
            .ThenInclude(query => query!.Product)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.RecurringBooking)
            .ThenInclude(query => query!.MarketplaceBooking)
            .ThenInclude(query => query!.StripeCheckoutSession)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByCustomer)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.PaidByOrganization)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.ProductVersion)
            .ThenInclude(query => query!.Product)
            .ThenInclude(query => query!.Organization)
            .Include(query => query.MarketplaceBooking)
            .ThenInclude(query => query!.StripeCheckoutSession);

        internal IQueryable<Database.Entities.Booking> AddSearchCriteria(
            BookingSearchCriteria searchCriteria,
            TimeProvider timeProvider,
            BookingAccessScope? accessScope)
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

            if (searchCriteria.RecurringBookingIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.RecurringBooking != null && searchCriteria.RecurringBookingIds.Contains(item.RecurringBooking.Id));
            }

            if (searchCriteria.Category is not null)
            {
                originalQuery = originalQuery.Where(item => item.Category == searchCriteria.Category.Value.ToBookingCategory());
            }

            if (searchCriteria.Channel is not null)
            {
                originalQuery = originalQuery.Where(item => item.Channel == searchCriteria.Channel.Value.ToBookingChannel());
            }

            if (searchCriteria.PaymentStatuses.Count != 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.MarketplaceBooking != null && searchCriteria.PaymentStatuses.Select(paymentStatus => paymentStatus.ToPaymentStatus())
                        .Contains(item.MarketplaceBooking.PaymentStatus));
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

            if (searchCriteria.LocationIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item => item.InvolvedLocations.Any(location =>
                    !location.DeletedAt.HasValue && searchCriteria.LocationIds.Contains(location.Id)));
            }

            if (searchCriteria.TeamIds.Count != 0)
            {
                originalQuery = originalQuery.Where(item =>
                    item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && searchCriteria.TeamIds.Contains(team.Id)));
            }

            if (!string.IsNullOrWhiteSpace(searchCriteria.NotesContains))
            {
                originalQuery =
                    originalQuery.Where(item => item.Notes != null && EF.Functions.ILike(item.Notes, $"%{searchCriteria.NotesContains}%"));
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

            if (accessScope is not null &&
                (accessScope.OrganizationIds.Count != 0 || accessScope.LocationIds.Count != 0 || accessScope.TeamIds.Count != 0))
            {
                originalQuery = originalQuery.Where(item =>
                    (accessScope.OrganizationIds.Count != 0 && item.InvolvedOrganizations.Any(organization =>
                        !organization.DeletedAt.HasValue && accessScope.OrganizationIds.Contains(organization.Id))) ||
                    (accessScope.LocationIds.Count != 0 && item.InvolvedLocations.Any(location =>
                        !location.DeletedAt.HasValue && accessScope.LocationIds.Contains(location.Id))) ||
                    (accessScope.TeamIds.Count != 0 &&
                     item.InvolvedTeams.Any(team => !team.DeletedAt.HasValue && accessScope.TeamIds.Contains(team.Id))));
            }

            return originalQuery;
        }
    }
}

public class BookingRepository(BookingDbContext dbContext, TimeProvider timeProvider)
    : RepositoryBase<BookingDbContext, Database.Entities.Booking>(dbContext, timeProvider), IBookingRepository
{
    public async Task<Database.Entities.Booking?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .AddSingleBookingDependentObjects(true)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetByIdsMinimalAsync(ICollection<string> ids, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => ids.Contains(query.Id))
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetByIdsWithValidMarketplaceAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => ids.Contains(query.Id) && !query.DeletedAt.HasValue && query.MarketplaceBooking != null)
            .Include(query => query.MarketplaceBooking)
            .AsSingleQuery()
            .ToListAsync(cancellationToken);

    public async Task<Database.Entities.Booking?> GetByIdUntrackedAsync(string id, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .AddSingleBookingMinimumDependentObjects(false)
            .FirstOrDefaultAsync(query => query.Id == id, cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetByRecurringBookingIdAsync(string recurringBookingId, DateTimeOffset from,
        DateTimeOffset? until, CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => !query.DeletedAt.HasValue &&
                            query.From >= from &&
                            (!until.HasValue || query.Until <= until) &&
                            query.RecurringBooking != null && query.RecurringBooking.Id == recurringBookingId)
            .AddSingleBookingDependentObjects(true)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetByRecurringBookingIdUntrackedAsync(
        string recurringBookingId,
        DateTimeOffset from,
        DateTimeOffset? until,
        CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query => !query.DeletedAt.HasValue &&
                            query.From >= from &&
                            (!until.HasValue || query.Until <= until) &&
                            query.RecurringBooking != null && query.RecurringBooking.Id == recurringBookingId)
            .AddSingleBookingDependentObjects(false)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetAllUntrackedAsync(CancellationToken cancellationToken) =>
        await DbContext.Booking
            .AddSingleBookingDependentObjects(false)
            .ToListAsync(cancellationToken);

    public async Task<ICollection<Database.Entities.Booking>> GetInArrearsByOrganizationBeforeAsync(
        string organizationId,
        DateTimeOffset startInclusive,
        DateTimeOffset endExclusive,
        CancellationToken cancellationToken) =>
        await DbContext.Booking
            .Where(query =>
                !query.DeletedAt.HasValue &&
                query.From < endExclusive &&
                query.Until > startInclusive &&
                query.MarketplaceBooking != null &&
                query.MarketplaceBooking.BillingMode == ProductPricingBillingMode.InArrears.ToProductPricingBillingMode() &&
                query.InvolvedOrganizations.Any(organization => !organization.DeletedAt.HasValue && organization.Id == organizationId))
            .AddSingleBookingDependentObjects(true)
            .ToListAsync(cancellationToken);

    public Database.Entities.Booking Add(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.CreatedAt = now;
        return DbContext.Booking.Add(booking).Entity;
    }

    public Database.Entities.Booking Update(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.ModifiedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public Database.Entities.Booking Remove(Database.Entities.Booking booking)
    {
        var now = TimeProvider.GetUtcNow();
        booking.DeletedAt = now;
        return DbContext.Booking.Update(booking).Entity;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Database.Entities.Booking>>, int)> GetPaginatedBookingsUntrackedAsync(
        PaginationInputParam paginationInputParam,
        BookingSearchCriteria searchCriteria,
        ICollection<BookingOrder> orderByFields,
        BookingAccessScope? accessScope,
        CancellationToken cancellationToken) =>
        await DbContext.Booking
            .AddSearchCriteria(searchCriteria, TimeProvider, accessScope)
            .AddSingleBookingMinimumDependentObjects(false)
            .ToPaginatedAsync(paginationInputParam, GetPaginationFields(orderByFields), cancellationToken);

    private static List<KeysetPaginationField<Database.Entities.Booking>> GetPaginationFields(ICollection<BookingOrder> orderByFields)
    {
        if (orderByFields.Count == 0)
        {
            return
            [
                KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.From),
                    query => query.From,
                    OrderDirection.Ascending)
            ];
        }

        return orderByFields.Select(orderField => orderField.Field switch
            {
                BookingOrderField.From => KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.From),
                    query => query.From,
                    orderField.Direction),
                BookingOrderField.To => KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.Until),
                    query => query.Until,
                    orderField.Direction),
                BookingOrderField.Notes => KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.Notes),
                    query => query.Notes,
                    orderField.Direction),
                BookingOrderField.Category => KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.Category),
                    query => query.Category,
                    orderField.Direction),
                BookingOrderField.Channel => KeysetPaginationField<Database.Entities.Booking>.Create(
                    nameof(Database.Entities.Booking.Channel),
                    query => query.Channel,
                    orderField.Direction),
                _ => throw new ArgumentOutOfRangeException()
            })
            .ToList();
    }
}
