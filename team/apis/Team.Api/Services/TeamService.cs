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
using Location = Team.Shared.Database.Entities.Location;
using Organization = Team.Shared.Database.Entities.Organization;

namespace Team.Api.Services;

public interface ITeamService
{
    Task<Shared.Models.Team> AddAsync(Shared.Models.Team team, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Shared.Models.Team> UpdateAsync(Shared.Models.Team team, bool updateTeamMembers, CancellationToken cancellationToken);
    Task<Shared.Models.Team> DeleteAsync(string teamId, CancellationToken cancellationToken);
    Task<Shared.Models.Team?> GetByIdAsync(string teamId, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(string? organizationId, CancellationToken cancellationToken);

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
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider,
    ITeamMemberService teamMemberService) : ITeamService
{
    public async Task<Shared.Models.Team> AddAsync(Shared.Models.Team team, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetNullableAsync(cancellationToken);

        Location? primaryLocation = null;
        if (team.PrimaryLocation is not null)
        {
            primaryLocation = await repositoryFactory.LocationRepository.GetByIdAsync(team.PrimaryLocation.Id, cancellationToken);
            if (primaryLocation is null)
            {
                throw new LocationNotFound();
            }

            if (primaryLocation.Organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (primaryLocation.Organization.Id != team.Organization.Id)
            {
                throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
            }
        }

        var organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
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
                    primaryLocation,
                    true,
                    cancellationToken);
            }
        }
        else
        {
            team.Id = randomHelper.Generate();
        }

        var teamEntity = mapper.MapTo(team, organization, primaryLocation);
        teamEntity.PrimaryLocation = primaryLocation;
        var rebuiltTeamMembers = await teamMemberService.BuildMembersAsync(
            team.TeamMembers,
            teamEntity,
            customer,
            organization,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        teamEntity.TeamMembers = rebuiltTeamMembers;
        teamEntity = repositoryFactory.TeamRepository.Add(teamEntity);

        repositoryFactory.TeamMemberRepository.AddRange(rebuiltTeamMembers);
        team = mapper.MapTo(teamEntity);

        await teamOutboxPublisher.PublishTeamAsync([team], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return team;
    }

    public async Task<Shared.Models.Team> UpdateAsync(Shared.Models.Team team, bool updateTeamMembers, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(team.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        Location? primaryLocation = null;
        if (team.PrimaryLocation is not null)
        {
            primaryLocation = await repositoryFactory.LocationRepository.GetByIdAsync(team.PrimaryLocation.Id, cancellationToken);
            if (primaryLocation is null)
            {
                throw new LocationNotFound();
            }

            if (primaryLocation.Organization.Id != team.Organization.Id)
            {
                throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
            }
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(existingTeam.Organization.Id, false, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        return await UpdateInternalAsync(team, existingTeam, customer, organization, primaryLocation, updateTeamMembers, cancellationToken);
    }

    public async Task<Shared.Models.Team> DeleteAsync(string teamId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        if (!organizationOfferingService.IsMoreInteractionAllowed(existingTeam.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!teamAuthorizationService.CanDelete(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedTeam = mapper.MapTo(repositoryFactory.TeamRepository.Remove(existingTeam));

        await teamOutboxPublisher.PublishTeamAsync([deletedTeam], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedTeam;
    }

    public async Task<Shared.Models.Team?> GetByIdAsync(string teamId, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(teamId))
        {
            return null;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
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
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
        {
            throw new InvalidOperationException();
        }

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId))
        {
            // Ensure we do not return other customer team by forcing CustomerId as search criteria
            searchCriteria.CustomerId = customer.Id;
        }
        else
        {
            // TODO: 20250117 - Morteza: We currently only support returning teams for others customer when we are part
            // of same organization meaning organization ID is then required. We for now do not support use cases where
            // team is created without organization attached.    
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                searchCriteria.OrganizationId,
                false,
                cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                if (organization.OrganizationMembers.All(member => member.Customer.Id != customer.Id))
                {
                    throw new Unauthorized();
                }
            }
            else
            {
                if (organization.OrganizationMembers.All(member => member.Customer.Id != searchCriteria.CustomerId))
                {
                    throw new Unauthorized();
                }
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.TeamRepository.GetPaginatedTeamsAsync(
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

    public async Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(string? organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                organizationId,
                false,
                cancellationToken);
            if (organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!organizationAuthorizationService.CanView(organization, customer))
            {
                throw new Unauthorized();
            }
        }

        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customer.Id, organizationId, cancellationToken);
        return teams.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Team> UpdateInternalAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        Customer? customer,
        Organization? organization,
        Location? primaryLocation,
        bool updateTeamMembers,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !teamAuthorizationService.CanModify(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        var rebuiltTeamMembers = updateTeamMembers
            ? await teamMemberService.BuildMembersAsync(
                team.TeamMembers,
                existingTeam,
                customer,
                organization,
                cancellationToken)
            : [];
        var teamMembers = updateTeamMembers
            ? await repositoryFactory.TeamMemberRepository.GetByTeamIdAsync(existingTeam.Id, cancellationToken)
            : null;

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (updateTeamMembers)
        {
            var itemsToRemove = teamMembers!
                .Where(teamMember => rebuiltTeamMembers.All(item => item.Customer.Id != teamMember.Customer.Id))
                .ToList();
            var updatedItems = teamMembers!
                .Where(teamMember => rebuiltTeamMembers.Any(item => item.Customer.Id == teamMember.Customer.Id))
                .Select(teamMember =>
                {
                    teamMember.DeletedAt = null;
                    return repositoryFactory.TeamMemberRepository.Update(teamMember);
                }).ToList();
            var addedItems = rebuiltTeamMembers
                .Where(teamMember => teamMembers!.All(item => item.Customer.Id != teamMember.Customer.Id))
                .Select(teamMember => repositoryFactory.TeamMemberRepository.Add(teamMember)).ToList();

            repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
            existingTeam.TeamMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();
        }

        team = mapper.MapTo(repositoryFactory.TeamRepository.Update(mapper.MergeTo(team, existingTeam, organization, primaryLocation)));

        await teamOutboxPublisher.PublishTeamAsync([team], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return team;
    }

    private async Task<Shared.Models.Team> EnrichTeamAsync(Customer? customer, Shared.Database.Entities.Team team,
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
            .Query(new Specification<Booking> { Criteria = query => !query.DeletedAt.HasValue && query.Team.Id == team.Id && query.From >= now })
            .AnyAsync(cancellationToken);

        return mappedTeam;
    }
}
