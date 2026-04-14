using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;
using Team.Shared.Services;
using Team.Shared.Services.Cache;
using Organization = Team.Shared.Models.Organization;

namespace Team.Api.UnitTests.Services.InvitationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class CancelInvitationShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Invitation_Is_Cancelled(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] IMapper mapper,
        [Frozen] ITemporalOutboxService temporalOutboxService,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ICachedTeamService cachedTeamService,
        [Frozen] IJoinInvitationRepository joinInvitationRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<InvitationService> logger,
        InvitationService sut,
        CancellationToken cancellationToken)
    {
        var team = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1" };
        var invitation = new JoinInvitation
        {
            Id = "inv-1",
            Status = InvitationStatusConstants.Pending,
            Role = TeamMemberRoleConstants.Member,
            Team = team,
            CreatedBy = new Customer { Id = "creator-1" }
        };
        var cachedTeam = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1" };
        var mappedInvitation = new Shared.Models.JoinInvitation
        {
            Id = "inv-1",
            Status = InvitationStatus.Cancelled,
            Team = new Shared.Models.Team { Id = "team-1", Organization = new Organization { Id = "org-1" } }
        };

        A.CallTo(() => repositoryFactory.JoinInvitationRepository).Returns(joinInvitationRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => joinInvitationRepository.GetByIdAsync("inv-1", cancellationToken)).Returns(invitation);
        A.CallTo(() => cachedTeamService.GetByIdAsync("team-1", cancellationToken)).Returns(cachedTeam);
        A.CallTo(() => teamAuthorizationService.CanCancelPeopleExistingInvitationsAsync(cachedTeam, "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => joinInvitationRepository.Update(invitation)).Returns(invitation);
        A.CallTo(() => mapper.MapTo(invitation)).Returns(mappedInvitation);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.CancelInvitationToJoinAsync("inv-1", cancellationToken);

        result.Id.ShouldBe("inv-1");
        result.Status.ShouldBe(InvitationStatus.Cancelled);
        A.CallTo(() => temporalOutboxService.SignalWorkflowInviteToJoinInvitationStatusChanged("inv-1", unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
