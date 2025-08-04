using Api.Shared.Services;
using Api.Shared.Services.Models;
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
using Customer = Team.Shared.Models.Customer;
using TeamMember = Team.Shared.Database.Entities.TeamMember;

namespace Team.Api.Services;

public interface IInvitationService
{
    Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(
        string teamId,
        ICollection<string> emails,
        CancellationToken cancellationToken);

    Task<JoinInvitation> AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<int> PendingInvitationsCountAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int )> GetMyPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class InvitationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ITeamAuthorizationService teamAuthorizationService,
    IMapper mapper,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    ITeamOutboxPublisher teamOutboxPublisher,
    ICachedCustomerService cachedCustomerService) : IInvitationService
{
    public async Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(
        string teamId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            return [];
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();
        if (!teamAuthorizationService.CanInvitePeople(team, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var existingMemberEmails = team.TeamMembers
            .SelectMany(item =>
                item.Customer.Identities.Where(identity => !string.IsNullOrWhiteSpace(identity.Email)).Select(identity => identity.Email))
            .ToList();

        emails = emails
            .Where(item => !existingMemberEmails.Any(existingMemberEmail =>
                string.Equals(item, existingMemberEmail, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
        if (emails.Count == 0)
        {
            return [];
        }

        var pendingInvitations = await repositoryFactory.JoinInvitationRepository
            .Query(new Specification<Shared.Database.Entities.JoinInvitation>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Team.Id == teamId && query.Status == InvitationStatusConstants.Pending
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var joinInvitations = new List<JoinInvitation>();

        foreach (var email in emails)
        {
            var matchingCustomerByEmail = await repositoryFactory.CustomerRepository.GetByEmailAsync(email, cancellationToken);
            var existingJoinInvitation = pendingInvitations.FirstOrDefault(item => (item.Email is not null &&
                                                                                    string.Equals(item.Email, email,
                                                                                        StringComparison.InvariantCultureIgnoreCase)) ||
                                                                                   (matchingCustomerByEmail is not null && item.Invitee is not null &&
                                                                                    item.Invitee.Id == matchingCustomerByEmail.Id));

            existingJoinInvitation = existingJoinInvitation is null
                ? repositoryFactory.JoinInvitationRepository.Add(new Shared.Database.Entities.JoinInvitation
                {
                    Id = randomHelper.Generate(),
                    Team = team,
                    Email = email,
                    Status = InvitationStatusConstants.Pending,
                    Role = TeamMemberRoleConstants.Member,
                    CreatedBy = customerEntity,
                    Invitee = matchingCustomerByEmail
                })
                : repositoryFactory.JoinInvitationRepository.Update(existingJoinInvitation);

            joinInvitations.Add(mapper.MapTo(existingJoinInvitation));

            if (matchingCustomerByEmail is null)
            {
                notificationOutboxPublisher.PublishInviteToJoinTeamNewCustomer(mapper.MapTo(team), customer, email, repositoryFactory.UnitOfWork);
            }
            else
            {
                notificationOutboxPublisher.PublishInviteToJoinTeamExistingCustomer(
                    mapper.MapTo(team),
                    customer,
                    mapper.MapTo(matchingCustomerByEmail)!,
                    repositoryFactory.UnitOfWork);
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return joinInvitations;
    }

    public async Task<JoinInvitation> AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken) ??
                             throw new TeamJoinInvitationNotFound();

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(joinInvitation.Team.Id, cancellationToken) ?? throw new TeamNotFound();
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (team.TeamMembers.All(item => item.Customer.Id != customer.Id))
        {
            repositoryFactory.TeamMemberRepository.Add(new TeamMember
            {
                Id = randomHelper.Generate(),
                Role = joinInvitation.Role,
                Status = TeamMemberStatusConstants.Active,
                Team = team,
                Customer = customerEntity
            });

            teamOutboxPublisher.PublishTeams([mapper.MapTo(team)], repositoryFactory.UnitOfWork);
        }

        joinInvitation.Status = InvitationStatusConstants.Accepted;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<JoinInvitation> RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken) ??
                             throw new TeamJoinInvitationNotFound();

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Rejected;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<JoinInvitation> CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken) ??
                             throw new TeamJoinInvitationNotFound();
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(joinInvitation.Team.Id, cancellationToken) ?? throw new TeamNotFound();
        if (!teamAuthorizationService.CanCancelPeopleExistingInvitations(team, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Cancelled;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<int> PendingInvitationsCountAsync(CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        return await repositoryFactory.JoinInvitationRepository.PendingInvitationsCountAsync(customer.Id, cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetMyPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        // Ensure we do not return another customer join invitation by forcing CustomerId as search criteria
        searchCriteria.InviteeId = customer.Id;

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.JoinInvitationRepository.GetPaginatedJoinInvitationsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(mapper.MapTo).ToList(), totalCount);
    }

    private static void EnsureCustomerAuthorizedToChangeJoinInvitationStatus(
        Shared.Database.Entities.JoinInvitation joinInvitation,
        Customer customer)
    {
        if (joinInvitation.Invitee is null && joinInvitation.Email is null)
        {
            throw new UnauthorizedAccessException();
        }

        if (joinInvitation.Invitee is not null && joinInvitation.Invitee.Id != customer.Id)
        {
            throw new UnauthorizedAccessException();
        }

        if (joinInvitation.Email is not null && !customer.Identities
                .Where(item => !string.IsNullOrWhiteSpace(item.Email))
                .Select(item => item.Email)
                .Any(item => string.Equals(item, joinInvitation.Email, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new UnauthorizedAccessException();
        }
    }
}
