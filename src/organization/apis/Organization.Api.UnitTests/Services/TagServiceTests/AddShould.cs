using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.UnitTests.Services.TagServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reject_User_Supplied_System_Host_Location_Tag_Ids(
        [Frozen] ICustomerService customerService,
        TagService sut,
        CancellationToken cancellationToken)
    {
        var tag = new Tag { Id = HostLocationSystemIds.ProductTag("location-1"), Name = "Forged Host tag", Type = OrganizationTagType.Product };
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken))
            .Returns((new Customer { Id = "customer-1" }, new Shared.Database.Entities.Customer { Id = "customer-1" }));

        var exception = await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.AddAsync(tag, false, cancellationToken));

        exception.Message.ShouldContain("system managed");
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_Custom_Tag_Exception_When_A_Matching_Custom_Tag_Already_Exists(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITagRepository tagRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        IDbTransactionBuilder transactionBuilder,
        IRandomHelper randomHelper,
        ICachedCustomerService cachedCustomerService,
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        IGraphQlMapper graphQlMapper,
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        ICachedTagService cachedTagService,
        ICachedOrganizationService cachedOrganizationService,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", Name = "Org 1", Type = OrganizationTypeConstants.Private };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var tag = new Tag
        {
            Id = "tag-1",
            Name = "Existing",
            Type = OrganizationTagType.Custom,
            Organization = new Shared.Models.Organization { Id = organization.Id }
        };

        A.CallTo(() => repositoryFactory.TagRepository).Returns(tagRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => tagRepository.GetByIdAsync(tag.Id, cancellationToken)).Returns((Shared.Database.Entities.Tag?)null);
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
            graphQlMapper,
            organizationOutboxPublisher,
            cachedTagService,
            cachedOrganizationService);

        await Should.ThrowAsync<CustomTagWithSameNameExist>(() => sut.AddAsync(tag, false, cancellationToken));
    }
}
