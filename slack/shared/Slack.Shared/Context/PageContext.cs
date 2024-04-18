using System.Text.Json;

namespace Slack.Shared.Context;

public enum PageType
{
    Home = 0,
    Bookings = 1,
    Locations = 2,
    Teams = 3,
    Settings = 4,
    Zones = 5,
    Desks = 6,
    Billing = 7
}

public class DateRange(DateTimeOffset? from, DateTimeOffset? to)
{
    public DateTimeOffset? From { get; set; } = from;
    public DateTimeOffset? To { get; set; } = to;
}

public class HomePage(PaginationContext bookingsPagination, DateTimeOffset selectedDate, bool includeMyBookingsOnly)
{
    public PaginationContext BookingsPagination { get; set; } = bookingsPagination;
    public DateTimeOffset SelectedDate { get; set; } = selectedDate;
    public bool IncludeMyBookingsOnly { get; set; } = includeMyBookingsOnly;
}

public record LocationsPage(PaginationContext LocationsPagination);

public record TeamsPage(PaginationContext TeamsPagination);

public record ZonesPage(PaginationContext ZonesPagination, string LocationId);

public class DesksPage(PaginationContext desksPagination, string locationId, DateTimeOffset selectedDate)
{
    public PaginationContext DesksPagination { get; set; } = desksPagination;
    public string LocationId { get; set; } = locationId;
    public DateTimeOffset SelectedDate { get; set; } = selectedDate;
}

public class BookingsPage(PaginationContext bookingsPagination, DateRange bookingsDateRange, bool includeMyBookingsOnly)
{
    public PaginationContext BookingsPagination { get; } = bookingsPagination;
    public DateRange BookingsDateRange { get; } = bookingsDateRange;
    public ICollection<string> LocationIds { get; set; } = [];
    public ICollection<string> TeamIds { get; set; } = [];
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
    public ZonesPage? ZonesPage { get; set; }
    public DesksPage? DesksPage { get; set; }
    public BookingsPage? BookingsPage { get; set; }
    public SettingsPage? SettingsPage { get; set; }
    public BillingPage? BillingPage { get; set; }

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
