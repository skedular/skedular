using Api.Shared.Services.Models;
using Booking.Api.Services.Authorization;
using Booking.Shared.Services.Cache;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using OrganizationMemberEntity = Booking.Shared.Database.Entities.OrganizationMember;

namespace Booking.Api.UnitTests.Services.Authorization.OrganizationAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanViewBookingsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_For_A_Non_Private_Organization_When_Sso_Is_Valid(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        OrganizationAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new OrganizationEntity { Id = organizationId, Type = OrganizationTypeConstants.Marketplace };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken)).Returns(true);

        var result = await sut.CanViewBookingsAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_For_A_Private_Organization_When_The_Customer_Is_Not_An_Active_Member(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        OrganizationAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new OrganizationEntity
        {
            Id = organizationId,
            Type = OrganizationTypeConstants.Private,
            OrganizationMembers =
            [
                new OrganizationMemberEntity
                {
                    Customer = new CustomerEntity { Id = customerId },
                    Status = OrganizationMemberStatusConstants.Inactive,
                    Role = OrganizationMemberRoleConstants.Member
                }
            ]
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);

        var result = await sut.CanViewBookingsAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeFalse();
    }
}
