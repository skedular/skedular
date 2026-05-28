using Microsoft.Extensions.Logging;
using Team.Api.Services.Authorization;

namespace Team.Api.UnitTests.Services.Authorization.TeamAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AuthorizeShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Authorization_Is_Granted(
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILogger<TeamAuthorizationService> logger,
        TeamAuthorizationService sut,
        CancellationToken cancellationToken)
    {
        var team = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1" };

        A.CallTo(() => organizationAuthorizationService.CanViewAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));

        var result = await sut.CanViewAsync(team, "customer-1", cancellationToken);

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
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen] ILogger<TeamAuthorizationService> logger,
        TeamAuthorizationService sut,
        CancellationToken cancellationToken)
    {
        var team = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1" };

        A.CallTo(() => organizationAuthorizationService.CanViewAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        var result = await sut.CanViewAsync(team, "customer-1", cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Warning)
            .MustHaveHappened();
    }
}
