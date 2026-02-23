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
    public ICollection<DayOfWeek> ByWeekDays { get; set; } = []; // (for weekly patterns, e.g. Mon/Wed/Fri)
    public int? ByMonthDay { get; set; } // (for monthly “day 15” style)
    public int? BySetPosition { get; set; } // (for “first Monday”, “last Friday” patterns)
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
