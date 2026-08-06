using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Repositories;
using Booking.Shared.Services.Cache;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.UnitTests.Services.Authorization.OrganizationAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetOrganizationsAndValidatePermissionsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_When_No_Ids_Or_Custom_Domains_Are_Provided(
        OrganizationAuthorizationService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await sut.GetOrganizationsAndValidatePermissionsAsync([], [], customerId, false, cancellationToken);

        result.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Any_Organization_Is_Not_Found(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        OrganizationAuthorizationService sut,
        string customerId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> organizationIds = [organizationId];

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(organizationIds, null, false, false, cancellationToken)).Returns([]);

        await Should.ThrowAsync<OrganizationNotFound>(() =>
            sut.GetOrganizationsAndValidatePermissionsAsync(organizationIds, [], customerId, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_A_New_Organization_Booking_Is_Not_Allowed_By_Offering(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        [Frozen]
        IOrganizationOfferingService organizationOfferingService,
        OrganizationAuthorizationService sut,
        string customerId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> organizationIds = [organizationId];
        var organization = new OrganizationEntity
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(organizationIds, null, false, false, cancellationToken))
            .Returns([organization]);
        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync(organizationId, customerId, cancellationToken)).Returns(false);

        await Should.ThrowAsync<NoMoreInteractionAllowed>(() =>
            sut.GetOrganizationsAndValidatePermissionsAsync(organizationIds, [], customerId, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Organizations_When_Add_Permission_And_Offering_Allow_It(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        [Frozen]
        IOrganizationOfferingService organizationOfferingService,
        [Frozen]
        ICachedOrganizationService cachedOrganizationService,
        OrganizationAuthorizationService sut,
        string customerId,
        string organizationId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<string> organizationIds = [organizationId];
        var organization = new OrganizationEntity
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Marketplace,
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdsOrCustomDomainsAsync(organizationIds, null, false, false, cancellationToken))
            .Returns([organization]);
        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken)).Returns(true);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync(organizationId, customerId, cancellationToken)).Returns(true);

        var result = await sut.GetOrganizationsAndValidatePermissionsAsync(organizationIds, [], customerId, false, cancellationToken);

        result.ShouldHaveSingleItem().ShouldBe(organization);
    }
}
