using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Team.Api.Models;
using Team.Api.Services.Authorization;
using Team.Shared.Mappers;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services.Cache;
using Organization = Team.Shared.Database.Entities.Organization;

namespace Team.Api.Services;

public interface ITeamMemberService
{
    Task<(PaginatedInfo, IReadOnlyList<Edge<TeamMember>>, int)> GetPaginatedMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        IReadOnlyList<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<TeamMember> ChangeRoleAsync(string id, TeamMemberRole memberRole, CancellationToken cancellationToken);
    Task<IReadOnlyList<TeamMember>> ChangeStatusAsync(IReadOnlyList<string> ids, TeamMemberStatus status, CancellationToken cancellationToken);
    Task<Shared.Models.Team> UpdateMembersAsync(string teamId, IReadOnlyList<TeamMember> members, CancellationToken cancellationToken);

    Task<Shared.Models.Team> UpdateMembersAsync(string teamId, IReadOnlyList<TeamMember> members, IReadOnlySet<TeamMembersPatchField> fieldsToUpdate,
        CancellationToken cancellationToken);

    public Task<List<Shared.Database.Entities.TeamMember>> BuildMembersAsync(
        IReadOnlyList<TeamMember> members,
        Shared.Database.Entities.Team existingTeam,
        string customerId,
        Organization? organization,
        CancellationToken cancellationToken);

    Task<TeamMember> RemoveAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<TeamMember>> RemoveAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<TeamMember> AddAsync(string teamId, TeamMember member, CancellationToken cancellationToken);
}

public class TeamMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICachedTeamService cachedTeamService,
    ICachedOrganizationService cachedOrganizationService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IEntityMapper entityMapper,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    ILogger<TeamMemberService> logger) : ITeamMemberService
{
    public async Task<(PaginatedInfo, IReadOnlyList<Edge<TeamMember>>, int)> GetPaginatedMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        IReadOnlyList<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var team = await cachedTeamService.GetByIdAsync(searchCriteria.TeamId, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanViewAsync(team, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.TeamMemberRepository.GetPaginatedTeamMembersUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        if (totalCount == 0)
        {
            logger.LogInformation("Paginated team members query returned no results for team {TeamId}", team.Id);
        }

        return (paginatedInfo, [.. entityMapper.MapTo(edges, entityMapper.MapTo(team))], totalCount);
    }

    public async Task<TeamMember> ChangeRoleAsync(string id, TeamMemberRole memberRole, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var teamMember = await repositoryFactory.TeamMemberRepository.GetByIdAsync(id, cancellationToken) ?? throw new TeamMemberNotFound();
        var team = await cachedTeamService.GetByIdAsync(teamMember.Team.Id, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanModifyAsync(team, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var myMemberDetails = team.TeamMembers.Single(item => item.Customer.Id == customerId);
        if (myMemberDetails.Role == TeamMemberRoleConstants.Administrator && memberRole == TeamMemberRole.Owner)
        {
            throw new UnauthorizedAccessException();
        }

        if (myMemberDetails.Role == TeamMemberRoleConstants.Member && memberRole == TeamMemberRole.Administrator)
        {
            throw new UnauthorizedAccessException();
        }

        var mappedRole = memberRole.ToTeamMemberRole();
        if (teamMember.Role == mappedRole)
        {
            return entityMapper.MapTo(teamMember, entityMapper.MapTo(team));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        teamMember.Role = mappedRole;
        repositoryFactory.TeamMemberRepository.Update(teamMember);

        teamOutboxPublisher.PublishTeams([entityMapper.MapTo(team)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Team member role updated for member {TeamMemberId} to role {Role}", id, memberRole);

        return entityMapper.MapTo(teamMember, entityMapper.MapTo(team));
    }

    public async Task<IReadOnlyList<TeamMember>> ChangeStatusAsync(
        IReadOnlyList<string> ids,
        TeamMemberStatus status,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var distinctTeamMemberIds = ids.Distinct().ToList();
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByIdsAsync(distinctTeamMemberIds, cancellationToken);
        if (teamMembers.Count != distinctTeamMemberIds.Count)
        {
            throw new TeamMemberNotFound();
        }

        // Exclude calling customer from the list
        teamMembers = [.. teamMembers.Where(item => item.Customer.Id != customerId)];
        if (teamMembers.Count == 0)
        {
            return [];
        }

        var teamIds = teamMembers.Select(item => item.Team.Id).Distinct().ToList();
        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, cancellationToken);
        foreach (var item in teamMembers)
        {
            if (!await teamAuthorizationService.CanModifyAsync(
                    teams.Single(organization => organization.Id == item.Team.Id), customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var mappedStatus = status.ToTeamMemberStatus();
        foreach (var organizationMember in teamMembers)
        {
            organizationMember.Status = mappedStatus;
            repositoryFactory.TeamMemberRepository.Update(organizationMember);
        }

        teamOutboxPublisher.PublishTeams([.. teams.Select(entityMapper.MapTo)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Team member statuses updated for {MemberCount} members to status {Status}", teamMembers.Count, status);

        return
        [
            .. teamMembers
                .Select(item => entityMapper.MapTo(item, entityMapper.MapTo(teams.Single(organization => organization.Id == item.Team.Id)))),
        ];
    }

    public async Task<Shared.Models.Team> UpdateMembersAsync(string teamId, IReadOnlyList<TeamMember> members, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
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

        var rebuiltTeamMembers = await BuildMembersAsync(members, existingTeam, customerId, organization, cancellationToken);
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByTeamIdAsync(existingTeam.Id, cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var itemsToRemove = teamMembers
            .Where(teamMember => rebuiltTeamMembers.All(item => item.Customer.Id != teamMember.Customer.Id))
            .ToList();
        var updatedItems = teamMembers
            .Where(teamMember => rebuiltTeamMembers.Any(item => item.Customer.Id == teamMember.Customer.Id))
            .Select(teamMember =>
            {
                teamMember.DeletedAt = null;
                return repositoryFactory.TeamMemberRepository.Update(teamMember);
            }).ToList();
        var addedItems = rebuiltTeamMembers
            .Where(teamMember => teamMembers.All(item => item.Customer.Id != teamMember.Customer.Id))
            .Select(teamMember => repositoryFactory.TeamMemberRepository.Add(teamMember)).ToList();

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        existingTeam.TeamMembers = [.. addedItems, .. updatedItems, .. itemsToRemove];

        var mappedTeam = entityMapper.MapTo(existingTeam);
        mappedTeam.TeamMembers = [.. mappedTeam.TeamMembers.Where(item => item.IsNotDeleted())];

        teamOutboxPublisher.PublishTeams([mappedTeam], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Team members update completed for team {TeamId}", existingTeam.Id);

        existingTeam.TeamMembers = [.. existingTeam.TeamMembers.Where(item => item.IsNotDeleted())];
        return entityMapper.MapTo(existingTeam);
    }

    public async Task<Shared.Models.Team> UpdateMembersAsync(
        string teamId,
        IReadOnlyList<TeamMember> members,
        IReadOnlySet<TeamMembersPatchField> fieldsToUpdate,
        CancellationToken cancellationToken)
    {
        var editUnits = string.Join(",", fieldsToUpdate);
        logger.LogInformation(
            "Team members patch autosave started. TeamId: {TeamId}, EditUnits: {EditUnits}",
            teamId,
            editUnits);

        try
        {
            if (fieldsToUpdate.Contains(TeamMembersPatchField.Members))
            {
                var updatedMembersTeam = await UpdateMembersAsync(teamId, members, cancellationToken);
                logger.LogInformation(
                    "Team members patch autosave completed. TeamId: {TeamId}, EditUnits: {EditUnits}",
                    updatedMembersTeam.Id,
                    editUnits);
                return updatedMembersTeam;
            }

            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();
            if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }

            var unchangedTeam = entityMapper.MapTo(existingTeam);
            logger.LogInformation(
                "Team members patch autosave completed with no changes. TeamId: {TeamId}, EditUnits: {EditUnits}",
                unchangedTeam.Id,
                editUnits);
            return unchangedTeam;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Team members patch autosave rejected by authorization. TeamId: {TeamId}, EditUnits: {EditUnits}",
                teamId,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Team members patch autosave failed. TeamId: {TeamId}, EditUnits: {EditUnits}",
                teamId,
                editUnits);
            throw;
        }
    }

    public async Task<List<Shared.Database.Entities.TeamMember>> BuildMembersAsync(
        IReadOnlyList<TeamMember> members,
        Shared.Database.Entities.Team existingTeam,
        string customerId,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var rebuiltTeamMembers = new List<Shared.Database.Entities.TeamMember>();
        if (organization is null)
        {
            var customersToAdd = await repositoryFactory.CustomerRepository.GetByIdsAsync(
                [
                    .. members
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                        .Where(item => item.Customer is not null)
                        .Select(item => item.Customer.Id),
                ],
                cancellationToken);

            rebuiltTeamMembers.AddRange(customersToAdd.Select(item => new Shared.Database.Entities.TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                Role = item.Id == customerId ? TeamMemberRoleConstants.Owner : TeamMemberRoleConstants.Member,
                Customer = item,
                Team = existingTeam,
                Status = TeamMemberStatusConstants.Active,
            }));
        }
        else
        {
            var organizationMemberIds = members
                .Where(item => item.OrganizationMember is not null)
                .Select(item => item.OrganizationMember!.Id)
                .ToList();
            var organizationMembersToAdd = organization.OrganizationMembers.Where(item => organizationMemberIds.Contains(item.Id)).ToList();

            rebuiltTeamMembers.AddRange(organizationMembersToAdd.Select(item => new Shared.Database.Entities.TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                Role = item.Customer.Id == customerId ? TeamMemberRoleConstants.Owner : TeamMemberRoleConstants.Member,
                Customer = item.Customer,
                Team = existingTeam,
                OrganizationMember = item,
                Status = TeamMemberStatusConstants.Active,
            }));
        }

        return rebuiltTeamMembers;
    }

    public async Task<TeamMember> RemoveAsync(string id, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingTeamMember = await repositoryFactory.TeamMemberRepository.GetByIdAsync(id, cancellationToken) ?? throw new TeamMemberNotFound();
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(existingTeamMember.Team.Id, cancellationToken) ??
                           throw new TeamNotFound();
        if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(
                               existingTeam.Organization.Id,
                               existingTeam.Organization.CustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        var teamMemberToRemove = existingTeam.TeamMembers.FirstOrDefault(item => item.Id == id);
        if (teamMemberToRemove is null)
        {
            return entityMapper.MapTo(existingTeamMember);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.TeamMemberRepository.Remove(teamMemberToRemove);

        var mappedTeam = entityMapper.MapTo(existingTeam);
        mappedTeam.TeamMembers = [.. mappedTeam.TeamMembers.Where(item => item.IsNotDeleted())];

        teamOutboxPublisher.PublishTeams([mappedTeam], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Single team member removal completed for member {TeamMemberId}", id);

        return entityMapper.MapTo(existingTeamMember);
    }

    public async Task<IReadOnlyList<TeamMember>> RemoveAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var distinctTeamMemberIds = ids.Distinct().ToList();
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByIdsAsync(distinctTeamMemberIds, cancellationToken);
        if (teamMembers.Count != distinctTeamMemberIds.Count)
        {
            throw new TeamMemberNotFound();
        }

        if (teamMembers.Count == 0)
        {
            return [];
        }

        var teamIds = teamMembers.Select(item => item.Team.Id).Distinct().ToList();
        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(teamIds, cancellationToken);
        foreach (var item in teamMembers)
        {
            if (!await teamAuthorizationService.CanModifyAsync(teams.Single(team => team.Id == item.Team.Id), customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.TeamMemberRepository.RemoveRange(teamMembers);

        teamOutboxPublisher.PublishTeams(
            [
                .. teams.Select(item =>
                {
                    var mapped = entityMapper.MapTo(item);
                    mapped.TeamMembers = [.. mapped.TeamMembers.Where(organizationMember => organizationMember.IsNotDeleted())];

                    return mapped;
                }),
            ],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        logger.LogInformation("Batch team member removal completed for {MemberCount} members", teamMembers.Count);

        return
        [
            .. teamMembers
                .Select(item => entityMapper.MapTo(item, entityMapper.MapTo(teams.Single(organization => organization.Id == item.Team.Id)))),
        ];
    }

    public async Task<TeamMember> AddAsync(string teamId, TeamMember member, CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanModifyAsync(existingTeam, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               existingTeam.Organization.Id,
                               null,
                               false,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationOfferingService.IsMoreInteractionAllowedAsync(organization.Id, customerId, cancellationToken))
        {
            throw new NoMoreInteractionAllowed();
        }

        ArgumentNullException.ThrowIfNull(member.OrganizationMember);

        var existingTeamMember = await repositoryFactory.TeamMemberRepository.GetByTeamIdAndOrganizationMemberIdAsync(
            existingTeam.Id,
            member.OrganizationMember.Id,
            cancellationToken);

        if (existingTeamMember is not null)
        {
            if (existingTeamMember.IsDeleted())
            {
                existingTeamMember.DeletedAt = null;
                existingTeamMember.Role = TeamMemberRoleConstants.Member;
                existingTeamMember.Status = TeamMemberStatusConstants.Active;

                await using var updateTransaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

                _ = repositoryFactory.TeamMemberRepository.Update(existingTeamMember);
                var mappedTeam = entityMapper.MapTo(existingTeam);
                mappedTeam.TeamMembers = [.. mappedTeam.TeamMembers.Where(item => item.IsNotDeleted())];

                teamOutboxPublisher.PublishTeams([mappedTeam], repositoryFactory.UnitOfWork);

                await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
                await updateTransaction.CommitAsync(cancellationToken);

                logger.LogInformation("Existing deleted team member reactivated for team {TeamId}", existingTeam.Id);
            }

            return entityMapper.MapTo(existingTeamMember);
        }

        var now = timeProvider.GetUtcNow();
        var organizationMemberToAdd = organization.OrganizationMembers.FirstOrDefault(item => item.Id == member.OrganizationMember!.Id) ??
                                      throw new OrganizationMemberNotFound();
        var teamMember = new Shared.Database.Entities.TeamMember
        {
            Id = randomHelper.Generate(),
            CreatedAt = now,
            Role = TeamMemberRoleConstants.Member,
            Customer = organizationMemberToAdd.Customer,
            Team = existingTeam,
            OrganizationMember = organizationMemberToAdd,
            Status = TeamMemberStatusConstants.Active,
        };

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var addedItem = repositoryFactory.TeamMemberRepository.Add(teamMember);

        teamOutboxPublisher.PublishTeams([entityMapper.MapTo(existingTeam)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Team member added to team {TeamId}", existingTeam.Id);

        return entityMapper.MapTo(addedItem);
    }
}
