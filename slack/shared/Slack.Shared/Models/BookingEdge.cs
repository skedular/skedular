using HotChocolate.Types.Pagination;

namespace Slack.Shared.Models;

public class BookingEdge(Booking node, string cursor) : Edge<Booking>(node, cursor);
