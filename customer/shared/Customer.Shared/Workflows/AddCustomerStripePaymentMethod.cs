using Customer.Shared.Workflows.Activities;
using Microsoft.Extensions.Logging;
using Temporalio.Workflows;

namespace Customer.Shared.Workflows;

public record AddCustomerStripePaymentMethodInput(string CustomerId, string ClientSecret, string SetupIntentId);

public record AddCustomerStripePaymentMethodState(
    AddCustomerStripePaymentMethodInput Args,
    StripePaymentMethodEventState? StripePaymentMethodEventState);

public record StripePaymentMethodEventState(string SetupIntentId, string RedirectStatus);

[Workflow]
public class AddCustomerStripePaymentMethod
{
    private static readonly TimeSpan s_maxTimeAllowedToReceiveStripePaymentMethodEvent = TimeSpan.FromMinutes(30);
    private readonly ILogger _logger = Workflow.Logger;
    private AddCustomerStripePaymentMethodState? _state;

    [WorkflowRun]
    public async Task<string> ExecuteAsync(AddCustomerStripePaymentMethodInput args)
    {
        _state = new AddCustomerStripePaymentMethodState(args, null);
        await AwaitPaymentMethodEventAsync();

        ArgumentNullException.ThrowIfNull(_state.StripePaymentMethodEventState);

        var redirectUrl = await Workflow.ExecuteActivityAsync(
            (StripeIntegrations activity) => activity.SetCustomerPaymentMethodAsync(
                new SetCustomerPaymentMethodInput(
                    _state.Args.CustomerId,
                    _state.StripePaymentMethodEventState.SetupIntentId,
                    _state.StripePaymentMethodEventState.RedirectStatus)),
            new ActivityOptions { StartToCloseTimeout = TimeSpan.FromSeconds(30), TaskQueue = Workflow.Info.TaskQueue });

        return redirectUrl;
    }

    [WorkflowSignal]
    public Task StripePaymentMethodEventReceivedAsync(StripePaymentMethodEventState state)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with { StripePaymentMethodEventState = state };

        return Task.CompletedTask;
    }

    private async Task AwaitPaymentMethodEventAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        try
        {
            _logger.LogInformation(
                "{tag} Await receiving Stripe payment method event for customer {customerId}...",
                GetLogPrefix(),
                _state.Args.CustomerId);

            await Workflow.WaitConditionAsync(
                () => _state.StripePaymentMethodEventState is not null,
                s_maxTimeAllowedToReceiveStripePaymentMethodEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{tag} Failed to receive Stripe payment method event for customer {customerId}...",
                GetLogPrefix(),
                _state.Args.CustomerId);

            throw;
        }

        _logger.LogInformation(
            "{tag} Received Stripe payment method event for customer {customerId}...",
            GetLogPrefix(),
            _state.Args.CustomerId);
    }

    private static string GetLogPrefix() => $"{nameof(AddCustomerStripePaymentMethod)}:{Workflow.Info.RunId}:";
}
