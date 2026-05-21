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
using TagEntity = Organization.Shared.Database.Entities.Tag;

namespace Organization.Api.UnitTests.Services.TagServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdatePatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Update_Only_Selected_Tag_Fields(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITagRepository tagRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICustomerService customerService,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] ICachedTagService cachedTagService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        TagService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization { Id = "org-1", CustomDomain = "acme", Name = "Acme" };
        var tag = new TagEntity
        {
            Id = "tag-1",
            Name = "Old name",
            Description = "Old description",
            Color = "#111111",
            Type = OrganizationTagType.Custom.ToOrganizationTagType(),
            Organization = organization
        };
        var customer = new Customer { Id = "customer-1" };
        var customerEntity = new Shared.Database.Entities.Customer { Id = customer.Id };
        var updatedTag = new Tag
        {
            Id = tag.Id,
            Name = "New name",
            Description = tag.Description,
            Color = "#222222",
            Type = OrganizationTagType.Custom
        };
        var mappedOrganization = new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain };
        var request = new OrganizationTagPatchRequest(
            tag.Id,
            OrganizationTagType.Custom,
            new HashSet<OrganizationTagPatchField> { OrganizationTagPatchField.Name, OrganizationTagPatchField.Color },
            updatedTag.Name,
            "Ignored description",
            updatedTag.Color);
        var stripeAuthorizeUrl = new Uri("https://example.test/stripe");

        A.CallTo(() => repositoryFactory.TagRepository).Returns(tagRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetCustomerAsync(cancellationToken)).Returns((customer, customerEntity));
        A.CallTo(() => tagRepository.GetByIdAsync(tag.Id, cancellationToken)).Returns(tag);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken)).Returns(true);
        A.CallTo(() => tagRepository.ExistsActiveWithNameAsync(organization.Id, tag.Type, updatedTag.Name, tag.Id, cancellationToken))
            .Returns(false);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => tagRepository.Update(tag)).Returns(tag);
        A.CallTo(() => graphQlMapper.MapTo(tag)).Returns(updatedTag);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);

        var result = await sut.UpdatePatchAsync(request, cancellationToken);

        result.ShouldBeSameAs(updatedTag);
        tag.Name.ShouldBe(updatedTag.Name);
        tag.Color.ShouldBe(updatedTag.Color);
        tag.Description.ShouldBe("Old description");
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == mappedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedTagService.UpdateByIdAsync(tag.Id, cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
