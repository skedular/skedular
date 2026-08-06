using System.Web;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;

namespace Organization.Api.UnitTests.Services.OrganizationXeroConnectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class OrganizationXeroConnectionServiceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Build_Authorize_Url_Using_The_Generated_Callback_Route(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IXeroTokenEncryptionService xeroTokenEncryptionService,
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        IXeroTokenRefreshService xeroTokenRefreshService,
        IXeroSdkClientFactory xeroSdkClientFactory,
        ICachedOrganizationService cachedOrganizationService,
        ITemporalOutboxService temporalOutboxService,
        IGraphQlMapper graphQlMapper,
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        IDbTransactionBuilder transactionBuilder,
        IRandomHelper randomHelper,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        var applicationConfiguration = new ApplicationConfiguration
        {
            ApiBaseDomain = new Uri("http://localhost:10200/"),
        };
        var xeroConfiguration = new XeroConfiguration
        {
            AuthorizeEndpoint = "https://login.xero.com/identity/connect/authorize",
            ClientId = "client-id-1",
            Scopes = "offline_access accounting.transactions",
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            CustomDomain = "org-1",
        };
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = "customer-1",
        };
        const string encryptedState = "encrypted-state";

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, "customer-1", cancellationToken)).Returns(true);
        A.CallTo(() => xeroTokenEncryptionService.Encrypt(A<string>._)).Returns(encryptedState);

        var sut = new OrganizationXeroConnectionService(
            applicationConfiguration,
            xeroConfiguration,
            repositoryFactory,
            customerService,
            organizationAuthorizationService,
            organizationStripeConnectAccountService,
            xeroTokenRefreshService,
            xeroTokenEncryptionService,
            xeroSdkClientFactory,
            cachedOrganizationService,
            temporalOutboxService,
            graphQlMapper,
            organizationOutboxPublisher,
            transactionBuilder,
            randomHelper,
            timeProvider);

        var result = await sut.GetAuthorizeUrlAsync("org-1", null, cancellationToken);

        result.Scheme.ShouldBe("https");
        result.Host.ShouldBe("login.xero.com");
        var query = HttpUtility.ParseQueryString(result.Query);
        query["client_id"].ShouldBe("client-id-1");
        query["scope"].ShouldBe("offline_access accounting.transactions");
        query["state"].ShouldBe(encryptedState);
        query["redirect_uri"].ShouldBe("http://localhost:10200/v1/organization/xero/oauth/callback");
    }
}
