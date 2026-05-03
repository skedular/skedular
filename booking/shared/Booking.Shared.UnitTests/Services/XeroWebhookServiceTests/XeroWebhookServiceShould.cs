using System.Security.Cryptography;
using System.Text;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Database.Entities;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Xero.NetStandard.OAuth2.Api;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Constants = Enterprise.Shared.Grpc.Constants;
using Invoice = Xero.NetStandard.OAuth2.Model.Accounting.Invoice;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;
using OrganizationModel = Api.Shared.Services.Grpc.Skedular.Organization.V1.Organization;

namespace Booking.Shared.UnitTests.Services.XeroWebhookServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class XeroWebhookServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Validate_Xero_Webhook_Signature([Frozen] XeroConfiguration xeroConfiguration, XeroWebhookService sut, string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(xeroConfiguration.WebhookKey));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        sut.IsSignatureValid(payload, signature).ShouldBeTrue();
        sut.IsSignatureValid(payload, "wrong-signature").ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Accounting_Invoice_Monitor_For_Invoice_Events(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] ITemporalService temporalService,
        XeroWebhookService sut,
        CancellationToken cancellationToken)
    {
        var payloadJson =
            """
            {
              "events": [
                {
                  "resourceType": "INVOICE",
                  "resourceId": "invoice-1"
                },
                {
                  "eventCategory": "INVOICE",
                  "resourceId": "invoice-1"
                },
                {
                  "resourceType": "CONTACT",
                  "resourceId": "ignored"
                }
              ]
            }
            """;
        var link = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            ExternalInvoiceId = "invoice-1",
            ExternalStatus = AccountingStatusConstants.PendingExport
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains("invoice-1")),
                cancellationToken))
            .Returns([link]);

        await sut.ProcessAsync(payloadJson, cancellationToken);

        A.CallTo(() => temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                A<MaintainAccountingInvoiceStateInput>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.LocalEntityType == AccountingEntityTypeConstants.MarketplaceBooking &&
                    input.LocalEntityId == "marketplace-booking-1" &&
                    input.ExternalInvoiceIdHint == "invoice-1"),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.SignalWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(
                A<MaintainOrganizationArrearsInvoiceAccountingStateInput>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Arrears_Invoice_Monitor_For_Arrears_Invoice_Events(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        [Frozen] ITemporalService temporalService,
        XeroWebhookService sut,
        CancellationToken cancellationToken)
    {
        const string PayloadJson =
            """
            {
              "events": [
                {
                  "resourceType": "INVOICE",
                  "resourceId": "invoice-1"
                }
              ]
            }
            """;
        var link = new AccountingInvoiceExportLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.OrganizationArrearsInvoice,
            LocalEntityId = "arrears-invoice-1",
            ExternalInvoiceId = "invoice-1",
            ExternalStatus = AccountingStatusConstants.PendingExport
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains("invoice-1")),
                cancellationToken))
            .Returns([link]);

        await sut.ProcessAsync(PayloadJson, cancellationToken);

        A.CallTo(() => temporalService.SignalWorkflowMaintainOrganizationArrearsInvoiceAccountingStateAsync(
                A<MaintainOrganizationArrearsInvoiceAccountingStateInput>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.OrganizationArrearsInvoiceId == "arrears-invoice-1"),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                A<MaintainAccountingInvoiceStateInput>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Resolve_Repeating_Invoice_Webhook_To_Stored_Template_Link(
        CallInvoker callInvoker,
        IRepositoryFactory repositoryFactory,
        IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        ITemporalService temporalService,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroWebhookService> logger,
        TimeProvider timeProvider,
        XeroConfiguration xeroConfiguration,
        OrganizationConfiguration organizationConfiguration,
        Guid generatedInvoiceId,
        Guid repeatingTemplateId,
        string tenantId,
        CancellationToken cancellationToken)
    {
        xeroConfiguration.WebhookKey = "webhook-secret";
        organizationConfiguration.ApiKey = "api-key";
        var sut = new TestableXeroWebhookService(
            xeroConfiguration,
            repositoryFactory,
            temporalService,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            organizationConfiguration,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    new Invoice { InvoiceID = generatedInvoiceId, RepeatingInvoiceID = repeatingTemplateId }
                ]
            }
        };
        var payloadJson =
            $$"""
              {
                "events": [
                  {
                    "resourceType": "INVOICE",
                    "resourceId": "{{generatedInvoiceId}}",
                    "tenantId": "{{tenantId}}"
                  }
                ]
              }
              """;
        var repeatingLink = new AccountingInvoiceExportLink
        {
            Id = "link-2",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-1",
            ExternalInvoiceId = repeatingTemplateId.ToString(),
            ExternalStatus = AccountingStatusConstants.PendingExport
        };
        var xeroConnection = new XeroConnection
        {
            Id = "xero-1",
            TenantId = tenantId,
            AccessTokenEncrypted = "encrypted-access",
            AccessTokenExpiresAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow.AddMinutes(30))
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains(generatedInvoiceId.ToString())),
                cancellationToken))
            .Returns([]);
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetByXeroTenantIdInput, OrganizationModel>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetByXeroTenantIdInput>.That.Matches(input => input.TenantId == tenantId)))
            .Returns(CreateResponse(new OrganizationModel { Id = "org-1" }));
        A.CallTo(() => callInvoker.AsyncUnaryCall(
                A<Method<Admin_GetXeroConnectionInput, XeroConnection>>._,
                A<string?>._,
                A<CallOptions>.That.Matches(options =>
                    options.CancellationToken == cancellationToken &&
                    options.Headers != null &&
                    options.Headers.Any(item => item.Key == Constants.ApiKey && item.Value == organizationConfiguration.ApiKey)),
                A<Admin_GetXeroConnectionInput>.That.Matches(input => input.OrganizationId == "org-1")))
            .Returns(CreateResponse(xeroConnection));
        A.CallTo(() => xeroTokenEncryptionService.Decrypt("encrypted-access")).Returns("access-token");
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(A.Fake<AccountingApi>());
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains(repeatingTemplateId.ToString())),
                cancellationToken))
            .Returns([repeatingLink]);

        await sut.ProcessAsync(payloadJson, cancellationToken);

        A.CallTo(() => temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                A<MaintainAccountingInvoiceStateInput>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.LocalEntityType == AccountingEntityTypeConstants.RecurringBooking &&
                    input.LocalEntityId == "recurring-booking-1" &&
                    input.ExternalInvoiceIdHint == generatedInvoiceId.ToString()),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Prefer_Concrete_Repeating_Invoice_Event_Over_Template_Event_When_Both_Appear(
        CallInvoker callInvoker,
        IRepositoryFactory repositoryFactory,
        IAccountingInvoiceExportLinkRepository accountingInvoiceLinkRepository,
        IAccountingInvoiceInstanceRepository accountingInvoiceInstanceRepository,
        ITemporalService temporalService,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        ILogger<XeroWebhookService> logger,
        TimeProvider timeProvider,
        XeroConfiguration xeroConfiguration,
        OrganizationConfiguration organizationConfiguration,
        Guid generatedInvoiceId,
        Guid repeatingTemplateId,
        string tenantId,
        CancellationToken cancellationToken)
    {
        xeroConfiguration.WebhookKey = "webhook-secret";
        organizationConfiguration.ApiKey = "api-key";
        var sut = new TestableXeroWebhookService(
            xeroConfiguration,
            repositoryFactory,
            temporalService,
            new OrganizationService.OrganizationServiceClient(callInvoker),
            organizationConfiguration,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider,
            logger)
        {
            InvoiceResponse = new Invoices
            {
                _Invoices =
                [
                    new Invoice { InvoiceID = generatedInvoiceId, RepeatingInvoiceID = repeatingTemplateId }
                ]
            }
        };
        var payloadJson =
            $$"""
              {
                "events": [
                  {
                    "eventCategory": "INVOICE",
                    "eventType": "CREATE",
                    "resourceId": "{{generatedInvoiceId}}",
                    "tenantId": "{{tenantId}}"
                  },
                  {
                    "eventCategory": "INVOICE",
                    "eventType": "CREATE",
                    "resourceId": "{{repeatingTemplateId}}",
                    "tenantId": "{{tenantId}}"
                  }
                ]
              }
              """;
        var repeatingLink = new AccountingInvoiceExportLink
        {
            Id = "link-3",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.RecurringBooking,
            LocalEntityId = "recurring-booking-1",
            ExternalInvoiceId = repeatingTemplateId.ToString(),
            ExternalInvoiceMode = AccountingInvoiceExportModeConstants.RepeatingInvoice,
            ExternalStatus = AccountingStatusConstants.PendingExport
        };

        var generatedInvoiceInstance = new AccountingInvoiceInstance
        {
            Id = "instance-1",
            AccountingInvoiceExportLinkId = repeatingLink.Id,
            AccountingInvoiceExportLink = repeatingLink,
            Provider = AccountingProviderConstants.Xero,
            ExternalInvoiceId = generatedInvoiceId.ToString(),
            OrganizationId = repeatingLink.OrganizationId,
            ExternalStatus = AccountingStatusConstants.Exported
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceExportLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => repositoryFactory.AccountingInvoiceInstanceRepository).Returns(accountingInvoiceInstanceRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids =>
                    ids.Count == 2 &&
                    ids.Contains(generatedInvoiceId.ToString()) &&
                    ids.Contains(repeatingTemplateId.ToString())),
                cancellationToken))
            .Returns([repeatingLink]);
        A.CallTo(() => accountingInvoiceInstanceRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<IReadOnlyList<string>>.That.Matches(ids =>
                    ids.Count == 2 &&
                    ids.Contains(generatedInvoiceId.ToString()) &&
                    ids.Contains(repeatingTemplateId.ToString())),
                cancellationToken))
            .Returns([generatedInvoiceInstance]);
        A.CallTo(() => xeroSdkClientFactory.CreateAccountingApi()).Returns(A.Fake<AccountingApi>());

        await sut.ProcessAsync(payloadJson, cancellationToken);

        A.CallTo(() => temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                A<MaintainAccountingInvoiceStateInput>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.LocalEntityType == AccountingEntityTypeConstants.RecurringBooking &&
                    input.LocalEntityId == "recurring-booking-1" &&
                    input.ExternalInvoiceIdHint == generatedInvoiceId.ToString()),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    private static AsyncUnaryCall<TResponse> CreateResponse<TResponse>(TResponse response)
        where TResponse : class =>
        new(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

    private sealed class TestableXeroWebhookService(
        XeroConfiguration xeroConfiguration,
        IRepositoryFactory repositoryFactory,
        ITemporalService temporalService,
        OrganizationService.OrganizationServiceClient organizationServiceClient,
        OrganizationConfiguration organizationConfiguration,
        IXeroSdkClientFactory xeroSdkClientFactory,
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        TimeProvider timeProvider,
        ILogger<XeroWebhookService> logger)
        : XeroWebhookService(
            xeroConfiguration,
            repositoryFactory,
            temporalService,
            organizationServiceClient,
            organizationConfiguration,
            xeroSdkClientFactory,
            xeroTokenEncryptionService,
            timeProvider,
            logger)
    {
        public Invoices? InvoiceResponse { get; init; }

        protected override Task<Invoices> GetInvoiceAsync(
            AccountingApi accountingApi,
            string accessToken,
            string tenantId,
            Guid invoiceId,
            CancellationToken cancellationToken) =>
            Task.FromResult(InvoiceResponse ?? new Invoices());
    }
}
