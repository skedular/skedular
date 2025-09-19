using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class TeamEdge(Team node, string cursor) : Edge<Team>(node, cursor);
