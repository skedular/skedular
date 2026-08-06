using Organization.Shared.Activities;
using Temporalio.Common;
using Temporalio.Exceptions;
using Temporalio.Workflows;

namespace Organization.Shared.Workflows;

public record AddOrganizationStripePaymentMethodInput(string OrganizationId, string ClientSecret, string SetupIntentId);

public record StripePaymentMethodEventState(string RedirectStatus, string? RedirectTo = null);

public record AddOrganizationStripePaymentMethodState(
    AddOrganizationStripePaymentMethodInput Args,
    StripePaymentMethodEventState? StripePaymentMethodEventState);

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
                new SetOrganizationPaymentMethodInput(args.OrganizationId, args.SetupIntentId, _state.StripePaymentMethodEventState.RedirectStatus,
                    _state.StripePaymentMethodEventState.RedirectTo)),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromSeconds(30),
                TaskQueue = Workflow.Info.TaskQueue,
                RetryPolicy = new RetryPolicy
                {
                    MaximumAttempts = 3,
                    MaximumInterval = TimeSpan.FromSeconds(5),
                },
            });

        return redirectUrl;
    }

    [WorkflowSignal]
    public Task StripePaymentMethodEventReceivedAsync(StripePaymentMethodEventState state)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            StripePaymentMethodEventState = state,
        };

        return Task.CompletedTask;
    }
}
