using Api.Shared.Services.OpenApi.UnityHub.Payment.V1;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Services;

namespace Payment.Api.Controllers;

[ApiController]
public class PaymentController(IPaymentService paymentService) : PaymentControllerBase
{
    public override async Task<IActionResult> AddOrganizationPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await paymentService.AddOrganizationPaymentMethodAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));
}
