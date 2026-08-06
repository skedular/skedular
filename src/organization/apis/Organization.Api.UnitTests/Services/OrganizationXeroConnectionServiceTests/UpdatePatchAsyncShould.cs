using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using XeroConnectionEntity = Organization.Shared.Database.Entities.OrganizationXeroConnection;

namespace Organization.Api.UnitTests.Services.OrganizationXeroConnectionServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Xero_Fields(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        OrganizationXeroConnectionService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Acme",
            CustomDomain = "acme",
        };
        var xeroConnection = new XeroConnectionEntity
        {
            Id = "xero-1",
            Organization = organization,
            BillingMode = XeroBillingModeConstants.Disabled,
            DefaultReferencePrefix = "OLD",
            DefaultSalesAccountCode = "200",
            IsActive = false,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true,
        };
        organization.OrganizationXeroConnection = xeroConnection;
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customer.Id,
        };
        var mappedOrganization =
            new Shared.Models.Organization
            {
                Id = organization.Id,
                Name = organization.Name,
                CustomDomain = organization.CustomDomain,
            };
        var stripeAuthorizeUrl = new Uri($"https://example.test/{organization.Id}");
        var request = new OrganizationXeroConnectionPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationXeroConnectionPatchField>
            {
                OrganizationXeroConnectionPatchField.BillingMode,
                OrganizationXeroConnectionPatchField.DefaultReferencePrefix,
            },
            null,
            null,
            OrganizationXeroBillingMode.Enabled,
            null,
            null,
            null,
            null,
            "Ignored",
            null,
            null,
            null,
            null,
            "NEW");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationXeroConnectionRepository.Update(xeroConnection)).Returns(xeroConnection);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(mappedOrganization);
        xeroConnection.BillingMode.ShouldBe(XeroBillingModeConstants.Enabled);
        xeroConnection.DefaultReferencePrefix.ShouldBe("NEW");
        xeroConnection.DefaultSalesAccountCode.ShouldBe("200");
        A.CallTo(() => organizationXeroConnectionRepository.Update(xeroConnection)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(items => items.SequenceEqual(new[] { mappedOrganization })),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
