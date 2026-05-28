using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Team.Api.Services;
using Team.Shared.Database.Entities;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services;
using Organization = Team.Shared.Models.Organization;

namespace Team.Api.UnitTests.Services.InvitationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AcceptInvitationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Invitation_Is_Accepted(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICustomerService customerService,
        [Frozen] IEntityMapper entityMapper,
        [Frozen] IRandomHelper randomHelper,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] ITeamOutboxPublisher teamOutboxPublisher,
        [Frozen] IJoinInvitationRepository joinInvitationRepository,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] ITeamMemberRepository teamMemberRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<InvitationService> logger,
        InvitationService sut,
        CancellationToken cancellationToken)
    {
        var customer = new Customer { Id = "customer-1", Identities = [] };
        var team = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1", TeamMembers = [] };
        var invitation = new JoinInvitation
        {
            Id = "inv-1",
            Status = InvitationStatusConstants.Pending,
            Role = TeamMemberRoleConstants.Member,
            Team = team,
            CreatedBy = new Customer { Id = "creator-1" },
            Invitee = customer
        };
        var mappedTeam = new Shared.Models.Team { Id = "team-1", Organization = new Organization { Id = "org-1" } };
        var mappedInvitation = new Shared.Models.JoinInvitation { Id = "inv-1", Team = mappedTeam };

        A.CallTo(() => repositoryFactory.JoinInvitationRepository).Returns(joinInvitationRepository);
        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.TeamMemberRepository).Returns(teamMemberRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => customerService.GetAsync(cancellationToken)).Returns(customer);
        A.CallTo(() => joinInvitationRepository.GetByIdAsync("inv-1", cancellationToken)).Returns(invitation);
        A.CallTo(() => teamRepository.GetByIdAsync("team-1", cancellationToken)).Returns(team);
        A.CallTo(() => randomHelper.Generate()).Returns("member-1");
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => joinInvitationRepository.Update(A<JoinInvitation>._))
            .Invokes((JoinInvitation inv) => inv.Status = InvitationStatusConstants.Accepted)
            .Returns(invitation);
        A.CallTo(() => entityMapper.MapTo(team)).Returns(mappedTeam);
        A.CallTo(() => entityMapper.MapTo(invitation)).Returns(mappedInvitation);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.AcceptInvitationToJoinAsync("inv-1", cancellationToken);

        result.Id.ShouldBe("inv-1");
        A.CallTo(() => temporalOutboxService.SignalWorkflowInviteToJoinInvitationStatusChanged("inv-1", unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => teamOutboxPublisher.PublishTeams(A<IReadOnlyList<Shared.Models.Team>>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
