using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Booking : ModelBase
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingCategory Category { get; set; }
    public BookingChannel Channel { get; set; }
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public IReadOnlyList<Customer> InvolvedCustomers { get; set; } = [];
    public IReadOnlyList<Organization> InvolvedOrganizations { get; set; } = [];
    public IReadOnlyList<Location> InvolvedLocations { get; set; } = [];
    public IReadOnlyList<Team> InvolvedTeams { get; set; } = [];
}
