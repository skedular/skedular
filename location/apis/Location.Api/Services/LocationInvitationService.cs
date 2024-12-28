using Api.Shared.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Database.Entities;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Customer = Location.Shared.Models.Customer;
using LocationMember = Location.Shared.Database.Entities.LocationMember;

namespace Location.Api.Services;

public interface ILocationInvitationService
{
    Task InviteMembersByEmailsAsync(
        string locationId,
        ICollection<string> emails,
        CancellationToken cancellationToken);

    Task AcceptInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken);
    Task CancelInvitationToJoinAsync(string id, CancellationToken cancellationToken);
}

public class LocationInvitationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    ILocationAuthorizationService locationAuthorizationService,
    IMapper mapper,
    IRandomHelper randomHelper,
    INotificationOutboxPublisher notificationOutboxPublisher,
    ILocationOutboxPublisher locationOutboxPublisher) : ILocationInvitationService
{
    public async Task InviteMembersByEmailsAsync(
        string locationId,
        ICollection<string> emails,
        CancellationToken cancellationToken)
    {
        if (emails.Count == 0)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, customerEntity) = await customerService.GetCustomerAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanInvitePeople(location, customer))
        {
            throw new Unauthorized();
        }

        var existingMemberEmails = location.LocationMembers
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
                    query.Location.Id == locationId &&
                    query.Status == InvitationStatus.Pending
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
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
                    Location = location,
                    Email = email,
                    Status = InvitationStatus.Pending,
                    MembershipType = LocationMembershipType.Member,
                    CreatedBy = customerEntity,
                    Invitee = matchingCustomerByEmail
                })
                : repositoryFactory.JoinInvitationRepository.Update(existingJoinInvitation);

            if (matchingCustomerByEmail is null)
            {
                await notificationOutboxPublisher.PublishInviteToJoinLocationNewCustomerAsync(
                    mapper.MapTo(location),
                    customer,
                    email,
                    repositoryFactory.JoinInvitationRepository.UnitOfWork,
                    cancellationToken);
            }
            else
            {
                await locationOutboxPublisher.PublishInvitesToJoinLocationNotificationAsync(
                    [mapper.MapTo(existingJoinInvitation)],
                    repositoryFactory.JoinInvitationRepository.UnitOfWork,
                    cancellationToken);

                await notificationOutboxPublisher.PublishInviteToJoinLocationExistingCustomerAsync(
                    mapper.MapTo(location),
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
            throw new LocationJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(joinInvitation.Location.Id,
                cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        if (location.LocationMembers.All(item => item.Customer.Id != customer.Id))
        {
            repositoryFactory.LocationMemberRepository.Add(new LocationMember
            {
                Id = randomHelper.Generate(),
                MembershipType = joinInvitation.MembershipType,
                Location = location,
                Customer = customerEntity
            });

            await locationOutboxPublisher.PublishLocationAsync(
                [mapper.MapTo(location)],
                repositoryFactory.LocationRepository.UnitOfWork,
                cancellationToken);
        }

        joinInvitation.Status = InvitationStatus.Accepted;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await locationOutboxPublisher.PublishInvitesToJoinLocationNotificationAsync(
            [mapper.MapTo(joinInvitation)],
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.JoinInvitationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.LocationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RejectInvitationToJoinAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var joinInvitation = await repositoryFactory.JoinInvitationRepository.GetByIdAsync(id, cancellationToken);
        if (joinInvitation is null)
        {
            throw new LocationJoinInvitationNotFound();
        }

        EnsureCustomerAuthorizedToChangeJoinInvitationStatus(joinInvitation, customer);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        joinInvitation.Status = InvitationStatus.Rejected;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await locationOutboxPublisher.PublishInvitesToJoinLocationNotificationAsync(
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
            throw new LocationJoinInvitationNotFound();
        }

        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(joinInvitation.Location.Id,
                cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanCancelPeopleExistingInvitations(location, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.JoinInvitationRepository.UnitOfWork,
            cancellationToken);

        joinInvitation.Status = InvitationStatus.Cancelled;
        joinInvitation = repositoryFactory.JoinInvitationRepository.Remove(joinInvitation);

        await locationOutboxPublisher.PublishInvitesToJoinLocationNotificationAsync(
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
