using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.UnitTests.Services.OrganizationOwnershipServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class VerifyAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Clear_Organization_And_My_Organizations_Caches(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen] IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen] IGraphQlMapper graphQlMapper,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        OrganizationOwnershipService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "acme",
            Name = "Acme",
            Type = OrganizationTypeConstants.Marketplace,
            IsOwnershipVerified = false,
            OrganizationMembers =
            [
                new OrganizationMember { CustomerId = "customer-1" },
                new OrganizationMember { CustomerId = "customer-2" }
            ]
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name, IsOwnershipVerified = true
        };
        var stripeAuthorizeUrl = new Uri("https://example.test/authorize");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(null, organization.CustomDomain, cancellationToken))
            .Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);

        await sut.VerifyAsync(null, organization.CustomDomain, cancellationToken);

        organization.IsOwnershipVerified.ShouldBe(true);
        A.CallTo(() => organizationOutboxPublisher.PublishOrganizations(
                A<IEnumerable<Shared.Models.Organization>>.That.Matches(organizations => organizations.Single() == mappedOrganization),
                unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(
                organization.Id,
                organization.CustomDomain,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
                A<IReadOnlyList<string>>.That.Matches(customerIds => customerIds.SequenceEqual(new[] { "customer-1", "customer-2" })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
    }
}
