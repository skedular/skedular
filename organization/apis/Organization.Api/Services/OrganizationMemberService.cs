using Api.Shared.Models;
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

    Task<OrganizationMember> ChangeMembershipTypeAsync(
        string organizationMemberId,
        string membershipType,
        CancellationToken cancellationToken);

    Task<ICollection<OrganizationMember>> ChangeStatusAsync(
        ICollection<string> organizationMemberIds,
        string status,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateMembersAsync(
        string organizationId,
        ICollection<OrganizationMember> members,
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

    public async Task<OrganizationMember> ChangeMembershipTypeAsync(
        string organizationMemberId,
        string membershipType,
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

        var myMembershipDetails =
            organization.OrganizationMembers.Single(item => item.Customer.Id == customer.Id);

        if (myMembershipDetails.Status != OrganizationMemberStatus.Active)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.MembershipType == OrganizationMembershipType.Administrator &&
            membershipType == OrganizationMembershipType.Owner)
        {
            throw new Unauthorized();
        }

        if (myMembershipDetails.MembershipType == OrganizationMembershipType.Member &&
            membershipType == OrganizationMembershipType.Administrator)
        {
            throw new Unauthorized();
        }

        if (organizationMember.MembershipType == membershipType)
        {
            return mapper.MapTo(organizationMember, mapper.MapTo(organization));
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationMemberRepository.UnitOfWork,
                cancellationToken);

        organizationMember.MembershipType = membershipType;
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
        ICollection<string> organizationMemberIds,
        string status,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctOrganizationMemberIds = organizationMemberIds.Distinct().ToList();
        var organizationMembers =
            await repositoryFactory.OrganizationMemberRepository.GetByIdsAsync(
                distinctOrganizationMemberIds,
                cancellationToken);
        if (organizationMembers.Count != distinctOrganizationMemberIds.Count)
        {
            throw new OrganizationMemberNotFound();
        }

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

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationMemberRepository.UnitOfWork,
                cancellationToken);

        foreach (var organizationMember in organizationMembers)
        {
            organizationMember.Status = status;
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

    public async Task<Shared.Models.Organization> UpdateMembersAsync(
        string organizationId,
        ICollection<OrganizationMember> members,
        CancellationToken cancellationToken)
    {
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationMemberRepository.UnitOfWork,
                cancellationToken);

        var updatedItems = new List<Shared.Database.Entities.OrganizationMember>();
        foreach (var organizationMember in organization.OrganizationMembers
                     .Where(organizationMember =>
                         members.Any(item => item.Id == organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                    organizationMember.Customer.Id,
                    cancellationToken);
            updatedItems.Add(repositoryFactory.OrganizationMemberRepository.Update(
                mapper.MergeToEntity(
                    members.Single(item => item.Id == organizationMember.Id),
                    organizationMember,
                    organization,
                    customer)));
        }

        var addedItems = new List<Shared.Database.Entities.OrganizationMember>();
        foreach (var organizationMember in members.Where(organizationMember =>
                     organization.OrganizationMembers.All(item => item.Id != organizationMember.Id)))
        {
            var customer =
                await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                    organizationMember.Customer.Id,
                    cancellationToken);
            addedItems.Add(repositoryFactory.OrganizationMemberRepository.Add(
                mapper.MapToEntity(organizationMember, organization, customer)));
        }

        organization.OrganizationMembers = addedItems.Concat(updatedItems).ToList();

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(organization)],
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        await repositoryFactory.OrganizationMemberRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
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

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
                repositoryFactory.OrganizationMemberRepository.UnitOfWork,
                cancellationToken);

        var customer =
            await repositoryFactory.CustomerRepository.UpsertNakedAsync(
                member.Customer.Id,
                cancellationToken);
        var organizationMemberToAdd =
            repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(member, organization, customer));

        organization.OrganizationMembers = organization.OrganizationMembers.Concat([organizationMemberToAdd]).ToList();

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

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(
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
