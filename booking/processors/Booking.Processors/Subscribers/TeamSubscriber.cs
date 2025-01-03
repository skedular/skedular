using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Booking.Processors.Mappers;
using Booking.Shared.Repositories;
using Enterprise.Shared.Kafka.Consume;
using Team = Booking.Shared.Database.Entities.Team;
using TeamMember = Booking.Shared.Database.Entities.TeamMember;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(
        EventContext eventContext,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    var organization = team.Organization is null
                        ? null
                        : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                            team.Organization.Id,
                            cancellationToken);
                    var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(
                        team.Id,
                        organization,
                        cancellationToken);
                    if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    if (existingTeam is null)
                    {
                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamDeletedEventAsync(existingTeam, cancellationToken);
                }
                break;

            case Type.InvitationToJoinTeamUpserted:
            case Type.InvitationToJoinTeamDeleted:
                break;
        }

        return EventSubscriberResults.Success;
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team? existingTeam,
        CancellationToken cancellationToken)
    {
        var organization = team.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.GetByIdAsync(team.Organization.Id, cancellationToken);

        existingTeam = existingTeam is null
            ? repositoryFactory.TeamRepository.Add(mapper.MapToEntity(team, organization))
            : repositoryFactory.TeamRepository.Update(
                mapper.MergeToEntity(team, existingTeam, organization));

        _ = await RebuildTeamMembersAsync(team, existingTeam, cancellationToken);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(
        Team existingTeam,
        CancellationToken cancellationToken)
    {
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Team> RebuildTeamMembersAsync(
        Shared.Models.Team team,
        Team existingTeam,
        CancellationToken cancellationToken)
    {
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByTeamIdAsync(
            existingTeam.Id,
            cancellationToken);
        var itemsToRemove = teamMembers
            .Where(teamMember => team.TeamMembers.All(item => item.Id != teamMember.Id))
            .ToList();
        var updatedItems = new List<TeamMember>();
        foreach (var teamMember in teamMembers
                     .Where(teamMember => team.TeamMembers.Any(item => item.Id == teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            var updatedTeamMember = mapper.MergeToEntity(
                team.TeamMembers.First(item => item.Id == teamMember.Id),
                teamMember,
                existingTeam,
                customer);
            updatedTeamMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.TeamMemberRepository.Update(updatedTeamMember));
        }

        var addedItems = new List<TeamMember>();
        foreach (var teamMember in team.TeamMembers
                     .Where(teamMember =>
                         teamMembers.All(item => item.Id != teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            addedItems.Add(repositoryFactory.TeamMemberRepository.Add(
                mapper.MapToEntity(teamMember, existingTeam, customer)));
        }

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        return existingTeam;
    }
}
