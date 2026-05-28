using System.Text.Json;

namespace Slack.Shared.Context;

public enum PageType
{
    Home = 0,
    Bookings = 1,
    Locations = 2,
    Teams = 3,
    Settings = 4,
    CustomTags = 6,
    Zones = 7,
    Desks = 8,
    Billing = 9
}

public class DateRange(DateTimeOffset? from, DateTimeOffset? to)
{
    public DateTimeOffset? From { get; set; } = from;
    public DateTimeOffset? To { get; set; } = to;
}

public class HomePage(PaginationContext pagination, DateTimeOffset selectedDate, bool includeMyBookingsOnly)
{
    public PaginationContext Pagination { get; set; } = pagination;
    public DateTimeOffset SelectedDate { get; set; } = selectedDate;
    public bool IncludeMyBookingsOnly { get; set; } = includeMyBookingsOnly;
}

public record LocationsPage(PaginationContext Pagination);

public record TeamsPage(PaginationContext Pagination);

public record ZonesPage(PaginationContext Pagination);

public record CustomTagsPage(PaginationContext Pagination);

public class ResourcesPage(PaginationContext pagination, string locationId)
{
    public PaginationContext Pagination { get; set; } = pagination;
    public string LocationId { get; set; } = locationId;
}

public class BookingsPage(PaginationContext pagination, DateRange bookingsDateRange, bool includeMyBookingsOnly)
{
    public PaginationContext Pagination { get; } = pagination;
    public DateRange BookingsDateRange { get; } = bookingsDateRange;
    public IReadOnlyList<string> LocationIds { get; set; } = [];
    public IReadOnlyList<string> TeamIds { get; set; } = [];
    public bool IncludeMyBookingsOnly { get; set; } = includeMyBookingsOnly;
}

public record SettingsPage;

public record BillingPage;

public class PageContext
{
    public PageType CurrentPageType { get; set; }
    public List<PageType> VisitedPagesHistory { get; set; } = [];
    public HomePage? HomePage { get; set; }
    public LocationsPage? LocationsPage { get; set; }
    public TeamsPage? TeamsPage { get; set; }
    public CustomTagsPage? CustomTagsPage { get; set; }
    public ZonesPage? ZonesPage { get; set; }
    public ResourcesPage? ResourcesPage { get; set; }
    public BookingsPage? BookingsPage { get; set; }
    public SettingsPage? SettingsPage { get; set; }
    public BillingPage? BillingPage { get; set; }

    public static PageContext New() => new();

    public PageContext PushCurrentPageToVisitedPagesAndClone()
    {
        var cloned = Clone();
        cloned.PushCurrentPageToVisitedPages();
        return cloned;
    }

    public PageContext Clone() => JsonSerializer.Deserialize<PageContext>(JsonSerializer.Serialize(this))!;
    public void PushCurrentPageToVisitedPages() => VisitedPagesHistory.Add(CurrentPageType);

    public PageType? PopLastVisitedPage()
    {
        if (VisitedPagesHistory.Count == 0)
        {
            return null;
        }

        var page = VisitedPagesHistory.Last();
        VisitedPagesHistory = VisitedPagesHistory.SkipLast(1).ToList();
        return page;
    }
}
