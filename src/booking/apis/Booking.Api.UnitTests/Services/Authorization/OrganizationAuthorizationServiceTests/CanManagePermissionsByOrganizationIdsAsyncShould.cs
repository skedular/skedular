using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using OrganizationMemberEntity = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Api.UnitTests.Services.Authorization.OrganizationAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanManagePermissionsByOrganizationIdsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Batch_Payment_Method_Permissions_Without_Cached_Lookups(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        OrganizationAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> organizationIds = [organizationId];
        var organization = CreateOrganization(organizationId, customerId);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsForAuthorizationAsync(
                organizationIds,
                customerId,
                cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync(
                organization,
                customerId,
                cancellationToken))
            .Returns(true);

        var result = await sut.CanModifyPaymentMethodByOrganizationIdsAsync(organizationIds, customerId, cancellationToken);

        result[organizationId].ShouldBeTrue();
        A.CallTo(() => organizationRepository.GetByIdsForAuthorizationAsync(
                organizationIds,
                customerId,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<IReadOnlyList<string>?>._,
                A<IReadOnlyList<string>?>._,
                A<bool>._,
                A<bool>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(
                A<string?>._,
                A<string?>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Batch_Cancellation_Override_Permissions_Without_Cached_Lookups(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        OrganizationAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> organizationIds = [organizationId];
        var organization = CreateOrganization(organizationId, customerId);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsForAuthorizationAsync(
                organizationIds,
                customerId,
                cancellationToken))
            .Returns([organization]);
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync(
                organization,
                customerId,
                cancellationToken))
            .Returns(true);

        var result = await sut.CanOverrideCancellationPolicyByOrganizationIdsAsync(organizationIds, customerId, cancellationToken);

        result[organizationId].ShouldBeTrue();
        A.CallTo(() => organizationRepository.GetByIdsForAuthorizationAsync(
                organizationIds,
                customerId,
                cancellationToken))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(
                A<IReadOnlyList<string>?>._,
                A<IReadOnlyList<string>?>._,
                A<bool>._,
                A<bool>._,
                cancellationToken))
            .MustNotHaveHappened();
        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(
                A<string?>._,
                A<string?>._,
                cancellationToken))
            .MustNotHaveHappened();
    }

    private static OrganizationEntity CreateOrganization(string organizationId, string customerId) => new()
    {
        Id = organizationId,
        OrganizationMembers =
        [
            new OrganizationMemberEntity
            {
                Customer = new CustomerEntity
                {
                    Id = customerId,
                },
                Status = OrganizationMemberStatusConstants.Active,
                Role = OrganizationMemberRoleConstants.Owner,
            },
        ],
    };
}
