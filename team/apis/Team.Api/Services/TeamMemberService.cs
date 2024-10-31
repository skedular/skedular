using Api.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using OrganizationMember = Team.Shared.Database.Entities.OrganizationMember;

namespace Team.Api.Services;

public interface ITeamMemberService
{
    Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)> GetPaginatedTeamMembersAsync(
        PaginationInputParam paginationInputParam,
        TeamMemberSearchCriteria searchCriteria,
        ICollection<TeamMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<TeamMember> ChangeMembershipTypeAsync(
        string teamMemberId,
        TeamMembershipType membershipType,
        CancellationToken cancellationToken);

    Task<Shared.Models.Team> UpdateMembersAsync(
        string teamId,
        ICollection<TeamMember> members,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class TeamMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ITeamAuthorizationService teamAuthorizationService,
    ITeamOutboxPublisher teamOutboxPublisher,
    IMapper mapper) : ITeamMemberService
{
    public async Task<(PaginatedInfo, ICollection<Edge<TeamMember>>, int)>
        GetPaginatedTeamMembersAsync(
            PaginationInputParam paginationInputParam,
            TeamMemberSearchCriteria searchCriteria,
            ICollection<TeamMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
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
        string teamMemberId,
        TeamMembershipType membershipType,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var teamMember = await repositoryFactory.TeamMemberRepository.GetByIdAsync(teamMemberId, cancellationToken);
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

        if (myMembershipDetails.MembershipType == TeamMembershipType.Administrator &&
            membershipType == TeamMembershipType.Owner)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.MembershipType == TeamMembershipType.Member &&
            membershipType == TeamMembershipType.Administrator)
        {
            throw new Unauthorized();
        }

        if (teamMember.MembershipType == membershipType)
        {
            return mapper.MapTo(teamMember, mapper.MapTo(team));
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TeamMemberRepository.UnitOfWork,
                cancellationToken);

        teamMember.MembershipType = membershipType;
        repositoryFactory.TeamMemberRepository.Update(teamMember);

        await teamOutboxPublisher.PublishTeamAsync(
            [mapper.MapTo(team)],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(teamMember, mapper.MapTo(team));
    }

    public async Task<Shared.Models.Team> UpdateMembersAsync(
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

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (customer is not null && !teamAuthorizationService.CanModify(team, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TeamMemberRepository.UnitOfWork,
                cancellationToken);

        var itemsToRemove = team.TeamMembers
            .Where(teamMember => members.All(item => item.Id != teamMember.Id))
            .ToList();

        var updatedItems = new List<Shared.Database.Entities.TeamMember>();
        foreach (var teamMember in team.TeamMembers
                     .Where(teamMember =>
                         members.Any(item => item.Id == teamMember.Id)))
        {
            var customerToAdd =
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
                    members.Single(item => item.Id == teamMember.Id),
                    teamMember,
                    team,
                    customerToAdd,
                    organizationMember)));
        }

        var addedItems = new List<Shared.Database.Entities.TeamMember>();
        foreach (var teamMember in members.Where(teamMember =>
                     team.TeamMembers.All(item => item.Id != teamMember.Id)))
        {
            var customerToAdd =
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
                mapper.MapToEntity(teamMember, team, customerToAdd, organizationMember)));
        }

        repositoryFactory.TeamMemberRepository.RemoveRange(itemsToRemove);
        team.TeamMembers = addedItems.Concat(updatedItems).ToList();

        await teamOutboxPublisher.PublishTeamAsync(
            [mapper.MapTo(team)],
            repositoryFactory.TeamRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.TeamMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.TeamRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(team);
    }
}
