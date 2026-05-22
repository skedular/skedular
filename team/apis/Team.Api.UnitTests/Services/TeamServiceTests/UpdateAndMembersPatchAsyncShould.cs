using Microsoft.Extensions.Logging;
using Team.Api.Models;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Mappers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Testing.Shared.Assertions;

namespace Team.Api.UnitTests.Services.TeamServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class UpdateAndMembersPatchAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Authorization_Rejection_And_Rethrow(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] IOrganizationOfferingService organizationOfferingService,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] ILogger<TeamService> logger,
        TeamService sut,
        CancellationToken cancellationToken)
    {
        var orgEntity = new Organization { Id = "org-1" };
        var teamEntity = new Shared.Database.Entities.Team { Id = "team-1", Organization = orgEntity, TeamMembers = [] };
        var teamModel = new Shared.Models.Team { Id = "team-1", TeamMembers = [] };
        var request = new TeamAndMembersPatchRequest(
            new Shared.Models.Team { Id = "team-1", Name = "Updated Team", TeamMembers = [] },
            new HashSet<TeamAndMembersPatchField> { TeamAndMembersPatchField.Team });

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => teamRepository.GetByIdAsync("team-1", cancellationToken)).Returns(teamEntity);
        A.CallTo(() => entityMapper.MapTo(teamEntity)).Returns(teamModel);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("cust-1");
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, false, cancellationToken))
            .Returns(orgEntity);
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => teamAuthorizationService.CanModifyAsync(teamEntity, "cust-1", cancellationToken))
            .Returns(new ValueTask<bool>(false));

        await Should.ThrowAsync<UnauthorizedAccessException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Warning)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("rejected by authorization"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Error_And_Rethrow_On_General_Failure(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] ILogger<TeamService> logger,
        TeamService sut,
        CancellationToken cancellationToken)
    {
        var request = new TeamAndMembersPatchRequest(
            new Shared.Models.Team { Id = "team-1", Name = "Updated Team", TeamMembers = [] },
            new HashSet<TeamAndMembersPatchField> { TeamAndMembersPatchField.Team });

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => teamRepository.GetByIdAsync("team-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("db failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLog(logger, LogLevel.Error)
            .Where(call => call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!
                .Contains("Team and members patch autosave failed"))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Autosave_Started(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] ILogger<TeamService> logger,
        TeamService sut,
        CancellationToken cancellationToken)
    {
        var request = new TeamAndMembersPatchRequest(
            new Shared.Models.Team { Id = "team-1", Name = "Updated Team", TeamMembers = [] },
            new HashSet<TeamAndMembersPatchField> { TeamAndMembersPatchField.Team });

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => teamRepository.GetByIdAsync("team-1", cancellationToken))
            .ThrowsAsync(new InvalidOperationException("forced early failure"));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.UpdateAsync(request, cancellationToken));

        LogAssertions.ACallToLogInfoContaining(logger, "Team and members patch autosave started")
            .MustHaveHappenedOnceExactly();
    }
}
