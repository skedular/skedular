using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.UnitTests.Services.TagServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Custom_Tag_Exception_When_A_Matching_Custom_Tag_Already_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITagRepository tagRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        IDbTransactionBuilder transactionBuilder,
        Enterprise.Shared.Random.IRandomHelper randomHelper,
        ICachedCustomerService cachedCustomerService,
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        Organization.Api.Mappers.IMapper mapper,
        Organization.Shared.Publishers.IOrganizationOutboxPublisher organizationOutboxPublisher,
        ICachedTagService cachedTagService,
        ICachedOrganizationService cachedOrganizationService,
        CancellationToken cancellationToken)
    {
        var organization = new Organization.Shared.Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            Type = OrganizationTypeConstants.Private
        };
        var customer = new Organization.Shared.Models.Customer { Id = "customer-1" };
        var customerEntity = new Organization.Shared.Database.Entities.Customer { Id = customer.Id };
        var tag = new Organization.Shared.Models.Tag
        {
            Id = "tag-1",
            Name = "Existing",
            Type = OrganizationTagType.Custom,
            Organization = new Organization.Shared.Models.Organization { Id = organization.Id }
        };

        A.CallTo(() => repositoryFactory.TagRepository).Returns(tagRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => tagRepository.GetByIdAsync(tag.Id, cancellationToken)).Returns((Organization.Shared.Database.Entities.Tag?)null);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(tag.Organization.Id, tag.Organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => tagRepository.ExistsActiveWithNameAsync(
                organization.Id,
                tag.Type.ToOrganizationTagType(),
                tag.Name,
                null,
                cancellationToken))
            .Returns(true);

        var sut = new TagService(
            transactionBuilder,
            repositoryFactory,
            randomHelper,
            cachedCustomerService,
            customerService,
            organizationAuthorizationService,
            organizationStripeConnectAccountService,
            mapper,
            organizationOutboxPublisher,
            cachedTagService,
            cachedOrganizationService);

        await Should.ThrowAsync<CustomTagWithSameNameExist>(() => sut.AddAsync(tag, false, cancellationToken));
    }
}