using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Customer.Processors.Mappers;
using Customer.Shared.Database.Entities;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Kafka.Consume;
using Microsoft.EntityFrameworkCore;
using OrganizationMember = Customer.Shared.Database.Entities.OrganizationMember;
using Team = Customer.Shared.Database.Entities.Team;
using TeamMember = Customer.Shared.Database.Entities.TeamMember;
using Type = Api.Shared.Clients.Events.Skedular.Team.V1.Value.Type;

namespace Customer.Processors.Subscribers;

public class TeamSubscriber(
    ILogger<TeamSubscriber> logger,
    IMapper mapper,
    IRepositoryFactory repositoryFactory,
    ICustomerPublisher customerPublisher) : IEventSubscriber<Key, Event>
{
    public async Task<EventSubscriberResult> HandleAsync(EventContext eventContext, Key key, Event @event,
        CancellationToken cancellationToken)
    {
        switch (@event.Metadata.Type)
        {
            case Type.TeamUpserted:
                {
                    ArgumentException.ThrowIfNullOrWhiteSpace(@event.Data.Team.OrganizationId);

                    var team = mapper.MapTo(@event);
                    var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
                    var existingTeam = await repositoryFactory.TeamRepository.UpsertNakedAsync(
                        team.Id,
                        organization,
                        cancellationToken);
                    if (existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

                        return EventSubscriberResults.Success;
                    }

                    await HandleTeamUpsertedEventAsync(team, existingTeam, organization, cancellationToken);
                }
                break;

            case Type.TeamDeleted:
                {
                    var team = mapper.MapTo(@event);
                    var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, true, cancellationToken);
                    if (existingTeam is not null && existingTeam.EventRaisedAt > team.EventRaisedAt)
                    {
                        logger.LogInformation("Ignoring Team event. Event timestamp is older that what is already processed.");

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
        Team existingTeam,
        Organization organization,
        CancellationToken cancellationToken)
    {
        existingTeam = repositoryFactory.TeamRepository.Update(mapper.MergeToEntity(team, existingTeam, organization));

        _ = await RebuildTeamMembersAsync(team, existingTeam, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task HandleTeamDeletedEventAsync(Team existingTeam, CancellationToken cancellationToken)
    {
        await UpdateCustomerDefaultTeamsAsync(existingTeam, cancellationToken);
        await UpdateTeamMembersDefaultTeamsAsync(existingTeam, existingTeam.TeamMembers, cancellationToken);
        _ = repositoryFactory.TeamRepository.Remove(existingTeam);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
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
        foreach (var teamMember in teamMembers.Where(teamMember => team.TeamMembers.Any(item => item.Id == teamMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(teamMember.Customer.Id, cancellationToken);
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            OrganizationMember? organizationMember = null;
            if (teamMember.OrganizationMember is not null)
            {
                var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Organization.Id,
                    cancellationToken);

                var organizationMemberCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(
                    teamMember.OrganizationMember.Customer.Id,
                    cancellationToken);
                ArgumentNullException.ThrowIfNull(organizationMemberCustomer);

                organizationMember = await repositoryFactory.OrganizationMemberRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Id,
                    organization,
                    organizationMemberCustomer,
                    cancellationToken);
            }

            var updatedTeamMember = mapper.MergeToEntity(
                team.TeamMembers.First(item => item.Id == teamMember.Id),
                teamMember,
                existingTeam,
                customer,
                organizationMember);
            updatedTeamMember.DeletedAt = null;
            updatedItems.Add(repositoryFactory.TeamMemberRepository.Update(updatedTeamMember));
        }

        var addedItems = new List<TeamMember>();
        foreach (var teamMember in team.TeamMembers.Where(teamMember => teamMembers.All(item => item.Id != teamMember.Id)))
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(teamMember.Customer.Id, cancellationToken);
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            OrganizationMember? organizationMember = null;
            if (teamMember.OrganizationMember is not null)
            {
                var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Organization.Id,
                    cancellationToken);

                var organizationMemberCustomer = await repositoryFactory.CustomerRepository.GetByIdAsync(
                    teamMember.OrganizationMember.Customer.Id,
                    cancellationToken);
                ArgumentNullException.ThrowIfNull(organizationMemberCustomer);

                organizationMember = await repositoryFactory.OrganizationMemberRepository.UpsertNakedAsync(
                    teamMember.OrganizationMember.Id,
                    organization,
                    organizationMemberCustomer,
                    cancellationToken);
            }

            addedItems.Add(repositoryFactory.TeamMemberRepository.Add(mapper.MapToEntity(teamMember, existingTeam, customer, organizationMember)));
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
            var member = await repositoryFactory.TeamMemberRepository.Query(
                    new Specification<TeamMember> { Criteria = query => query.Id == teamMemberId }
                        .AddInclude(query => query.Customer))
                .FirstAsync(cancellationToken);

            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(member.Customer.Id, cancellationToken);
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            var existingTeamIds = customer.PreferredTeams.Select(item => item.Id).Distinct().ToList();
            customer.PreferredTeams = customer.PreferredTeams.Where(item => item.Id != existingTeam.Id).ToList();
            var newTeamIds = customer.PreferredTeams.Select(item => item.Id).Distinct().ToList();
            customer = repositoryFactory.CustomerRepository.Update(customer);

            if (newTeamIds.Count != existingTeamIds.Count || newTeamIds.Except(existingTeamIds).Any())
            {
                await customerPublisher.PublishCustomersAsync([mapper.MapTo(customer)!], cancellationToken);
            }
        }
    }

    private async Task UpdateCustomerDefaultTeamsAsync(Team team, CancellationToken cancellationToken)
    {
        var customerIds = team.PreferredByCustomers.Select(customer => customer.Id).ToList();
        foreach (var customerId in customerIds)
        {
            var customer = await repositoryFactory.CustomerRepository.GetByIdAsync(customerId, cancellationToken);
            if (customer is null)
            {
                throw new CustomerNotFound();
            }

            customer.PreferredTeams = customer.PreferredTeams.Where(item => item.Id != team.Id).ToList();
            _ = repositoryFactory.CustomerRepository.Update(customer);
        }
    }
}
