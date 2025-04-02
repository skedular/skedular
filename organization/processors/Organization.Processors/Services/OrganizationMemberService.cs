using Enterprise.Shared.Exceptions;
using Organization.Processors.Mappers;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Processors.Services;

public interface IOrganizationMemberService
{
    Task AddMembersAsync(string organizationId, IReadOnlyCollection<OrganizationMember> members, CancellationToken cancellationToken);
}

public class OrganizationMemberService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper,
    IOrganizationPublisher organizationPublisher) : IOrganizationMemberService
{
    public async Task AddMembersAsync(string organizationId, IReadOnlyCollection<OrganizationMember> members, CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        // TODO: 20240823 - Morteza: Need to ensure de-activated tenant members won't be added back to the organization  
        var newMembers = members.Where(member => organization.OrganizationMembers.All(organizationMember => member.Id != organizationMember.Id));
        foreach (var member in newMembers)
        {
            var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(member.Customer.Id, cancellationToken);
            _ = repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(member, organization, customer));
        }

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await organizationPublisher.PublishOrganizationsAsync([mapper.MapTo(organization)], cancellationToken);
    }
}
