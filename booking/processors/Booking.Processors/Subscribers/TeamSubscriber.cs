using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Booking.Processors.Mappers;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using Confluent.Kafka;
using Enterprise.Shared.Kafka.Consume;
using Team = Booking.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Booking.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory) : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(Headers headers, Key key, Event @event, CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation(
                            "Ignoring Team event. Event timestamp is older that what is already processed.");

                        return;
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

                        return;
                    }

                    if (existingTeam is null)
                    {
                        return;
                    }

                    await HandleTeamDeletedEventAsync(existingTeam, cancellationToken);
                }
                break;

            case Type.NotificationUpserted:
            case Type.NotificationDeleted:
            default:
                return;
        }
    }

    private async Task HandleTeamUpsertedEventAsync(
        Shared.Models.Team team,
        Team? existingTeam,
        CancellationToken cancellationToken)
    {
        var organization = team.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

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
        var itemsToRemove = existingTeam.TeamMembers
            .Where(teamMember => team.TeamMembers.All(item => item.Id != teamMember.Id))
            .ToList();
        var updatedItems = new List<TeamMember>();
        foreach (var teamMember in existingTeam.TeamMembers
                     .Where(teamMember => team.TeamMembers.Any(item => item.Id == teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            updatedItems.Add(repositoryFactory.TeamMemberRepository.Update(mapper.MergeToEntity(
                team.TeamMembers.Single(item => item.Id == teamMember.Id),
                teamMember,
                existingTeam,
                customer)));
        }

        var addedItems = new List<TeamMember>();
        foreach (var teamMember in team.TeamMembers
                     .Where(teamMember =>
                         existingTeam.TeamMembers.All(item => item.Id != teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            addedItems.Add(repositoryFactory.TeamMemberRepository.Add(
                mapper.MapToEntity(teamMember, existingTeam, customer)));
        }

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).ToList();

        return existingTeam;
    }
}
