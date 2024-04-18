using Slack.Shared.Context;

namespace Slack.Api.Services;

public interface ITeamsPageContextService
{
    TeamsPage GetDefaultTeamsPageContext();
}

public class TeamsPageContextService : ITeamsPageContextService
{
    public TeamsPage GetDefaultTeamsPageContext() => new(new PaginationContext());
}
