using Enterprise.Shared.Time;

namespace Slack.Shared.Models;

public static class CustomerExtensions
{
    public static TimeZoneInfo GetTimezone(this Database.Entities.Customer customer) =>
        customer.Timezone.ToTimezoneInfo();
}
