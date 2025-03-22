using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface IDesksPageContextService
{
    ResourcesPage GetDefaultDesksPageContext(string locationId);
}

public class DesksPageContextService : IDesksPageContextService
{
    public ResourcesPage GetDefaultDesksPageContext(string locationId) => new(new PaginationContext(), locationId);
}
