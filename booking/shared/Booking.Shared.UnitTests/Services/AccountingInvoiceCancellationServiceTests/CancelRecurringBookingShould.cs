using Api.Shared.Services;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using RecurringBookingEntity = Booking.Shared.Database.Entities.RecurringBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using Xero.NetStandard.OAuth2.Api;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;

namespace Booking.Shared.UnitTests.Services.AccountingInvoiceCancellationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelRecurringBookingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Link_As_Cancelled_Without_Xero_Call_When_Live_Repeating_Template_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        AccountingInvoiceCancellationService sut,
        string recurringBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Link_As_Transition_Required_When_Live_Repeating_Template_Cannot_Be_Cancelled_Because_Xero_Connection_Is_Not_Ready(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] CallInvoker callInvoker,
        AccountingInvoiceCancellationService sut,
        string recurringBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(new XeroConnection { Id = "xero-1", IsActive = false }));

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants
            .TransitionRequired);
        accountingInvoiceLink.ExportConfigurationMessage.ShouldNotBeNull();
        accountingInvoiceLink.ExportConfigurationMessage.ShouldContain("requires cancellation");
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Link_As_Transition_Required_When_Organization_Xero_Connection_Lookup_Fails(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen] TimeProvider timeProvider,
        [Frozen] CallInvoker callInvoker,
        string recurringBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var sut = new AccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Throws(new RpcException(new Status(StatusCode.Unavailable, "organization unavailable")));

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants
            .TransitionRequired);
        accountingInvoiceLink.ExportConfigurationMessage.ShouldNotBeNull();
        accountingInvoiceLink.LastError.ShouldNotBeNull();
        accountingInvoiceLink.LastError.ShouldContain("organization unavailable");
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Keep_Cancelled_Link_Terminal_Without_Retrying_Xero_Cancellation(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] CallInvoker callInvoker,
        AccountingInvoiceCancellationService sut,
        string recurringBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Cancelled,
            ExportConfigurationState = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled,
            LastError = "old error"
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        accountingInvoiceLink.LastError.ShouldBeNull();
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Local_Cancelled_Link_When_Internal_Recurring_Invoice_Exists_Without_External_Provider_Link(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        AccountingInvoiceCancellationService sut,
        string recurringBookingId,
        string organizationId,
        string invoiceNumber,
        string invoiceUrl,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                InvoiceNumber = invoiceNumber,
                InvoiceUrl = invoiceUrl,
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceExportLink?>(null));
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Skedular,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceExportLink?>(null));

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        A.CallTo(() => accountingInvoiceLinkRepository.Add(A<AccountingInvoiceExportLink>.That.Matches(link =>
                link.Provider == AccountingProviderConstants.Skedular &&
                link.LocalEntityType == AccountingEntityTypeConstants.RecurringBooking &&
                link.LocalEntityId == recurringBookingId &&
                link.OrganizationId == organizationId &&
                link.ExternalInvoiceNumber == invoiceNumber &&
                link.ExternalInvoiceUrl == invoiceUrl &&
                link.ExternalStatus == AccountingStatusConstants.Cancelled &&
                link.ExportConfigurationState == Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Live_Repeating_Invoice_Template_In_Xero_When_Connection_Is_Ready(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] TimeProvider timeProvider,
        [Frozen] CallInvoker callInvoker,
        [Frozen] AccountingApi accountingApi,
        string recurringBookingId,
        string organizationId,
        string accessTokenEncrypted,
        CancellationToken cancellationToken)
    {
        var sut = new TestAccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var externalInvoiceId = Guid.CreateVersion7().ToString();
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Id = Guid.CreateVersion7().ToString(),
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = externalInvoiceId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            IsActive = true,
            HasRefreshToken = true,
            TenantId = Guid.CreateVersion7().ToString(),
            AccessTokenEncrypted = accessTokenEncrypted,
            AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1).ToTimestamp()
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceLink.Id,
                cancellationToken))
            .Returns([]);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);
        A.CallTo(() => xeroTokenEncryptionService.Decrypt(accessTokenEncrypted)).Returns("access-token");

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        sut.CancelLiveRepeatingInvoiceCalls.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Live_Repeating_Template_And_All_Concrete_Invoices_When_They_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        [Frozen] OrganizationConfiguration organizationConfiguration,
        [Frozen] TimeProvider timeProvider,
        [Frozen] CallInvoker callInvoker,
        [Frozen] AccountingApi accountingApi,
        string recurringBookingId,
        string organizationId,
        string accessTokenEncrypted,
        CancellationToken cancellationToken)
    {
        var sut = new TestAccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationBillingService.OrganizationBillingServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Id = Guid.CreateVersion7().ToString(),
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };
        var accountingInvoiceInstance = new AccountingInvoiceInstance
        {
            Id = Guid.CreateVersion7().ToString(),
            Provider = AccountingProviderConstants.Xero,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalStatus = AccountingStatusConstants.Sent,
            AccountingInvoiceExportLinkId = accountingInvoiceLink.Id,
            OrganizationId = organizationId
        };
        var secondAccountingInvoiceInstance = new AccountingInvoiceInstance
        {
            Id = Guid.CreateVersion7().ToString(),
            Provider = AccountingProviderConstants.Xero,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalStatus = AccountingStatusConstants.Exported,
            AccountingInvoiceExportLinkId = accountingInvoiceLink.Id,
            OrganizationId = organizationId
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            IsActive = true,
            HasRefreshToken = true,
            TenantId = Guid.CreateVersion7().ToString(),
            AccessTokenEncrypted = accessTokenEncrypted,
            AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1).ToTimestamp()
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceLink.Id,
                cancellationToken))
            .Returns([accountingInvoiceInstance, secondAccountingInvoiceInstance]);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);
        A.CallTo(() => xeroTokenEncryptionService.Decrypt(accessTokenEncrypted)).Returns("access-token");

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceInstance.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        secondAccountingInvoiceInstance.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        sut.CancelLiveRepeatingInvoiceCalls.ShouldBe(1);
        sut.CancelLiveStandardInvoiceCalls.ShouldBe(2);
        sut.CancelledRepeatingInvoiceIds.ShouldBe([Guid.Parse(accountingInvoiceLink.ExternalInvoiceId)]);
        sut.CancelledStandardInvoiceIds.ShouldContain(Guid.Parse(accountingInvoiceInstance.ExternalInvoiceId));
        sut.CancelledStandardInvoiceIds.ShouldContain(Guid.Parse(secondAccountingInvoiceInstance.ExternalInvoiceId));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Existing_Export_Configuration_When_Recurring_Invoice_Is_Already_Paid(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        AccountingInvoiceCancellationService sut,
        string recurringBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var recurringBooking = new RecurringBookingEntity
        {
            Id = recurringBookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = recurringBookingId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.Paid,
            ExportConfigurationState = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Active,
            ExportConfigurationMessage = "Existing configuration"
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.RecurringBooking,
                recurringBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelRecurringBookingAsync(recurringBooking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Paid);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Active);
        accountingInvoiceLink.ExportConfigurationMessage.ShouldBe("Existing configuration");
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<XeroConnection> CreateResponse(XeroConnection response) =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private sealed class TestAccountingInvoiceCancellationService(
        OrganizationConfiguration organizationConfiguration,
        OrganizationBillingService.OrganizationBillingServiceClient organizationBillingServiceClient,
        IRepositoryFactory repositoryFactory,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider)
        : AccountingInvoiceCancellationService(
            organizationConfiguration,
            organizationBillingServiceClient,
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider)
    {
        public int CancelLiveRepeatingInvoiceCalls { get; private set; }
        public int CancelLiveStandardInvoiceCalls { get; private set; }
        public List<Guid> CancelledRepeatingInvoiceIds { get; } = [];
        public List<Guid> CancelledStandardInvoiceIds { get; } = [];

        protected override Task CancelLiveRepeatingInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid externalInvoiceId,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CancelLiveRepeatingInvoiceCalls++;
            CancelledRepeatingInvoiceIds.Add(externalInvoiceId);
            return Task.CompletedTask;
        }

        protected override Task CancelLiveStandardInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid externalInvoiceId,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CancelLiveStandardInvoiceCalls++;
            CancelledStandardInvoiceIds.Add(externalInvoiceId);
            return Task.CompletedTask;
        }
    }
}
