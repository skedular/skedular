using System.Linq.Expressions;
using AutoFixture.Xunit3;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using FakeItEasy;
using Temporalio.Api.Enums.V1;
using Temporalio.Client;

namespace Booking.Shared.UnitTests.Services.TemporalServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TemporalServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Start_Workflow_Generate_Location_Resources_Slots_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowHandle<GenerateLocationResourcesSlots> workflowHandle,
        TemporalService sut,
        GenerateLocationResourcesSlotsInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        args = args with { LocationId = "loc-1" };

        A.CallTo(() => temporalHelperService.ToId("generate-location-resources-slots-loc-1")).Returns(expectedId);
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
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowHandle<GenerateResourcesSlots> workflowHandle,
        TemporalService sut,
        GenerateResourcesSlotsInput args,
        string locationId,
        string expectedId,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => temporalHelperService.ToId($"generate-resources-slots-{locationId}")).Returns(expectedId);
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
    public async Task Start_Workflow_Book_Marketplace_Booking_Subscription_Resources_With_Correct_Options(
        [Frozen] TemporalConfiguration temporalConfiguration,
        [Frozen] ITemporalClient temporalClient,
        [Frozen] ITemporalHelperService temporalHelperService,
        WorkflowHandle<BookMarketplaceBookingSubscriptionResources> workflowHandle,
        TemporalService sut,
        BookMarketplaceBookingSubscriptionResourcesInput args,
        string expectedId,
        CancellationToken cancellationToken)
    {
        args = args with { MarketplaceBookingSubscriptionId = "sub-1" };

        A.CallTo(() => temporalHelperService.ToId("sub-1")).Returns(expectedId);
        A.CallTo(() => temporalClient.StartWorkflowAsync(
                A<Expression<Func<BookMarketplaceBookingSubscriptionResources, Task>>>._,
                A<WorkflowOptions>.That.Matches(options =>
                    options.Id == expectedId &&
                    options.TaskQueue == temporalConfiguration.Worker.TaskQueue)))
            .Returns(workflowHandle);

        await sut.StartWorkflowBookMarketplaceBookingSubscriptionResourcesAsync(args, cancellationToken);

        A.CallTo(() => temporalClient.StartWorkflowAsync(
            A<Expression<Func<BookMarketplaceBookingSubscriptionResources, Task>>>._,
            A<WorkflowOptions>._)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Pay_Booking_Via_Card_Workflow_With_Correct_Handle(
        [Frozen] ITemporalClient temporalClient,
        [Frozen] ITemporalHelperService temporalHelperService,
        TemporalService sut,
        SetPaymentStatusArgs args,
        string bookingId,
        string expectedWorkflowId,
        CancellationToken cancellationToken)
    {
        var workflowHandle = new WorkflowHandle<PayBookingViaCard>(temporalClient, expectedWorkflowId);

        A.CallTo(() => temporalHelperService.ToId($"paid_via_card-{bookingId}")).Returns(expectedWorkflowId);
        A.CallTo(() => temporalClient.GetWorkflowHandle<PayBookingViaCard>(expectedWorkflowId)).Returns(workflowHandle);

        await Should.ThrowAsync<NullReferenceException>(() =>
            sut.SignalPayBookingViaCardWorkflowAsync(bookingId, args, cancellationToken));

        A.CallTo(() => temporalClient.GetWorkflowHandle<PayBookingViaCard>(expectedWorkflowId)).MustHaveHappenedOnceExactly();
    }
}
