using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Mappers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using ProductVersion = Booking.Shared.Models.ProductVersion;

namespace Booking.Shared.UnitTests.Services.MarketplaceBookingSubscriptionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class MarketplaceBookingSubscriptionServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task AddAsync_Throws_MarketplaceEventProductRecurringBookingNotSupported_When_Product_Is_Event(
        [Frozen] IRepositoryFactory repositoryFactory,
        MarketplaceBookingSubscriptionService sut,
        ICustomerRepository customerRepository,
        IProductVersionRepository productVersionRepository,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1" };
        var subscription = new Shared.Models.MarketplaceBookingSubscription
        {
            InvolvedCustomers = [new Shared.Models.Customer { Id = "customer-1" }],
            MarketplaceBooking = new Shared.Models.MarketplaceBooking
            {
                ProductVersion = new ProductVersion { Id = "product-version-1" },
                ProductPricing = ProductPricing.Empty("pricing-1"),
                PaymentMethod = PaymentMethod.Card
            }
        };
        var productVersion = new Database.Entities.ProductVersion
        {
            Id = "product-version-1",
            Type = ProductTypeConstants.Event,
            OrganizationTags = [new OrganizationTag { Type = OrganizationTagTypeConstants.Product }],
            PricingOptions = [ProductPricing.Empty("pricing-1")],
            Product = new Product { Organization = new Organization { Id = "org-1" } }
        };

        A.CallTo(() => repositoryFactory.CustomerRepository).Returns(customerRepository);
        A.CallTo(() => repositoryFactory.ProductVersionRepository).Returns(productVersionRepository);
        A.CallTo(() => customerRepository.GetByIdsAsync(A<IReadOnlyList<string>>.That.Contains("customer-1"), true, cancellationToken))
            .Returns([customer]);
        A.CallTo(() => productVersionRepository.GetByIdAsync("product-version-1", cancellationToken))
            .Returns(productVersion);

        await Should.ThrowAsync<MarketplaceEventProductRecurringBookingNotSupported>(() =>
            sut.AddAsync(subscription, customer, [], [], cancellationToken));
    }

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
            sut.DeleteAsync(existingSubscription, deletedByCustomer, MarketplaceBookingSubscriptionCancellationMode.Immediate, cancellationToken));
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
            sut.DeleteAsync(existingSubscription, deletedByCustomer, MarketplaceBookingSubscriptionCancellationMode.Immediate, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_User_Delete_When_Inside_Cancellation_Window(
        [Frozen] TimeProvider timeProvider,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
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
        var deletedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Remove(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(deletedSubscription);

        // Act
        var result = await sut.DeleteAsync(
            existingSubscription,
            deletedByCustomer,
            MarketplaceBookingSubscriptionCancellationMode.Immediate,
            cancellationToken);

        // Assert
        result.ShouldBe(deletedSubscription);
        existingSubscription.LastModifiedByCustomer.ShouldBe(deletedByCustomer);
        existingSubscription.CancelledAt.ShouldBe(now);
        existingSubscription.Status.ShouldBe(MarketplaceBookingSubscriptionStatus.Cancelled.ToMarketplaceBookingSubscriptionStatus());
        existingSubscription.AutoRenew.ShouldBeFalse();
        existingSubscription.CancelAtPeriodEnd.ShouldBeFalse();
        A.CallTo(() => marketplaceRefundService.CreateImmediateSubscriptionCancellationRefundAsync(existingSubscription, deletedByCustomer,
            cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Remove(existingSubscription)).MustNotHaveHappened();
        A.CallTo(() => temporalOutboxService.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
                existingSubscription.Id,
                unitOfWork))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Schedules_Cancellation_At_Period_End_When_Requested(
        [Frozen] TimeProvider timeProvider,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] IMarketplaceRefundService marketplaceRefundService,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] IMapper mapper,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        MarketplaceBookingSubscriptionService sut,
        CancellationToken cancellationToken)
    {
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var nextRenewalAt = now.AddDays(3);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            nextRenewalAt,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(120, 100)]);
        existingSubscription.AutoRenew = true;
        var updatedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(updatedSubscription);

        var result = await sut.DeleteAsync(
            existingSubscription,
            deletedByCustomer,
            MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
            cancellationToken);

        result.ShouldBe(updatedSubscription);
        existingSubscription.CancelledAt.ShouldBe(now);
        existingSubscription.NextRenewalAt.ShouldBe(nextRenewalAt);
        existingSubscription.AutoRenew.ShouldBeFalse();
        existingSubscription.DeletedByCustomer.ShouldBeNull();
        existingSubscription.LastModifiedByCustomer.ShouldBe(deletedByCustomer);
        A.CallTo(() => marketplaceRefundService.CreateImmediateSubscriptionCancellationRefundAsync(
                existingSubscription,
                deletedByCustomer,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Remove(existingSubscription)).MustNotHaveHappened();
        A.CallTo(() => temporalOutboxService.SignalWorkflowBookMarketplaceBookingSubscriptionResourcesDeleted(
                existingSubscription.Id,
                unitOfWork))
            .MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Derives_Next_Renewal_At_When_Scheduling_Cancellation_At_Period_End(
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
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            null,
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(120, 100)]);
        existingSubscription.AutoRenew = true;
        existingSubscription.MarketplaceBooking.ProductPricing = existingSubscription.MarketplaceBooking.ProductPricing with
        {
            PurchaseCadence = ProductPricingCadence.Monthly
        };
        var updatedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(updatedSubscription);

        _ = await sut.DeleteAsync(
            existingSubscription,
            deletedByCustomer,
            MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
            cancellationToken);

        existingSubscription.NextRenewalAt.ShouldBe(existingSubscription.StartedAt.AddMonths(1));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_At_Period_End_Cancellation_Even_When_Immediate_Cancellation_Window_Has_Passed(
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
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            now.AddMinutes(20),
            ProductPricingCancellationPolicyType.FullRefundBeforeCutoff,
            [new ProductPricingCancellationRefundRule(30, 100)]);
        existingSubscription.AutoRenew = true;
        var updatedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(updatedSubscription);

        var result = await sut.DeleteAsync(
            existingSubscription,
            deletedByCustomer,
            MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
            cancellationToken);

        result.ShouldBe(updatedSubscription);
        existingSubscription.CancelAtPeriodEnd.ShouldBeTrue();
        existingSubscription.AutoRenew.ShouldBeFalse();
        existingSubscription.DeletedByCustomer.ShouldBeNull();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task DeleteAsync_Allows_At_Period_End_Cancellation_When_Product_Has_No_Immediate_Cancellation_Policy(
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
        var deletedByCustomer = new Customer();
        var now = new DateTimeOffset(2026, 3, 18, 8, 0, 0, TimeSpan.Zero);
        var existingSubscription = CreateSubscription(
            now.AddDays(-1),
            now.AddDays(7),
            ProductPricingCancellationPolicyType.NoCancellation,
            []);
        existingSubscription.AutoRenew = true;
        var updatedSubscription = new Shared.Models.MarketplaceBookingSubscription { Id = existingSubscription.Id };

        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.Update(existingSubscription)).Returns(existingSubscription);
        A.CallTo(() => mapper.MapTo(existingSubscription)).Returns(updatedSubscription);

        var result = await sut.DeleteAsync(
            existingSubscription,
            deletedByCustomer,
            MarketplaceBookingSubscriptionCancellationMode.AtPeriodEnd,
            cancellationToken);

        result.ShouldBe(updatedSubscription);
        existingSubscription.CancelAtPeriodEnd.ShouldBeTrue();
        existingSubscription.AutoRenew.ShouldBeFalse();
        existingSubscription.DeletedByCustomer.ShouldBeNull();
    }

    private static MarketplaceBookingSubscription CreateSubscription(
        DateTimeOffset startedAt,
        DateTimeOffset? nextRenewalAt,
        ProductPricingCancellationPolicyType cancellationPolicyType,
        IReadOnlyList<ProductPricingCancellationRefundRule> cancellationRefundRules) =>
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
