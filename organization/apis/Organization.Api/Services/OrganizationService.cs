using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Context;
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
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Organization.Shared.Workflows;
using Constants = Enterprise.Shared.Constants;
using Customer = Organization.Shared.Models.Customer;
using IndustrySubCategory = Organization.Shared.Database.Entities.IndustrySubCategory;
using OrganizationMember = Organization.Shared.Database.Entities.OrganizationMember;
using OrganizationOffering = Organization.Shared.Database.Entities.OrganizationOffering;
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
    Task<Shared.Models.Organization> DeleteAsync(string? id, string? customDomain, CancellationToken cancellationToken);

    Task<Shared.Models.Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization?> GetByIdOrCustomDomainPublicAsync(string? id, string? customDomain, CancellationToken cancellationToken);
    Task<Shared.Models.Organization?> GetByAzureTenantAsync(CancellationToken cancellationToken);
    Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateMarketplaceListingMetadataAsync(
        string? organizationId,
        string? organizationCustomDomain,
        ListingMetadata marketplaceListingMetadata,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateOrganizationBillingCycleAsync(
        string? organizationId,
        string? organizationCustomDomain,
        OrganizationBillingCycle billingCycle,
        CancellationToken cancellationToken);
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
    ITemporalOutboxService temporalOutboxService,
    IMapper mapper,
    TimeProvider timeProvider,
    IContext context,
    ICachedOrganizationService cachedOrganizationService,
    IOrganizationDefaultValuesProvider organizationDefaultValuesProvider) : IOrganizationService
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

        if (!string.IsNullOrWhiteSpace(organization.Id) || !string.IsNullOrWhiteSpace(organization.CustomDomain))
        {
            var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                organization.Id,
                organization.CustomDomain,
                cancellationToken);
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

        if (string.IsNullOrWhiteSpace(organization.CustomDomain))
        {
            organization.CustomDomain = randomHelper.GenerateAlphanumericNumeric(10).ToLowerInvariant();
        }

        organization.IsOwnershipVerified = false;

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

        AddDefaultOrganizationTags(organizationEntity);

        repositoryFactory.OrganizationMemberRepository.AddRange(organizationMembers);
        organization = mapper.MapTo(
            organizationEntity,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organizationEntity.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        temporalOutboxService.StartWorkflowScheduleRenewOrganizationOffering(
            new ScheduleRenewOrganizationOfferingInput(
                organization.Id,
                organizationOffering.Id,
                organizationOffering.End.GetNextOfferingPeriodStart()),
            repositoryFactory.UnitOfWork);

        temporalOutboxService.StartWorkflowOrganizationDailyAnalytics(
            new GenerateOrganizationDailyAnalyticsInput(organization.Id, timeProvider.GetUtcNow().AddDays(1)),
            repositoryFactory.UnitOfWork);

        temporalOutboxService.StartWorkflowNewOrganizationJoined(
            new NewOrganizationJoinedInput(organization.Id, organization.CustomDomain),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (customer is not null)
        {
            await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync([customer.Id], cancellationToken);
        }

        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(Shared.Models.Organization organization, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organization.Id,
                                       organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        return await UpdateInternalAsync(organization, existingOrganization, customer, cancellationToken);
    }

    public async Task<Shared.Models.Organization> DeleteAsync(string? id, string? customDomain, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               id,
                               customDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanDeleteAsync(organization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.CustomDomain = null;

        var deletedOrganization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Remove(organization),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));

        organizationOutboxPublisher.PublishOrganizations([deletedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
            organization.OrganizationMembers.Select(item => item.CustomerId).ToList(),
            cancellationToken);

        return deletedOrganization;
    }

    public async Task<Shared.Models.Organization?> GetByIdOrCustomDomainAsync(
        string? id,
        string? customDomain,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(id, customDomain, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        string? customerId = null;
        if (!ignoreAuthorizationCheck)
        {
            customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        }

        return await EnrichOrganizationAsync(customerId, organization, ignoreAuthorizationCheck, cancellationToken);
    }

    public async Task<Shared.Models.Organization?> GetByIdOrCustomDomainPublicAsync(
        string? id,
        string? customDomain,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(id, customDomain, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        if (organization.Type.ToOrganizationType() == OrganizationType.Private)
        {
            return await EnrichOrganizationAsync(await cachedCustomerService.GetIdAsync(cancellationToken), organization, false, cancellationToken);
        }

        return EnrichOrganizationPublic(organization);
    }

    public async Task<Shared.Models.Organization?> GetByAzureTenantAsync(CancellationToken cancellationToken)
    {
        var tenantId = context.GetAzureTenantId();
        if (tenantId == Guid.Empty)
        {
            return null;
        }

        var azureTenantId = tenantId.ToString();
        var organization = await repositoryFactory.OrganizationRepository.GetByAzureTenantIdUntrackedAsync(azureTenantId, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        var customerId = await cachedCustomerService.GetNullableIdAsync(cancellationToken);
        return await EnrichOrganizationAsync(customerId, organization, false, cancellationToken);
    }

    public async Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken)
    {
        var verifiableToken = context.GetVerifiableToken();
        if (string.IsNullOrWhiteSpace(verifiableToken))
        {
            return [];
        }

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);

        return mapper.MapTo(
            await cachedOrganizationService.GetMyOrganizationsByCustomerIdAsync(customerId, cancellationToken)).ToList();
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int)> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        // Ensure we do not return another customer organization by forcing CustomerId as search criteria
        searchCriteria = searchCriteria with { CustomerId = customerId };

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationRepository.GetPaginatedOrganizationsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        var mappedOrganizations = new List<Edge<Shared.Models.Organization>>();
        foreach (var edge in edges)
        {
            mappedOrganizations.Add(
                new Edge<Shared.Models.Organization>(await EnrichOrganizationAsync(customerId, edge.Node, false, cancellationToken), edge.Cursor));
        }

        return (paginatedInfo, mappedOrganizations, totalCount);
    }

    public async Task<Shared.Models.Organization> UpdateMarketplaceListingMetadataAsync(
        string? organizationId,
        string? organizationCustomDomain,
        ListingMetadata marketplaceListingMetadata,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationId,
                                       organizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganization.MarketplaceListingMetadata = marketplaceListingMetadata;

        var organization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Update(existingOrganization),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
            existingOrganization.OrganizationMembers.Select(item => item.CustomerId).ToList(),
            cancellationToken);

        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateOrganizationBillingCycleAsync(
        string? organizationId,
        string? organizationCustomDomain,
        OrganizationBillingCycle billingCycle,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationId,
                                       organizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganization.BillingCycle = billingCycle.ToOrganizationBillingCycle();

        var organization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Update(existingOrganization),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return organization;
    }

    private async Task<Shared.Models.Organization> UpdateInternalAsync(
        Shared.Models.Organization organization,
        Shared.Database.Entities.Organization existingOrganization,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
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

        // Don't change CustomDomain if no custom domain provided
        organization.CustomDomain = string.IsNullOrWhiteSpace(organization.CustomDomain)
            ? existingOrganization.CustomDomain
            : organization.CustomDomain.ToLowerInvariant();

        // Do not allow a changing organization type, the organization type is immutable
        organization.Type = existingOrganization.Type.ToOrganizationType();

        // Preserve ownership verification status, it has its own service to handle it
        var isOwnershipVerified = existingOrganization.IsOwnershipVerified;

        existingOrganization = mapper.MergeTo(organization, existingOrganization, industrySubCategoryEntities);

        // Restoring the ownership verification status
        existingOrganization.IsOwnershipVerified = isOwnershipVerified;

        organization = mapper.MapTo(
            repositoryFactory.OrganizationRepository.Update(existingOrganization),
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([organization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return organization;
    }

    private async Task<Shared.Models.Organization> EnrichOrganizationAsync(
        string? customerId,
        Shared.Database.Entities.Organization organization,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (!ignoreAuthorizationCheck)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

            if (!await organizationAuthorizationService.CanViewAsync(organization, customerId, cancellationToken))
            {
                if (!await organizationAuthorizationService.CanViewMinimumAsync(organization, customerId, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }

                organization.ListingMetadata = ListingMetadata.Empty;
                organization.MarketplaceListingMetadata = ListingMetadata.Empty;
                organization.Website = null;
                organization.ContactEmail = null;
                organization.ContactPhone = null;
                organization.OrganizationMembers = [];
                organization.TermsOfUse = null;
                organization.OrganizationOfferings = [];
                organization.DailyMemberCountRecordings = [];
                organization.IndustrySubCategories = [];
                organization.JoinInvitations = [];
                organization.AzureTenants = [];
                organization.OrganizationSsoSettings = null;
                organization.Tags = [];
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

        mappedOrganization.CanModify = ignoreAuthorizationCheck || await organizationAuthorizationService.CanModifyAsync(
            organization,
            customerId!,
            cancellationToken);
        mappedOrganization.CanDelete = ignoreAuthorizationCheck || await organizationAuthorizationService.CanDeleteAsync(
            organization,
            customerId!,
            cancellationToken);
        mappedOrganization.CanInvitePeople = ignoreAuthorizationCheck || await organizationAuthorizationService.CanInvitePeopleAsync(
            organization,
            customerId!,
            cancellationToken);
        mappedOrganization.CanViewAnalytics = ignoreAuthorizationCheck || await organizationAuthorizationService.CanViewAnalyticsAsync(
            organization,
            customerId!,
            cancellationToken);

        if (!ignoreAuthorizationCheck)
        {
            var organizationMember = organization.OrganizationMembers.FirstOrDefault(item => item.CustomerId == customerId);
            if (organizationMember is not null)
            {
                mappedOrganization.IsMyOnboardingDone = organizationMember.IsOrganizationOnboardingDone ?? false;
            }
        }
        else
        {
            mappedOrganization.IsMyOnboardingDone = false;
        }

        return mappedOrganization;
    }

    private Shared.Models.Organization EnrichOrganizationPublic(Shared.Database.Entities.Organization organization)
    {
        organization.OrganizationMembers = [];
        organization.TermsOfUse = null;
        organization.OrganizationOfferings = [];
        organization.DailyMemberCountRecordings = [];
        organization.JoinInvitations = [];
        organization.AzureTenants = [];
        organization.OrganizationSsoSettings = null;
        organization.OrganizationStripePaymentMethods = [];
        organization.OrganizationStripeCustomer = null;
        organization.BillingDetails = null;
        organization.OrganizationStripeConnectAccounts = [];
        organization.OrganizationBankAccounts = [];
        organization.OrganizationTaxDetails = null;

        return mapper.MapTo(organization, Constants.EmptyUri);
    }

    private void AddDefaultOrganizationTags(Shared.Database.Entities.Organization organizationEntity)
    {
        var tags = organizationDefaultValuesProvider.GetDefaultTags(organizationEntity);
        foreach (var tag in tags)
        {
            repositoryFactory.TagRepository.Add(tag);
        }
    }
}
