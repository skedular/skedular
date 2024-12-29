using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Organization = Team.Shared.Database.Entities.Organization;

namespace Team.Api.Services;

public interface ITeamMemberService
{
    Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)> GetPaginatedMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        ICollection<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<TeamMember> ChangeMembershipTypeAsync(
        string id,
        TeamMembershipType membershipType,
        CancellationToken cancellationToken);

    Task<ICollection<TeamMember>> ChangeStatusAsync(
        ICollection<string> ids,
        TeamMemberStatus status,
        CancellationToken cancellationToken);

    Task<Shared.Models.Team> UpdateAsync(
        string teamId,
        ICollection<TeamMember> members,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    public Task<List<Shared.Database.Entities.TeamMember>> BuildMembersAsync(
        ICollection<TeamMember> members,
        Shared.Database.Entities.Team existingTeam,
        Customer? customer,
        Organization? organization,
        CancellationToken cancellationToken);

    Task<TeamMember> RemoveAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<TeamMember>> RemoveAsync(ICollection<string> ids, CancellationToken cancellationToken);
}

public class TeamMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ITeamAuthorizationService teamAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IMapper mapper,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : ITeamMemberService
{
    public async Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)>
        GetPaginatedMembersAsync(
            PaginationInputParam paginationInputParam,
            TeamMemberSearchCriteria searchCriteria,
            ICollection<TeamMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(searchCriteria.TeamId, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (!teamAuthorizationService.CanView(team, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.TeamMemberRepository.GetPaginatedTeamMembersAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(team)).ToList(), totalCount);
    }

    public async Task<TeamMember> ChangeMembershipTypeAsync(
        string id,
        TeamMembershipType membershipType,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var teamMember = await repositoryFactory.TeamMemberRepository.GetByIdAsync(id, cancellationToken);
        if (teamMember is null)
        {
            throw new TeamMemberNotFound();
        }

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(
            teamMember.Team.Id,
            cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (!teamAuthorizationService.CanModify(team, customer))
        {
            throw new Unauthorized();
        }

        var myMembershipDetails =
            team.TeamMembers.Single(item => item.Customer.Id == customer.Id);

        if (myMembershipDetails.MembershipType == TeamMembershipTypeConstants.Administrator &&
            membershipType == TeamMembershipType.Owner)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.MembershipType == TeamMembershipTypeConstants.Member &&
            membershipType == TeamMembershipType.Administrator)
        {
            throw new Unauthorized();
        }

        var mappedMembershipType = membershipType switch
        {
            TeamMembershipType.Owner => TeamMembershipTypeConstants.Owner,
            TeamMembershipType.Administrator => TeamMembershipTypeConstants.Administrator,
            TeamMembershipType.Member => TeamMembershipTypeConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (teamMember.MembershipType == mappedMembershipType)
        {
            return mapper.MapTo(teamMember, mapper.MapTo(team));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TeamMemberRepository.UnitOfWork,
            cancellationToken);

        teamMember.MembershipType = mappedMembershipType;
        repositoryFactory.TeamMemberRepository.Update(teamMember);

        await teamOutboxPublisher.PublishTeamAsync(
            [mapper.MapTo(team)],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(teamMember, mapper.MapTo(team));
    }

    public async Task<ICollection<TeamMember>> ChangeStatusAsync(
        ICollection<string> ids,
        TeamMemberStatus status,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctTeamMemberIds = ids.Distinct().ToList();
        var teamMembers =
            await repositoryFactory.TeamMemberRepository.GetByIdsAsync(
                distinctTeamMemberIds,
                cancellationToken);
        if (teamMembers.Count != distinctTeamMemberIds.Count)
        {
            throw new TeamMemberNotFound();
        }

        // Exclude calling customer from the list
        teamMembers = teamMembers.Where(item => item.Customer.Id != customer.Id).ToList();

        if (teamMembers.Count == 0)
        {
            return [];
        }

        var teamIds = teamMembers.Select(item => item.Team.Id).Distinct().ToList();
        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(
            teamIds,
            cancellationToken);

        if (!teamMembers.All(
                item => teamAuthorizationService.CanModify(
                    teams.Single(organization => organization.Id == item.Team.Id),
                    customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TeamMemberRepository.UnitOfWork,
            cancellationToken);

        var mappedStatus = status switch
        {
            TeamMemberStatus.Active => TeamMemberStatusConstants.Active,
            TeamMemberStatus.Inactive => TeamMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var organizationMember in teamMembers)
        {
            organizationMember.Status = mappedStatus;
            repositoryFactory.TeamMemberRepository.Update(organizationMember);
        }

        await teamOutboxPublisher.PublishTeamAsync(
            teams.Select(mapper.MapTo),
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return teamMembers.Select(item => mapper.MapTo(item,
            mapper.MapTo(teams.Single(organization => organization.Id == item.Team.Id)))).ToList();
    }

    public async Task<Shared.Models.Team> UpdateAsync(
        string teamId,
        ICollection<TeamMember> members,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        var existingTeam = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        if (customer is not null && !teamAuthorizationService.CanModify(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        Organization? organization = null;
        if (existingTeam.Organization is not null)
        {
            organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                existingTeam.Organization.Id,
                cancellationToken);
            ArgumentNullException.ThrowIfNull(organization);
            if (!ignoreAuthorizationCheck &&
                !organizationOfferingService.IsMoreInteractionAllowed(organization, customer!))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        var rebuiltTeamMembers = await BuildMembersAsync(
            members,
            existingTeam,
            customer,
            organization,
            cancellationToken);
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByTeamIdAsync(
            existingTeam.Id,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TeamMemberRepository.UnitOfWork,
            cancellationToken);

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
        existingTeam.TeamMembers = addedItems.Concat(updatedItems).Concat(itemsToRemove).ToList();

        await teamOutboxPublisher.PublishTeamAsync(
            [mapper.MapTo(existingTeam)],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(existingTeam);
    }

    public async Task<List<Shared.Database.Entities.TeamMember>> BuildMembersAsync(
        ICollection<TeamMember> members,
        Shared.Database.Entities.Team existingTeam,
        Customer? customer,
        Organization? organization,
        CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var rebuiltTeamMembers = new List<Shared.Database.Entities.TeamMember>();
        if (organization is null)
        {
            var customersToAdd = await repositoryFactory.CustomerRepository.GetByIdsAsync(
                members
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                    .Where(item => item.Customer is not null)
                    .Select(item => item.Customer.Id)
                    .ToList(),
                cancellationToken);

            rebuiltTeamMembers.AddRange(customersToAdd.Select(item => new Shared.Database.Entities.TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                MembershipType =
                    customer is not null && item.Id == customer.Id
                        ? TeamMembershipTypeConstants.Owner
                        : TeamMembershipTypeConstants.Member,
                Customer = item,
                Team = existingTeam
            }));
        }
        else
        {
            var organizationMemberIds = members
                .Where(item => item.OrganizationMember is not null)
                .Select(item => item.OrganizationMember!.Id)
                .ToList();
            var organizationMembersToAdd =
                organization.OrganizationMembers
                    .Where(item => organizationMemberIds.Contains(item.Id)).ToList();

            rebuiltTeamMembers.AddRange(organizationMembersToAdd.Select(item => new Shared.Database.Entities.TeamMember
            {
                Id = randomHelper.Generate(),
                CreatedAt = now,
                MembershipType = customer is not null && item.Customer.Id == customer.Id
                    ? TeamMembershipTypeConstants.Owner
                    : TeamMembershipTypeConstants.Member,
                Customer = item.Customer,
                Team = existingTeam,
                OrganizationMember = item
            }));
        }

        return rebuiltTeamMembers;
    }

    public async Task<TeamMember> RemoveAsync(
        string id,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTeamMember = await repositoryFactory.TeamMemberRepository.GetByIdAsync(id, cancellationToken);
        if (existingTeamMember is null)
        {
            throw new TeamMemberNotFound();
        }

        var existingTeam =
            await repositoryFactory.TeamRepository.GetByIdAsync(existingTeamMember.Team.Id, cancellationToken);
        if (existingTeam is null)
        {
            throw new TeamNotFound();
        }

        if (!teamAuthorizationService.CanModify(existingTeam, customer))
        {
            throw new Unauthorized();
        }

        if (existingTeam.Organization is not null)
        {
            var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
                existingTeam.Organization.Id,
                cancellationToken);
            ArgumentNullException.ThrowIfNull(organization);

            if (!organizationOfferingService.IsMoreInteractionAllowed(organization, customer))
            {
                throw new NoMoreInteractionAllowed();
            }
        }

        var teamMemberToRemove = existingTeam.TeamMembers.FirstOrDefault(item => item.Id == id);
        if (teamMemberToRemove is null)
        {
            return mapper.MapTo(existingTeamMember);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TeamMemberRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.TeamMemberRepository.Remove(teamMemberToRemove);

        await teamOutboxPublisher.PublishTeamAsync(
            [mapper.MapTo(existingTeam)],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(existingTeamMember);
    }

    public async Task<ICollection<TeamMember>> RemoveAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctTeamMemberIds = ids.Distinct().ToList();
        var teamMembers = await repositoryFactory.TeamMemberRepository.GetByIdsAsync(
            distinctTeamMemberIds,
            cancellationToken);
        if (teamMembers.Count != distinctTeamMemberIds.Count)
        {
            throw new TeamMemberNotFound();
        }

        if (teamMembers.Count == 0)
        {
            return [];
        }

        var teamIds = teamMembers.Select(item => item.Team.Id).Distinct().ToList();
        var teams = await repositoryFactory.TeamRepository.GetByIdsAsync(
            teamIds,
            cancellationToken);

        if (!teamMembers.All(
                item => teamAuthorizationService.CanModify(
                    teams.Single(team => team.Id == item.Team.Id),
                    customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.TeamMemberRepository.RemoveRange(teamMembers);

        await teamOutboxPublisher.PublishTeamAsync(
            teams.Select(item =>
            {
                var mapped = mapper.MapTo(item);
                mapped.TeamMembers = mapped.TeamMembers
                    .Where(organizationMember => organizationMember.DeletedAt is null).ToList();

                return mapped;
            }),
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return teamMembers.Select(item => mapper.MapTo(item,
            mapper.MapTo(teams.Single(organization => organization.Id == item.Team.Id)))).ToList();
    }
}
