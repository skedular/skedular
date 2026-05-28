using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Grpc.Core;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using Product = Booking.Shared.Database.Entities.Product;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;

namespace Booking.Shared.UnitTests.Services.XeroRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetProcessingAvailabilityAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Blocked_Reason_When_Subscription_Invoice_Instance_Has_Not_Been_Correlated(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] CallInvoker callInvoker,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.PendingAccounting,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            RefundAmount = 25m,
            RequestedAt = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero)
        };
        var subscription = CreateSubscriptionForRefund("INV-APR-2", "https://xero.example/inv-apr-2");
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-2",
            ExternalInvoiceId = "7f868f37-4567-4de3-9cdc-1d80dafc4fe8",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalInvoiceNumber = "TEMPLATE-001"
        };
        var sut = new TestableXeroRefundService(organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker), repositoryFactory, xeroSdkClientFactory,
            xeroTokenEncryptionService, TimeProvider.System);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("subscription-1", cancellationToken)).Returns(subscription);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                "recurring-booking-2",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync("link-1", cancellationToken))
            .Returns([]);

        var result = await sut.GetProcessingAvailabilityAsync(refund, cancellationToken);

        result.CanProcessInXero.ShouldBeFalse();
        result.BlockedReason.ShouldNotBeNull();
        result.BlockedReason.ShouldContain("concrete Xero invoice instance");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Available_When_Subscription_Refund_Has_A_Correlated_Invoice_Instance(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen] CallInvoker callInvoker,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.PendingAccounting,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            RefundAmount = 25m,
            RequestedAt = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero)
        };
        var subscription = CreateSubscriptionForRefund("INV-APR-2", "https://xero.example/inv-apr-2");
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-2",
            ExternalInvoiceId = "7f868f37-4567-4de3-9cdc-1d80dafc4fe8",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalInvoiceNumber = "TEMPLATE-001"
        };
        var invoiceInstance = new AccountingInvoiceInstance
        {
            Id = "instance-1",
            AccountingInvoiceExportLinkId = "link-1",
            Provider = AccountingProviderConstants.Xero,
            ExternalInvoiceId = "4dded1af-9766-4b33-8d51-c11f509c466f",
            ExternalInvoiceNumber = "INV-APR-2",
            ExternalInvoiceUrl = "https://xero.example/inv-apr-2",
            ExternalStatus = AccountingStatusConstants.Exported,
            OrganizationId = "org-1"
        };
        var sut = new TestableXeroRefundService(organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker), repositoryFactory, xeroSdkClientFactory,
            xeroTokenEncryptionService, TimeProvider.System);

        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("subscription-1", cancellationToken)).Returns(subscription);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                "recurring-booking-2",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync("link-1", cancellationToken))
            .Returns([invoiceInstance]);

        var result = await sut.GetProcessingAvailabilityAsync(refund, cancellationToken);

        result.CanProcessInXero.ShouldBeTrue();
        result.BlockedReason.ShouldBeNull();
    }

    private static MarketplaceBookingSubscriptionEntity CreateSubscriptionForRefund(
        string recurringBookingInvoiceNumber,
        string recurringBookingInvoiceUrl) =>
        new()
        {
            Id = "subscription-1",
            StartedAt = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
            Status = MarketplaceBookingSubscriptionStatusConstants.Active,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = "subscription-marketplace-booking",
                ProductPricing = ProductPricing.Empty("pricing-1") with { PurchaseCadence = ProductPricingCadence.Monthly },
                ProductVersion = new ProductVersion
                {
                    Id = "pv-subscription",
                    Product = new Product { Organization = new Organization { BillingCycle = OrganizationBillingCycleConstants.Monthly } }
                }
            },
            RecurringBookings =
            [
                new RecurringBookingEntity
                {
                    Id = "recurring-booking-1",
                    StartDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    EndDate = new DateTimeOffset(2026, 4, 1, 0, 0, 0, TimeSpan.Zero),
                    MarketplaceBooking = new MarketplaceBooking
                    {
                        Id = "marketplace-booking-1",
                        InvoiceNumber = "INV-APR-1",
                        InvoiceUrl = "https://xero.example/inv-apr-1",
                        ProductPricing =
                            ProductPricing.Empty("pricing-recurring-1") with { PurchaseCadence = ProductPricingCadence.Daily },
                        ProductVersion = new ProductVersion
                        {
                            Product = new Product { Organization = new Organization { BillingCycle = OrganizationBillingCycleConstants.Monthly } }
                        }
                    }
                },
                new RecurringBookingEntity
                {
                    Id = "recurring-booking-2",
                    StartDate = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
                    EndDate = new DateTimeOffset(2026, 4, 2, 0, 0, 0, TimeSpan.Zero),
                    MarketplaceBooking = new MarketplaceBooking
                    {
                        Id = "marketplace-booking-2",
                        InvoiceNumber = recurringBookingInvoiceNumber,
                        InvoiceUrl = recurringBookingInvoiceUrl,
                        ProductPricing =
                            ProductPricing.Empty("pricing-recurring-2") with { PurchaseCadence = ProductPricingCadence.Daily },
                        ProductVersion = new ProductVersion
                        {
                            Product = new Product { Organization = new Organization { BillingCycle = OrganizationBillingCycleConstants.Monthly } }
                        }
                    }
                }
            ]
        };

    private sealed class TestableXeroRefundService(
        OrganizationConfiguration organizationConfiguration,
        OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
        IRepositoryFactory repositoryFactory,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider)
        : XeroRefundService(
            organizationConfiguration,
            organizationBillingServiceClient,
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider)
    {
        protected override Task<Invoices> GetInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid invoiceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(new Invoices());

        protected override Task<CreditNotes> CreateCreditNotesAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            CreditNotes creditNotes,
            string idempotencyKey,
            CancellationToken cancellationToken) =>
            Task.FromResult(new CreditNotes());
    }
}
