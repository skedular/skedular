using System.Reflection;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using MarketplaceBooking = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using Organization = Booking.Shared.Database.Entities.Organization;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using Product = Booking.Shared.Database.Entities.Product;
using ProductVersion = Booking.Shared.Database.Entities.ProductVersion;
using XeroInvoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;

namespace Booking.Shared.UnitTests.Services.XeroRefundServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ProcessAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Not_Match_A_Credit_Note_Without_A_Settlement(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        AccountingApi accountingApi,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var creditNoteId = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f");
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            ExternalRefundId = creditNoteId.ToString(),
            RefundAmount = 25m,
            Status = MarketplaceRefundStatusConstants.Completed,
        };
        var connection = CreateXeroConnection();
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory, xeroSdkClientFactory, xeroTokenEncryptionService, TimeProvider.System, logger)
        {
            CreditNoteResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = creditNoteId,
                    },
                ],
            },
            PaymentResponse = new Payments(),
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(connection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ReconcileAsync(refund, TimeProvider.System.GetUtcNow().AddDays(-1), cancellationToken);

        result.ShouldBeFalse();
        refund.ReconciliationStatus.ShouldBe("Unsettled");
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Match_A_Credit_Note_When_The_Settlement_Payment_Exists(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        AccountingApi accountingApi,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var creditNoteId = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f");
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            ExternalRefundId = creditNoteId.ToString(),
            RefundAmount = 25m,
            Status = MarketplaceRefundStatusConstants.Completed,
        };
        var connection = CreateXeroConnection();
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory, xeroSdkClientFactory, xeroTokenEncryptionService, TimeProvider.System, logger)
        {
            CreditNoteResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = creditNoteId,
                    },
                ],
            },
            PaymentResponse = new Payments
            {
                _Payments =
                [
                    new Payment
                    {
                        CreditNote = new CreditNote
                        {
                            CreditNoteID = creditNoteId,
                        },
                        Amount = 25m,
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(connection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ReconcileAsync(refund, TimeProvider.System.GetUtcNow().AddDays(-1), cancellationToken);

        result.ShouldBeTrue();
        refund.ReconciliationStatus.ShouldBe("Matched");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Find_An_Unchanged_Credit_Note_By_Id_When_It_Is_Outside_The_Modified_Since_Window(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        AccountingApi accountingApi,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var creditNoteId = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f");
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            OrganizationId = "org-1",
            ExternalRefundId = creditNoteId.ToString(),
            RefundAmount = 25m,
            Status = MarketplaceRefundStatusConstants.Completed,
        };
        var connection = CreateXeroConnection();
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory, xeroSdkClientFactory, xeroTokenEncryptionService, TimeProvider.System, logger)
        {
            CreditNoteResponse = new CreditNotes(),
            CreditNoteByIdResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = creditNoteId,
                    },
                ],
            },
            PaymentResponse = new Payments(),
            HistoricalPaymentResponse = new Payments
            {
                _Payments =
                [
                    new Payment
                    {
                        CreditNote = new CreditNote
                        {
                            CreditNoteID = creditNoteId,
                        },
                        Amount = 25m,
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(connection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ReconcileAsync(refund, TimeProvider.System.GetUtcNow().AddDays(-1), cancellationToken);

        result.ShouldBeTrue();
        refund.ReconciliationStatus.ShouldBe("Matched");
        sut.CreditNoteByIdRequested.ShouldBeTrue();
        sut.HistoricalPaymentLookupRequested.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Failed_When_Refund_Is_Not_For_A_One_Time_Marketplace_Booking(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            LocalEntityType = "UnsupportedEntity",
        };
        var sut = new TestableXeroRefundService(organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker), repositoryFactory, xeroSdkClientFactory,
            xeroTokenEncryptionService, TimeProvider.System, logger);

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        result.LastError.ShouldNotBeNull();
        result.LastError.ShouldContain("supports only marketplace bookings and subscription billing windows");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Failed_When_Original_Xero_Invoice_Link_Is_Missing(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            RefundAmount = 25m,
        };
        var sut = new TestableXeroRefundService(organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker), repositoryFactory, xeroSdkClientFactory,
            xeroTokenEncryptionService, TimeProvider.System, logger);

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns((AccountingInvoiceExportLink?)null);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        result.LastError.ShouldNotBeNull();
        result.LastError.ShouldContain("original Xero invoice link");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_Refund_When_Xero_Credit_Note_Is_Created(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        AccountingApi accountingApi,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            RefundAmount = 25m,
            Currency = "NZD",
            RequestedAt = new DateTimeOffset(2026, 4, 7, 10, 0, 0, TimeSpan.Zero),
        };
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            ExternalInvoiceId = "4dded1af-9766-4b33-8d51-c11f509c466f",
            ExternalInvoiceNumber = "INV-001",
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = "tenant-1",
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(TimeProvider.System.GetUtcNow().AddMinutes(30)),
        };
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    CreateInvoice(
                        Guid.Parse(invoiceLink.ExternalInvoiceId),
                        0m,
                        [
                            new Payment
                            {
                                Account = new Account
                                {
                                    Code = "090",
                                },
                                Code = "090",
                            },
                        ],
                        [
                            new LineItem
                            {
                                AccountCode = "200",
                                TaxType = "OUTPUT2",
                            },
                        ]),
                ],
            },
            CreditNoteResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f"),
                        CreditNoteNumber = "CN-001",
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        result.AccountingProvider.ShouldBe(AccountingProviderConstants.Xero);
        result.ExternalRefundId.ShouldBe("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f");
        result.ExternalRefundNumber.ShouldBe("CN-001");
        result.LastError.ShouldBeNull();
        sut.CapturedCreditNotes.ShouldNotBeNull();
        sut.CapturedCreditNotes!._CreditNotes.ShouldHaveSingleItem();
        sut.CapturedCreditNotes._CreditNotes![0].LineItems.ShouldHaveSingleItem();
        sut.CapturedCreditNotes._CreditNotes[0].LineItems![0].AccountCode.ShouldBe("200");
        sut.CapturedCreditNotes._CreditNotes[0].LineItems[0].TaxType.ShouldBe("OUTPUT2");
        sut.CapturedAllocations.ShouldBeNull();
        sut.CapturedPayment.ShouldNotBeNull();
        sut.CapturedPayment!.CreditNote.ShouldNotBeNull();
        sut.CapturedPayment.CreditNote!.CreditNoteID.ShouldBe(Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f"));
        sut.CapturedPayment.Account.ShouldNotBeNull();
        sut.CapturedPayment.Account!.Code.ShouldBe("090");
        sut.CapturedPayment.Amount.ShouldBe(25m);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Allocate_Credit_Note_When_Original_Invoice_Still_Has_Outstanding_Balance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        AccountingApi accountingApi,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            RefundAmount = 25m,
            Currency = "NZD",
            RequestedAt = new DateTimeOffset(2026, 4, 7, 10, 0, 0, TimeSpan.Zero),
        };
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            ExternalInvoiceId = "4dded1af-9766-4b33-8d51-c11f509c466f",
            ExternalInvoiceNumber = "INV-001",
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = "tenant-1",
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(TimeProvider.System.GetUtcNow().AddMinutes(30)),
        };
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    CreateInvoice(
                        Guid.Parse(invoiceLink.ExternalInvoiceId),
                        25m,
                        null,
                        [
                            new LineItem
                            {
                                AccountCode = "200",
                                TaxType = "OUTPUT2",
                            },
                        ]),
                ],
            },
            CreditNoteResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f"),
                        CreditNoteNumber = "CN-001",
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        sut.CapturedPayment.ShouldBeNull();
        sut.CapturedAllocations.ShouldNotBeNull();
        sut.CapturedAllocations!._Allocations.ShouldHaveSingleItem();
        sut.CapturedAllocations._Allocations![0].Amount.ShouldBe(25m);
        sut.CapturedAllocations._Allocations[0].Invoice!.InvoiceID.ShouldBe(Guid.Parse(invoiceLink.ExternalInvoiceId));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Failed_When_Subscription_Refund_Cannot_Be_Correlated_To_A_Concrete_Invoice_Instance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            RefundAmount = 25m,
            RequestedAt = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero),
        };
        var subscription = CreateSubscriptionForRefund(
            "INV-APR-2",
            "https://xero.example/inv-apr-2");
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-2",
            ExternalInvoiceId = "7f868f37-4567-4de3-9cdc-1d80dafc4fe8",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalInvoiceNumber = "TEMPLATE-001",
        };
        var sut = new TestableXeroRefundService(organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker), repositoryFactory, xeroSdkClientFactory,
            xeroTokenEncryptionService, TimeProvider.System, logger);

        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
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
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        result.LastError.ShouldNotBeNull();
        result.LastError.ShouldContain("concrete Xero invoice instance");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Complete_Subscription_Refund_When_Current_Billing_Window_Resolves_To_A_Concrete_Invoice_Instance(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen]
        IMarketplaceBookingSubscriptionRepository marketplaceBookingSubscriptionRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        AccountingApi accountingApi,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            LocalEntityId = "subscription-1",
            RefundAmount = 25m,
            Currency = "NZD",
            RequestedAt = new DateTimeOffset(2026, 4, 20, 10, 0, 0, TimeSpan.Zero),
        };
        var subscription = CreateSubscriptionForRefund(
            "INV-APR-2",
            "https://xero.example/inv-apr-2");
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-2",
            ExternalInvoiceId = "7f868f37-4567-4de3-9cdc-1d80dafc4fe8",
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalInvoiceNumber = "TEMPLATE-001",
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
            OrganizationId = "org-1",
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = "tenant-1",
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(TimeProvider.System.GetUtcNow().AddMinutes(30)),
        };
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    CreateInvoice(
                        Guid.Parse(invoiceInstance.ExternalInvoiceId),
                        0m,
                        [
                            new Payment
                            {
                                Account = new Account
                                {
                                    Code = "090",
                                },
                                Code = "090",
                            },
                        ],
                        [
                            new LineItem
                            {
                                AccountCode = "200",
                                TaxType = "OUTPUT2",
                            },
                        ]),
                ],
            },
            CreditNoteResponse = new CreditNotes
            {
                _CreditNotes =
                [
                    new CreditNote
                    {
                        CreditNoteID = Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f"),
                        CreditNoteNumber = "CN-001",
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => repositoryFactory.MarketplaceBookingSubscriptionRepository).Returns(marketplaceBookingSubscriptionRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => marketplaceBookingSubscriptionRepository.GetByIdAsync("subscription-1", cancellationToken)).Returns(subscription);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                "recurring-booking-2",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync("link-1", cancellationToken))
            .Returns([invoiceInstance]);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Completed);
        result.AccountingProvider.ShouldBe(AccountingProviderConstants.Xero);
        result.ExternalRefundId.ShouldBe("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f");
        result.ExternalRefundNumber.ShouldBe("CN-001");
        result.LastError.ShouldBeNull();
        sut.CapturedAllocations.ShouldBeNull();
        sut.CapturedPayment.ShouldNotBeNull();
        sut.CapturedPayment!.Account!.Code.ShouldBe("090");
        sut.CapturedPayment.CreditNote!.CreditNoteID.ShouldBe(Guid.Parse("eb8e81fe-ebfb-47c6-9d0c-0fdc40d4eb0f"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Failed_When_Original_Xero_Invoice_Does_Not_Provide_A_Tax_Type(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IMarketplaceRefundRepository marketplaceRefundRepository,
        [Frozen]
        IAccountingInvoiceExportLinkRepository accountingInvoiceExportLinkRepository,
        [Frozen]
        IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen]
        AccountingApi accountingApi,
        [Frozen]
        CallInvoker callInvoker,
        [Frozen]
        OrganizationConfiguration organizationConfiguration,
        ILogger<XeroRefundService> logger,
        CancellationToken cancellationToken)
    {
        organizationConfiguration.ApiKey = "api-key";
        var refund = new MarketplaceRefund
        {
            Id = "refund-1",
            Status = MarketplaceRefundStatusConstants.Processing,
            OrganizationId = "org-1",
            LocalEntityType = MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            RefundAmount = 25m,
            Currency = "NZD",
            RequestedAt = new DateTimeOffset(2026, 4, 7, 10, 0, 0, TimeSpan.Zero),
        };
        var invoiceLink = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            OrganizationId = "org-1",
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            ExternalInvoiceId = "4dded1af-9766-4b33-8d51-c11f509c466f",
            ExternalInvoiceNumber = "INV-001",
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = "tenant-1",
            IsActive = true,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(TimeProvider.System.GetUtcNow().AddMinutes(30)),
            DefaultSalesAccountCode = "200",
        };
        var sut = new TestableXeroRefundService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            TimeProvider.System,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    new XeroInvoice
                    {
                        InvoiceID = Guid.Parse(invoiceLink.ExternalInvoiceId),
                        Contact = new Contact
                        {
                            ContactID = Guid.Parse("050f7fd4-f1d7-465b-8bd2-31ae88daecf7"),
                        },
                        LineAmountTypes = LineAmountTypes.Inclusive,
                        CurrencyCode = CurrencyCode.NZD,
                        LineItems =
                        [
                            new LineItem
                            {
                                AccountCode = "200",
                            },
                        ],
                    },
                ],
            },
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceExportLinkRepository);
        A.CallTo(() => repositoryFactory.MarketplaceRefundRepository).Returns(marketplaceRefundRepository);
        A.CallTo(() => accountingInvoiceExportLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                "marketplace-booking-1",
                cancellationToken))
            .Returns(invoiceLink);
        A.CallTo(() => marketplaceRefundRepository.Update(refund)).Returns(refund);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);

        var result = await sut.ProcessAsync(refund, cancellationToken);

        result.Status.ShouldBe(MarketplaceRefundStatusConstants.Failed);
        result.LastError.ShouldNotBeNull();
        result.LastError.ShouldContain("tax type");
    }

    private static AsyncUnaryCall<XeroConnection> CreateResponse(XeroConnection response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private static XeroConnection CreateXeroConnection() => new()
    {
        Id = "xero-1",
        TenantId = "tenant-1",
        IsActive = true,
        AccessTokenEncrypted = "encrypted-access",
        AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(TimeProvider.System.GetUtcNow().AddMinutes(30)),
    };

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
                ProductPricing = ProductPricing.Empty("pricing-1") with
                {
                    PurchaseCadence = ProductPricingCadence.Monthly,
                },
                ProductVersion = new ProductVersion
                {
                    Id = "pv-subscription",
                    Product = new Product
                    {
                        Organization = new Organization
                        {
                            BillingCycle = OrganizationBillingCycleConstants.Monthly,
                        },
                    },
                },
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
                            ProductPricing.Empty("pricing-recurring-1") with
                            {
                                PurchaseCadence = ProductPricingCadence.Daily,
                            },
                        ProductVersion = new ProductVersion
                        {
                            Product = new Product
                            {
                                Organization = new Organization
                                {
                                    BillingCycle = OrganizationBillingCycleConstants.Monthly,
                                },
                            },
                        },
                    },
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
                            ProductPricing.Empty("pricing-recurring-2") with
                            {
                                PurchaseCadence = ProductPricingCadence.Daily,
                            },
                        ProductVersion = new ProductVersion
                        {
                            Product = new Product
                            {
                                Organization = new Organization
                                {
                                    BillingCycle = OrganizationBillingCycleConstants.Monthly,
                                },
                            },
                        },
                    },
                },
            ],
        };

    private static XeroInvoice CreateInvoice(
        Guid invoiceId,
        decimal amountDue,
        List<Payment>? payments,
        List<LineItem> lineItems)
    {
        var invoice = new XeroInvoice
        {
            InvoiceID = invoiceId,
            Contact = new Contact
            {
                ContactID = Guid.Parse("050f7fd4-f1d7-465b-8bd2-31ae88daecf7"),
            },
            LineAmountTypes = LineAmountTypes.Inclusive,
            CurrencyCode = CurrencyCode.NZD,
            LineItems = lineItems,
        };

        SetReadOnlyProperty(invoice, nameof(XeroInvoice.AmountDue), amountDue);
        SetReadOnlyProperty(invoice, nameof(XeroInvoice.Payments), payments);
        return invoice;
    }

    private static void SetReadOnlyProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
    {
        var backingFieldName = $"<{propertyName}>k__BackingField";
        var field = typeof(TTarget).GetField(backingFieldName, BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? throw new InvalidOperationException($"Backing field not found for property {propertyName}.");
        field.SetValue(target, value);
    }

    private sealed class TestableXeroRefundService(
        OrganizationConfiguration organizationConfiguration,
        OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
        IRepositoryFactory repositoryFactory,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider,
        ILogger<XeroRefundService> logger)
        : XeroRefundService(
            organizationConfiguration,
            organizationBillingServiceClient,
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider,
            logger)
    {
        public Invoices? InvoiceResponse { get; init; }
        public CreditNotes? CreditNoteResponse { get; init; }
        public CreditNotes? CreditNoteByIdResponse { get; init; }
        public Payments? PaymentResponse { get; init; }
        public Payments? HistoricalPaymentResponse { get; init; }
        public bool CreditNoteByIdRequested { get; private set; }
        public bool HistoricalPaymentLookupRequested { get; private set; }
        public CreditNotes? CapturedCreditNotes { get; private set; }
        public Allocations? CapturedAllocations { get; private set; }
        public Payment? CapturedPayment { get; private set; }

        protected override Task<Invoices> GetInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid invoiceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(InvoiceResponse ?? new Invoices());

        protected override Task<CreditNotes> CreateCreditNotesAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            CreditNotes creditNotes,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CapturedCreditNotes = creditNotes;
            return Task.FromResult(CreditNoteResponse ?? new CreditNotes());
        }

        protected override Task<CreditNotes> GetCreditNotesAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            DateTime modifiedSince,
            Guid creditNoteId,
            CancellationToken cancellationToken) =>
            Task.FromResult(CreditNoteResponse ?? new CreditNotes());

        protected override Task<CreditNote?> GetCreditNoteByIdAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid creditNoteId,
            CancellationToken cancellationToken)
        {
            CreditNoteByIdRequested = true;
            return Task.FromResult(CreditNoteByIdResponse?._CreditNotes?.FirstOrDefault());
        }

        protected override Task<Payments> GetPaymentsAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            DateTime? modifiedSince,
            Guid creditNoteId,
            CancellationToken cancellationToken)
        {
            if (modifiedSince is null)
            {
                HistoricalPaymentLookupRequested = true;
                return Task.FromResult(HistoricalPaymentResponse ?? new Payments());
            }

            return Task.FromResult(PaymentResponse ?? new Payments());
        }

        protected override Task<Allocations> CreateCreditNoteAllocationAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid creditNoteId,
            Allocations allocations,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CapturedAllocations = allocations;
            return Task.FromResult(allocations);
        }

        protected override Task<Payments> CreatePaymentAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Payment payment,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CapturedPayment = payment;
            return Task.FromResult(new Payments
            {
                _Payments = [payment],
            });
        }
    }
}
