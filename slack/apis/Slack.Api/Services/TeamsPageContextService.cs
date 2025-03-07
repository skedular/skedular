using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface ITeamsPageContextService
{
    TeamsPage GetPreferredTeamsPageContext();
}

public class TeamsPageContextService : ITeamsPageContextService
{
    public TeamsPage GetPreferredTeamsPageContext() => new(new PaginationContext());
}
