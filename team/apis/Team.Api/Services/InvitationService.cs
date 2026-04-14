using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Models;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services;
using Team.Shared.Services.Cache;
using Team.Shared.Workflows;
using Customer = Team.Shared.Database.Entities.Customer;
using TeamMember = Team.Shared.Database.Entities.TeamMember;

namespace Team.Api.Services;

public interface IInvitationService
{
    Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(string teamId, ICollection<string> emails, CancellationToken cancellationToken);
    Task<JoinInvitation> AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<int> PendingInvitationsCountAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetMyPaginatedJoinInvitationsAsync(
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
    ITemporalOutboxService temporalOutboxService,
    ITeamOutboxPublisher teamOutboxPublisher,
    ICachedCustomerService cachedCustomerService,
    ICachedTeamService cachedTeamService,
    ILogger<InvitationService> logger) : IInvitationService
{
    public async Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(
        string teamId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            logger.LogInformation("Invite members request skipped because email list is empty for team {TeamId}", teamId);
            return [];
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var customer = await customerService.GetAsync(cancellationToken);
        var team = await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanInvitePeopleAsync(team, customer.Id, cancellationToken))
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
            logger.LogInformation("Invite members request produced no new invitations for team {TeamId}", teamId);
            return [];
        }

        var pendingInvitations =
            await repositoryFactory.JoinInvitationRepository.GetByTeamIdAsync(teamId, InvitationStatus.Pending, cancellationToken);

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
                    CreatedBy = customer,
                    Invitee = matchingCustomerByEmail
                })
                : repositoryFactory.JoinInvitationRepository.Update(existingJoinInvitation);

            joinInvitations.Add(mapper.MapTo(existingJoinInvitation));

            temporalOutboxService.StartWorkflowInviteToJoin(
                new InviteToJoinTeamInput(existingJoinInvitation.Id, matchingCustomerByEmail is null),
                repositoryFactory.UnitOfWork);
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Invitation create flow completed for team {TeamId} with {InvitationCount} invitations", teamId, joinInvitations.Count);

        return joinInvitations;
    }

    public async Task<JoinInvitation> AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await customerService.GetAsync(cancellationToken);
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
                Customer = customer
            });

            teamOutboxPublisher.PublishTeams([mapper.MapTo(team)], repositoryFactory.UnitOfWork);
        }

        joinInvitation.Status = InvitationStatusConstants.Accepted;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Update(joinInvitation);

        temporalOutboxService.SignalWorkflowInviteToJoinInvitationStatusChanged(joinInvitation.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Invitation {InvitationId} accepted for team {TeamId}", joinInvitation.Id, team.Id);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<JoinInvitation> RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken) ??
                             throw new TeamJoinInvitationNotFound();

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Rejected;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Update(joinInvitation);

        temporalOutboxService.SignalWorkflowInviteToJoinInvitationStatusChanged(joinInvitation.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Invitation {InvitationId} rejected", joinInvitation.Id);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<JoinInvitation> CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken) ??
                             throw new TeamJoinInvitationNotFound();
        var team = await cachedTeamService.GetByIdAsync(joinInvitation.Team.Id, cancellationToken) ?? throw new TeamNotFound();
        if (!await teamAuthorizationService.CanCancelPeopleExistingInvitationsAsync(team, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Cancelled;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Update(joinInvitation);

        temporalOutboxService.SignalWorkflowInviteToJoinInvitationStatusChanged(joinInvitation.Id, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        logger.LogInformation("Invitation {InvitationId} cancelled", joinInvitation.Id);

        return mapper.MapTo(joinInvitation);
    }

    public async Task<int> PendingInvitationsCountAsync(CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var count = await repositoryFactory.JoinInvitationRepository.PendingInvitationsCountAsync(
            customer.Id,
            customer.Identities.Where(item => !string.IsNullOrWhiteSpace(item.Email)).Select(item => item.Email!).ToList(),
            cancellationToken);

        if (count == 0)
        {
            logger.LogInformation("Pending invitations query returned zero results for customer {CustomerId}", customer.Id);
        }

        return count;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetMyPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinTeamInvitationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        // Ensure we do not return another customer join invitation by forcing CustomerId as search criteria
        searchCriteria = searchCriteria with { InviteeId = customerId };

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.JoinInvitationRepository.GetPaginatedJoinInvitationsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);
        if (totalCount == 0)
        {
            logger.LogInformation("Paginated invitations query returned zero results for customer {CustomerId}", customerId);
        }

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
