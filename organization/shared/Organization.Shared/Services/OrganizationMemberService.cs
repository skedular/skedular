using Api.Shared.Services;
using Organization.Shared.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Shared.Services;

/// <summary>
///     Service for managing organization members.
/// </summary>
public interface IOrganizationMemberService
{
    /// <summary>
    ///     Adds new members to the specified organization.
    ///     Only adds members that are not already part of the organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="members">The collection of members to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="OrganizationNotFound">Thrown when the organization is not found.</exception>
    Task AddMembersAsync(string organizationId, IReadOnlyCollection<OrganizationMember> members, CancellationToken cancellationToken);
}

/// <summary>
///     Implementation of the organization member service.
/// </summary>
public class OrganizationMemberService(IRepositoryFactory repositoryFactory, IEntityMapper entityMapper, IOrganizationPublisher organizationPublisher)
    : IOrganizationMemberService
{
    /// <summary>
    ///     Adds new members to the specified organization.
    ///     Only adds members that are not already part of the organization.
    /// </summary>
    /// <param name="organizationId">The ID of the organization.</param>
    /// <param name="members">The collection of members to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="OrganizationNotFound">Thrown when the organization is not found.</exception>
    public async Task AddMembersAsync(string organizationId, IReadOnlyCollection<OrganizationMember> members, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        // TODO: 20240823 - Morteza: Need to ensure de-activated tenant members won't be added back to the organization  
        var newMembers = members.Where(member => organization.OrganizationMembers.All(organizationMember => member.Id != organizationMember.Id));
        foreach (var member in newMembers)
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(member.Customer.Id, cancellationToken);
            _ = repositoryFactory.OrganizationMemberRepository.Add(entityMapper.MapToEntity(member, organization, customer));
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await organizationPublisher.PublishOrganizationsAsync([entityMapper.MapTo(organization)], cancellationToken);
    }
}
