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

namespace Team.Api.UnitTests.Services.TeamServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DeleteShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Delete_Completes(
        [Frozen] IDbTransactionBuilder transactionBuilder,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ICachedCustomerService cachedCustomerService,
        [Frozen] ITeamAuthorizationService teamAuthorizationService,
        [Frozen] IMapper mapper,
        [Frozen] ITeamOutboxPublisher teamOutboxPublisher,
        [Frozen] ICachedTeamService cachedTeamService,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IDbContextTransaction transaction,
        [Frozen] ILogger<TeamService> logger,
        TeamService sut,
        CancellationToken cancellationToken)
    {
        var teamEntity = new Shared.Database.Entities.Team
        {
            Id = "team-1", OrganizationId = "org-1", Organization = new Organization { Id = "org-1" }
        };
        var deletedTeamEntity = new Shared.Database.Entities.Team { Id = "team-1", OrganizationId = "org-1", Organization = teamEntity.Organization };
        var deletedTeamModel = new Shared.Models.Team { Id = "team-1", Organization = new Shared.Models.Organization { Id = "org-1" } };

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => teamRepository.GetByIdAsync("team-1", cancellationToken)).Returns(teamEntity);
        A.CallTo(() => teamAuthorizationService.CanDeleteAsync(teamEntity, "customer-1", cancellationToken)).Returns(new ValueTask<bool>(true));
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => teamRepository.Remove(teamEntity)).Returns(deletedTeamEntity);
        A.CallTo(() => mapper.MapTo(deletedTeamEntity)).Returns(deletedTeamModel);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.DeleteAsync("team-1", cancellationToken);

        result.Id.ShouldBe("team-1");
        A.CallTo(() => teamOutboxPublisher.PublishTeams(A<ICollection<Shared.Models.Team>>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedTeamService.RemoveByIdAsync("team-1", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
