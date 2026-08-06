using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using BillingDetailsEntity = Organization.Shared.Database.Entities.OrganizationBillingDetails;

namespace Organization.Api.UnitTests.Services.OrganizationBillingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Billing_Details_When_Organization_Has_None(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationBillingDetailsRepository organizationBillingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        OrganizationBillingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Acme",
        };
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customer.Id,
        };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id,
            CustomDomain = organization.CustomDomain,
            Name = organization.Name,
        };
        var request = new OrganizationBillingDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationBillingDetailsPatchField>
            {
                OrganizationBillingDetailsPatchField.Email,
            },
            "Acme",
            "billing@acme.test",
            null,
            null,
            null,
            null,
            null,
            null,
            "1 Example Road",
            null,
            null,
            "Auckland",
            null,
            "1010",
            "New Zealand",
            "NZ");
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationBillingDetailsRepository).Returns(organizationBillingDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => randomHelper.Generate()).Returns("billing-details-1");
        A.CallTo(() => organizationBillingDetailsRepository.Add(A<BillingDetailsEntity>._))
            .ReturnsLazily(call => call.GetArgument<BillingDetailsEntity>(0)!);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedOrganization);
        organization.BillingDetails.ShouldNotBeNull();
        organization.BillingDetails.Id.ShouldBe("billing-details-1");
        organization.BillingDetails.Email.ShouldBe(request.Email);
        A.CallTo(() => organizationBillingDetailsRepository.Add(
                A<BillingDetailsEntity>.That.Matches(billingDetails =>
                    billingDetails.Organization == organization &&
                    billingDetails.AddressLine1 == request.AddressLine1 &&
                    billingDetails.Country == request.Country)))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Billing_Details_Field(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationBillingDetailsRepository organizationBillingDetailsRepository,
        [Frozen]
        ICustomerService customerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
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
        OrganizationBillingService sut,
        CancellationToken cancellationToken)
    {
        var billingDetails = new BillingDetailsEntity
        {
            Id = "billing-details-1",
            CompanyName = "Old company",
            Email = "old@acme.test",
            AddressLine1 = "1 Example Road",
            Zipcode = "1010",
            Country = "New Zealand",
        };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Acme",
            BillingDetails = billingDetails,
        };
        var customer = new Customer
        {
            Id = "customer-1",
        };
        var customerEntity = new Shared.Database.Entities.Customer
        {
            Id = customer.Id,
        };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id,
            CustomDomain = organization.CustomDomain,
            Name = organization.Name,
        };
        var request = new OrganizationBillingDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationBillingDetailsPatchField>
            {
                OrganizationBillingDetailsPatchField.CompanyName,
            },
            "New company",
            "ignored@acme.test",
            null,
            null,
            null,
            null,
            null,
            null,
            "Ignored address",
            null,
            null,
            null,
            null,
            "9999",
            "Ignored country",
            "NZ");
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationBillingDetailsRepository).Returns(organizationBillingDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedOrganization);
        billingDetails.CompanyName.ShouldBe("New company");
        billingDetails.Email.ShouldBe("old@acme.test");
        billingDetails.AddressLine1.ShouldBe("1 Example Road");
        A.CallTo(() => organizationBillingDetailsRepository.Update(billingDetails)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
