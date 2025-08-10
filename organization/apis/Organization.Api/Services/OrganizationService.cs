using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
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
    Task<Shared.Models.Organization> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Organization?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
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
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ITemporalOutboxPublisher temporalOutboxPublisher,
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
            organization.UniqueAlphanumericName = randomHelper.GenerateAlphanumericNumeric(10).ToLowerInvariant();
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
        organization = mapper.MapTo(
            organizationEntity,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organizationEntity.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        temporalOutboxPublisher.StartWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                organizationOffering.Id,
                organizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (customer is null || organizationAuthorizationService.CanViewMemberPersonalDetails(organizationEntity, customer))
        {
            return organization;
        }

        var memberVisibilityPolicy = organizationEntity.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        foreach (var member in organization.OrganizationMembers.Where(item => item.Customer.Id != customer.Id))
        {
            member.Customer = member.Customer.Redact(memberVisibilityPolicy);
            foreach (var identity in member.Customer.Identities)
            {
                identity.Email = identity.Email.FullRedact(memberVisibilityPolicy);
            }
        }

        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(Shared.Models.Organization organization, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organization.Id, cancellationToken) ??
                                   throw new OrganizationNotFound();

        return await UpdateInternalAsync(organization, existingOrganization, customer, cancellationToken);
    }

    public async Task<Shared.Models.Organization> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(id, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanDelete(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        organization.UniqueAlphanumericName = null;
        var deletedOrganization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Remove(organization),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));

        organizationOutboxPublisher.PublishOrganizations([deletedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (organizationAuthorizationService.CanViewMemberPersonalDetails(organization, customer))
        {
            return deletedOrganization;
        }

        var memberVisibilityPolicy = organization.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        foreach (var member in deletedOrganization.OrganizationMembers.Where(item => item.Customer.Id != customer.Id))
        {
            member.Customer = member.Customer.Redact(memberVisibilityPolicy);
            foreach (var identity in member.Customer.Identities)
            {
                identity.Email = identity.Email.FullRedact(memberVisibilityPolicy);
            }
        }

        return deletedOrganization;
    }

    public async Task<Shared.Models.Organization?> GetByIdAsync(string id, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(id, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        return await EnrichOrganizationAsync(customer, organization, ignoreAuthorizationCheck, cancellationToken);
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

        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        return await EnrichOrganizationAsync(customer, organization, false, cancellationToken);
    }

    public async Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organizations = await repositoryFactory.OrganizationRepository.GetByCustomerIdAsync(customer.Id, cancellationToken);

        var result = new List<Shared.Models.Organization>();
        foreach (var organization in organizations)
        {
            result.Add(await EnrichOrganizationAsync(customer, organization, false, cancellationToken));
        }

        return result;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        // Ensure we do not return another customer organization by forcing CustomerId as search criteria
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
                new Edge<Shared.Models.Organization>(await EnrichOrganizationAsync(customer, edge.Node, false, cancellationToken), edge.Cursor));
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
            throw new UnauthorizedAccessException();
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

        // Don't change UniqueAlphanumericName if no unique name provided
        organization.UniqueAlphanumericName = string.IsNullOrWhiteSpace(organization.UniqueAlphanumericName)
            ? existingOrganization.UniqueAlphanumericName
            : organization.UniqueAlphanumericName.ToLowerInvariant();
        
        organization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Update(mapper.MergeTo(organization, existingOrganization, industrySubCategoryEntities)),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (customer is null || organizationAuthorizationService.CanViewMemberPersonalDetails(existingOrganization, customer))
        {
            return organization;
        }

        var memberVisibilityPolicy = existingOrganization.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        foreach (var member in organization.OrganizationMembers.Where(item => item.Customer.Id != customer.Id))
        {
            member.Customer = member.Customer.Redact(memberVisibilityPolicy);
            foreach (var identity in member.Customer.Identities)
            {
                identity.Email = identity.Email.FullRedact(memberVisibilityPolicy);
            }
        }

        return organization;
    }

    private async Task<Shared.Models.Organization> EnrichOrganizationAsync(
        Customer? customer,
        Shared.Database.Entities.Organization organization,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (!ignoreAuthorizationCheck)
        {
            ArgumentNullException.ThrowIfNull(customer);
        }

        if (!ignoreAuthorizationCheck)
        {
            if (!organizationAuthorizationService.CanView(organization, customer!))
            {
                if (!organizationAuthorizationService.CanViewMinimum(organization, customer!))
                {
                    throw new UnauthorizedAccessException();
                }

                organization.About = null;
                organization.Website = null;
                organization.PaymentMethodEventRaisedAt = null;
                organization.DailyMemberCountLastRecordedAt = null;
                organization.ContactEmail = null;
                organization.ContactPhone = null;
                organization.OrganizationMembers = [];
                organization.TermsOfUse = null;
                organization.OrganizationOfferings = [];
                organization.DailyMemberCountRecordings = [];
                organization.IndustrySubCategories = [];
                organization.Locations = [];
                organization.Teams = [];
                organization.JoinInvitations = [];
                organization.AzureTenants = [];
                organization.OrganizationSsoSettings = null;
                organization.Tags = [];
                organization.InvolvedBookings = [];
                organization.OrganizationStripePaymentMethods = [];
                organization.OrganizationStripeCustomer = null;
                organization.BillingDetails = null;
                organization.OrganizationStripeConnectAccounts = [];
                organization.OrganizationBankAccounts = [];
                organization.OrganizationTaxDetails = null;
                organization.PhysicalAddress = null;

                return mapper.MapTo(
                    organization,
                    organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
            }
        }

        var mappedOrganization = mapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));

        mappedOrganization.CanModify = ignoreAuthorizationCheck || organizationAuthorizationService.CanModify(organization, customer!);
        mappedOrganization.CanDelete = ignoreAuthorizationCheck || organizationAuthorizationService.CanDelete(organization, customer!);
        mappedOrganization.CanInvitePeople = ignoreAuthorizationCheck || organizationAuthorizationService.CanInvitePeople(organization, customer!);
        mappedOrganization.CanViewAnalytics = ignoreAuthorizationCheck || organizationAuthorizationService.CanViewAnalytics(organization, customer!);
        mappedOrganization.HasLocation = organization.Locations.Count != 0;
        mappedOrganization.HasTeam = organization.Teams.Count != 0;

        var now = timeProvider.GetUtcNow();
        mappedOrganization.HasFutureBooking = await repositoryFactory.BookingRepository
            .Query(new Specification<Booking>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue && query.InvolvedOrganizations.Select(item => item.Id).Contains(organization.Id) && query.From >= now
            })
            .AnyAsync(cancellationToken);


        if (!ignoreAuthorizationCheck)
        {
            var organizationMember = await memoryCache.GetOrCreateAsync(
                $"organization-{organization.Id}-customer-{customer!.Id}-member",
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
        }
        else
        {
            mappedOrganization.IsMyOnboardingDone = false;
        }

        if (ignoreAuthorizationCheck || organizationAuthorizationService.CanViewMemberPersonalDetails(organization, customer!))
        {
            return mappedOrganization;
        }

        var memberVisibilityPolicy = organization.MemberVisibilityPolicy.ToOrganizationMemberVisibilityPolicy();
        foreach (var member in mappedOrganization.OrganizationMembers.Where(item => item.Customer.Id != customer!.Id))
        {
            member.Customer = member.Customer.Redact(memberVisibilityPolicy);
            foreach (var identity in member.Customer.Identities)
            {
                identity.Email = identity.Email.FullRedact(memberVisibilityPolicy);
            }
        }

        return mappedOrganization;
    }
}
