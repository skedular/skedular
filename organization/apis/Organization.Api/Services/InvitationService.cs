using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Customer = Organization.Shared.Models.Customer;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;

namespace Organization.Api.Services;

public interface IInvitationService
{
    Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(
        string organizationId,
        ICollection<string> emails,
        CancellationToken cancellationToken);

    Task<JoinInvitation> AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<JoinInvitation> CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task<int> PendingInvitationsCountAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int )> GetMyPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinOrganizationInvitationOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class InvitationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ICachedCustomerService cachedCustomerService) : IInvitationService
{
    public async Task<ICollection<JoinInvitation>> InviteMembersByEmailsAsync(
        string organizationId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            return [];
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanInvitePeople(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        var existingMemberEmails = organization.OrganizationMembers
            .SelectMany(item => item.Customer.Identities
                .Where(identity => !string.IsNullOrWhiteSpace(identity.Email))
                .Select(identity => identity.Email))
            .ToList();

        emails = emails
            .Where(item => !existingMemberEmails
                .Any(existingMemberEmail => string.Equals(item, existingMemberEmail, StringComparison.InvariantCultureIgnoreCase)))
            .ToList();
        if (emails.Count == 0)
        {
            return [];
        }

        var pendingInvitations = await repositoryFactory.JoinInvitationRepository
            .Query(new Specification<Shared.Database.Entities.JoinInvitation>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.Organization.Id == organizationId && query.Status == InvitationStatusConstants.Pending
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var joinInvitations = new List<JoinInvitation>();

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
                ? repositoryFactory.JoinInvitationRepository.Add(new Shared.Database.Entities.JoinInvitation
                {
                    Id = randomHelper.Generate(),
                    Organization = organization,
                    Email = email,
                    Status = InvitationStatusConstants.Pending,
                    Role = OrganizationMemberRoleConstants.Member,
                    CreatedBy = customerEntity,
                    Invitee = matchingCustomerByEmail
                })
                : repositoryFactory.JoinInvitationRepository.Update(existingJoinInvitation);

            joinInvitations.Add(mapper.MapTo(existingJoinInvitation));

            if (matchingCustomerByEmail is null)
            {
                notificationOutboxPublisher.PublishInviteToJoinOrganizationNewCustomer(
                    mapper.MapTo(organization),
                    customer,
                    email,
                    repositoryFactory.UnitOfWork);
            }
            else
            {
                notificationOutboxPublisher.PublishInviteToJoinOrganizationExistingCustomer(
                    mapper.MapTo(organization),
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
                             throw new OrganizationJoinInvitationNotFound();

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(joinInvitation.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationMembers.All(item => item.Customer.Id != customer.Id))
        {
            repositoryFactory.OrganizationMemberRepository.Add(new OrganizationMember
            {
                Id = randomHelper.Generate(),
                Role = joinInvitation.Role,
                Status = OrganizationMemberStatusConstants.Active,
                Organization = organization,
                Customer = customerEntity
            });

            organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);
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
                             throw new OrganizationJoinInvitationNotFound();

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
                             throw new OrganizationJoinInvitationNotFound();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(joinInvitation.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanCancelPeopleExistingInvitations(organization, customer))
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
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        return await repositoryFactory.JoinInvitationRepository.PendingInvitationsCountAsync(customer.Id, cancellationToken);
    }

    public async Task<(PaginatedInfo, ICollection<Edge<JoinInvitation>>, int)> GetMyPaginatedJoinInvitationsAsync(
        PaginationInputParam paginationInputParam,
        JoinInvitationSearchCriteria searchCriteria,
        ICollection<JoinOrganizationInvitationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
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
                .Select(item => item.Email).Any(item => string.Equals(item, joinInvitation.Email, StringComparison.InvariantCultureIgnoreCase)))
        {
            throw new UnauthorizedAccessException();
        }
    }
}
