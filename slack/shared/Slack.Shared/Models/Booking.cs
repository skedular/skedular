using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Booking : ModelBase
{
    public DateTimeOffset From { get; set; }
    public DateTimeOffset Until { get; set; }
    public string? Notes { get; set; }
    public BookingCategory Category { get; set; }
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<Customer> InvolvedCustomers { get; set; } = [];
    public ICollection<Organization> InvolvedOrganizations { get; set; } = [];
    public ICollection<Location> InvolvedLocations { get; set; } = [];
    public ICollection<Team> InvolvedTeams { get; set; } = [];
}
