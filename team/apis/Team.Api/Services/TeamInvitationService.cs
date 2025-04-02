using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Team.Api.Mappers;
using Team.Api.Services.Authorization;
using Team.Shared.Database.Entities;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Customer = Team.Shared.Models.Customer;
using TeamMember = Team.Shared.Database.Entities.TeamMember;

namespace Team.Api.Services;

public interface ITeamInvitationService
{
    Task InviteMembersByEmailsAsync(
        string teamId,
        ICollection<string> emails,
        CancellationToken cancellationToken);

    Task AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
}

public class TeamInvitationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ITeamAuthorizationService teamAuthorizationService,
    IMapper mapper,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    ITeamOutboxPublisher teamOutboxPublisher) : ITeamInvitationService
{
    public async Task InviteMembersByEmailsAsync(
        string teamId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(teamId);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var team =
            await repositoryFactory.TeamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (!teamAuthorizationService.CanInvitePeople(team, customer))
        {
            throw new Unauthorized();
        }

        var existingMemberEmails = team.TeamMembers
            .SelectMany(item =>
                item.Customer.Identities
                    .Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
                    .Select(identity => identity.Email))
            .ToList();

        emails = emails.Where(item =>
                !existingMemberEmails.Any(existingMemberEmail =>
                    string.Equals(item, existingMemberEmail, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
        if (emails.Count == 0)
        {
            return;
        }

        var pendingInvitations = await repositoryFactory.JoinInvitationRepository
            .Query(new Specification<JoinInvitation>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue &&
                    query.Team.Id == teamId &&
                    query.Status == InvitationStatusConstants.Pending
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var email in emails)
        {
            var matchingCustomerByEmail =
                await repositoryFactory.CustomerRepository.GetByEmailAsync(email, cancellationToken);
            var existingJoinInvitation = pendingInvitations.FirstOrDefault(item =>
                (item.Email is not null &&
                 string.Equals(item.Email, email, StringComparison.InvariantCultureIgnoreCase)) || (
                    matchingCustomerByEmail is not null && item.Invitee is not null &&
                    item.Invitee.Id == matchingCustomerByEmail.Id));

            existingJoinInvitation = existingJoinInvitation is null
                ? repositoryFactory.JoinInvitationRepository.Add(new JoinInvitation
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

            if (matchingCustomerByEmail is null)
            {
                await notificationOutboxPublisher.PublishInviteToJoinTeamNewCustomerAsync(
                    mapper.MapTo(team),
                    customer,
                    email,
                    repositoryFactory.UnitOfWork,
                    cancellationToken);
            }
            else
            {
                await teamOutboxPublisher.PublishInvitesToJoinTeamNotificationAsync(
                    [mapper.MapTo(existingJoinInvitation)],
                    repositoryFactory.UnitOfWork,
                    cancellationToken);

                await notificationOutboxPublisher.PublishInviteToJoinTeamExistingCustomerAsync(
                    mapper.MapTo(team),
                    customer,
                    mapper.MapTo(matchingCustomerByEmail)!,
                    repositoryFactory.UnitOfWork,
                    cancellationToken);
            }
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new TeamJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        var team =
            await repositoryFactory.TeamRepository.GetByIdAsync(joinInvitation.Team.Id,
                cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

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

            await teamOutboxPublisher.PublishTeamsAsync([mapper.MapTo(team)], repositoryFactory.UnitOfWork, cancellationToken);
        }

        joinInvitation.Status = InvitationStatusConstants.Accepted;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await teamOutboxPublisher.PublishInvitesToJoinTeamNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new TeamJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Rejected;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await teamOutboxPublisher.PublishInvitesToJoinTeamNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new TeamJoinInvitationNotFound();
        }

        var team = await repositoryFactory.TeamRepository.GetByIdAsync(joinInvitation.Team.Id, cancellationToken);
        if (team is null)
        {
            throw new TeamNotFound();
        }

        if (!teamAuthorizationService.CanCancelPeopleExistingInvitations(team, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        joinInvitation.Status = InvitationStatusConstants.Cancelled;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await teamOutboxPublisher.PublishInvitesToJoinTeamNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static void EnsureCustomerAuthorizedToChangeJoinInvitationStatus(
        JoinInvitation joinInvitation,
        Customer customer)
    {
        if (joinInvitation.Invitee is null && joinInvitation.Email is null)
        {
            throw new Unauthorized();
        }

        if (joinInvitation.Invitee is not null && joinInvitation.Invitee.Id != customer.Id)
        {
            throw new Unauthorized();
        }

        if (joinInvitation.Email is not null && !customer.Identities
                .Where(item => !string.IsNullOrWhiteSpace(item.Email))
                .Select(item => item.Email).Any(item => string.Equals(item, joinInvitation.Email, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Unauthorized();
        }
    }
}
