using FakeItEasy;
using Organization.Shared.Activities;
using Organization.Shared.UnitTests.Fixtures;
using Organization.Shared.Workflows;
using Temporalio.Client;
using Temporalio.Testing;
using Temporalio.Worker;

namespace Organization.Shared.UnitTests.Workflows.InviteToJoinOrganizationTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InviteToJoinOrganizationShould
{
    [Theory]
    [AutoFakeItEasyData([typeof(ActivitiesFixtureCustomizer), typeof(InviteNewCustomerToJoinOrganizationInputFixtureCustomizer)])]
    public async Task Call_SendNewCustomerToJoin(
        EmailIntegrations mockEmailActivity,
        InvitationIntegrations mockInvitationActivity,
        InviteToJoinOrganizationInput input,
        string taskQueue,
        string workflowId)
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<InviteToJoinOrganization>()
                .AddAllActivities(mockEmailActivity)
                .AddAllActivities(mockInvitationActivity));

        await worker.ExecuteAsync(async () =>
            {
                var handle = await env.Client.StartWorkflowAsync(
                    (InviteToJoinOrganization wf) => wf.ExecuteAsync(input),
                    new WorkflowOptions(workflowId, worker.Options.TaskQueue!));

                await handle.GetResultAsync();

                A.CallTo(() => mockEmailActivity.SendInviteCustomerToJoinOrganizationNewCustomerAsync(input.JoinInvitationId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => mockEmailActivity.SendInviteCustomerToJoinOrganizationExistingCustomerAsync(A<string>._)).MustNotHaveHappened();
            },
            TestContext.Current.CancellationToken);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ActivitiesFixtureCustomizer), typeof(InviteExistingCustomerToJoinOrganizationInputFixtureCustomizer)])]
    public async Task Call_SendExistingCustomerToJoin(
        EmailIntegrations mockEmailActivity,
        InvitationIntegrations mockInvitationActivity,
        InviteToJoinOrganizationInput input,
        string taskQueue,
        string workflowId)
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<InviteToJoinOrganization>()
                .AddAllActivities(mockInvitationActivity)
                .AddAllActivities(mockEmailActivity));
        await worker.ExecuteAsync(async () =>
            {
                var handle = await env.Client.StartWorkflowAsync(
                    (InviteToJoinOrganization wf) => wf.ExecuteAsync(input),
                    new WorkflowOptions(workflowId, worker.Options.TaskQueue!));

                await handle.GetResultAsync();

                A.CallTo(() => mockEmailActivity.SendInviteCustomerToJoinOrganizationExistingCustomerAsync(input.JoinInvitationId))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => mockEmailActivity.SendInviteCustomerToJoinOrganizationNewCustomerAsync(A<string>._)).MustNotHaveHappened();
            },
            TestContext.Current.CancellationToken);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ActivitiesFixtureCustomizer)])]
    public async Task Call_ExpireInvitation(
        EmailIntegrations mockEmailActivity,
        InvitationIntegrations mockInvitationActivity,
        InviteToJoinOrganizationInput input,
        string taskQueue,
        string workflowId)
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<InviteToJoinOrganization>()
                .AddAllActivities(mockInvitationActivity)
                .AddAllActivities(mockEmailActivity));
        await worker.ExecuteAsync(async () =>
            {
                var handle = await env.Client.StartWorkflowAsync(
                    (InviteToJoinOrganization wf) => wf.ExecuteAsync(input),
                    new WorkflowOptions(workflowId, worker.Options.TaskQueue!));

                await handle.GetResultAsync();

                A.CallTo(() => mockInvitationActivity.ExpireInvitationAsync(A<string>._)).MustHaveHappenedOnceExactly();
            },
            TestContext.Current.CancellationToken);
    }

    [Theory]
    [AutoFakeItEasyData([typeof(ActivitiesFixtureCustomizer)])]
    public async Task Call_InvitationStatusChange(
        EmailIntegrations mockEmailActivity,
        InvitationIntegrations mockInvitationActivity,
        InviteToJoinOrganizationInput input,
        string taskQueue,
        string workflowId)
    {
        await using var env = await WorkflowEnvironment.StartTimeSkippingAsync();
        using var worker = new TemporalWorker(
            env.Client,
            new TemporalWorkerOptions(taskQueue)
                .AddWorkflow<InviteToJoinOrganization>()
                .AddAllActivities(mockEmailActivity)
                .AddAllActivities(mockInvitationActivity));
        await worker.ExecuteAsync(async () =>
            {
                var handle = await env.Client.StartWorkflowAsync(
                    (InviteToJoinOrganization wf) => wf.ExecuteAsync(input),
                    new WorkflowOptions(workflowId, worker.Options.TaskQueue!));
                await handle.SignalAsync(wf => wf.InvitationStatusChangedAsync());

                await handle.GetResultAsync();

                A.CallTo(() => mockInvitationActivity.ExpireInvitationAsync(A<string>._)).MustNotHaveHappened();
            },
            TestContext.Current.CancellationToken);
    }
}
