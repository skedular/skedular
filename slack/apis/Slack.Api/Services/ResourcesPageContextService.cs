using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IResourcesPageContextService
{
    ResourcesPage GetDefaultDesksPageContext(string locationId);
}

public class ResourcesPageContextService : IResourcesPageContextService
{
    public ResourcesPage GetDefaultDesksPageContext(string locationId) => new(new PaginationContext(), locationId);
}
