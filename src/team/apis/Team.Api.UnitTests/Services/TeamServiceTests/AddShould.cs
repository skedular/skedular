using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Team.Api.Services;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;

namespace Team.Api.UnitTests.Services.TeamServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Add_Completes(
        [Frozen]
        IDbTransactionBuilder transactionBuilder,
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        IRandomHelper randomHelper,
        [Frozen]
        ICachedCustomerService cachedCustomerService,
        [Frozen]
        IOrganizationAuthorizationService organizationAuthorizationService,
        [Frozen]
        IOrganizationOfferingService organizationOfferingService,
        [Frozen]
        ITeamOutboxPublisher teamOutboxPublisher,
        [Frozen]
        IEntityMapper entityMapper,
        [Frozen]
        ITeamMemberService teamMemberService,
        [Frozen]
        ICachedTeamService cachedTeamService,
        [Frozen]
        IOrganizationRepository organizationRepository,
        [Frozen]
        ITeamRepository teamRepository,
        [Frozen]
        IUnitOfWork unitOfWork,
        [Frozen]
        IDbContextTransaction transaction,
        [Frozen]
        ILogger<TeamService> logger,
        TeamService sut,
        CancellationToken cancellationToken)
    {
        var organizationEntity = new Organization
        {
            Id = "org-1",
        };
        var teamToAdd = new Shared.Models.Team
        {
            Name = "Operations",
            Organization = new Shared.Models.Organization
            {
                Id = "org-1",
            },
            TeamMembers = [],
        };
        var teamEntity = new Shared.Database.Entities.Team
        {
            Id = "team-1",
            Name = "Operations",
            OrganizationId = "org-1",
            Organization = organizationEntity,
            TeamMembers = [],
        };
        var teamModel = new Shared.Models.Team
        {
            Id = "team-1",
            Name = "Operations",
            Organization = new Shared.Models.Organization
            {
                Id = "org-1",
            },
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => cachedCustomerService.GetIdAsync(cancellationToken)).Returns("customer-1");
        A.CallTo(() => organizationRepository.UpsertNakedAsync("org-1", cancellationToken)).Returns(organizationEntity);
        A.CallTo(() => organizationAuthorizationService.CanModifyAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationOfferingService.CanCreateTeamAsync("org-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => organizationOfferingService.IsMoreInteractionAllowedAsync("org-1", "customer-1", cancellationToken))
            .Returns(new ValueTask<bool>(true));
        A.CallTo(() => randomHelper.Generate()).Returns("team-1");
        A.CallTo(() => entityMapper.MapTo(teamToAdd, organizationEntity, null)).Returns(teamEntity);
        A.CallTo(() => teamMemberService.BuildMembersAsync(teamToAdd.TeamMembers, teamEntity, "customer-1", organizationEntity, cancellationToken))
            .Returns([]);
        A.CallTo(() => transactionBuilder.BeginTransactionAsync(unitOfWork, cancellationToken)).Returns(transaction);
        A.CallTo(() => teamRepository.Add(teamEntity)).Returns(teamEntity);
        A.CallTo(() => entityMapper.MapTo(teamEntity)).Returns(teamModel);
        A.CallTo(() => unitOfWork.SaveChangesAsync(cancellationToken)).Returns(1);
        A.CallTo(() => transaction.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var result = await sut.AddAsync(teamToAdd, cancellationToken);

        result.Id.ShouldBe("team-1");
        A.CallTo(() => teamOutboxPublisher.PublishTeams(A<IReadOnlyList<Shared.Models.Team>>._, unitOfWork))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedTeamService.UpdateByIdAsync("team-1", cancellationToken)).MustHaveHappenedOnceExactly();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
