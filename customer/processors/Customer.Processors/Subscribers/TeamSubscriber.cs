using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Customer.Processors.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using Team = Customer.Shared.Database.Entities.Team;
using Type = Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICustomerPublisher customerPublisher) : IEventSubscriber<Key, Event>
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
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
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
        await UpdateCustomerDefaultTeamsAsync(existingTeam, cancellationToken);
        await UpdateTeamMembersDefaultTeamsAsync(existingTeam, existingTeam.TeamMembers, cancellationToken);
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.CustomerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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

            OrganizationMember? organizationMember = null;
            if (teamMember.OrganizationMember is not null)
            {
                var organization =
                    await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                        teamMember.OrganizationMember.Organization!.Id,
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

            updatedItems.Add(repositoryFactory.TeamMemberRepository.Update(mapper.MergeToEntity(
                team.TeamMembers.Single(item => item.Id == teamMember.Id),
                teamMember,
                existingTeam,
                customer,
                organizationMember)));
        }

        var addedItems = new List<TeamMember>();
        foreach (var teamMember in team.TeamMembers
                     .Where(teamMember =>
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
                        teamMember.OrganizationMember.Organization!.Id,
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
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();
        await UpdateTeamMembersDefaultTeamsAsync(existingTeam, itemsToRemove, cancellationToken);

        return existingTeam;
    }

    private async Task UpdateTeamMembersDefaultTeamsAsync(
        Team existingTeam,
        IEnumerable<TeamMember> teamMembersToRemove,
        CancellationToken cancellationToken)
    {
        var teamMemberIds = teamMembersToRemove.Select(teamMember => teamMember.Id).ToList();
        foreach (var teamMemberId in teamMemberIds)
        {
            var member = await repositoryFactory.TeamMemberRepository
                .Query(new Specification<TeamMember> { Criteria = query => query.Id == teamMemberId }
                    .AddInclude(query => query.Customer))
                .FirstAsync(cancellationToken);

            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(
                member.Customer.Id,
                cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            customer.DefaultTeams = customer.DefaultTeams.Where(team => team.Id != existingTeam.Id).ToList();
            customer = repositoryFactory.CustomerRepository.Update(customer);
            await customerPublisher.PublishCustomerAsync([mapper.MapTo(customer)!], cancellationToken);
        }
    }

    private async Task UpdateCustomerDefaultTeamsAsync(Team team, CancellationToken cancellationToken)
    {
        var customerIds = team.DefaultedByCustomers.Select(customer => customer.Id).ToList();
        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
            ArgumentNullException.ThrowIfNull(customer);

            customer.DefaultTeams = customer.DefaultTeams.Where(item => item.Id != team.Id).ToList();
            _ = repositoryFactory.CustomerRepository.Update(customer);
        }
    }
}
