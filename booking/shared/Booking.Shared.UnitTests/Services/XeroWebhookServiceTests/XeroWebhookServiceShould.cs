using System.Security.Cryptography;
using System.Text;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using AutoFixture.Xunit3;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Workflows;
using Enterprise.Shared.Accounting.Configurations;
using FakeItEasy;
using OrganizationConfiguration = Api.Shared.Clients.Configurations.Grpc.OrganizationConfiguration;

namespace Booking.Shared.UnitTests.Services.XeroWebhookServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class XeroWebhookServiceShould
{
    [Fact]
    public void Validate_Xero_Webhook_Signature()
    {
        var xeroConfiguration = new XeroConfiguration { WebhookKey = "webhook-secret" };
        var sut = new XeroWebhookService(
            xeroConfiguration,
            A.Fake<IRepositoryFactory>(),
            A.Fake<ITemporalService>(),
            A.Fake<OrganizationService.OrganizationServiceClient>(),
            new OrganizationConfiguration { ApiKey = "api-key" });
        const string payload = "{\"events\":[{\"resourceType\":\"INVOICE\",\"resourceId\":\"invoice-1\"}]}";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("webhook-secret"));
        var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(payload)));

        sut.IsSignatureValid(payload, signature).ShouldBeTrue();
        sut.IsSignatureValid(payload, "wrong-signature").ShouldBeFalse();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Signal_Accounting_Invoice_Monitor_For_Invoice_Events(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
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
        var link = new AccountingInvoiceLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.MarketplaceBooking,
            LocalEntityId = "marketplace-booking-1",
            ExternalInvoiceId = "invoice-1",
            ExternalStatus = AccountingStatusConstants.PendingExport
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<ICollection<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains("invoice-1")),
                cancellationToken))
            .Returns([link]);

        await sut.ProcessAsync(payloadJson, cancellationToken);

        A.CallTo(() => temporalService.SignalWorkflowMaintainAccountingInvoiceStateAsync(
                A<MaintainAccountingInvoiceStateInput>.That.Matches(input =>
                    input.OrganizationId == "org-1" &&
                    input.LocalEntityType == AccountingEntityTypeConstants.MarketplaceBooking &&
                    input.LocalEntityId == "marketplace-booking-1"),
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
        [Frozen] IAccountingInvoiceLinkRepository accountingInvoiceLinkRepository,
        [Frozen] ITemporalService temporalService,
        XeroWebhookService sut,
        CancellationToken cancellationToken)
    {
        const string payloadJson =
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
        var link = new AccountingInvoiceLink
        {
            Id = "link-1",
            Provider = AccountingProviderConstants.Xero,
            OrganizationId = "org-1",
            LocalEntityType = AccountingEntityTypeConstants.OrganizationArrearsInvoice,
            LocalEntityId = "arrears-invoice-1",
            ExternalInvoiceId = "invoice-1",
            ExternalStatus = AccountingStatusConstants.PendingExport
        };

        A.CallTo(() => repositoryFactory.AccountingInvoiceLinkRepository).Returns(accountingInvoiceLinkRepository);
        A.CallTo(() => accountingInvoiceLinkRepository.GetByProviderAndExternalInvoiceIdsAsync(
                AccountingProviderConstants.Xero,
                A<ICollection<string>>.That.Matches(ids => ids.Count == 1 && ids.Contains("invoice-1")),
                cancellationToken))
            .Returns([link]);

        await sut.ProcessAsync(payloadJson, cancellationToken);

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
}
