using Customer.Shared.Workflows.Activities;
using Temporalio.Exceptions;
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
    private AddCustomerStripePaymentMethodState? _state;

    [WorkflowRun]
    public async Task<string> ExecuteAsync(AddCustomerStripePaymentMethodInput args)
    {
        _state = new AddCustomerStripePaymentMethodState(args, null);

        if (!await Workflow.WaitConditionAsync(() => _state.StripePaymentMethodEventState is not null, TimeSpan.FromMinutes(30)))
        {
            throw new ApplicationFailureException($"Failed to receive Stripe payment method event for customer {_state.Args.CustomerId}");
        }

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
}
