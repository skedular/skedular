using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Microsoft.Extensions.Logging;
using Team.Api.Services.Authorization;
using Team.Shared.Services.Cache;
using Organization = Team.Shared.Database.Entities.Organization;
using Offering = Api.Shared.Services.Models.Offering;

namespace Team.Api.UnitTests.Services.Authorization.OrganizationOfferingServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CanCreateTeamShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Organization_Is_Not_Private(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1", Type = OrganizationTypeConstants.Marketplace, Offering = new Offering { Code = OfferingCode.PayAsYouGoV1 }, Teams = []
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);

        await Should.ThrowAsync<TeamNotAllowedForOrganizationType>(async () => await sut.CanCreateTeamAsync("org-1", cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_True_When_Private_Organization_Is_Within_Offering_Limit(
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        [Frozen] ILogger<OrganizationOfferingService> logger,
        OrganizationOfferingService sut,
        CancellationToken cancellationToken)
    {
        var organization = new Organization
        {
            Id = "org-1", Type = OrganizationTypeConstants.Private, Offering = new Offering { Code = OfferingCode.PayAsYouGoV1 }, Teams = []
        };

        A.CallTo(() => cachedOrganizationService.GetByIdOrCustomDomainAsync("org-1", null, cancellationToken))
            .Returns(organization);

        var result = await sut.CanCreateTeamAsync("org-1", cancellationToken);

        result.ShouldBeTrue();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
