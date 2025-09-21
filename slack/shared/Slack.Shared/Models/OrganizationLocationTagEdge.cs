using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationLocationTagEdge(OrganizationLocationTag node, string cursor) : Edge<OrganizationLocationTag>(node, cursor);
