using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationTagEdge(OrganizationTag node, string cursor) : Edge<OrganizationTag>(node, cursor);
