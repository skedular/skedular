using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class RecurringBooking : ModelBaseWithDeleted
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public BookingChannel Channel { get; set; }
    public BookingFrequency Frequency { get; set; } // (Daily, Weekly, Monthly, etc.)
    public int Interval { get; set; } // (every N units, e.g. every 2 weeks)
    public int? ByMonthDay { get; set; } // (for monthly patterns, e.g. day 15 or -1 for last day)
    public int? BySetPosition { get; set; } // (for monthly patterns, e.g. 1st/2nd/.../-1 weekday from ByWeekDays)
    public ICollection<DayOfWeek> ByWeekDays { get; set; } = []; // (for weekly patterns, e.g. Mon/Wed/Fri)
    public RecurringBookingEndType EndType { get; set; } // (Never, UntilDate, AfterOccurrences)
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }
    public ICollection<DateTimeOffset> SkippedDates { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public MarketplaceBooking? MarketplaceBooking { get; set; }
    public ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public ICollection<Team> InvolvedTeams { get; set; } = [];
    public Customer? CreatedByCustomer { get; set; }
    public Customer? LastModifiedByCustomer { get; set; }
    public Customer? DeletedByCustomer { get; set; }
}
