using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.OpenApi.Skedular.Customer.V1;
using Customer.Api.Services;
using Enterprise.Shared.Version;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using Version = Api.Shared.Services.OpenApi.Skedular.Customer.V1.Version;

namespace Customer.Api.Controllers;

[ApiController]
public class CustomerController(
    IVersionService versionService,
    CustomerConfiguration customerConfiguration,
    IWorkaroundService workaroundService,
    IPaymentService paymentService,
    ITopicEventSender topicEventSender)
    : CustomerControllerBase
{
    public override Task<ActionResult<Version>> GetVersion(CancellationToken cancellationToken = default)
    {
        var version = versionService.GetVersion();

        return Task.FromResult<ActionResult<Version>>(new Version
        {
            Major = version.Major, Minor = version.Minor, Build = version.Build, Revision = version.Revision
        });
    }

    public override async Task<IActionResult> RaiseGraphqlChange(
        string topicName,
        string id,
        // ReSharper disable once InconsistentNaming
        string x_API_Key,
        CancellationToken cancellationToken = default)
    {
        if (x_API_Key != customerConfiguration.ApiKey)
        {
            return Unauthorized();
        }

        await topicEventSender.SendAsync(topicName, id, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> Republish(string customerId, CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishCustomerAsync(customerId, cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> RepublishAll(CancellationToken cancellationToken = default)
    {
        await workaroundService.RepublishAllCustomersAsync(cancellationToken);

        return Ok();
    }

    public override async Task<IActionResult> AddCustomerPaymentMethod(
        // ReSharper disable InconsistentNaming
        string setup_intent,
        string setup_intent_client_secret,
        string redirect_status,
        // ReSharper restore InconsistentNaming
        CancellationToken cancellationToken = default) =>
        Redirect(await paymentService.HandleStripePaymentMethodEventAsync(setup_intent_client_secret, redirect_status, cancellationToken));
}
