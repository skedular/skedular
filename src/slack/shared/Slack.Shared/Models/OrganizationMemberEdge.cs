using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationMemberEdge(OrganizationMember node, string cursor) : Edge<OrganizationMember>(node, cursor);
