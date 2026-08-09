using Booking.Shared.Activities;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Email;
using Microsoft.EntityFrameworkCore.Storage;
using Temporalio.Testing;

namespace Booking.Shared.UnitTests.Activities.MarketplaceBookingModificationIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DispatchShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Unresolved_Recipient_For_Recovery(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingModificationRepository modificationRepository,
        [Frozen]
        IEmailService emailService,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IDbContextTransaction transaction,
        MarketplaceBookingModificationNotificationIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var modification = new MarketplaceBookingModification
        {
            Id = "modification-1",
        };
        var delivery = new MarketplaceBookingModificationNotificationDelivery
        {
            Id = "delivery-1",
            MarketplaceBookingModificationId = modification.Id,
            RecipientCustomerId = null,
            Status = MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending,
        };
        modification.NotificationDeliveries = [delivery];

        A.CallTo(() => repositoryFactory.MarketplaceBookingModificationRepository).Returns(modificationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => modificationRepository.GetByIdAsync(modification.Id, environment.CancellationTokenSource.Token)).Returns(modification);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, environment.CancellationTokenSource.Token)).Returns(transaction);

        await environment.RunAsync(() =>
            sut.DispatchMarketplaceBookingModificationAsync(new DispatchMarketplaceBookingModificationNotificationInput(modification.Id)));

        delivery.Status.ShouldBe(MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired);
        delivery.LastError.ShouldNotBeNull();
        delivery.LastError!.ShouldContain("recipient");
        A.CallTo(() => emailService.SendRawEmailAsync(
            A<string>._, A<string>._, A<string>._, A<string>._,
            A<IReadOnlyList<string>>._, A<IReadOnlyList<string>>._,
            A<IReadOnlyList<string>>._, A<IReadOnlyList<EmailAttachment>>._,
            environment.CancellationTokenSource.Token)).MustNotHaveHappened();
    }
}
