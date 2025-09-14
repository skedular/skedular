using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using HotChocolate.Types.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services;

public interface IOrganizationMemberService
{
    Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)> GetPaginatedOrganizationMembersAsync(
        PaginationInputParam paginationInputParam,
        OrganizationMemberSearchCriteria searchCriteria,
        ICollection<OrganizationMemberOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<OrganizationMember> ChangeRoleAsync(string organizationMemberId, OrganizationMemberRole memberRole, CancellationToken cancellationToken);

    Task<ICollection<OrganizationMember>> ChangeStatusAsync(
        ICollection<string> ids,
        OrganizationMemberStatus status,
        CancellationToken cancellationToken);

    Task<ICollection<OrganizationMember>> RemoveAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> AdminAddMemberAsync(string organizationId, OrganizationMember member, CancellationToken cancellationToken);

    Task CompleteOrganizationMemberOnboardingAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);
}

public class OrganizationMemberService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IMapper mapper,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationMemberService
{
    public async Task<(PaginatedInfo, ICollection<Edge<OrganizationMember>>, int)> GetPaginatedOrganizationMembersAsync(
        PaginationInputParam paginationInputParam,
        OrganizationMemberSearchCriteria searchCriteria,
        ICollection<OrganizationMemberOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               searchCriteria.OrganizationId,
                               searchCriteria.OrganizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanViewAsync(organization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationMemberRepository.GetPaginatedOrganizationMembersAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo,
            mapper.MapTo(
                    edges,
                    mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)))
                .ToList(),
            totalCount);
    }

    public async Task<OrganizationMember> ChangeRoleAsync(
        string organizationMemberId,
        OrganizationMemberRole memberRole,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationMemberId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organizationMember = await repositoryFactory.OrganizationMemberRepository.GetByIdAsync(organizationMemberId, cancellationToken) ??
                                 throw new OrganizationMemberNotFound();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationMember.Organization.Id,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var myMemberDetails = organization.OrganizationMembers.Single(item => item.Customer.Id == customer.Id);
        if (myMemberDetails.Status != OrganizationMemberStatusConstants.Active)
        {
            throw new UnauthorizedAccessException();
        }

        if (myMemberDetails.Role == OrganizationMemberRoleConstants.Administrator && memberRole == OrganizationMemberRole.Owner)
        {
            throw new UnauthorizedAccessException();
        }

        if (myMemberDetails.Role == OrganizationMemberRoleConstants.Member && memberRole == OrganizationMemberRole.Administrator)
        {
            throw new UnauthorizedAccessException();
        }

        var mappedRole = memberRole.ToOrganizationMemberRole();
        if (organizationMember.Role == mappedRole)
        {
            return mapper.MapTo(
                organizationMember,
                mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationMember.Role = mappedRole;
        repositoryFactory.OrganizationMemberRepository.Update(organizationMember);

        organizationOutboxPublisher.PublishOrganizations(
            [mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(
            organizationMember,
            mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)));
    }

    public async Task<ICollection<OrganizationMember>> ChangeStatusAsync(
        ICollection<string> ids,
        OrganizationMemberStatus status,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var distinctOrganizationMemberIds = ids.Distinct().ToList();
        var organizationMembers =
            await repositoryFactory.OrganizationMemberRepository.GetByIdsAsync(distinctOrganizationMemberIds, cancellationToken);
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
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            null,
            cancellationToken);


        foreach (var item in organizationMembers)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(
                    organizations.Single(organization => organization.Id == item.Organization.Id),
                    customer.Id,
                    cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var mappedStatus = status.ToOrganizationMemberStatus();

        foreach (var organizationMember in organizationMembers)
        {
            organizationMember.Status = mappedStatus;
            repositoryFactory.OrganizationMemberRepository.Update(organizationMember);
        }

        organizationOutboxPublisher.PublishOrganizations(
            organizations.Select(item =>
                mapper.MapTo(item, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(item.Id))),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return organizationMembers
            .Select(item =>
            {
                var matchedOrganization = organizations.Single(organization => organization.Id == item.Organization.Id);
                return mapper.MapTo(
                    item,
                    mapper.MapTo(
                        matchedOrganization,
                        organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(matchedOrganization.Id)));
            })
            .ToList();
    }

    public async Task<ICollection<OrganizationMember>> RemoveAsync(ICollection<string> ids, CancellationToken cancellationToken)
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
        var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
            organizationIds,
            null,
            cancellationToken);

        foreach (var item in organizationMembers)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(
                    organizations.Single(organization => organization.Id == item.Organization.Id),
                    customer.Id,
                    cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.OrganizationMemberRepository.RemoveRange(organizationMembers);

        organizationOutboxPublisher.PublishOrganizations(
            organizations.Select(item =>
            {
                var mapped = mapper.MapTo(item, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(item.Id));
                mapped.OrganizationMembers = mapped.OrganizationMembers.Where(organizationMember => organizationMember.DeletedAt is null).ToList();

                return mapped;
            }),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationMembers.Select(item =>
        {
            var matchedOrganization = organizations.Single(organization => organization.Id == item.Organization.Id);
            return mapper.MapTo(
                item,
                mapper.MapTo(
                    matchedOrganization,
                    organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(matchedOrganization.Id)));
        }).ToList();
    }

    public async Task<Shared.Models.Organization> AdminAddMemberAsync(
        string organizationId,
        OrganizationMember member,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (organization.OrganizationMembers.Any(item => item.Id == member.Id))
        {
            return mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var customer = await repositoryFactory.CustomerRepository.UpsertNakedAsync(member.Customer.Id, cancellationToken);
        var organizationMember = await repositoryFactory.OrganizationMemberRepository.GetByIdAsync(member.Id, cancellationToken);
        if (organizationMember is null)
        {
            repositoryFactory.OrganizationMemberRepository.Add(mapper.MapToEntity(member, organization, customer));
        }
        else
        {
            repositoryFactory.OrganizationMemberRepository.Update(mapper.MergeToEntity(member, organizationMember, organization, customer));
        }

        var mappedOrganization = mapper.MapTo(organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedOrganization;
    }

    public async Task CompleteOrganizationMemberOnboardingAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               organizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var matchingOrganizationMember = organization.OrganizationMembers.FirstOrDefault(item => item.Customer.Id == customer.Id);
        if (matchingOrganizationMember is null)
        {
            throw new UnauthorizedAccessException();
        }

        matchingOrganizationMember.IsOrganizationOnboardingDone = true;

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.RemoveByIdOrUniqueAlphanumericNameAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            cancellationToken);
    }
}
