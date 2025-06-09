using Microsoft.Extensions.Logging;
using Organization.Shared.Workflows.Activities;
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
    private static readonly TimeSpan s_maxTimeAllowedToReceiveStripePaymentMethodEvent = TimeSpan.FromMinutes(30);
    private readonly ILogger _logger = Workflow.Logger;
    private AddOrganizationStripePaymentMethodState? _state;

    [WorkflowRun]
    public async Task<string> ExecuteAsync(AddOrganizationStripePaymentMethodInput args)
    {
        _state = new AddOrganizationStripePaymentMethodState(args, null);
        await AwaitPaymentMethodEventAsync();

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

    private async Task AwaitPaymentMethodEventAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        try
        {
            _logger.LogInformation(
                "{tag} Await receiving Stripe payment method event for organization {organizationId}...",
                GetLogPrefix(),
                _state.Args.OrganizationId);

            await Workflow.WaitConditionAsync(
                () => _state.StripePaymentMethodEventState is not null,
                s_maxTimeAllowedToReceiveStripePaymentMethodEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{tag} Failed to receive Stripe payment method event for organization {organizationId}...",
                GetLogPrefix(),
                _state.Args.OrganizationId);

            throw;
        }

        _logger.LogInformation(
            "{tag} Received Stripe payment method event for organization {organizationId}...",
            GetLogPrefix(),
            _state.Args.OrganizationId);
    }

    private static string GetLogPrefix() => $"{nameof(AddOrganizationStripePaymentMethod)}:{Workflow.Info.RunId}:";
}
