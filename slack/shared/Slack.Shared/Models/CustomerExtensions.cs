using Enterprise.Shared.Time;

namespace Slack.Shared.Models;

public static class CustomerExtensions
{
    extension(Database.Entities.Customer customer)
    {
        public TimeZoneInfo GetTimezone() => customer.Timezone.ToTimezoneInfo();
    }
}
