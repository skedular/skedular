using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using static Testing.Shared.Assertions.LogAssertions;

namespace Organization.Api.UnitTests.Services.OrganizationOwnershipServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UnverifyAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task PersistPublishAndClearCaches(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationOutboxPublisher organizationOutboxPublisher,
        [Frozen]
        IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        ILogger<OrganizationOwnershipService> logger,
        OrganizationOwnershipService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Shared.Database.Entities.Organization
        {
            Id = "org-1",
            CustomDomain = "garden-host",
            Name = "Garden Host",
            Type = OrganizationTypeConstants.Host,
            IsOwnershipVerified = true,
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    CustomerId = "customer-1",
                },
            ],
        };
        var mappedOrganization = new Shared.Models.Organization
        {
            Id = organization.Id,
            CustomDomain = organization.CustomDomain,
            Name = organization.Name,
            Type = OrganizationType.Host,
            IsOwnershipVerified = false,
        };
        var stripeAuthorizeUrl = new Uri("https://example.test/authorize");

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync(organization.Id, null, cancellationToken))
            .Returns(organization);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))
            .Returns(stripeAuthorizeUrl);
        A.CallTo(() => graphQlMapper.MapTo(organization, stripeAuthorizeUrl)).Returns(mappedOrganization);

        await sut.UnverifyAsync(organization.Id, null, cancellationToken);

        organization.IsOwnershipVerified.ShouldBe(false);
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
                A<IReadOnlyList<string>>.That.Matches(customerIds => customerIds.SequenceEqual(new[] { "customer-1" })),
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        ACallToLog(logger, LogLevel.Warning).MustHaveHappenedOnceExactly();
    }
}
