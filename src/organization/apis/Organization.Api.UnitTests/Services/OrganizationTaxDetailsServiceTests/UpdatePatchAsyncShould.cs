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
using Organization.Shared.Services.Cache;
using TaxDetailsEntity = Organization.Shared.Database.Entities.OrganizationTaxDetails;
using TaxDetailsModel = Organization.Shared.Models.OrganizationTaxDetails;

namespace Organization.Api.UnitTests.Services.OrganizationTaxDetailsServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Tax_Details_When_Organization_Has_None(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", CustomDomain = "acme", Name = "Acme" };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var entity = new TaxDetailsEntity { Id = "tax-details-1", TaxId = "NZ123", TaxRatePercentage = 15 };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name
        };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField>
            {
                OrganizationTaxDetailsPatchField.TaxId, OrganizationTaxDetailsPatchField.TaxRatePercentage
            },
            true,
            entity.TaxId,
            entity.TaxRatePercentage);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => randomHelper.Generate()).Returns(entity.Id);
        A.CallTo(() => graphQlMapper.MapToEntity(
                A<TaxDetailsModel>.That.Matches(taxDetails =>
                    taxDetails.Id == entity.Id &&
                    taxDetails.IsRegistered &&
                    taxDetails.TaxId == entity.TaxId &&
                    taxDetails.TaxRatePercentage == entity.TaxRatePercentage),
                organization))
            .Returns(entity);
        A.CallTo(() => organizationTaxDetailsRepository.Add(entity)).Returns(entity);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedOrganization);
        organization.OrganizationTaxDetails.ShouldBeSameAs(entity);
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Tax_Details_Field(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var taxDetails = new TaxDetailsEntity { Id = "tax-details-1", TaxId = "OLD", TaxRatePercentage = 10 };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1", CustomDomain = "acme", Name = "Acme", OrganizationTaxDetails = taxDetails
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name
        };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField> { OrganizationTaxDetailsPatchField.TaxId },
            null,
            "NEW",
            20);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
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
        taxDetails.TaxId.ShouldBe("NEW");
        taxDetails.TaxRatePercentage.ShouldBe(10);
        A.CallTo(() => organizationTaxDetailsRepository.Update(taxDetails)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Tax_Details_Registration_State_Without_Changing_Values(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var taxDetails = new TaxDetailsEntity { Id = "tax-details-1", IsRegistered = true, TaxId = "NZ123", TaxRatePercentage = 15 };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1", CustomDomain = "acme", Name = "Acme", OrganizationTaxDetails = taxDetails
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name
        };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField> { OrganizationTaxDetailsPatchField.IsRegistered },
            false,
            taxDetails.TaxId,
            taxDetails.TaxRatePercentage);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
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
        taxDetails.IsRegistered.ShouldBeFalse();
        taxDetails.TaxId.ShouldBe("NZ123");
        taxDetails.TaxRatePercentage.ShouldBe(15);
        A.CallTo(() => organizationTaxDetailsRepository.Update(taxDetails)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Create_Unregistered_Tax_Details_Without_Tax_Id_Or_Rate_When_Organization_Has_None(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", CustomDomain = "acme", Name = "Acme" };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var entity = new TaxDetailsEntity { Id = "tax-details-1", IsRegistered = false, TaxId = string.Empty, TaxRatePercentage = 0 };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name
        };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField> { OrganizationTaxDetailsPatchField.IsRegistered },
            false,
            null,
            null);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => randomHelper.Generate()).Returns(entity.Id);
        A.CallTo(() => graphQlMapper.MapToEntity(
                A<TaxDetailsModel>.That.Matches(taxDetails =>
                    taxDetails.Id == entity.Id &&
                    !taxDetails.IsRegistered &&
                    taxDetails.TaxId == string.Empty &&
                    taxDetails.TaxRatePercentage == 0),
                organization))
            .Returns(entity);
        A.CallTo(() => organizationTaxDetailsRepository.Add(entity)).Returns(entity);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedOrganization);
        organization.OrganizationTaxDetails.ShouldBeSameAs(entity);
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Clear_Tax_Id_And_Rate_When_Tax_Details_Are_Not_Registered(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var taxDetails = new TaxDetailsEntity { Id = "tax-details-1", IsRegistered = false, TaxId = "NZ123", TaxRatePercentage = 15 };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1", CustomDomain = "acme", Name = "Acme", OrganizationTaxDetails = taxDetails
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name
        };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField>
            {
                OrganizationTaxDetailsPatchField.TaxId, OrganizationTaxDetailsPatchField.TaxRatePercentage
            },
            false,
            null,
            null);
        var stripeAuthorizeUrl = Constants.EmptyUri;

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
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
        taxDetails.TaxId.ShouldBe(string.Empty);
        taxDetails.TaxRatePercentage.ShouldBe(0);
        A.CallTo(() => organizationTaxDetailsRepository.Update(taxDetails)).MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == updatedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_Registered_Tax_Details_When_Effective_Tax_Id_Or_Rate_Is_Missing(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationTaxDetailsRepository organizationTaxDetailsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationTaxDetailsService sut,
        CancellationToken cancellationToken)
    {
        var taxDetails = new TaxDetailsEntity { Id = "tax-details-1", IsRegistered = true, TaxId = "NZ123", TaxRatePercentage = 15 };
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1", CustomDomain = "acme", Name = "Acme", OrganizationTaxDetails = taxDetails
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var request = new OrganizationTaxDetailsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationTaxDetailsPatchField>
            {
                OrganizationTaxDetailsPatchField.TaxId, OrganizationTaxDetailsPatchField.TaxRatePercentage
            },
            true,
            string.Empty,
            null);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationTaxDetailsRepository).Returns(organizationTaxDetailsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);

        await Should.ThrowAsync<ArgumentException>(() => sut.UpdatePatchAsync(request, cancellationToken));

        A.CallTo(() => organizationTaxDetailsRepository.Update(taxDetails)).MustNotHaveHappened();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustNotHaveHappened();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustNotHaveHappened();
    }
}
