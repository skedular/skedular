using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Temporalio.Exceptions;
using Temporalio.Testing;
using Testing.Shared.Assertions;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingFailureNotificationIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DispatchShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Persist_A_Failed_Email_Then_Fail_The_Activity_For_Temporal_Retry(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        IMarketplaceBookingFailureDeliveryRepository deliveryRepository,
        [Frozen]
        IMarketplaceBookingFailureEventRepository eventRepository,
        [Frozen]
        IMarketplaceBookingFailureNotificationService notificationService,
        [Frozen]
        IEmailService emailService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingFailureNotificationIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var delivery = new MarketplaceBookingFailureDelivery
        {
            Id = "delivery-1",
            Audience = MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
            Channel = MarketplaceBookingFailureDeliveryChannelConstants.Email,
            RecipientEmail = "customer@example.test",
            Status = MarketplaceBookingFailureDeliveryStatusConstants.Pending,
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.OneTimeBooking,
            Deliveries = [delivery],
        };

        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureDeliveryRepository).Returns(deliveryRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureEventRepository).Returns(eventRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => failureRepository.GetByIdAsync(failure.Id, environment.CancellationTokenSource.Token)).Returns(failure);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, environment.CancellationTokenSource.Token)).Returns(transaction);
        A.CallTo(() => notificationService.RenderAsync(failure, false, "there", environment.CancellationTokenSource.Token))
            .Returns(("subject", "text", "html"));
        A.CallTo(() => emailService.SendRawEmailAsync(
                A<string>._, A<string>._, A<string>._, A<string>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
                A<IReadOnlyList<string>>._, A<IReadOnlyList<EmailAttachment>>._,
                environment.CancellationTokenSource.Token))
            .ThrowsAsync(new InvalidOperationException("mail unavailable"));

        await Should.ThrowAsync<ApplicationFailureException>(() => environment.RunAsync(() =>
            sut.DispatchAsync(new DispatchMarketplaceBookingFailureNotificationsInput(failure.Id))));

        delivery.Status.ShouldBe(MarketplaceBookingFailureDeliveryStatusConstants.Failed);
        delivery.AttemptCount.ShouldBe(1);
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => eventRepository.Add(A<MarketplaceBookingFailureEvent>.That.Matches(item =>
            item.EventType == MarketplaceBookingFailureEventTypeConstants.DeliveryFailed))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Existing_InApplication_Delivery_Sent_Without_Creating_A_Duplicate(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingFailureRepository failureRepository,
        [Frozen]
        IMarketplaceBookingFailureDeliveryRepository deliveryRepository,
        [Frozen]
        IMarketplaceBookingFailureEventRepository eventRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        ILogger<MarketplaceBookingFailureNotificationIntegrations> logger,
        MarketplaceBookingFailureNotificationIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var delivery = new MarketplaceBookingFailureDelivery
        {
            Id = "delivery-1",
            Audience = MarketplaceBookingFailureDeliveryAudienceConstants.Customer,
            Channel = MarketplaceBookingFailureDeliveryChannelConstants.InApplication,
            Status = MarketplaceBookingFailureDeliveryStatusConstants.Pending,
        };
        var failure = new MarketplaceBookingFailure
        {
            Id = "failure-1",
            Category = MarketplaceBookingFailureCategoryConstants.AvailabilityConflict,
            Scope = MarketplaceBookingFailureScopeConstants.RecurringOccurrence,
            Deliveries = [delivery],
        };

        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureRepository).Returns(failureRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureDeliveryRepository).Returns(deliveryRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingFailureEventRepository).Returns(eventRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => failureRepository.GetByIdAsync(failure.Id, environment.CancellationTokenSource.Token)).Returns(failure);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, environment.CancellationTokenSource.Token)).Returns(transaction);

        await environment.RunAsync(() =>
            sut.DispatchAsync(new DispatchMarketplaceBookingFailureNotificationsInput(failure.Id)));

        delivery.Status.ShouldBe(MarketplaceBookingFailureDeliveryStatusConstants.Sent);
        delivery.AttemptCount.ShouldBe(1);
        A.CallTo(() => deliveryRepository.Update(delivery)).MustHaveHappenedOnceExactly();
        A.CallTo(() => eventRepository.Add(A<MarketplaceBookingFailureEvent>.That.Matches(item =>
            item.MarketplaceBookingFailureId == failure.Id &&
            item.EventType == MarketplaceBookingFailureEventTypeConstants.DeliverySucceeded))).MustHaveHappenedOnceExactly();
        LogAssertions.ACallToLogInfoContaining(logger, "Dispatching marketplace booking failure notifications")
            .MustHaveHappenedOnceExactly();
    }
}
