using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Booking = Organization.Shared.Database.Entities.Booking;
using Customer = Organization.Shared.Models.Customer;
using IndustrySubCategory = Organization.Shared.Database.Entities.IndustrySubCategory;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;
using Tag = Organization.Shared.Database.Entities.Tag;
using TermsOfUse = Organization.Shared.Database.Entities.TermsOfUse;

namespace Organization.Api.Services;

public interface IOrganizationService
{
    Task<Shared.Models.Organization> AddAsync(
        Shared.Models.Organization organization,
        string? offeringCode,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateAsync(Shared.Models.Organization organization, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> DeleteAsync(string organizationId, CancellationToken cancellationToken);
    Task<Shared.Models.Organization?> GetByIdAsync(string organizationId, CancellationToken cancellationToken);
    Task<Shared.Models.Organization?> GetByAzureTenantAsync(CancellationToken cancellationToken);
    Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int )> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);

    void ClearOrganizationMemberCache(Shared.Database.Entities.Organization organization, Customer customer);
}

public class OrganizationService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IMapper mapper,
    TimeProvider timeProvider,
    IContext context,
    IMemoryCache memoryCache) : IOrganizationService
{
    public async Task<Shared.Models.Organization> AddAsync(
        Shared.Models.Organization organization,
        string? offeringCode,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (!organization.AgreedToTermsOfUse || string.IsNullOrWhiteSpace(organization.TermsOfUse?.Id))
        {
            throw new OrganizationTermsOfUseAgreementMissing();
        }

        Customer? customer = null;
        Shared.Database.Entities.Customer? customerEntity = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, customerEntity) = await customerService.GetNullableAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(organization.Id))
        {
            var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
            if (existingOrganization is not null)
            {
                if (!ignoreAuthorizationCheck && customer is null)
                {
                    throw new CustomerNotFound();
                }

                return await UpdateInternalAsync(organization, existingOrganization, customer, cancellationToken);
            }
        }
        else
        {
            organization.Id = randomHelper.Generate();
        }

        var termsOfUse = await repositoryFactory.TermsOfUseRepository
            .Query(new Specification<TermsOfUse> { Criteria = query => !query.DeletedAt.HasValue })
            .FirstAsync(cancellationToken);

        if (organization.TermsOfUse?.Id != termsOfUse.Id)
        {
            throw new OrganizationTermsOfUseAgreementMissing();
        }

        var industrySubCategoryIds = organization.IndustrySubCategories.Select(item => item.Id).ToList();
        var industrySubCategories = await repositoryFactory.IndustrySubCategoryRepository
            .Query(new Specification<IndustrySubCategory> { Criteria = query => industrySubCategoryIds.Contains(query.Id) }
                .AddInclude(query => query.IndustryMainCategory))
            .ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationEntity = mapper.MapTo(organization, termsOfUse, industrySubCategories);

        var organizationMembers = new List<OrganizationMember>();
        if (customerEntity is not null)
        {
            organizationMembers.Add(new OrganizationMember
            {
                Id = randomHelper.Generate(),
                Role = OrganizationMemberRoleConstants.Owner,
                Status = OrganizationMemberStatusConstants.Active,
                Customer = customerEntity,
                Organization = organizationEntity
            });
        }

        var now = timeProvider.GetUtcNow();
        var finalOfferingCode = string.IsNullOrWhiteSpace(offeringCode) ? OfferingCode.FreeTierV1 : offeringCode.ToOfferingCode();
        var organizationOffering = new OrganizationOffering
        {
            Id = randomHelper.Generate(),
            CreatedAt = now,
            Organization = organizationEntity,
            Code = finalOfferingCode,
            Start = now,
            End = now.GetOfferingPeriodStart().GetOfferingPeriodEnd(),
            AutoRenew = true,
            UnitPrice = finalOfferingCode.GetOffering().UnitPrice
        };

        organizationEntity.HasAttachedPaymentMethod = false;
        organizationEntity.OrganizationMembers = organizationMembers;
        organizationEntity.OrganizationOfferings = [organizationOffering];
        organizationEntity = repositoryFactory.OrganizationRepository.Add(organizationEntity);

        repositoryFactory.TagRepository.Add(
            new Tag
            {
                Id = randomHelper.Generate(),
                Name = "Desk",
                Type = OrganizationTagTypeConstants.ResourceDesk,
                Color = "#87CEEB",
                Organization = organizationEntity
            });

        repositoryFactory.TagRepository.Add(
            new Tag
            {
                Id = randomHelper.Generate(),
                Name = "Room",
                Type = OrganizationTagTypeConstants.ResourceRoom,
                Color = "#98FB98",
                Organization = organizationEntity
            });

        repositoryFactory.TagRepository.Add(
            new Tag
            {
                Id = randomHelper.Generate(),
                Name = "Parking",
                Type = OrganizationTagTypeConstants.ResourceParking,
                Color = "#20B2AA",
                Organization = organizationEntity
            });

        repositoryFactory.TagRepository.Add(
            new Tag
            {
                Id = randomHelper.Generate(),
                Name = "Others",
                Type = OrganizationTagTypeConstants.ResourceOthers,
                Color = "#8A2BE2",
                Organization = organizationEntity
            });

        repositoryFactory.OrganizationMemberRepository.AddRange(organizationMembers);
        organization = mapper.MapTo(organizationEntity);

        await organizationOutboxPublisher.PublishOrganizationsAsync([organization], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(Shared.Models.Organization organization, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        return await UpdateInternalAsync(organization, existingOrganization, customer, cancellationToken);
    }

    public async Task<Shared.Models.Organization> DeleteAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanDelete(organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var deletedOrganization = mapper.MapTo(repositoryFactory.OrganizationRepository.Remove(organization));

        await organizationOutboxPublisher.PublishOrganizationsAsync([deletedOrganization], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedOrganization;
    }

    public async Task<Shared.Models.Organization?> GetByIdAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        return await EnrichOrganizationAsync(customer, organization, cancellationToken);
    }

    public async Task<Shared.Models.Organization?> GetByAzureTenantAsync(CancellationToken cancellationToken)
    {
        var tenantId = context.GetAzureTenantId();
        if (tenantId == Guid.Empty)
        {
            return null;
        }

        var azureTenantId = tenantId.ToString();
        var organization = await repositoryFactory.OrganizationRepository.GetByAzureTenantIdAsync(azureTenantId, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        return await EnrichOrganizationAsync(customer, organization, cancellationToken);
    }

    public async Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);

        var result = new List<Shared.Models.Organization>();
        foreach (var organization in organizations)
        {
            result.Add(await EnrichOrganizationAsync(customer, organization, cancellationToken));
        }

        return result;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        // Ensure we do not return other customer organization by forcing CustomerId as search criteria
        searchCriteria.CustomerId = customer.Id;

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationRepository.GetPaginatedOrganizationsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedOrganizations = new List<Edge<Shared.Models.Organization>>();
        foreach (var edge in edges)
        {
            mappedOrganizations.Add(
                new Edge<Shared.Models.Organization>(edge.Cursor, await EnrichOrganizationAsync(customer, edge.Node, cancellationToken)));
        }

        return (paginatedInfo, mappedOrganizations, totalCount);
    }

    public void ClearOrganizationMemberCache(Shared.Database.Entities.Organization organization, Customer customer) =>
        memoryCache.Remove($"organization-{organization.Id}-customer-{customer.Id}-member");

    private async Task<Shared.Models.Organization> UpdateInternalAsync(
        Shared.Models.Organization organization,
        Shared.Database.Entities.Organization existingOrganization,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var industrySubCategoryIds = organization.IndustrySubCategories.Select(item => item.Id).ToList();
        var industrySubCategoryEntities = industrySubCategoryIds.Count == 0
            ? []
            : await repositoryFactory.IndustrySubCategoryRepository
                .Query(new Specification<IndustrySubCategory>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && industrySubCategoryIds.Contains(query.Id)
                    }
                    .AddInclude(query => query.IndustryMainCategory))
                .ToListAsync(cancellationToken);

        organization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Update(mapper.MergeTo(organization, existingOrganization, industrySubCategoryEntities)));

        await organizationOutboxPublisher.PublishOrganizationsAsync([organization], repositoryFactory.UnitOfWork, cancellationToken);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organization;
    }

    private async Task<Shared.Models.Organization> EnrichOrganizationAsync(
        Customer customer,
        Shared.Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        if (!organizationAuthorizationService.CanView(organization, customer))
        {
            throw new Unauthorized();
        }

        var mappedOrganization = mapper.MapTo(organization);

        mappedOrganization.CanModify = organizationAuthorizationService.CanModify(organization, customer);
        mappedOrganization.CanDelete = organizationAuthorizationService.CanDelete(organization, customer);
        mappedOrganization.CanInvitePeople = organizationAuthorizationService.CanInvitePeople(organization, customer);
        mappedOrganization.CanViewAnalytics = organizationAuthorizationService.CanViewAnalytics(organization, customer);
        mappedOrganization.HasLocation = organization.Locations.Count != 0;
        mappedOrganization.HasTeam = organization.Teams.Count != 0;

        var now = timeProvider.GetUtcNow();
        mappedOrganization.HasFutureBooking = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Organization.Id == organization.Id && query.From >= now
            })
            .AnyAsync(cancellationToken);


        var organizationMember = await memoryCache.GetOrCreateAsync(
            $"organization-{organization.Id}-customer-{customer.Id}-member",
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(1);

                return await repositoryFactory.OrganizationMemberRepository
                    .Query(new Specification<OrganizationMember>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Organization.Id == organization.Id &&
                                            query.CustomerId == customer.Id
                    })
                    .FirstOrDefaultAsync(cancellationToken);
            });

        if (organizationMember is not null)
        {
            mappedOrganization.IsMyOnboardingDone = organizationMember.IsOrganizationOnboardingDone ?? false;
        }

        return mappedOrganization;
    }
}
