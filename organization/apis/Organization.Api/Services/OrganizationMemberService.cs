using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationMemberService
{
    Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int )> GetPaginatedOrganizationMembersAsync(
        PaginationInputParam paginationInputParam,
        OrganizationMemberSearchCriteria searchCriteria,
        ICollection<OrganizationMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<OrganizationMember> ChangeRoleAsync(
        string organizationMemberId,
        OrganizationMemberRole memberRole,
        CancellationToken cancellationToken);

    Task<ICollection<OrganizationMember>> ChangeStatusAsync(
        ICollection<string> ids,
        OrganizationMemberStatus status,
        CancellationToken cancellationToken);

    Task<ICollection<OrganizationMember>> RemoveAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> AddMemberAsync(
        string organizationId,
        OrganizationMember member,
        CancellationToken cancellationToken);

    Task CompleteOrganizationMemberOnboardingAsync(
        string organizationId,
        CancellationToken cancellationToken);
}

public class OrganizationMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationService organizationService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IMapper mapper) : IOrganizationMemberService
{
    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)>
        GetPaginatedOrganizationMembersAsync(
            PaginationInputParam paginationInputParam,
            OrganizationMemberSearchCriteria searchCriteria,
            ICollection<OrganizationMemberOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId,
                cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanView(organization, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.OrganizationMemberRepository.GetPaginatedOrganizationMembersAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(organization)).ToList(), totalCount);
    }

    public async Task<OrganizationMember> ChangeRoleAsync(
        string organizationMemberId,
        OrganizationMemberRole memberRole,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organizationMember =
            await repositoryFactory.OrganizationMemberRepository.GetByIdAsync(organizationMemberId, cancellationToken);
        if (organizationMember is null)
        {
            throw new OrganizationMemberNotFound();
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
            organizationMember.Organization.Id,
            cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new Unauthorized();
        }

        var myMemberDetails = organization.OrganizationMembers.Single(item => item.Customer.Id == customer.Id);
        if (myMemberDetails.Status != OrganizationMemberStatusConstants.Active)
        {
            throw new Unauthorized();
        }

        if (myMemberDetails.Role == OrganizationMemberRoleConstants.Administrator &&
            memberRole == OrganizationMemberRole.Owner)
        {
            throw new Unauthorized();
        }

        if (myMemberDetails.Role == OrganizationMemberRoleConstants.Member &&
            memberRole == OrganizationMemberRole.Administrator)
        {
            throw new Unauthorized();
        }

        var mappedRole = memberRole switch
        {
            OrganizationMemberRole.Owner => OrganizationMemberRoleConstants.Owner,
            OrganizationMemberRole.Administrator => OrganizationMemberRoleConstants.Administrator,
            OrganizationMemberRole.Member => OrganizationMemberRoleConstants.Member,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (organizationMember.Role == mappedRole)
        {
            return mapper.MapTo(organizationMember, mapper.MapTo(organization));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        organizationMember.Role = mappedRole;
        repositoryFactory.OrganizationMemberRepository.Update(organizationMember);

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(organization)],
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mapper.MapTo(organizationMember, mapper.MapTo(organization));
    }

    public async Task<ICollection<OrganizationMember>> ChangeStatusAsync(
        ICollection<string> ids,
        OrganizationMemberStatus status,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctOrganizationMemberIds = ids.Distinct().ToList();
        var organizationMembers =
            await repositoryFactory.OrganizationMemberRepository.GetByIdsAsync(
                distinctOrganizationMemberIds,
                cancellationToken);
        if (organizationMembers.Count != distinctOrganizationMemberIds.Count)
        {
            throw new OrganizationMemberNotFound();
        }

        // Exclude calling customer from the list
        organizationMembers = organizationMembers.Where(item => item.Customer.Id != customer.Id).ToList();

        if (organizationMembers.Count == 0)
        {
            return [];
        }

        var organizationIds = organizationMembers.Select(item => item.Organization.Id).Distinct().ToList();
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(
            organizationIds,
            cancellationToken);

        if (!organizationMembers.All(
                item => organizationAuthorizationService.CanModify(
                    organizations.Single(organization => organization.Id == item.Organization.Id),
                    customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        var mappedStatus = status switch
        {
            OrganizationMemberStatus.Active => OrganizationMemberStatusConstants.Active,
            OrganizationMemberStatus.Inactive => OrganizationMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var organizationMember in organizationMembers)
        {
            organizationMember.Status = mappedStatus;
            repositoryFactory.OrganizationMemberRepository.Update(organizationMember);
        }

        await organizationOutboxPublisher.PublishOrganizationAsync(
            organizations.Select(mapper.MapTo),
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return organizationMembers.Select(item => mapper.MapTo(item,
            mapper.MapTo(organizations.Single(organization => organization.Id == item.Organization.Id)))).ToList();
    }

    public async Task<ICollection<OrganizationMember>> RemoveAsync(
        ICollection<string> ids,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctOrganizationMemberIds = ids.Distinct().ToList();
        var organizationMembers = await repositoryFactory.OrganizationMemberRepository.GetByIdsAsync(
            distinctOrganizationMemberIds,
            cancellationToken);
        if (organizationMembers.Count != distinctOrganizationMemberIds.Count)
        {
            throw new OrganizationMemberNotFound();
        }

        // Exclude calling customer from the list
        organizationMembers = organizationMembers.Where(item => item.Customer.Id != customer.Id).ToList();
        if (organizationMembers.Count == 0)
        {
            return [];
        }

        var organizationIds = organizationMembers.Select(item => item.Organization.Id).Distinct().ToList();
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(
            organizationIds,
            cancellationToken);

        if (!organizationMembers.All(
                item => organizationAuthorizationService.CanModify(
                    organizations.Single(organization => organization.Id == item.Organization.Id),
                    customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.OrganizationMemberRepository.RemoveRange(organizationMembers);

        await organizationOutboxPublisher.PublishOrganizationAsync(
            organizations.Select(item =>
            {
                var mapped = mapper.MapTo(item);
                mapped.OrganizationMembers = mapped.OrganizationMembers
                    .Where(organizationMember => organizationMember.DeletedAt is null).ToList();

                return mapped;
            }),
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return organizationMembers.Select(item => mapper.MapTo(item,
            mapper.MapTo(organizations.Single(organization => organization.Id == item.Organization.Id)))).ToList();
    }

    public async Task<Shared.Models.Organization> AddMemberAsync(
        string organizationId,
        OrganizationMember member,
        CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (organization.OrganizationMembers.Any(item => item.Id == member.Id))
        {
            return mapper.MapTo(organization);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(
            member.Customer.Id,
            cancellationToken);

        var organizationMember = await repositoryFactory.OrganizationMemberRepository.GetByIdAsync(
            member.Id,
            cancellationToken);

        if (organizationMember is null)
        {
            repositoryFactory.OrganizationMemberRepository.Add(
                mapper.MapToEntity(member, organization, customer));
        }
        else
        {
            repositoryFactory.OrganizationMemberRepository.Update(
                mapper.MergeToEntity(member, organizationMember, organization, customer));
        }

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(organization)],
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task CompleteOrganizationMemberOnboardingAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationMemberRepository.UnitOfWork,
            cancellationToken);

        var matchingOrganizationMember =
            organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customer.Id);

        if (matchingOrganizationMember is null)
        {
            throw new Unauthorized();
        }

        matchingOrganizationMember.IsOrganizationOnboardingDone = true;

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await repositoryFactory.OrganizationRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        organizationService.ClearOrganizationMemberCache(organization, customer);
    }
}
