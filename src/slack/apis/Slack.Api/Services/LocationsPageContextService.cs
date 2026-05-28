using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface ILocationsPageContextService
{
    LocationsPage GetPreferredLocationsPageContext();
}

public class LocationsPageContextService : ILocationsPageContextService
{
    public LocationsPage GetPreferredLocationsPageContext() => new(new PaginationContext());
}
