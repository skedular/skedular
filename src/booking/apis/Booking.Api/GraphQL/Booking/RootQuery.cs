using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Sanitization;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Types.Composite;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[QueryType]
public class RootQuery(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<IReadOnlyList<MarketplaceBookingFailureDetails>> MarketplaceBookingFailuresAsync(
        [Service]
        ICachedCustomerService cachedCustomerService,
        [Service]
        IMarketplaceBookingFailureReadService failureReadService,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var failures = await failureReadService.GetVisibleToCustomerAsync(customerId, cancellationToken);
        return [.. failures.Select(graphQlMapper.MapTo)];
    }

    public IEnumerable<BookingCategoryDetails> BookingCategories() =>
    [
        new()
        {
            Category = BookingCategory.WorkingFromHome,
            Name = BookingCategoryConstants.WorkingFromHome.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.WorkingFromOffice,
            Name = BookingCategoryConstants.WorkingFromOffice.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.WorkingFromCoworkingSpace,
            Name = BookingCategoryConstants.WorkingFromCoworkingSpace.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.SickLeave,
            Name = BookingCategoryConstants.SickLeave.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.AnnualLeave,
            Name = BookingCategoryConstants.AnnualLeave.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.WellbeingLeave,
            Name = BookingCategoryConstants.WellbeingLeave.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.ClientOffice,
            Name = BookingCategoryConstants.ClientOffice.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.Vacation,
            Name = BookingCategoryConstants.Vacation.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.TravelingForWork,
            Name = BookingCategoryConstants.TravelingForWork.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.NonWorkingDay,
            Name = BookingCategoryConstants.NonWorkingDay.ToBookingCategoryName(),
        },
    ];

    public IEnumerable<BookingCategoryDetails> MarketplaceBookingCategories() =>
    [
        new()
        {
            Category = BookingCategory.WorkingFromOffice,
            Name = BookingCategoryConstants.WorkingFromOffice.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.WorkingFromCoworkingSpace,
            Name = BookingCategoryConstants.WorkingFromCoworkingSpace.ToBookingCategoryName(),
        },
        new()
        {
            Category = BookingCategory.ClientOffice,
            Name = BookingCategoryConstants.ClientOffice.ToBookingCategoryName(),
        },
    ];

    [UseResolverScope]
    public async Task<BookingDetails?> BookingAsync(string id, [Service] IBookingService bookingService, CancellationToken cancellationToken) =>
        graphQlMapper.MapTo(await bookingService.GetByIdAsync(id, cancellationToken));

    [UseResolverScope]
    public async Task<BookingSpacesQuotaStatusDetails> BookingSpacesQuotaStatusAsync(
        string organizationId,
        [Service]
        ISpacesBookingQuotaService spacesBookingQuotaService,
        CancellationToken cancellationToken)
    {
        var decision = await spacesBookingQuotaService.GetQuotaStatusAsync(organizationId, cancellationToken);

        return new BookingSpacesQuotaStatusDetails
        {
            OrganizationId = organizationId,
            CurrentPeriodStartUtc = decision.CurrentPeriodStartUtc,
            CurrentPeriodEndUtc = decision.CurrentPeriodEndUtc,
            PlanCode = decision.PlanCode,
            QuotaLimit = decision.PlanCode == 4 ? null : decision.QuotaLimit,
            CurrentUsage = decision.CurrentUsage,
            AttemptedCurrentPeriodCount = decision.AttemptedCurrentPeriodCount,
            ExcludedOutOfPeriodCount = decision.ExcludedOutOfPeriodCount,
            TotalAttemptedInstanceCount = decision.TotalAttemptedInstanceCount,
            RemainingQuota = decision.PlanCode == 4 ? null : decision.RemainingQuota,
            QuotaExceeded = !decision.CanCreate && decision.ReasonCode != SpacesQuotaReasonCode.MissingOfferingState,
            ReasonCode =
                new SpacesQuotaReasonCodeDetails
                {
                    Type = decision.ReasonCode,
                    Name = decision.ReasonCode.ToSpacesQuotaReasonCodeName(),
                },
            UpgradePlans =
            [
                .. decision.UpgradePlans.Select(upgrade => new UpgradePlanDetails
                {
                    PlanCode = upgrade.PlanCode,
                    Name = upgrade.Name,
                    Availability = upgrade.Availability,
                    PriceDescription = upgrade.PriceDescription,
                }),
            ],
        };
    }

    [UseResolverScope]
    public async Task<SpacesPublicBookingAvailabilityDetails> SpacesPublicBookingAvailabilityAsync(
        string organizationId,
        [Service]
        ISpacesBookingQuotaService spacesBookingQuotaService,
        CancellationToken cancellationToken)
    {
        var decision = await spacesBookingQuotaService.GetQuotaStatusAsync(organizationId, cancellationToken);
        return new SpacesPublicBookingAvailabilityDetails
        {
            Available = decision.CanCreate,
            Message = decision.CanCreate
                ? "Bookings are available."
                : "Bookings are currently unavailable for this workspace.",
        };
    }

    [UseResolverScope]
    [Lookup]
    [Internal]
    public async Task<BookingDetails?> BookingByIdAsync(
        [ID]
        string id,
        [Service]
        IBookingService bookingService,
        CancellationToken cancellationToken) =>
        await BookingAsync(id, bookingService, cancellationToken);

    [Lookup]
    [Internal]
    public OrganizationDetails? OrganizationById([ID] string id) => new(id, string.Empty);

    [UseResolverScope]
    public async Task<Connection<BookingEdge>> BookingsAsync(
        string? after,
        int? first,
        string? before,
        int? last,
        BookingWhereInput where,
        IEnumerable<BookingOrderInput>? orderBy,
        [Service]
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        where.LocationIds = where.LocationIds.RemoveInvalidIds();
        where.TeamIds = where.TeamIds.RemoveInvalidIds();
        where.CustomerIds = where.CustomerIds.RemoveInvalidIds();
        where.RecurringBookingIds = where.RecurringBookingIds.RemoveInvalidIds();

        var (paginatedInfo, edges, totalCount) = await bookingService.GetPaginatedBookingsAsync(
            new PaginationInputParam(after, first, before, last),
            new BookingSearchCriteria(
                where.FromGt,
                where.FromGte,
                where.FromLt,
                where.FromLte,
                where.ToGt,
                where.ToGte,
                where.ToLt,
                where.ToLte,
                where.NotesContains,
                where.NameContains,
                where.Category,
                where.Channel,
                where.PaymentStatuses.ToSafeCollection(),
                where.IncludeMineOnly,
                where.IncludeFutureBookingsOnly,
                where.OrganizationId,
                where.OrganizationCustomDomain,
                where.LocationIds.ToSafeCollection(),
                where.TeamIds.ToSafeCollection(),
                where.CustomerIds.ToSafeCollection(),
                where.RecurringBookingIds.ToSafeCollection(),
                null),
            [.. orderBy.ToSafeCollection().Select(item => new BookingOrder(item.Direction, item.Field))],
            false,
            cancellationToken);

        return new Connection<BookingEdge>
        {
            PageInfo = new PageInfo
            {
                HasNextPage = paginatedInfo.HasNextPage,
                HasPreviousPage = paginatedInfo.HasPreviousPage,
                StartCursor = paginatedInfo.StartCursor,
                EndCursor = paginatedInfo.EndCursor,
            },
            Edges = edges.Select(graphQlMapper.MapTo),
            TotalCount = totalCount,
        };
    }

    [UseResolverScope]
    public async Task<IEnumerable<BookingDetails>> AllBookingsAsync(
        BookingWhereInput where,
        [Service]
        IBookingService bookingService,
        CancellationToken cancellationToken)
    {
        var result = await BookingsAsync(null, null, null, null, where, [], bookingService, cancellationToken);
        return result.Edges.Select(item => item.Node);
    }
}
