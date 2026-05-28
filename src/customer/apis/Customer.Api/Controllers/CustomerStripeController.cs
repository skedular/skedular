using Api.Shared.Services.OpenApi.Skedular.Customer.Stripe.V1;
using Customer.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Controllers;

[ApiController]
public class CustomerStripeController(IPaymentService paymentService) : CustomerStripeControllerBase
{
    public override async Task<IActionResult> AddCustomerPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        string? redirect_to = null,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(
            await paymentService.HandleStripePaymentMethodEventAsync(setup_intent_client_secret, redirect_status, redirect_to, cancellationToken));
}
