using Api.Shared.Services.OpenApi.Skedular.Payment.V1;
using Microsoft.AspNetCore.Mvc;
using Payment.Api.Services;

namespace Payment.Api.Controllers;

[ApiController]
public class PaymentController(IOrganizationPaymentService organizationPaymentService, IWorkaroundService workaroundService) : PaymentControllerBase
{
    public override async Task<IActionResult> AddOrganizationPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await organizationPaymentService.AddPaymentMethodAsync(
            setup_intent,
            setup_intent_client_secret,
            redirect_status,
            cancellationToken));

    public override async Task<IActionResult> RepublishAllOrganizationStripeConnectAccounts(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllOrganizationStripeConnectAccountsAsync(cancellationToken);

        return Ok();
    }
}
