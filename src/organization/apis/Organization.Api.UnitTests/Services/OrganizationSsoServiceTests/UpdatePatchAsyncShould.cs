using Enterprise.Shared.Database;
using Enterprise.Shared.Security.Sso;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using OrganizationSsoSettingsEntity = Organization.Shared.Database.Entities.OrganizationSsoSettings;

namespace Organization.Api.UnitTests.Services.OrganizationSsoServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Apply_Sso_Settings_Patch_And_Return_Latest_Organization(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationSsoSettingsRepository organizationSsoSettingsRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] ISamlAssertionConsumerService samlAssertionConsumerService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbContextTransaction transaction,
        OrganizationSsoService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", Name = "Acme", CustomDomain = "acme" };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var ssoSettings = new OrganizationSsoSettings
        {
            EntityId = "entity",
            LoginUrl = "https://login.example.com",
            AppFederationMetadataUrl = "https://login.example.com/metadata",
            IsActive = true,
            Organization = new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain }
        };
        var request = new OrganizationSsoSettingsPatchRequest(
            organization.Id,
            organization.CustomDomain,
            new HashSet<OrganizationSsoSettingsPatchField> { OrganizationSsoSettingsPatchField.SsoSettings },
            ssoSettings);
        var entity = new OrganizationSsoSettingsEntity
        {
            Organization = organization,
            EntityId = ssoSettings.EntityId,
            LoginUrl = ssoSettings.LoginUrl,
            AppFederationMetadataUrl = ssoSettings.AppFederationMetadataUrl,
            IsActive = true
        };
        var updatedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, Name = organization.Name, CustomDomain = organization.CustomDomain
        };
        var stripeAuthorizeUrl = new Uri("https://stripe.example.com/acme");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationSsoSettingsRepository).Returns(organizationSsoSettingsRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => samlAssertionConsumerService.ValidateMetadataAsync(ssoSettings.AppFederationMetadataUrl, cancellationToken))
            .Returns(true);
        A.CallTo(() => samlAssertionConsumerService.ValidateCertificateAsync(ssoSettings.AppFederationMetadataUrl, cancellationToken))
            .Returns(true);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => graphQlMapper.MapToEntity(ssoSettings, organization)).Returns(entity);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(updatedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedOrganization);
        organization.OrganizationSsoSettings.ShouldBeSameAs(entity);
        A.CallTo(() => organizationSsoSettingsRepository.Add(entity)).MustHaveHappenedOnceExactly();
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
    public async Task Reject_Patch_When_Sso_Settings_Field_Is_Not_Selected(
        OrganizationSsoService sut,
        OrganizationSsoSettings ssoSettings,
        CancellationToken cancellationToken)
    {
        var request = new OrganizationSsoSettingsPatchRequest("org-1", "acme", new HashSet<OrganizationSsoSettingsPatchField>(), ssoSettings);

        await Should.ThrowAsync<ArgumentException>(() => sut.UpdatePatchAsync(request, cancellationToken));
    }
}
