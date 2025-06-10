using Organization.Shared.Workflows.Activities;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record AddOrganizationStripePaymentMethodInput(string OrganizationId, string ClientSecret, string SetupIntentId);

public record AddOrganizationStripePaymentMethodState(
    AddOrganizationStripePaymentMethodInput Args,
    StripePaymentMethodEventState? StripePaymentMethodEventState);

public record StripePaymentMethodEventState(string SetupIntentId, string RedirectStatus);

[Workflow]
public class AddOrganizationStripePaymentMethod
{
    private AddOrganizationStripePaymentMethodState? _state;

    [WorkflowRun]
    public async Task<string> ExecuteAsync(AddOrganizationStripePaymentMethodInput args)
    {
        _state = new AddOrganizationStripePaymentMethodState(args, null);

        if (!await Workflow.WaitConditionAsync(() => _state.StripePaymentMethodEventState is not null, TimeSpan.FromMinutes(30)))
        {
            throw new ApplicationFailureException($"Failed to receive Stripe payment method event for organization {_state.Args.OrganizationId}");
        }

        ArgumentNullException.ThrowIfNull(_state.StripePaymentMethodEventState);

        var redirectUrl = await Workflow.ExecuteActivityAsync(
            (StripeIntegrations activity) => activity.SetOrganizationPaymentMethodAsync(
                new SetOrganizationPaymentMethodInput(
                    _state.Args.OrganizationId,
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
