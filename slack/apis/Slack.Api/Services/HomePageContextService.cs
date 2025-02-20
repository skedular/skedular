using Enterprise.Shared.Time;
using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IHomePageContextService
{
    HomePage GetDefaultHomePageContext(string timezone);
}

public class HomePageContextService(TimeProvider timeProvider) : IHomePageContextService
{
    public HomePage GetDefaultHomePageContext(string timezone) =>
        new(new PaginationContext(), timeProvider.GetUtcNow().StartOfDay(timezone.ToTimezoneInfo()), true);
}
