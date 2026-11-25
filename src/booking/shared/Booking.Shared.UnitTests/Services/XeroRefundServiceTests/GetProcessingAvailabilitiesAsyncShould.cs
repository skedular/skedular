using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

namespace Booking.Shared.UnitTests.Services.XeroRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetProcessingAvailabilitiesAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_Repository_Queries_For_Ineligible_Refunds(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var refunds = new List<MarketplaceRefund>
        {
            new()
            {
                Id = "refund-requested",
                Status = MarketplaceRefundStatusConstants.Requested,
            },
            new()
            {
                Id = "refund-empty",
                Status = MarketplaceRefundStatusConstants.Processing,
                RefundAmount = 0m,
            },
        };
        var sut = new XeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger);

        var result = await sut.GetProcessingAvailabilitiesAsync(refunds, cancellationToken);

        result.Keys.ShouldBe(refunds.Select(item => item.Id));
        result.Values.ShouldAllBe(item => !item.CanProcessInXero);
        A.CallTo(repositoryFactory).Where(call => call.Method.Name.Contains("Repository")).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Batch_Repository_Queries_For_Eligible_Refunds(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var refunds = new List<MarketplaceRefund>
        {
            new()
            {
                Id = "refund-1",
                LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                LocalEntityId = "booking-1",
                Status = MarketplaceRefundStatusConstants.Processing,
                RefundAmount = 10m,
            },
            new()
            {
                Id = "refund-2",
                LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
                LocalEntityId = "booking-2",
                Status = MarketplaceRefundStatusConstants.Completed,
                RefundAmount = 20m,
            },
        };
        var links = refunds.Select(refund => new AccountingInvoiceExportLink
        {
            Id = $"link-{refund.Id}",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = refund.LocalEntityType,
            LocalEntityId = refund.LocalEntityId,
            ExternalInvoiceId = $"invoice-{refund.Id}",
        }).ToList();
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(links);
        var sut = new XeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger);

        var result = await sut.GetProcessingAvailabilitiesAsync(refunds, cancellationToken);

        result.Values.ShouldAllBe(item => item.CanProcessInXero);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                A<string>._,
                A<string>._,
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Batch_Subscription_And_Invoice_Target_Queries(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var requestedAt = TimeProvider.System.GetUtcNow();
        var organization = new OrganizationEntity
        {
            Id = "org-1",
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
        };
        var productVersion = new ProductVersionEntity
        {
            Id = "product-version-1",
            Product = new ProductEntity
            {
                Id = "product-1",
                Organization = organization,
            },
        };
        var refunds = new List<MarketplaceRefund>();
        var subscriptions = new List<MarketplaceBookingSubscriptionEntity>();
        var links = new List<AccountingInvoiceExportLink>();
        for (var index = 1; index <= 2; index++)
        {
            var refund = new MarketplaceRefund
            {
                Id = $"refund-{index}",
                LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
                LocalEntityId = $"subscription-{index}",
                Status = MarketplaceRefundStatusConstants.Processing,
                RefundAmount = index * 10m,
                RequestedAt = requestedAt,
            };
            var recurringBooking = new RecurringBookingEntity
            {
                Id = $"recurring-{index}",
                StartDate = requestedAt.AddDays(-1),
                EndDate = requestedAt.AddDays(1),
                MarketplaceBooking = new MarketplaceBookingEntity
                {
                    ProductPricing = ProductPricing.Empty("pricing"),
                },
            };
            var subscription = new MarketplaceBookingSubscriptionEntity
            {
                Id = refund.LocalEntityId,
                StartedAt = requestedAt.AddDays(-2),
                MarketplaceBooking = new MarketplaceBookingEntity
                {
                    ProductVersion = productVersion,
                },
                RecurringBookings = [recurringBooking],
            };
            refunds.Add(refund);
            subscriptions.Add(subscription);
            links.Add(new AccountingInvoiceExportLink
            {
                Id = $"link-{index}",
                Provider = AccountingProviderConstants.Xero,
                LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
                LocalEntityId = recurringBooking.Id,
                ExternalInvoiceId = $"invoice-{index}",
            });
        }

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsForXeroRefundAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(subscriptions);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(links);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([]);
        var sut = new XeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger);

        var result = await sut.GetProcessingAvailabilitiesAsync(refunds, cancellationToken);

        result.Values.ShouldAllBe(item => item.CanProcessInXero);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsForXeroRefundAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync(
                A<string>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Prefer_Live_Recurring_Cycle_For_Active_Subscription_Batch(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var requestedAt = TimeProvider.System.GetUtcNow();
        var organization = new OrganizationEntity
        {
            Id = "org-live-cycle",
            BillingCycle = OrganizationBillingCycleConstants.Monthly,
        };
        var productVersion = new ProductVersionEntity
        {
            Id = "product-version-live-cycle",
            Product = new ProductEntity
            {
                Id = "product-live-cycle",
                Organization = organization,
            },
        };
        var liveBooking = new RecurringBookingEntity
        {
            Id = "recurring-live",
            StartDate = requestedAt.AddDays(-2),
            EndDate = requestedAt.AddDays(2),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty("live"),
            },
        };
        var deletedBooking = new RecurringBookingEntity
        {
            Id = "recurring-deleted",
            StartDate = requestedAt.AddDays(-1),
            EndDate = requestedAt.AddDays(3),
            DeletedAt = requestedAt.AddHours(-1),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty("deleted"),
            },
        };
        var subscription = new MarketplaceBookingSubscriptionEntity
        {
            Id = "subscription-live-cycle",
            Status = MarketplaceBookingSubscriptionStatusConstants.Active,
            StartedAt = requestedAt.AddDays(-3),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductVersion = productVersion,
            },
            RecurringBookings = [liveBooking, deletedBooking],
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-live-cycle",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = subscription.Id,
            Status = MarketplaceRefundStatusConstants.Processing,
            RefundAmount = 10m,
            RequestedAt = requestedAt,
        };
        AccountingInvoiceExportLink[] links =
        [
            new()
            {
                Id = "link-live",
                Provider = AccountingProviderConstants.Xero,
                LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
                LocalEntityId = liveBooking.Id,
                ExternalInvoiceId = "invoice-live",
            },
            new()
            {
                Id = "link-deleted",
                Provider = AccountingProviderConstants.Xero,
                LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
                LocalEntityId = deletedBooking.Id,
                ExternalInvoiceId = "template-deleted",
                ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            },
        ];
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsForXeroRefundAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([subscription]);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(links);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([]);
        var sut = new XeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger);

        var result = await sut.GetProcessingAvailabilitiesAsync([refund], cancellationToken);

        result[refund.Id].CanProcessInXero.ShouldBeTrue();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_Full_History_When_The_Primary_Subscription_Target_Is_Missing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var requestedAt = TimeProvider.System.GetUtcNow();
        var productVersion = new ProductVersionEntity
        {
            Id = "product-version-history",
            Product = new ProductEntity
            {
                Id = "product-history",
                Organization = new OrganizationEntity
                {
                    Id = "org-history",
                    BillingCycle = OrganizationBillingCycleConstants.Monthly,
                },
            },
        };
        var liveBooking = new RecurringBookingEntity
        {
            Id = "recurring-live",
            StartDate = requestedAt.AddDays(-2),
            EndDate = requestedAt.AddDays(2),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty("live"),
            },
        };
        var olderLiveBooking = new RecurringBookingEntity
        {
            Id = "recurring-older-live",
            StartDate = requestedAt.AddDays(-3),
            EndDate = requestedAt.AddDays(1),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty("older-live"),
            },
        };
        var historicalBooking = new RecurringBookingEntity
        {
            Id = "recurring-historical",
            StartDate = requestedAt.AddDays(-1),
            EndDate = requestedAt.AddDays(1),
            DeletedAt = requestedAt.AddHours(-1),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductPricing = ProductPricing.Empty("historical"),
            },
        };
        var lightSubscription = new MarketplaceBookingSubscriptionEntity
        {
            Id = "subscription-history",
            Status = MarketplaceBookingSubscriptionStatusConstants.Active,
            StartedAt = requestedAt.AddDays(-4),
            MarketplaceBooking = new MarketplaceBookingEntity
            {
                ProductVersion = productVersion,
            },
            RecurringBookings = [liveBooking, olderLiveBooking],
        };
        var fullSubscription = new MarketplaceBookingSubscriptionEntity
        {
            Id = lightSubscription.Id,
            Status = lightSubscription.Status,
            StartedAt = lightSubscription.StartedAt,
            MarketplaceBooking = lightSubscription.MarketplaceBooking,
            RecurringBookings = [historicalBooking, liveBooking, olderLiveBooking],
        };
        var refund = new MarketplaceRefund
        {
            Id = "refund-history",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = lightSubscription.Id,
            Status = MarketplaceRefundStatusConstants.Processing,
            RefundAmount = 10m,
            RequestedAt = requestedAt,
        };
        var olderLiveLink = new AccountingInvoiceExportLink
        {
            Id = "link-older-live",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = olderLiveBooking.Id,
            ExternalInvoiceId = "invoice-older-live",
        };
        var historicalLink = new AccountingInvoiceExportLink
        {
            Id = "link-historical",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = historicalBooking.Id,
            ExternalInvoiceId = "invoice-historical",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
        };
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsForXeroRefundAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([lightSubscription]);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([fullSubscription]);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntitiesAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .ReturnsLazily(call => Task.FromResult<IReadOnlyList<AccountingInvoiceExportLink>>(
                call.GetArgument<IReadOnlyCollection<string>>(2)!.Contains(historicalBooking.Id)
                    ? [historicalLink]
                    : call.GetArgument<IReadOnlyCollection<string>>(2)!.Contains(olderLiveBooking.Id)
                        ? [olderLiveLink]
                        : []));
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns([]);
        var sut = new XeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger);

        var result = await sut.GetProcessingAvailabilitiesAsync([refund], cancellationToken);

        result[refund.Id].CanProcessInXero.ShouldBeFalse();
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdsAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
