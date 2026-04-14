using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using BookingEntity = Booking.Shared.Database.Entities.Booking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using Xero.NetStandard.OAuth2.Api;

namespace Booking.Shared.UnitTests.Services.AccountingInvoiceCancellationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelBookingShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Mark_Link_As_Cancelled_Without_Xero_Call_When_Live_Standard_Invoice_Does_Not_Exist(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        OrganizationConfiguration organizationConfiguration,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string bookingId,
        string marketplaceBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var sut = new AccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = marketplaceBookingId,
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Local_Cancelled_Link_When_Internal_Invoice_Exists_Without_External_Provider_Link(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        OrganizationConfiguration organizationConfiguration,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string bookingId,
        string marketplaceBookingId,
        string organizationId,
        string invoiceNumber,
        string invoiceUrl,
        CancellationToken cancellationToken)
    {
        var sut = new AccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = marketplaceBookingId,
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
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceExportLink?>(null));
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Skedular,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceExportLink?>(null));

        await sut.CancelBookingAsync(booking, cancellationToken);

        A.CallTo(() => accountingInvoiceLinkRepository.Add(A<AccountingInvoiceExportLink>.That.Matches(link =>
                link.Provider == AccountingProviderConstants.Skedular &&
                link.LocalEntityType == AccountingEntityTypeConstants.MarketplaceBooking &&
                link.LocalEntityId == marketplaceBookingId &&
                link.OrganizationId == organizationId &&
                link.ExternalInvoiceNumber == invoiceNumber &&
                link.ExternalInvoiceUrl == invoiceUrl &&
                link.ExternalStatus == AccountingStatusConstants.Cancelled &&
                link.ExportConfigurationState == Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Live_Standard_Invoice_In_Xero_When_Connection_Is_Ready(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        OrganizationConfiguration organizationConfiguration,
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string bookingId,
        string marketplaceBookingId,
        string organizationId,
        string accessTokenEncrypted,
        CancellationToken cancellationToken)
    {
        var accountingApi = A.Fake<AccountingApi>();
        var sut = new TestAccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = marketplaceBookingId,
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
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceId = externalInvoiceId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.StandardInvoice,
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
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetLatestByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceLink.Id,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceInstance?>(null));
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);
        A.CallTo(() => xeroTokenEncryptionService.Decrypt(accessTokenEncrypted)).Returns("access-token");

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        sut.CancelLiveStandardInvoiceCalls.ShouldBe(1);
        sut.CancelledStandardInvoiceIdempotencyKeys.ShouldContain($"{accountingInvoiceLink.Id}:cancel-standard");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Live_Standard_Invoice_In_Xero_Using_Latest_Invoice_Instance_When_Available(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        [Frozen] IXeroSdkClientFactory xeroSdkClientFactory,
        [Frozen] IXeroTokenEncryptionService xeroTokenEncryptionService,
        OrganizationConfiguration organizationConfiguration,
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string bookingId,
        string marketplaceBookingId,
        string organizationId,
        string accessTokenEncrypted,
        CancellationToken cancellationToken)
    {
        var accountingApi = A.Fake<AccountingApi>();
        var sut = new TestAccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = marketplaceBookingId,
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
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceId = Guid.CreateVersion7().ToString(),
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.PendingExport
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
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetLatestByAccountingInvoiceExportLinkIdAsync(
                accountingInvoiceLink.Id,
                cancellationToken))
            .Returns(accountingInvoiceInstance);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>._,
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == organizationId)))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(accountingApi);
        A.CallTo(() => xeroTokenEncryptionService.Decrypt(accessTokenEncrypted)).Returns("access-token");

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceInstance.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        sut.CancelledStandardInvoiceIds.ShouldContain(Guid.Parse(accountingInvoiceInstance.ExternalInvoiceId));
        sut.CancelledStandardInvoiceIdempotencyKeys.ShouldContain($"{accountingInvoiceLink.Id}:cancel-standard");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Existing_Export_Configuration_When_Invoice_Is_Already_Paid(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        OrganizationConfiguration organizationConfiguration,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider,
        CallInvoker callInvoker,
        string bookingId,
        string marketplaceBookingId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var sut = new AccountingInvoiceCancellationService(
            organizationConfiguration,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider);
        var booking = new BookingEntity
        {
            Id = bookingId,
            MarketplaceBooking = new MarketplaceBooking
            {
                Id = marketplaceBookingId,
                ProductVersion = new ProductVersion
                {
                    Product = new Product { Organization = new OrganizationEntity { Id = organizationId } }
                }
            }
        };
        var accountingInvoiceLink = new AccountingInvoiceExportLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceMode = Booking.Shared.Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Paid,
            ExportConfigurationState = Booking.Shared.Models.AccountingInvoiceExportConfigurationStateConstants.Active,
            ExportConfigurationMessage = "Existing configuration"
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelBookingAsync(booking, cancellationToken);

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
        OrganizationService.OrganizationServiceClient organizationServiceClient,
        IRepositoryFactory repositoryFactory,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider)
        : AccountingInvoiceCancellationService(
            organizationConfiguration,
            organizationServiceClient,
            repositoryFactory,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider)
    {
        public int CancelLiveStandardInvoiceCalls { get; private set; }
        public List<Guid> CancelledStandardInvoiceIds { get; } = [];
        public List<string> CancelledStandardInvoiceIdempotencyKeys { get; } = [];

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
            CancelledStandardInvoiceIdempotencyKeys.Add(idempotencyKey);
            return Task.CompletedTask;
        }
    }
}
