using AutoFixture.Xunit3;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Context;
using FakeItEasy;

namespace Booking.Api.UnitTests.Services.Authorization.OrganizationSsoAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsSsoValidAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Sso_Is_Not_Active(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        OrganizationSsoAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = organizationId };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);

        var result = await sut.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_The_Sso_Email_Matches_A_Customer_Identity(
        [Frozen] IContext context,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        OrganizationSsoAuthorizationService sut,
        string organizationId,
        string customerId,
        string email,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = organizationId, OrganizationSsoSettings = new OrganizationSsoSetting { IsActive = true } };
        var customer = new Customer { Id = customerId, Identities = [new Identity { Email = email }] };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => context.GetUserSsoContext(organizationId)).Returns(new UserSsoContext(email));
        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken)).Returns(customer);

        var result = await sut.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeTrue();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_The_Sso_Email_Does_Not_Match_A_Customer_Identity(
        [Frozen] IContext context,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        OrganizationSsoAuthorizationService sut,
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = new Organization { Id = organizationId, OrganizationSsoSettings = new OrganizationSsoSetting { IsActive = true } };
        var customer = new Customer { Id = customerId, Identities = [new Identity { Email = "customer@example.com" }] };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken)).Returns(organization);
        A.CallTo(() => context.GetUserSsoContext(organizationId)).Returns(new UserSsoContext("sso@example.com"));
        A.CallTo(() => cachedCustomerService.GetByIdAsync(customerId, cancellationToken)).Returns(customer);

        var result = await sut.IsSsoValidAsync(organizationId, customerId, cancellationToken);

        result.ShouldBeFalse();
    }
}
