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
using Booking = Organization.Shared.Database.Entities.Booking;
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

    Task<Shared.Models.Organization> DeleteAsync(
        string? id,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization?> GetByIdOrUniqueAlphanumericNamePublicAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization?> GetByAzureTenantAsync(CancellationToken cancellationToken);
    Task<ICollection<Shared.Models.Organization>> GetMyOrganizationsAsync(CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Shared.Models.Organization>>, int )> GetPaginatedOrganizationsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationSearchCriteria searchCriteria,
        ICollection<OrganizationOrder> orderByFields,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateMarketplaceListingMetadataAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        ListingMetadata marketplaceListingMetadata,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateOrganizationBillingCycleAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
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

        if (!string.IsNullOrWhiteSpace(organization.Id) || !string.IsNullOrWhiteSpace(organization.UniqueAlphanumericName))
        {
            var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                organization.Id,
                organization.UniqueAlphanumericName,
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

        if (string.IsNullOrWhiteSpace(organization.UniqueAlphanumericName))
        {
            organization.UniqueAlphanumericName = randomHelper.GenerateAlphanumericNumeric(10).ToLowerInvariant();
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
            new NewOrganizationJoinedInput(organization.Id, organization.UniqueAlphanumericName),
            repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateAsync(Shared.Models.Organization organization, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organization.Id,
                                       organization.UniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        return await UpdateInternalAsync(organization, existingOrganization, customer, cancellationToken);
    }

    public async Task<Shared.Models.Organization> DeleteAsync(
        string? id,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               id,
                               organizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanDeleteAsync(organization, customer.Id, cancellationToken))
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

        await cachedOrganizationService.RemoveByIdOrUniqueAlphanumericNameAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            cancellationToken);

        return deletedOrganization;
    }

    public async Task<Shared.Models.Organization?> GetByIdOrUniqueAlphanumericNameAsync(
        string? id,
        string? uniqueAlphanumericName,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(id, uniqueAlphanumericName, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        Shared.Database.Entities.Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            customer = await cachedCustomerService.GetAsync(cancellationToken);
        }

        return await EnrichOrganizationAsync(customer, organization, ignoreAuthorizationCheck, cancellationToken);
    }

    public async Task<Shared.Models.Organization?> GetByIdOrUniqueAlphanumericNamePublicAsync(
        string? id,
        string? uniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(id, uniqueAlphanumericName, cancellationToken);
        if (organization is null)
        {
            return null;
        }

        if (organization.Type.ToOrganizationType() == OrganizationType.Private)
        {
            return await EnrichOrganizationAsync(await cachedCustomerService.GetAsync(cancellationToken), organization, false, cancellationToken);
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

        await cachedOrganizationService.UpdateAsync(organizations, cancellationToken);

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
        searchCriteria = searchCriteria with { CustomerId = customer.Id };

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.OrganizationRepository.GetPaginatedOrganizationsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        await cachedOrganizationService.UpdateAsync(edges.Select(item => item.Node).ToList(), cancellationToken);

        var mappedOrganizations = new List<Edge<Shared.Models.Organization>>();
        foreach (var edge in edges)
        {
            mappedOrganizations.Add(
                new Edge<Shared.Models.Organization>(await EnrichOrganizationAsync(customer, edge.Node, false, cancellationToken), edge.Cursor));
        }

        return (paginatedInfo, mappedOrganizations, totalCount);
    }

    public async Task<Shared.Models.Organization> UpdateMarketplaceListingMetadataAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        ListingMetadata marketplaceListingMetadata,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organizationId,
                                       organizationUniqueAlphanumericName,
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

        await cachedOrganizationService.UpdateByIdOrUniqueAlphanumericNameAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            cancellationToken);

        return organization;
    }

    public async Task<Shared.Models.Organization> UpdateOrganizationBillingCycleAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        OrganizationBillingCycle billingCycle,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organizationId,
                                       organizationUniqueAlphanumericName,
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

        await cachedOrganizationService.UpdateByIdOrUniqueAlphanumericNameAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            cancellationToken);

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

        // Don't change UniqueAlphanumericName if no unique name provided
        organization.UniqueAlphanumericName = string.IsNullOrWhiteSpace(organization.UniqueAlphanumericName)
            ? existingOrganization.UniqueAlphanumericName
            : organization.UniqueAlphanumericName.ToLowerInvariant();

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

        await cachedOrganizationService.UpdateByIdOrUniqueAlphanumericNameAsync(
            organization.Id,
            organization.UniqueAlphanumericName,
            cancellationToken);

        return organization;
    }

    private async Task<Shared.Models.Organization> EnrichOrganizationAsync(
        Shared.Database.Entities.Customer? customer,
        Shared.Database.Entities.Organization organization,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        if (!ignoreAuthorizationCheck)
        {
            ArgumentNullException.ThrowIfNull(customer);

            if (!await organizationAuthorizationService.CanViewAsync(organization, customer.Id, cancellationToken))
            {
                if (!await organizationAuthorizationService.CanViewMinimumAsync(organization, customer.Id, cancellationToken))
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

        mappedOrganization.CanModify = ignoreAuthorizationCheck ||
                                       await organizationAuthorizationService.CanModifyAsync(organization, customer!.Id, cancellationToken);
        mappedOrganization.CanDelete = ignoreAuthorizationCheck ||
                                       await organizationAuthorizationService.CanDeleteAsync(organization, customer!.Id, cancellationToken);
        mappedOrganization.CanInvitePeople = ignoreAuthorizationCheck ||
                                             await organizationAuthorizationService.CanInvitePeopleAsync(organization, customer!.Id,
                                                 cancellationToken);
        mappedOrganization.CanViewAnalytics = ignoreAuthorizationCheck ||
                                              await organizationAuthorizationService.CanViewAnalyticsAsync(organization, customer!.Id,
                                                  cancellationToken);
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
            var organizationMember = organization.OrganizationMembers.FirstOrDefault(item => item.CustomerId == customer!.Id);
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
        organization.InvolvedBookings = [];
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
