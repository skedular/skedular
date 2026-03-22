using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
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
        string? organizationCustomDomain,
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
    ICachedCustomerService cachedCustomerService,
    ICachedOrganizationService cachedOrganizationService,
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

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

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
            else if (!string.IsNullOrWhiteSpace(team.Organization.CustomDomain))
            {
                if (primaryLocation.Organization.CustomDomain != team.Organization.CustomDomain)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else
            {
                throw new InvalidOperationException("Either organizationId or organizationCustomDomain must be provided.");
            }
        }

        Organization organization;
        if (!string.IsNullOrWhiteSpace(team.Organization.Id))
        {
            organization = await repositoryFactory.OrganizationRepository.UpsertNakedAsync(team.Organization.Id, cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(team.Organization.CustomDomain))
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                team.Organization.Id,
                team.Organization.CustomDomain,
                false,
                cancellationToken) ?? throw new OrganizationNotFound();
        }
        else
        {
            throw new InvalidOperationException("Either organizationId or organizationCustomDomain must be provided.");
        }

        if (!await organizationAuthorizationService.CanModifyAsync(organization.Id, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!await organizationOfferingService.CanCreateTeamAsync(organization.Id, cancellationToken) ||
            !await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (string.IsNullOrWhiteSpace(team.Id))
        {
            team.Id = randomHelper.Generate();
        }
        else
        {
            var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(team.Id, cancellationToken);
            if (existingTeam is not null)
            {
                return await UpdateInternalAsync(team, existingTeam, customerId, organization, primaryLocation, true, cancellationToken);
            }
        }

        var teamEntity = mapper.MapTo(team, organization, primaryLocation);
        teamEntity.PrimaryLocation = primaryLocation;
        var rebuiltTeamMembers = await teamMemberService.BuildMembersAsync(team.TeamMembers, teamEntity, customerId, organization, cancellationToken);

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

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
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
            else if (!string.IsNullOrWhiteSpace(existingTeam.Organization.CustomDomain))
            {
                if (primaryLocation.Organization.CustomDomain != existingTeam.Organization.CustomDomain)
                {
                    throw new TeamPrimaryLocationOrganizationDoesNotMatchTeamOrganization();
                }
            }
            else
            {
                throw new InvalidOperationException("Either organizationId or organizationCustomDomain must be provided.");
            }
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               existingTeam.Organization.Id,
                               existingTeam.Organization.CustomDomain,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        return await UpdateInternalAsync(team, existingTeam, customerId, organization, primaryLocation, updateTeamMembers, cancellationToken);
    }

    public async Task<Shared.Models.Team> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(id, cancellationToken) ?? throw new TeamNotFound();

        if (!await teamAuthorizationService.CanDeleteAsync(existingTeam, customerId, cancellationToken))
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

        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        var team = await cachedTeamService.GetByIdAsync(id, cancellationToken);
        if (team is null)
        {
            return null;
        }

        return await EnrichTeamAsync(customerId, team, cancellationToken);
    }

    public async Task<bool> HasFutureBookingAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var team = await cachedTeamService.GetByIdAsync(id, cancellationToken) ?? throw new LocationNotFound();
        if (team.Organization.CustomDomain == Constants.SkedularPublicLocationsCustomDomainName)
        {
            return false;
        }

        if (!ignoreAuthorizationCheck)
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            if (!await teamAuthorizationService.CanViewAsync(team, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        return await repositoryFactory.BookingRepository.AnyBookingExistsUntrackedAsync(team.Id, timeProvider.GetUtcNow(), cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Team>>, int)> GetPaginatedTeamsAsync(
        PaginationInputParam paginationInputParam,
        TeamSearchCriteria searchCriteria,
        ICollection<TeamOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) &&
            string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain) &&
            string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
        {
            throw new InvalidOperationException();
        }

        if (string.IsNullOrWhiteSpace(searchCriteria.OrganizationId) && string.IsNullOrWhiteSpace(searchCriteria.OrganizationCustomDomain))
        {
            // Ensure we do not return another customer team by forcing CustomerId as search criteria
            searchCriteria = searchCriteria with { CustomerId = customerId };
        }
        else
        {
            // TODO: 20250117 - Morteza: We currently only support returning teams for others customer when we are part
            // of same organization meaning organization ID is then required. We for now do not support use cases where
            // team is created without organization attached.    
            var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(
                                   searchCriteria.OrganizationId,
                                   searchCriteria.OrganizationCustomDomain,
                                   cancellationToken) ??
                               throw new OrganizationNotFound();

            if (!await organizationAuthorizationService.CanViewAsync(organization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            if (string.IsNullOrWhiteSpace(searchCriteria.CustomerId))
            {
                if (organization.OrganizationMembers.All(member => member.Customer.Id != customerId))
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

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.TeamRepository.GetPaginatedTeamsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedTeams = new List<Edge<Shared.Models.Team>>();
        foreach (var edge in edges)
        {
            mappedTeams.Add(new Edge<Shared.Models.Team>(await EnrichTeamAsync(customerId, edge.Node, cancellationToken), edge.Cursor));
        }

        return (paginatedInfo, mappedTeams, totalCount);
    }

    public async Task<ICollection<Shared.Models.Team>> GetMyTeamsAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        Organization? organization = null;
        if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
            if (!await organizationAuthorizationService.CanViewAsync(organization.Id, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var teams = await repositoryFactory.TeamRepository.GetByCustomerIdUntrackedAsync(customerId, organization?.Id, cancellationToken);

        await cachedTeamService.UpdateAsync(teams, cancellationToken);

        return teams.Select(mapper.MapTo).ToList();
    }

    private async Task<Shared.Models.Team> UpdateInternalAsync(
        Shared.Models.Team team,
        Shared.Database.Entities.Team existingTeam,
        string customerId,
        Organization organization,
        Location? primaryLocation,
        bool updateTeamMembers,
        CancellationToken cancellationToken)
    {
        if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var rebuiltTeamMembers = updateTeamMembers
            ? await teamMemberService.BuildMembersAsync(team.TeamMembers, existingTeam, customerId, organization, cancellationToken)
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
        string? customerId,
        Shared.Database.Entities.Team team,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(customerId) && !await teamAuthorizationService.CanViewAsync(team, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var mappedTeam = mapper.MapTo(team);
        if (!string.IsNullOrWhiteSpace(customerId))
        {
            mappedTeam.Permissions = new Permissions
            {
                CanView = await teamAuthorizationService.CanViewAsync(team, customerId, cancellationToken),
                CanModify = await teamAuthorizationService.CanModifyAsync(team, customerId, cancellationToken),
                CanDelete = await teamAuthorizationService.CanDeleteAsync(team, customerId, cancellationToken),
                CanInvitePeople = await teamAuthorizationService.CanInvitePeopleAsync(team, customerId, cancellationToken),
                CanCancelPeopleExistingInvitations =
                    await teamAuthorizationService.CanCancelPeopleExistingInvitationsAsync(team, customerId, cancellationToken)
            };
        }

        return mappedTeam;
    }
}
