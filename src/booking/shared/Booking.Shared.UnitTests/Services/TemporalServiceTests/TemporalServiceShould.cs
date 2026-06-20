using System.Linq.Expressions;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TemporalServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_Rollover_Spaces_Booking_Usage_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        WorkflowHandle<RolloverSpacesBookingUsage> workflowHandle,
        TemporalService sut,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.RolloverSpacesBookingUsage()).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<RolloverSpacesBookingUsage, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.UseExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowRolloverSpacesBookingUsageAsync(cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<RolloverSpacesBookingUsage, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_Generate_Location_Resources_Slots_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        WorkflowHandle<GenerateLocationResourcesSlots> workflowHandle,
        TemporalService sut,
        GenerateLocationResourcesSlotsInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        args = args with { LocationId = "loc-1" };

        A.CallTo(() => workflowIdService.GenerateLocationResourcesSlots("loc-1")).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<GenerateLocationResourcesSlots, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowGenerateLocationResourcesSlotsAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<GenerateLocationResourcesSlots, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_Generate_Resources_Slots_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        WorkflowHandle<GenerateResourcesSlots> workflowHandle,
        TemporalService sut,
        GenerateResourcesSlotsInput args,
        string locationId,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.GenerateResourcesSlots(locationId)).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<GenerateResourcesSlots, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue)))
            .Returns(workflowHandle);

        await sut.StartWorkflowGenerateResourcesSlotsAsync(locationId, args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<GenerateResourcesSlots, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Pay_Booking_Via_Card_Workflow_With_Correct_Handle(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        TemporalService sut,
        SetPaymentStatusArgs args,
        string bookingId,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<PayBookingViaCard>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.PayBookingViaCard(bookingId)).Returns(expectedWorkflowId);
        A.CallTo(() => temporalClient.GetWorkflowHandle<PayBookingViaCard>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalPayBookingViaCardWorkflowAsync(bookingId, args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<PayBookingViaCard>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_Run_Organization_Arrears_Billing_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        WorkflowHandle<RunOrganizationArrearsBilling> workflowHandle,
        TemporalService sut,
        CancellationToken cancellationToken)
    {
        var args = new RunOrganizationArrearsBillingInput(
            new OrganizationArrearsBillingConfiguration(
                "org-1",
                OrganizationBillingCycle.Monthly));
        const string expectedId = "organization-arrears-billing-org-1";

        A.CallTo(() => workflowIdService.RunOrganizationArrearsBilling("org-1")).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<RunOrganizationArrearsBilling, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.StartWorkflowRunOrganizationArrearsBillingAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<RunOrganizationArrearsBilling, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Run_Organization_Arrears_Billing_Workflow_Run_Now_With_Correct_Handle(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalHelperService temporalHelperService,
        TemporalService sut,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var configuration = new OrganizationArrearsBillingConfiguration("org-1", OrganizationBillingCycle.Weekly);
        var workflowHandle = new WorkflowHandle<RunOrganizationArrearsBilling>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.RunOrganizationArrearsBilling("org-1")).Returns(expectedWorkflowId);
        A.CallTo(() => temporalHelperService.IsRunningAsync<RunOrganizationArrearsBilling>(expectedWorkflowId, cancellationToken)).Returns(true);
        A.CallTo(() => temporalClient.GetWorkflowHandle<RunOrganizationArrearsBilling>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalRunOrganizationArrearsBillingWorkflowRunNowAsync(configuration, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<RunOrganizationArrearsBilling>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Maintain_Accounting_Invoice_State_Workflow_When_Running(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalHelperService temporalHelperService,
        TemporalService sut,
        MaintainAccountingInvoiceStateInput args,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<MaintainAccountingInvoiceState>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.MaintainAccountingInvoiceState(args.LocalEntityType, args.LocalEntityId))
            .Returns(expectedWorkflowId);
        A.CallTo(() => temporalHelperService.IsRunningAsync<MaintainAccountingInvoiceState>(expectedWorkflowId, cancellationToken)).Returns(true);
        A.CallTo(() => temporalClient.GetWorkflowHandle<MaintainAccountingInvoiceState>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalWorkflowMaintainAccountingInvoiceStateAsync(args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<MaintainAccountingInvoiceState>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Maintain_Accounting_Invoice_State_Workflow_When_Not_Running(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowHandle<MaintainAccountingInvoiceState> workflowHandle,
        TemporalService sut,
        MaintainAccountingInvoiceStateInput args,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.MaintainAccountingInvoiceState(args.LocalEntityType, args.LocalEntityId))
            .Returns(expectedWorkflowId);
        A.CallTo(() => temporalHelperService.IsRunningAsync<MaintainAccountingInvoiceState>(expectedWorkflowId, cancellationToken)).Returns(false);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<MaintainAccountingInvoiceState, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedWorkflowId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.SignalWorkflowMaintainAccountingInvoiceStateAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<MaintainAccountingInvoiceState, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Maintain_Organization_Arrears_Invoice_Accounting_State_Workflow_When_Running(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalHelperService temporalHelperService,
        TemporalService sut,
        MaintainOrganizationArrearsInvoiceAccountingStateInput args,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<MaintainOrganizationArrearsInvoiceAccountingState>(temporalClient, expectedWorkflowId);

        A.CallTo(() => workflowIdService.MaintainOrganizationArrearsInvoiceAccountingState(args.OrganizationArrearsInvoiceId))
            .Returns(expectedWorkflowId);
        A.CallTo(() => temporalHelperService.IsRunningAsync<MaintainOrganizationArrearsInvoiceAccountingState>(expectedWorkflowId, cancellationToken))
            .Returns(true);
        A.CallTo(() => temporalClient.GetWorkflowHandle<MaintainOrganizationArrearsInvoiceAccountingState>(expectedWorkflowId))
            .Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<MaintainOrganizationArrearsInvoiceAccountingState>(expectedWorkflowId))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Maintain_Organization_Arrears_Invoice_Accounting_State_Workflow_When_Not_Running(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] IWorkflowIdService workflowIdService,
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowHandle<MaintainOrganizationArrearsInvoiceAccountingState> workflowHandle,
        TemporalService sut,
        MaintainOrganizationArrearsInvoiceAccountingStateInput args,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => workflowIdService.MaintainOrganizationArrearsInvoiceAccountingState(args.OrganizationArrearsInvoiceId))
            .Returns(expectedWorkflowId);
        A.CallTo(() => temporalHelperService.IsRunningAsync<MaintainOrganizationArrearsInvoiceAccountingState>(expectedWorkflowId, cancellationToken))
            .Returns(false);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<MaintainOrganizationArrearsInvoiceAccountingState, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedWorkflowId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue &&
                    options.IdReusePolicy == WorkflowIdReusePolicy.AllowDuplicate &&
                    options.IdConflictPolicy == WorkflowIdConflictPolicy.TerminateExisting)))
            .Returns(workflowHandle);

        await sut.SignalWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<MaintainOrganizationArrearsInvoiceAccountingState, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }
}
