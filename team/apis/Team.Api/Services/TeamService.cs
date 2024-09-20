using Api.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Booking = Team.Shared.Database.Entities.Booking;
using Customer = Team.Shared.Models.Customer;
using TeamMember = Team.Shared.Database.Entities.TeamMember;
using Organization = Team.Shared.Database.Entities.Organization;

namespace Team.Api.Services;

public interface ITeamService
{
    Task<Shared.Models.Team> AddAsync(
        Shared.Models.Team team,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Team> UpdateAsync(Shared.Models.Team team, CancellationToken cancellationToken);
    Task<Shared.Models.Team> DeleteAsync(string teamId, CancellationToken cancellationToken);

    Task<Shared.Models.Team?> GetByIdAsync(
        string teamId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(
        string? organizationId,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Team>>, int )> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class TeamService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider) : ITeamService
{
    public async Task<Shared.Models.Team> AddAsync(
        Shared.Models.Team team,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerOptionalAsync(cancellationToken);
        Organization? organization = null;
        if (team.Organization is not null)
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(
                team.Organization.Id,
                cancellationToken);

            if (!ignoreAuthorizationCheck)
            {
                if (customer is null)
                {
                    throw new CustomerNotFound();
                }

                if (!organizationAuthorizationService.CanModify(organization, customer))
                {
                    throw new Unauthorized();
                }

                if (!organizationOfferingService.CanCreateTeam(organization) ||
                    !organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
                {
                    throw new NoMoreInteractionAllowed();
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(team.Id))
        {
            var existingTeam =
                await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
            if (existingTeam is not null)
            {
                if (!ignoreAuthorizationCheck && customer is null)
                {
                    throw new CustomerNotFound();
                }

                return await UpdateInternalAsync(
                    team,
                    existingTeam,
                    customer,
                    organization,
                    cancellationToken);
            }
        }
        else
        {
            team.Id = randomHelper.Generate();
        }

        var teamEntity = mapper.MapTo(team, organization);
        var teamMembers = await BuildTeamMembersAsync(team, teamEntity, customer, organization, cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TeamRepository.UnitOfWork,
                cancellationToken);

        teamEntity.TeamMembers = teamMembers;
        teamEntity = repositoryFactory.TeamRepository.Add(teamEntity);

        repositoryFactory.TeamMemberRepository.AddRange(teamMembers);
        team = mapper.MapTo(teamEntity);

        await teamOutboxPublisher.PublishTeamAsync(
            [team],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return team;
    }

    public async Task<Shared.Models.Team> UpdateAsync(
        Shared.Models.Team team,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(team.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTeam =
            await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        Organization? organization = null;
        if (existingTeam.Organization is not null)
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                existingTeam.Organization.Id,
                cancellationToken);
            ArgumentNullException.ThrowIfNull(organization);
            if (!organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        return await UpdateInternalAsync(team, existingTeam, customer, organization, cancellationToken);
    }

    public async Task<Shared.Models.Team> DeleteAsync(
        string teamId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTeam =
            await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        if (existingTeam.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingTeam.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!teamAuthorizationService.CanDelete(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TeamRepository.UnitOfWork,
                cancellationToken);

        var deletedTeam = mapper.MapTo(repositoryFactory.TeamRepository.Remove(existingTeam));

        await teamOutboxPublisher.PublishTeamAsync([deletedTeam],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedTeam;
    }

    public async Task<Shared.Models.Team?> GetByIdAsync(
        string teamId,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamId))
        {
            return null;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            return null;
        }

        return await EnrichTeamAsync(customer, team, cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Team>>, int)>
        GetPaginatedTeamsAsync(
            PaginationInputParam paginationInputParam,
            TeamSearchCriteria searchCriteria,
            ICollection<TeamOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        // Ensure we do not return other customer team by forcing CustomerId as search criteria
        searchCriteria.CustomerId = customer.Id;

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.TeamRepository.GetPaginatedTeamsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        var mappedTeams = new List<Edge<Shared.Models.Team>>();
        foreach (var edge in edges)
        {
            mappedTeams.Add(
                new Edge<Shared.Models.Team>(
                    edge.Cursor,
                    await EnrichTeamAsync(customer, edge.Node, cancellationToken)));
        }

        return (paginatedInfo, mappedTeams, totalCount);
    }

    public async Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(
        string? organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            var organization =
                await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new Unauthorized();
            }
        }

        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(
            customer.Id,
            organizationId,
            cancellationToken);
        return teams.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Team> UpdateInternalAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        Customer? customer,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !teamAuthorizationService.CanModify(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        var teamMembers = await BuildTeamMembersAsync(team, existingTeam, customer, organization, cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TeamRepository.UnitOfWork,
                cancellationToken);

        var itemsToRemove = existingTeam.TeamMembers
            .Where(teamMember => teamMembers.All(item => item.Customer.Id != teamMember.Customer.Id))
            .ToList();
        var updatedItems = existingTeam.TeamMembers
            .Where(teamMember => teamMembers.Any(item => item.Customer.Id == teamMember.Customer.Id))
            .Select(teamMember => repositoryFactory.TeamMemberRepository.Update(teamMember)).ToList();
        var addedItems = teamMembers
            .Where(teamMember => existingTeam.TeamMembers.All(item => item.Customer.Id != teamMember.Customer.Id))
            .Select(teamMember => repositoryFactory.TeamMemberRepository.Add(teamMember)).ToList();

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).ToList();

        team =
            mapper.MapTo(
                repositoryFactory.TeamRepository.Update(mapper.MergeTo(team, existingTeam)));

        await teamOutboxPublisher.PublishTeamAsync(
            [team],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return team;
    }

    private async Task<List<TeamMember>> BuildTeamMembersAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        Customer? customer,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var teamMembers = new List<TeamMember>();
        if (organization is null)
        {
            var customersToAdd = await repositoryFactory.CustomerRepository.GetByIdsAsync(
                team.TeamMembers
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    .Where(item => item.Customer is not null)
                    .Select(item => item.Customer.Id)
                    .ToList(),
                cancellationToken);

            teamMembers.AddRange(customersToAdd.Select(item => new TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                MembershipType =
                    customer is not null && item.Id == customer.Id
                        ? TeamMembershipType.Owner
                        : TeamMembershipType.Member,
                Customer = item,
                Team = existingTeam
            }));
        }
        else
        {
            var organizationMemberIds = team.TeamMembers
                .Where(item => item.OrganizationMember is not null)
                .Select(item => item.OrganizationMember!.Id)
                .ToList();
            var organizationMembersToAdd =
                organization.OrganizationMembers
                    .Where(item => organizationMemberIds.Contains(item.Id)).ToList();

            teamMembers.AddRange(organizationMembersToAdd.Select(item => new TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                MembershipType = customer is not null && item.Customer.Id == customer.Id
                    ? TeamMembershipType.Owner
                    : TeamMembershipType.Member,
                Customer = item.Customer,
                Team = existingTeam,
                OrganizationMember = item
            }));
        }

        return teamMembers;
    }

    private async Task<Shared.Models.Team> EnrichTeamAsync(
        Customer? customer,
        Shared.Database.Entities.Team team,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !teamAuthorizationService.CanView(team, customer))
        {
            throw new Unauthorized();
        }

        var mappedTeam = mapper.MapTo(team);

        if (customer is not null)
        {
            mappedTeam.Permissions = new Permissions
            {
                CanView = teamAuthorizationService.CanView(team, customer),
                CanModify = teamAuthorizationService.CanModify(team, customer),
                CanDelete = teamAuthorizationService.CanDelete(team, customer),
                CanInvitePeople = teamAuthorizationService.CanInvitePeople(team, customer),
                CanCancelPeopleExistingInvitations =
                    teamAuthorizationService.CanCancelPeopleExistingInvitations(team, customer)
            };
        }

        var now = timeProvider.GetUtcNow();
        mappedTeam.HasFutureBooking = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Team.Id == team.Id && query.From >= now
            })
            .AnyAsync(cancellationToken);

        return mappedTeam;
    }
}
