using Enterprise.Shared.Time;
using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IDesksPageContextService
{
    DesksPage GetDefaultDesksPageContext(string locationId);
}

public class DesksPageContextService(TimeProvider timeProvider) : IDesksPageContextService
{
    public DesksPage GetDefaultDesksPageContext(string locationId) =>
        new(
            new PaginationContext(),
            locationId,
            timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc));
}
