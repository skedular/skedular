using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using FakeItEasy;
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
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
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
        var accountingInvoiceLink = new AccountingInvoiceLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceMode = Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        A.CallTo(() => accountingInvoiceLinkRepository.Update(accountingInvoiceLink)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Local_Cancelled_Link_When_Internal_Invoice_Exists_Without_External_Provider_Link(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
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

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceLink?>(null));
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Skedular,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(Task.FromResult<AccountingInvoiceLink?>(null));

        await sut.CancelBookingAsync(booking, cancellationToken);

        A.CallTo(() => accountingInvoiceLinkRepository.Add(A<AccountingInvoiceLink>.That.Matches(link =>
                link.Provider == AccountingProviderConstants.Skedular &&
                link.LocalEntityType == AccountingEntityTypeConstants.MarketplaceBooking &&
                link.LocalEntityId == marketplaceBookingId &&
                link.OrganizationId == organizationId &&
                link.ExternalInvoiceNumber == invoiceNumber &&
                link.ExternalInvoiceUrl == invoiceUrl &&
                link.ExternalStatus == AccountingStatusConstants.Cancelled &&
                link.ExportConfigurationState == Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cancel_Live_Standard_Invoice_In_Xero_When_Connection_Is_Ready(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
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
        var externalInvoiceId = Guid.NewGuid().ToString();
        var accountingInvoiceLink = new AccountingInvoiceLink
        {
            Id = Guid.NewGuid().ToString(),
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceId = externalInvoiceId,
            ExternalInvoiceMode = Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Sent
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            IsActive = true,
            HasRefreshToken = true,
            TenantId = Guid.NewGuid().ToString(),
            AccessTokenEncrypted = accessTokenEncrypted,
            AccessTokenExpiresAt = DateTimeOffset.UtcNow.AddHours(1).ToTimestamp()
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
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

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Cancelled);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Models.AccountingInvoiceExportConfigurationStateConstants.Cancelled);
        sut.CancelLiveStandardInvoiceCalls.ShouldBe(1);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Preserve_Existing_Export_Configuration_When_Invoice_Is_Already_Paid(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
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
        var accountingInvoiceLink = new AccountingInvoiceLink
        {
            Provider = AccountingProviderConstants.Xero,
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = marketplaceBookingId,
            ExternalInvoiceMode = Models.AccountingInvoiceExportModeConstants.StandardInvoice,
            ExternalStatus = AccountingStatusConstants.Paid,
            ExportConfigurationState = Models.AccountingInvoiceExportConfigurationStateConstants.Active,
            ExportConfigurationMessage = "Existing configuration"
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndLocalEntityAsync(
                AccountingProviderConstants.Xero,
                AccountingEntityTypeConstants.MarketplaceBooking,
                marketplaceBookingId,
                cancellationToken))
            .Returns(accountingInvoiceLink);

        await sut.CancelBookingAsync(booking, cancellationToken);

        accountingInvoiceLink.ExternalStatus.ShouldBe(AccountingStatusConstants.Paid);
        accountingInvoiceLink.ExportConfigurationState.ShouldBe(Models.AccountingInvoiceExportConfigurationStateConstants.Active);
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

        protected override Task CancelLiveStandardInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid externalInvoiceId,
            string idempotencyKey,
            CancellationToken cancellationToken)
        {
            CancelLiveStandardInvoiceCalls++;
            return Task.CompletedTask;
        }
    }
}
