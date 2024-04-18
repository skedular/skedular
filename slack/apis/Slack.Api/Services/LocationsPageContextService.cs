using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface ILocationsPageContextService
{
    LocationsPage GetDefaultLocationsPageContext();
}

public class LocationsPageContextService : ILocationsPageContextService
{
    public LocationsPage GetDefaultLocationsPageContext() => new(new PaginationContext());
}
