namespace Api.Shared.Services.Models;

public record BookingSchedule(DateTimeOffset From, DateTimeOffset Until);

public record BookingSchedules(ICollection<BookingSchedule> Schedules);
