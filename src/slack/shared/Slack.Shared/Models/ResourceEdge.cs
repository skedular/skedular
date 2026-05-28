using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class ResourceEdge(Resource node, string cursor) : Edge<Resource>(node, cursor);
