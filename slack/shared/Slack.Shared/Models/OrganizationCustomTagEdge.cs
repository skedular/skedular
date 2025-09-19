using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationCustomTagEdge(OrganizationCustomTag node, string cursor) : Edge<OrganizationCustomTag>(node, cursor);
