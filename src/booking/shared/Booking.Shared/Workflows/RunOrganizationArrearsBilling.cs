using Booking.Shared.Activities;
using Booking.Shared.Models;
using Temporalio.Common;
using Temporalio.Workflows;

namespace Booking.Shared.Workflows;

public record RunOrganizationArrearsBillingInput(OrganizationArrearsBillingConfiguration Configuration, bool RunNowRequested = false);

public record RunOrganizationArrearsBillingState(
    OrganizationArrearsBillingConfiguration Configuration,
    bool RunNowRequested,
    bool ConfigurationChanged,
    bool Stopped);

[Workflow]
public class RunOrganizationArrearsBilling
{
    private RunOrganizationArrearsBillingState? _state;

    [WorkflowRun]
    public async Task ExecuteAsync(RunOrganizationArrearsBillingInput args)
    {
        // Persisted invoice lines are the durable source of truth for segment-level dedupe, so the
        // workflow only needs to keep scheduling/control state in history.
        _state = new RunOrganizationArrearsBillingState(args.Configuration, args.RunNowRequested, false, false);

        while (!_state.Stopped)
        {
            // Schedule calculation happens in an activity to keep workflow code deterministic and to
            // follow the project rule that services are injected into activities, not workflows.
            var runAt = await Workflow.ExecuteActivityAsync(
                (OrganizationArrearsBillingIntegrations activity) => activity.GetNextRunAtAsync(
                    new GetOrganizationArrearsBillingNextRunAtInput(_state.Configuration)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromSeconds(5),
                    },
                });
            var delay = runAt - Workflow.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await Workflow.WaitConditionAsync(() => _state.RunNowRequested || _state.ConfigurationChanged || _state.Stopped, delay);
            }

            if (_state.Stopped)
            {
                return;
            }

            if (_state.ConfigurationChanged)
            {
                // A billing-cycle update should reschedule from the new configuration before another
                // invoice batch is attempted.
                _state = _state with
                {
                    ConfigurationChanged = false,
                };

                continue;
            }

            var billingPeriod = await Workflow.ExecuteActivityAsync(
                (OrganizationArrearsBillingIntegrations activity) => activity.GetBillingPeriodForRunAtAsync(
                    new GetOrganizationArrearsBillingPeriodInput(runAt, _state.RunNowRequested, _state.Configuration)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(1),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromSeconds(5),
                    },
                });

            await Workflow.ExecuteActivityAsync(
                (OrganizationArrearsBillingIntegrations activity) => activity.GenerateOrganizationArrearsInvoicesAsync(
                    new GenerateOrganizationArrearsInvoicesInput(
                        _state.Configuration.OrganizationId,
                        billingPeriod,
                        _state.Configuration.BillingCycle)),
                new ActivityOptions
                {
                    StartToCloseTimeout = TimeSpan.FromMinutes(2),
                    TaskQueue = Workflow.Info.TaskQueue,
                    RetryPolicy = new RetryPolicy
                    {
                        MaximumAttempts = 3,
                        MaximumInterval = TimeSpan.FromSeconds(5),
                    },
                });

            _state = _state with
            {
                RunNowRequested = false,
            };
        }
    }

    [WorkflowSignal]
    public Task RunNowAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            RunNowRequested = true,
        };
        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task UpdateConfigurationAsync(OrganizationArrearsBillingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            Configuration = configuration,
            ConfigurationChanged = true,
        };
        return Task.CompletedTask;
    }

    [WorkflowSignal]
    public Task StopAsync()
    {
        ArgumentNullException.ThrowIfNull(_state);

        _state = _state with
        {
            Stopped = true,
        };
        return Task.CompletedTask;
    }
}
