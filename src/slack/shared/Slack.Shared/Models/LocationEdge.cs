using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class LocationEdge(Location node, string cursor) : Edge<Location>(node, cursor);
