using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Confluent.Kafka;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Kafka.Consume;
using Team.Processors.Mappers;
using Team.Shared.Database.Entities;
using Team.Shared.Repositories;
using OrganizationMember = Team.Shared.Database.Entities.OrganizationMember;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Team.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    ApplicationConfiguration applicationConfiguration,
    IMapper mapper,
    IRepositoryFactory repositoryFactory)
    : IEventSubscriber<Key, Event>
{
    public async Task HandleAsync(
        Headers headers,
        Key key,
        Event @event,
        CancellationToken cancellationToken)
    {
        if (@event.Metadata.DomainSource == applicationConfiguration.DomainSource)
        {
            // Event raised previously by this domain, ignoring it.
            return;
        }

        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam =
                        await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.ModifiedAt > team.ModifiedAt)
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
                    var existingTeam =
                        await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.ModifiedAt > team.ModifiedAt)
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
        Shared.Database.Entities.Team? existingTeam,
        CancellationToken cancellationToken)
    {
        var organization = team.Organization is null
            ? null
            : await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id,
                cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        if (existingTeam is null)
        {
            _ = repositoryFactory.TeamRepository.Add(mapper.MapToEntity(team, organization));
        }
        else
        {
            existingTeam = repositoryFactory.TeamRepository.Update(
                mapper.MergeToEntity(team, existingTeam, organization));
            _ = await RebuildTeamMembersAsync(team, existingTeam, cancellationToken);
        }

        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(
        Shared.Database.Entities.Team existingTeam,
        CancellationToken cancellationToken)
    {
        repositoryFactory.TeamMemberRepository.RemoveRange(existingTeam.TeamMembers);
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Shared.Database.Entities.Team> RebuildTeamMembersAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        CancellationToken cancellationToken)
    {
        var itemsToRemove = existingTeam.TeamMembers
            .Where(teamMember => team.TeamMembers.All(item => item.Id != teamMember.Id))
            .ToList();
        var updatedItems = new List<TeamMember>();
        foreach (var teamMember in existingTeam.TeamMembers
                     .Where(teamMember =>
                         team.TeamMembers.Any(item => item.Id == teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            OrganizationMember? organizationMember = null;
            if (teamMember.OrganizationMember is not null)
            {
                var organization =
                    await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                        teamMember.OrganizationMember.Organization.Id,
                        cancellationToken);

                var organizationMemberCustomer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Customer.Id,
                    cancellationToken);

                organizationMember =
                    await repositoryFactory.OrganizationMemberRepository.UpsertNakedAsync(
                        teamMember.OrganizationMember.Id,
                        organization,
                        organizationMemberCustomer,
                        cancellationToken);
            }

            updatedItems.Add(repositoryFactory.TeamMemberRepository.Update(
                mapper.MergeToEntity(
                    team.TeamMembers.Single(item => item.Id == teamMember.Id),
                    teamMember,
                    existingTeam,
                    customer,
                    organizationMember)));
        }

        var addedItems = new List<TeamMember>();
        foreach (var teamMember in team.TeamMembers.Where(teamMember =>
                     existingTeam.TeamMembers.All(item => item.Id != teamMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(teamMember.Customer.Id,
                    cancellationToken);

            OrganizationMember? organizationMember = null;
            if (teamMember.OrganizationMember is not null)
            {
                var organization =
                    await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                        teamMember.OrganizationMember.Organization.Id,
                        cancellationToken);

                var organizationMemberCustomer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Customer.Id,
                    cancellationToken);

                organizationMember =
                    await repositoryFactory.OrganizationMemberRepository.UpsertNakedAsync(
                        teamMember.OrganizationMember.Id,
                        organization,
                        organizationMemberCustomer,
                        cancellationToken);
            }

            addedItems.Add(repositoryFactory.TeamMemberRepository.Add(
                mapper.MapToEntity(teamMember, existingTeam, customer, organizationMember)));
        }

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).ToList();

        return existingTeam;
    }
}
