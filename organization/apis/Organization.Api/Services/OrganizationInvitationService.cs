using Api.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Customer = Organization.Shared.Models.Customer;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;

namespace Organization.Api.Services;

public interface IOrganizationInvitationService
{
    Task InviteMembersByEmailsAsync(
        string organizationId,
        ICollection<string> emails,
        CancellationToken cancellationToken);

    Task AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
}

public class OrganizationInvitationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IOrganizationInvitationService
{
    public async Task InviteMembersByEmailsAsync(
        string organizationId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanInvitePeople(organization, customer))
        {
            throw new Unauthorized();
        }

        var existingMemberEmails = organization.OrganizationMembers
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
                    query.Organization.Id == organizationId &&
                    query.Status == InvitationStatus.Pending
            }).ToListAsync(cancellationToken);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.JoinInvitationRepository.UnitOfWork,
                cancellationToken);

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
                    Organization = organization,
                    Email = email,
                    Status = InvitationStatus.Pending,
                    MembershipType = OrganizationMembershipType.Member,
                    CreatedBy = customerEntity,
                    Invitee = matchingCustomerByEmail
                })
                : repositoryFactory.JoinInvitationRepository.Update(existingJoinInvitation);

            if (matchingCustomerByEmail is null)
            {
                await notificationOutboxPublisher.PublishInviteToJoinOrganizationNewCustomerAsync(
                    mapper.MapTo(organization),
                    customer,
                    email,
                    repositoryFactory.JoinInvitationRepository.UnitOfWork,
                    cancellationToken);
            }
            else
            {
                await organizationOutboxPublisher.PublishInvitesToJoinOrganizationNotificationAsync(
                    [mapper.MapTo(existingJoinInvitation)],
                    repositoryFactory.JoinInvitationRepository.UnitOfWork,
                    cancellationToken);

                await notificationOutboxPublisher.PublishInviteToJoinOrganizationExistingCustomerAsync(
                    mapper.MapTo(organization),
                    customer,
                    mapper.MapTo(matchingCustomerByEmail)!,
                    repositoryFactory.JoinInvitationRepository.UnitOfWork,
                    cancellationToken);
            }
        }

        await repositoryFactory.JoinInvitationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new OrganizationJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(joinInvitation.Organization.Id,
                cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.JoinInvitationRepository.UnitOfWork,
                cancellationToken);

        if (organization.OrganizationMembers.All(item => item.Customer.Id != customer.Id))
        {
            repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
            {
                Id = randomHelper.Generate(),
                MembershipType = joinInvitation.MembershipType,
                Organization = organization,
                Customer = customerEntity
            });

            await organizationOutboxPublisher.PublishOrganizationAsync(
                [mapper.MapTo(organization)],
                repositoryFactory.OrganizationRepository.UnitOfWork,
                cancellationToken);
        }

        joinInvitation.Status = InvitationStatus.Accepted;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await organizationOutboxPublisher.PublishInvitesToJoinOrganizationNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.JoinInvitationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new OrganizationJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.JoinInvitationRepository.UnitOfWork,
                cancellationToken);

        joinInvitation.Status = InvitationStatus.Rejected;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await organizationOutboxPublisher.PublishInvitesToJoinOrganizationNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.JoinInvitationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new OrganizationJoinInvitationNotFound();
        }

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(joinInvitation.Organization.Id,
                cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanCancelPeopleExistingInvitations(organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.JoinInvitationRepository.UnitOfWork,
                cancellationToken);

        joinInvitation.Status = InvitationStatus.Cancelled;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await organizationOutboxPublisher.PublishInvitesToJoinOrganizationNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.JoinInvitationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
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
                .Select(item => item.Email).Any(item =>
                    string.Equals(item, joinInvitation.Email, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new Unauthorized();
        }
    }
}
