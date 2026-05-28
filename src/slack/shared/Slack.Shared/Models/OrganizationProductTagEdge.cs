using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationProductTagEdge(OrganizationProductTag node, string cursor) : Edge<OrganizationProductTag>(node, cursor);
