using Api.Shared.Services.Models;

namespace Booking.Shared.Workflows;

public record PayBookingByCardState(PaymentStatus? PaymentStatus, bool BookingDeleted);
