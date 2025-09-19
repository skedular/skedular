using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class OrganizationZoneEdge(OrganizationZone node, string cursor) : Edge<OrganizationZone>(node, cursor);
