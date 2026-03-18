using Api.Shared.Services;
using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using FakeItEasy;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Testing.Shared;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingSubscriptionServiceTests;

public class MarketplaceBookingSubscriptionServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingSubscriptionCancellationNotAllowed_When_User_Delete_Has_No_Cancellation_Policy(
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            now.AddDays(7),
            ProductPricingCancellationPolicyType.NoCancellation,
            []);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Act & Assert
        await Should.ThrowAsync<MarketplaceBookingSubscriptionCancellationNotAllowed>(() =>
            sut.DeleteAsync(existingSubscription, deletedByCustomer, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Throws_MarketplaceBookingSubscriptionCancellationNotAllowed_When_User_Delete_Is_After_Cancellation_Deadline(
        [Frozen] TimeProvider timeProvider,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            now.AddMinutes(20),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(30, 100)]);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        // Act & Assert
        await Should.ThrowAsync<MarketplaceBookingSubscriptionCancellationNotAllowed>(() =>
            sut.DeleteAsync(existingSubscription, deletedByCustomer, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_User_Delete_When_Inside_Cancellation_Window(
        [Frozen] TimeProvider timeProvider,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IMapper mapper,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        // Arrange
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            now.AddDays(3),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(120, 100)]);
        var deletedSubscription = new Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Remove(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(deletedSubscription);

        // Act
        var result = await sut.DeleteAsync(existingSubscription, deletedByCustomer, cancellationToken);

        // Assert
        result.ShouldBe(deletedSubscription);
        existingSubscription.DeletedByCustomer.ShouldBe(deletedByCustomer);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    private static MarketplaceBookingSubscription CreateSubscription(
        DateTimeOffset startedAt,
        DateTimeOffset? nextRenewalAt,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        ICollection<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
        new()
        {
            Id = "subscription-1",
            StartedAt = startedAt,
            NextRenewalAt = nextRenewalAt,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    CancellationPolicyType = cancellationPolicyType, CancellationRefundRules = cancellationRefundRules
                }
            }
        };
}
