using Api.Shared.Services.Models;
using Microsoft.Extensions.Logging;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Services.Cache;

namespace Team.Api.UnitTests.Services.Authorization.OrganizationAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AuthorizeShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Authorization_Is_Granted(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        [Frozen] ILogger<OrganizationAuthorizationService> logger,
        OrganizationAuthorizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1",
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Role = OrganizationMemberRoleConstants.Owner,
                    Status = OrganizationMemberStatusConstants.Active,
                    Customer = new Customer { Id = "customer-1" }
                }
            ]
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(new ValueTask<Organization?>(organization));
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));

        var result = await sut.CanModifyAsync("org-1", "customer-1", cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Warning_When_Authorization_Is_Denied(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
        [Frozen] ILogger<OrganizationAuthorizationService> logger,
        OrganizationAuthorizationService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1",
            OrganizationMembers =
            [
                new OrganizationMember
                {
                    Role = OrganizationMemberRoleConstants.Member,
                    Status = OrganizationMemberStatusConstants.Active,
                    Customer = new Customer { Id = "customer-1" }
                }
            ]
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(new ValueTask<Organization?>(organization));
        A.CallTo(() => organizationSsoAuthorizationService.IsSsoValidAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));

        var result = await sut.CanModifyAsync("org-1", "customer-1", cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappened();
    }
}
