using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Organization = Team.Shared.Models.Organization;

namespace Team.Api.UnitTests.Services.TeamMemberServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class ChangeStatusShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Statuses_Change(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] ITeamOutboxPublisher teamOutboxPublisher,
        [Frozen] IMapper mapper,
        [Frozen] ITeamMemberRepository teamMemberRepository,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<TeamMemberService> logger,
        TeamMemberService sut,
        CancellationToken cancellationToken)
    {
        var team = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1" };
        var teamMember = new TeamMember
        {
            Id = "member-1",
            Team = team,
            Customer = new Customer { Id = "customer-2" },
            Role = TeamMemberRoleConstants.Member,
            Status = TeamMemberStatusConstants.Active
        };
        var mappedTeam = new Shared.Models.Team { Id = "team-1", Organization = new Organization { Id = "org-1" } };
        var mappedMember = new Shared.Models.TeamMember { Id = "member-1", Team = mappedTeam };

        A.CallTo(() => repositoryFactory.TeamMemberRepository).Returns(teamMemberRepository);
        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => teamMemberRepository.GetByIdsAsync(A<IReadOnlyList<string>>._, cancellationToken)).Returns([teamMember]);
        A.CallTo(() => teamRepository.GetByIdsAsync(A<IReadOnlyList<string>>._, cancellationToken)).Returns([team]);
        A.CallTo(() => teamAuthorizationService.CanModifyAsync(team, "customer-1", cancellationToken)).Returns(new ValueTask<bool>(true));
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => mapper.MapTo(team)).Returns(mappedTeam);
        A.CallTo(() => mapper.MapTo(teamMember, mappedTeam)).Returns(mappedMember);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.ChangeStatusAsync(["member-1"], TeamMemberStatus.Inactive, cancellationToken);

        result.Count.ShouldBe(1);
        A.CallTo(() => teamOutboxPublisher.PublishTeams(A<IReadOnlyList<Shared.Models.Team>>._, unitOfWork)).MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
