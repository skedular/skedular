using Api.Shared.Services.Models;

namespace Booking.Shared.Workflows.Activities;

public record CreateCheckoutSessionAsyncResponse(PaymentStatus PaymentStatus);
