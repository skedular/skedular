using Enterprise.Shared.Time;
using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IHomePageContextService
{
    HomePage GetDefaultHomePageContext();
}

public class HomePageContextService(TimeProvider timeProvider) : IHomePageContextService
{
    public HomePage GetDefaultHomePageContext() =>
        new(new PaginationContext(), timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc), true);
}
