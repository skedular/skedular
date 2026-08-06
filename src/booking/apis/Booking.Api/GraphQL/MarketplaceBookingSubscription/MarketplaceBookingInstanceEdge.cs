using Booking.Api.GraphQL.RecurringBooking;
using HotChocolate.Types.Pagination;

namespace Booking.Api.GraphQL.MarketplaceBookingSubscription;

public sealed class MarketplaceBookingInstanceEdge(RecurringBookingDetails node, string cursor) : Edge<RecurringBookingDetails>(node, cursor);
