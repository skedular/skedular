using Team.Api.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;

namespace Team.Api.Services;

public interface IWorkaroundService
{
    Task RepublishTeamAsync(string teamId, CancellationToken cancellationToken);
    Task RepublishAllTeamsAsync(CancellationToken cancellationToken);
}

public class WorkaroundService(IRepositoryFactory repositoryFactory, IMapper mapper, ITeamPublisher teamPublisher) : IWorkaroundService
{
    public async Task RepublishTeamAsync(string teamId, CancellationToken cancellationToken)
    {
        var team = await repositoryFactory.TeamRepository.GetByIdUntrackedAsync(teamId, cancellationToken);
        if (team is null)
        {
            return;
        }

        await teamPublisher.PublishTeamsAsync([mapper.MapTo(team)], cancellationToken);
    }

    public async Task RepublishAllTeamsAsync(CancellationToken cancellationToken)
    {
        var teams = await repositoryFactory.TeamRepository.GetAllUntrackedAsync(cancellationToken);
        await teamPublisher.PublishTeamsAsync(teams.Select(mapper.MapTo).ToList(), cancellationToken);
    }
}
