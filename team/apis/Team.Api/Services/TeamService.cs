using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Booking = Team.Shared.Database.Entities.Booking;
using Customer = Team.Shared.Database.Entities.Customer;
using Location = Team.Shared.Database.Entities.Location;
using Organization = Team.Shared.Database.Entities.Organization;

namespace Team.Api.Services;

public interface ITeamService
{
    Task<Shared.Models.Team> AddAsync(Shared.Models.Team team, CancellationToken cancellationToken);
    Task<Shared.Models.Team> UpdateAsync(Shared.Models.Team team, bool updateTeamMembers, CancellationToken cancellationToken);
    Task<Shared.Models.Team> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Team?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<bool> HasFutureBookingAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);

    Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Team>>, int)> GetPaginatedTeamsAsync(
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
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider,
    ITeamMemberService teamMemberService,
    ICachedTeamService cachedTeamService) : ITeamService
{
    public async Task<Shared.Models.Team> AddAsync(Shared.Models.Team team, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(team.Organization);

        var customer = await customerService.GetAsync(cancellationToken);

        Location? primaryLocation = null;
        if (team.PrimaryLocation is not null)
        {
            primaryLocation = await repositoryFactory.LocationRepository.GetByIdAsync(team.PrimaryLocation.Id, cancellationToken) ??
                              throw new LocationNotFound();
            if (primaryLocation.Organization is null)
            {
                throw new OrganizationNotFound();
            }

            if (!string.IsNullOrWhiteSpace(team.Organization.Id))
            {
                if (primaryLocation.Organization.Id != team.Organization.Id)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else if (!string.IsNullOrWhiteSpace(team.Organization.UniqueAlphanumericName))
            {
                if (primaryLocation.Organization.UniqueAlphanumericName != team.Organization.UniqueAlphanumericName)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else
            {
                throw new InvalidOperationException("Either organizationId or organizationUniqueAlphanumericName must be provided.");
            }
        }

        Organization organization;
        if (!string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(team.Organization.UniqueAlphanumericName))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                team.Organization.Id,
                team.Organization.UniqueAlphanumericName,
                false,
                cancellationToken) ?? throw new OrganizationNotFound();
        }
        else
        {
            throw new InvalidOperationException("Either organizationId or organizationUniqueAlphanumericName must be provided.");
        }

        if (!await organizationAuthorizationService.CanModifyAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await organizationOfferingService.CanCreateTeamAsync(organization.Id, cancellationToken) ||
            !await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!string.IsNullOrWhiteSpace(team.Id))
        {
            var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
            if (existingTeam is not null)
            {
                return await UpdateInternalAsync(team, existingTeam, customer, organization, primaryLocation, true, cancellationToken);
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
            customer.Id,
            organization,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        teamEntity.TeamMembers = rebuiltTeamMembers;
        teamEntity = repositoryFactory.TeamRepository.Add(teamEntity);

        repositoryFactory.TeamMemberRepository.AddRange(rebuiltTeamMembers);
        team = mapper.MapTo(teamEntity);

        teamOutboxPublisher.PublishTeams([team], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTeamService.UpdateByIdAsync(team.Id, cancellationToken);

        return team;
    }

    public async Task<Shared.Models.Team> UpdateAsync(Shared.Models.Team team, bool updateTeamMembers, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(team.Id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken) ?? throw new TeamNotFound();
        Location? primaryLocation = null;

        if (team.PrimaryLocation is not null)
        {
            primaryLocation = await repositoryFactory.LocationRepository.GetByIdAsync(team.PrimaryLocation.Id, cancellationToken) ??
                              throw new LocationNotFound();

            if (!string.IsNullOrWhiteSpace(existingTeam.Organization.Id))
            {
                if (primaryLocation.Organization.Id != existingTeam.Organization.Id)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else if (!string.IsNullOrWhiteSpace(existingTeam.Organization.UniqueAlphanumericName))
            {
                if (primaryLocation.Organization.UniqueAlphanumericName != existingTeam.Organization.UniqueAlphanumericName)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else
            {
                throw new InvalidOperationException("Either organizationId or organizationUniqueAlphanumericName must be provided.");
            }
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               existingTeam.Organization.Id,
                               existingTeam.Organization.UniqueAlphanumericName,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customer.Id, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        return await UpdateInternalAsync(team, existingTeam, customer, organization, primaryLocation, updateTeamMembers, cancellationToken);
    }

    public async Task<Shared.Models.Team> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(id, cancellationToken) ?? throw new TeamNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(existingTeam.Organization.Id, customer.Id, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!await teamAuthorizationService.CanDeleteAsync(existingTeam, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedTeam = mapper.MapTo(repositoryFactory.TeamRepository.Remove(existingTeam));

        teamOutboxPublisher.PublishTeams([deletedTeam], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTeamService.RemoveByIdAsync(existingTeam.Id, cancellationToken);

        return deletedTeam;
    }

    public async Task<Shared.Models.Team?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        var team = await cachedTeamService.GetByIdAsync(id, cancellationToken);
        if (team is null)
        {
            return null;
        }

        return await EnrichTeamAsync(customer, team, cancellationToken);
    }

    public async Task<bool> HasFutureBookingAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var team = await cachedTeamService.GetByIdAsync(id, cancellationToken) ?? throw new LocationNotFound();
        if (team.Organization.UniqueAlphanumericName == "skedularpubliclocations")
        {
            return false;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        if (customer is not null && !await teamAuthorizationService.CanViewAsync(team, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var now = timeProvider.GetUtcNow();
        return await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.InvolvedTeams.Select(item => item.Id).Contains(team.Id) && query.From >= now
            })
            .AnyAsync(cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Team>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueAlphanumericName) &&
            string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
        {
            throw new InvalidOperationException();
        }

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) && string.IsNullOrWhiteSpace(searchCriteria.OrganizationUniqueAlphanumericName))
        {
            // Ensure we do not return another customer team by forcing CustomerId as search criteria
            searchCriteria = searchCriteria with { CustomerId = customer.Id };
        }
        else
        {
            // TODO: 20250117 - Morteza: We currently only support returning teams for others customer when we are part
            // of same organization meaning organization ID is then required. We for now do not support use cases where
            // team is created without organization attached.    
            var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                   searchCriteria.OrganizationId,
                                   searchCriteria.OrganizationUniqueAlphanumericName,
                                   false,
                                   cancellationToken) ??
                               throw new OrganizationNotFound();

            if (!await organizationAuthorizationService.CanViewAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            if (string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                if (organization.OrganizationMembers.All(member => member.Customer.Id != customer.Id))
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                if (organization.OrganizationMembers.All(member => member.Customer.Id != searchCriteria.CustomerId))
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.TeamRepository.GetPaginatedTeamsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        await cachedTeamService.UpdateAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        var mappedTeams = new List<Edge<Shared.Models.Team>>();
        foreach (var edge in edges)
        {
            mappedTeams.Add(new Edge<Shared.Models.Team>(await EnrichTeamAsync(customer, edge.Node, cancellationToken), edge.Cursor));
        }

        return (paginatedInfo, mappedTeams, totalCount);
    }

    public async Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        Organization? organization = null;
        if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               organizationUniqueAlphanumericName,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
            if (!await organizationAuthorizationService.CanViewAsync(organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdAsync(customer.Id, organization?.Id, cancellationToken);

        await cachedTeamService.UpdateAsync(teams, cancellationToken);

        return teams.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Team> UpdateInternalAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        Customer customer,
        Organization organization,
        Location? primaryLocation,
        bool updateTeamMembers,
        CancellationToken cancellationToken)
    {
        if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var rebuiltTeamMembers = updateTeamMembers
            ? await teamMemberService.BuildMembersAsync(team.TeamMembers, existingTeam, customer.Id, organization, cancellationToken)
            : [];
        var teamMembers = updateTeamMembers
            ? await repositoryFactory.TeamMemberRepository.GetByTeamIdAsync(existingTeam.Id, cancellationToken)
            : null;

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (updateTeamMembers)
        {
            var itemsToRemove = teamMembers!.Where(teamMember => rebuiltTeamMembers.All(item => item.Customer.Id != teamMember.Customer.Id)).ToList();
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

        teamOutboxPublisher.PublishTeams([team], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTeamService.UpdateByIdAsync(team.Id, cancellationToken);

        return team;
    }

    private async Task<Shared.Models.Team> EnrichTeamAsync(
        Customer? customer,
        Shared.Database.Entities.Team team,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !await teamAuthorizationService.CanViewAsync(team, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var mappedTeam = mapper.MapTo(team);
        if (customer is not null)
        {
            mappedTeam.Permissions = new Permissions
            {
                CanView = await teamAuthorizationService.CanViewAsync(team, customer.Id, cancellationToken),
                CanModify = await teamAuthorizationService.CanModifyAsync(team, customer.Id, cancellationToken),
                CanDelete = await teamAuthorizationService.CanDeleteAsync(team, customer.Id, cancellationToken),
                CanInvitePeople = await teamAuthorizationService.CanInvitePeopleAsync(team, customer.Id, cancellationToken),
                CanCancelPeopleExistingInvitations =
                    await teamAuthorizationService.CanCancelPeopleExistingInvitationsAsync(team, customer.Id, cancellationToken)
            };
        }

        return mappedTeam;
    }
}
