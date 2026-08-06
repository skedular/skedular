using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Marketplace.Api.Models;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Mappers;
using Marketplace.Shared.Models;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Marketplace.Shared.Services.Cache;
using Organization = Marketplace.Shared.Database.Entities.Organization;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Services;

public interface IProductService
{
    Task<Product> AddAsync(
        string? id,
        string? organizationId,
        string? organizationCustomDomain,
        ProductVersion productVersion,
        CancellationToken cancellationToken);

    Task<Product> EnsureHostLocationDraftAsync(
        string id,
        string organizationId,
        string productTagId,
        string locationName,
        CancellationToken cancellationToken);

    Task RemoveHostLocationDraftAsync(string id, string organizationId, CancellationToken cancellationToken);

    Task<Product> UpdateAsync(ProductPatchRequest request, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> DeleteAsync(IReadOnlyList<string> productIds, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> ActivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> DeactivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        IReadOnlyList<ProductOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class ProductService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMarketplaceOutboxPublisher marketplaceOutboxPublisher,
    IEntityMapper entityMapper,
    ICachedProductService cachedProductService,
    ILogger<ProductService> logger,
    ISpacesAccessEvaluator spacesAccessEvaluator,
    TimeProvider timeProvider) : IProductService
{
    public async Task<Product> EnsureHostLocationDraftAsync(
        string id,
        string organizationId,
        string productTagId,
        string locationName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        ArgumentException.ThrowIfNullOrWhiteSpace(productTagId);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationName);

        if (!HostLocationSystemIds.IsProduct(id) || !HostLocationSystemIds.IsProductTag(productTagId) ||
            id[HostLocationSystemIds.ProductPrefix.Length..] != productTagId[HostLocationSystemIds.ProductTagPrefix.Length..])
        {
            throw new InvalidOperationException("Host Location draft Product identifiers do not match.");
        }

        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct is not null)
        {
            if (existingProduct.Organization.Id != organizationId || existingProduct.Organization.Type != OrganizationTypeConstants.Host)
            {
                throw new UnauthorizedAccessException();
            }

            return entityMapper.MapTo(existingProduct);
        }

        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (organization.Type != OrganizationTypeConstants.Host)
        {
            throw new InvalidOperationException("System-managed Host Location Products require a Host organization.");
        }

        var tags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            [productTagId],
            organizationId,
            null,
            cancellationToken);
        var organizationTags = tags.Where(item => item.Id == productTagId).ToList();
        ValidateHostLocationProductTag(organization, organizationTags);

        var productVersion = new ProductVersion
        {
            Id = randomHelper.Generate(),
            Type = ProductType.Event,
            Currency = Currency.Nzd,
            ListingMetadata = new ListingMetadata($"Complete the listing details for {locationName}.", locationName, "", []),
            PricingOptions = [],
        };

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        var productEntity = entityMapper.MapTo(new Product
        {
            Id = id,
            Inactive = true,
        }, organization);
        var productVersionEntity = entityMapper.MapTo(productVersion, productEntity, organizationTags);
        productEntity.ProductVersions.Add(productVersionEntity);
        repositoryFactory.ProductVersionRepository.Add(productVersionEntity);
        var product = entityMapper.MapTo(repositoryFactory.ProductRepository.Add(productEntity));
        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedProductService.UpdateByIdAsync(product.Id, cancellationToken);

        logger.LogInformation(
            "System-managed Host Location draft Product provisioned. OrganizationId: {OrganizationId}, ProductId: {ProductId}",
            organizationId,
            product.Id);
        return product;
    }

    public async Task RemoveHostLocationDraftAsync(string id, string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        if (!HostLocationSystemIds.IsProduct(id))
        {
            throw new InvalidOperationException("Only system-managed Host Location Products can be removed through this operation.");
        }

        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct is null)
        {
            return;
        }

        if (existingProduct.Organization.Id != organizationId || existingProduct.Organization.Type != OrganizationTypeConstants.Host)
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);
        repositoryFactory.ProductRepository.RemoveRange([existingProduct]);
        var deletedProduct = entityMapper.MapTo(existingProduct);
        marketplaceOutboxPublisher.PublishProducts([deletedProduct], repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedProductService.RemoveByIdAsync(id, cancellationToken);
        logger.LogInformation(
            "System-managed Host Location draft Product removed. OrganizationId: {OrganizationId}, ProductId: {ProductId}",
            organizationId,
            id);
    }

    public async Task<Product> AddAsync(
        string? id,
        string? organizationId,
        string? organizationCustomDomain,
        ProductVersion productVersion,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(id))
        {
            var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(id, cancellationToken);
            if (existingProduct is not null)
            {
                if (!string.IsNullOrWhiteSpace(organizationId))
                {
                    if (existingProduct.Organization.Id != organizationId)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
                {
                    if (existingProduct.Organization.CustomDomain != organizationCustomDomain)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Either organizationId or organizationCustomDomain must be provided.");
                }

                return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
            }
        }
        else
        {
            id = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationId,
                                       organizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (existingOrganization.Type == OrganizationTypeConstants.Host && !HostLocationSystemIds.IsProduct(id))
        {
            throw new InvalidOperationException(
                "Host Products are provisioned automatically when a Host Location is created.");
        }

        ApplyHostDefaults(existingOrganization, productVersion);
        Validate(productVersion.Type, productVersion.PricingOptions, existingOrganization.Type == OrganizationTypeConstants.Host);
        EnsureSpacesOperationalAccess(existingOrganization, false);
        if (!await organizationAuthorizationService.CanModifyProductAsync(existingOrganization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var tagIds = productVersion.OrganizationTags.Select(item => item.Id).ToList();
        var tags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            tagIds,
            organizationId,
            organizationCustomDomain,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationTags = tags.Where(item => tagIds.Contains(item.Id)).ToList();
        ValidateHostLocationProductTag(existingOrganization, organizationTags);
        var productEntity = entityMapper.MapTo(new Product
        {
            Id = id,
            Inactive = true,
        }, existingOrganization);

        var productVersionEntity = entityMapper.MapTo(productVersion, productEntity, organizationTags);
        productEntity.ProductVersions.Add(productVersionEntity);
        repositoryFactory.ProductVersionRepository.Add(productVersionEntity);

        var product = entityMapper.MapTo(repositoryFactory.ProductRepository.Add(productEntity));

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedProductService.UpdateByIdAsync(product.Id, cancellationToken);

        return product;
    }

    public async Task<Product> UpdateAsync(ProductPatchRequest request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        var editUnits = string.Join(",", request.FieldsToUpdate);
        logger.LogInformation(
            "Product patch autosave started. ProductId: {ProductId}, EditUnits: {EditUnits}",
            request.Id,
            editUnits);

        try
        {
            var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
            var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(request.Id, cancellationToken) ??
                                  throw new ProductNotFound();
            var currentVersion = entityMapper.MapTo(existingProduct).ProductVersions.OrderByDescending(item => item.CreatedAt).First();

            Apply(request, currentVersion);
            var updatedProduct = await UpdateInternalAsync(currentVersion, existingProduct, customer, cancellationToken);
            logger.LogInformation(
                "Product patch autosave completed. ProductId: {ProductId}, EditUnits: {EditUnits}",
                updatedProduct.Id,
                editUnits);
            return updatedProduct;
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(
                exception,
                "Product patch autosave rejected by authorization. ProductId: {ProductId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Product patch autosave failed. ProductId: {ProductId}, EditUnits: {EditUnits}",
                request.Id,
                editUnits);
            throw;
        }
    }

    public async Task<IReadOnlyList<Product>> DeleteAsync(IReadOnlyList<string> productIds, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProducts = await repositoryFactory.ProductRepository.GetByIdsAsync(productIds, cancellationToken) ?? throw new ProductNotFound();
        foreach (var existingProduct in existingProducts)
        {
            if (!await organizationAuthorizationService.CanModifyProductAsync(existingProduct.Organization.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ProductRepository.RemoveRange(existingProducts);
        var deletedProducts = existingProducts.Select(entityMapper.MapTo).ToList();

        marketplaceOutboxPublisher.PublishProducts(deletedProducts, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var deletedProduct in deletedProducts)
        {
            await cachedProductService.RemoveByIdAsync(deletedProduct.Id, cancellationToken);
        }

        return deletedProducts;
    }

    public async Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existingProduct = await cachedProductService.GetByIdAsync(id, cancellationToken);
        if (existingProduct?.Organization is
            {
                Type: OrganizationTypeConstants.Host,
                IsOwnershipVerified: not true,
            })
        {
            return null;
        }

        return existingProduct is null ? null : entityMapper.MapTo(existingProduct);
    }

    public async Task<IReadOnlyList<Product>> ActivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var products = await repositoryFactory.ProductRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = products.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
            organizationIds,
            null,
            cancellationToken);
        foreach (var item in existingOrganizations)
        {
            EnsureSpacesOperationalAccess(item, true);
            if (!await organizationAuthorizationService.CanModifyProductAsync(item.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        foreach (var product in products.Where(product => product.Organization.Type == OrganizationTypeConstants.Host))
        {
            var currentVersion = product.ProductVersions.OrderByDescending(version => version.CreatedAt).First();
            if (currentVersion.PricingOptions.Count == 0 || string.IsNullOrWhiteSpace(currentVersion.ListingMetadata?.Title))
            {
                throw new InvalidOperationException(
                    "Complete the Host Product listing and add at least one pricing option before activation.");
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var product in products)
        {
            product.Inactive = false;
            repositoryFactory.ProductRepository.Update(product);
        }

        var updatedProducts = products
            .Select(product =>
                entityMapper.MapTo(product, entityMapper.MapTo(existingOrganizations.Single(item => item.Id == product.Organization.Id))))
            .ToList();

        marketplaceOutboxPublisher.PublishProducts(updatedProducts, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var updatedProduct in updatedProducts)
        {
            await cachedProductService.UpdateByIdAsync(updatedProduct.Id, cancellationToken);
        }

        return updatedProducts;
    }

    public async Task<IReadOnlyList<Product>> DeactivateAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var products = await repositoryFactory.ProductRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = products.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
            organizationIds,
            null,
            cancellationToken);
        foreach (var item in existingOrganizations)
        {
            if (!await organizationAuthorizationService.CanModifyProductAsync(item.Id, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var product in products)
        {
            product.Inactive = true;
            repositoryFactory.ProductRepository.Update(product);
        }

        var updatedProducts = products
            .Select(product =>
                entityMapper.MapTo(product, entityMapper.MapTo(existingOrganizations.Single(item => item.Id == product.Organization.Id))))
            .ToList();

        marketplaceOutboxPublisher.PublishProducts(updatedProducts, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var updatedProduct in updatedProducts)
        {
            await cachedProductService.UpdateByIdAsync(updatedProduct.Id, cancellationToken);
        }

        return updatedProducts;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        IReadOnlyList<ProductOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        if (searchCriteria.IncludeInactive)
        {
            var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
            var organizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
                searchCriteria.OrganizationIds,
                searchCriteria.OrganizationCustomDomains,
                cancellationToken);
            if (organizations.Count == 0)
            {
                throw new UnauthorizedAccessException();
            }

            foreach (var organization in organizations)
            {
                if (!await organizationAuthorizationService.CanModifyProductAsync(organization.Id, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }
            }

            searchCriteria = searchCriteria with
            {
                IncludeUnverifiedHost = true,
            };
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.ProductRepository.GetPaginatedProductsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(item => new Edge<Product>(entityMapper.MapTo(item.Node), item.Cursor)).ToList(), totalCount);
    }

    private async Task<Product> UpdateInternalAsync(
        ProductVersion productVersion,
        Shared.Database.Entities.Product existingProduct,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        ApplyHostDefaults(existingProduct.Organization, productVersion);
        Validate(productVersion.Type, productVersion.PricingOptions, existingProduct.Organization.Type == OrganizationTypeConstants.Host);
        EnsureSpacesOperationalAccess(existingProduct.Organization, false);
        if (customer is not null &&
            !await organizationAuthorizationService.CanModifyProductAsync(existingProduct.Organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var tagIds = productVersion.OrganizationTags.Select(item => item.Id).ToList();
        var tags = await repositoryFactory.OrganizationTagRepository.GetActiveByIdsForOrganizationAsync(
            tagIds,
            existingProduct.Organization.Id,
            null,
            cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationTags = tags.Where(item => tagIds.Contains(item.Id)).ToList();
        ValidateHostLocationProductTag(existingProduct.Organization, organizationTags);

        var productVersionEntity =
            repositoryFactory.ProductVersionRepository.Add(entityMapper.MapTo(productVersion, existingProduct, organizationTags));
        existingProduct = entityMapper.MergeTo(existingProduct, existingProduct.Organization);

        var product = entityMapper.MapTo(existingProduct);
        product.ProductVersions = [entityMapper.MapTo(productVersionEntity, existingProduct)];

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedProductService.UpdateByIdAsync(product.Id, cancellationToken);

        return product;
    }

    private void EnsureSpacesOperationalAccess(Organization organization, bool publishing)
    {
        if (organization.Type == OrganizationTypeConstants.Host)
        {
            if (publishing && organization.IsOwnershipVerified != true)
            {
                logger.LogWarning(
                    "Host product publication denied because ownership is not verified. OrganizationId: {OrganizationId}",
                    organization.Id);
                throw new UnauthorizedAccessException("Host organization must be verified before publishing products.");
            }

            return;
        }

        if (organization.Type != OrganizationTypeConstants.Marketplace)
        {
            return;
        }

        var decision = spacesAccessEvaluator.Evaluate(
            timeProvider.GetUtcNow(),
            organization.Offering,
            SpacesAccessAction.CreateOrModify);
        if (!decision.Allowed)
        {
            logger.LogWarning(
                "Spaces product mutation denied for organization {OrganizationId}. Status: {Status}, ReasonCode: {ReasonCode}",
                organization.Id,
                decision.Status,
                decision.ReasonCode);
            throw new SpacesAccessDenied(decision);
        }
    }

    private static void ApplyHostDefaults(Organization organization, ProductVersion productVersion)
    {
        if (organization.Type == OrganizationTypeConstants.Host)
        {
            productVersion.Type = ProductType.Event;
        }
    }

    private static void ValidateHostLocationProductTag(Organization organization, IReadOnlyCollection<OrganizationTag> organizationTags)
    {
        if (organization.Type != OrganizationTypeConstants.Host)
        {
            return;
        }

        if (organizationTags.Count != 1 ||
            !HostLocationSystemIds.IsProductTag(organizationTags.Single().Id))
        {
            throw new InvalidOperationException(
                "Host Product requires exactly one system-managed Product tag provisioned for its Location.");
        }
    }

    private static void Validate(ProductType productType, IReadOnlyList<ProductPricing> options, bool allowHostRecurringEvent)
    {
        foreach (var option in options)
        {
            Validate(productType, option, allowHostRecurringEvent);
        }
    }

    private static void Apply(ProductPatchRequest request, ProductVersion productVersion)
    {
        foreach (var field in request.FieldsToUpdate)
        {
            switch (field)
            {
                case ProductPatchField.Type:
                    productVersion.Type = request.ProductVersion.Type;
                    break;
                case ProductPatchField.Currency:
                    productVersion.Currency = request.ProductVersion.Currency;
                    break;
                case ProductPatchField.Tags:
                    productVersion.OrganizationTags = request.ProductVersion.OrganizationTags;
                    break;
                case ProductPatchField.FeatureImages:
                    productVersion.FeatureImages = request.ProductVersion.FeatureImages;
                    break;
                case ProductPatchField.PricingOptions:
                    productVersion.PricingOptions = request.ProductVersion.PricingOptions;
                    break;
                case ProductPatchField.ListingMetadata:
                    productVersion.ListingMetadata = request.ProductVersion.ListingMetadata;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(request.FieldsToUpdate), field,
                        $"Unexpected value for {nameof(request.FieldsToUpdate)}: {field}. Update enum mapping or caller input.");
            }
        }
    }

    private static (int DurationStepMinutes, string DurationStepLabel) GetDurationStepDetails(ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Per15Minutes => (15, "15-minute"),
            ProductPricingCadence.Per30Minutes => (30, "30-minute"),
            ProductPricingCadence.PerHour => (60, "60-minute"),
            _ => (OpeningHoursDetails.BookingSlotSizeInMinutes, $"{OpeningHoursDetails.BookingSlotSizeInMinutes}-minute"),
        };

    public static void Validate(ProductType productType, ProductPricing pricing, bool allowHostRecurringEvent)
    {
        if (pricing.AvailableDays is not null && pricing.AvailableDays.Distinct().Count() != pricing.AvailableDays.Count)
        {
            throw new ProductPricingAvailableDaysInvalid();
        }

        if (pricing.RequiredDaysPerWeek is not null)
        {
            if (pricing.PurchaseCadence != ProductPricingCadence.Weekly)
            {
                throw new ProductPricingWeeklyDaySelectionOnlySupportedForWeeklyPricing();
            }

            var availableDayCount = pricing.AvailableDays is { Count: > 0 } ? pricing.AvailableDays.Count : 7;
            if (pricing.RequiredDaysPerWeek <= 0 ||
                pricing.RequiredDaysPerWeek > 7 ||
                pricing.RequiredDaysPerWeek > availableDayCount)
            {
                throw new ProductPricingWeeklyDaySelectionRangeInvalid();
            }
        }

        if (!allowHostRecurringEvent && productType == ProductType.Event && IsSubscriptionCadence(pricing.PurchaseCadence))
        {
            throw new ProductPricingEventRequiresExplicitTimeBooking();
        }

        if (productType == ProductType.Event && pricing.SupportsSubscriptionAutoRenewal)
        {
            throw new ProductPricingEventAutoRenewalNotSupported();
        }

        if (pricing.AcceptedPaymentMethods.Count <= 0)
        {
            throw new ProductPricingAcceptedPaymentMethodsRequired();
        }

        if (pricing.BillingMode == ProductPricingBillingMode.NotSet)
        {
            throw new ProductPricingBillingModeRequired();
        }

        ValidateCancellationPolicy(pricing);

        var (durationStepMinutes, durationStepLabel) = GetDurationStepDetails(pricing.BookingCadence);

        if (pricing.MinDurationMinutes is not null && pricing.MaxDurationMinutes is not null)
        {
            if (pricing.MinDurationMinutes <= 0)
            {
                throw new ProductPricingMinDurationMustBePositive();
            }

            if (pricing.MinDurationMinutes % durationStepMinutes != 0)
            {
                throw new ProductPricingMinDurationIncrementInvalid(durationStepLabel);
            }

            if (pricing.MaxDurationMinutes <= 0)
            {
                throw new ProductPricingMaxDurationMustBePositive();
            }

            if (pricing.MaxDurationMinutes % durationStepMinutes != 0)
            {
                throw new ProductPricingMaxDurationIncrementInvalid(durationStepLabel);
            }

            if (pricing.MaxDurationMinutes < pricing.MinDurationMinutes)
            {
                throw new ProductPricingMaxDurationMustNotBeLessThanMinDuration();
            }
        }
        else if (pricing.MinDurationMinutes is not null && pricing.MaxDurationMinutes is null)
        {
            if (pricing.MinDurationMinutes <= 0)
            {
                throw new ProductPricingMinDurationMustBePositive();
            }

            if (pricing.MinDurationMinutes % durationStepMinutes != 0)
            {
                throw new ProductPricingMinDurationIncrementInvalid(durationStepLabel);
            }
        }
        else if (pricing.MinDurationMinutes is null && pricing.MaxDurationMinutes is not null)
        {
            if (pricing.MaxDurationMinutes <= 0)
            {
                throw new ProductPricingMaxDurationMustBePositive();
            }

            if (pricing.MaxDurationMinutes % durationStepMinutes != 0)
            {
                throw new ProductPricingMaxDurationIncrementInvalid(durationStepLabel);
            }
        }
    }

    private static bool IsSubscriptionCadence(ProductPricingCadence cadence) =>
        cadence is ProductPricingCadence.Daily or
            ProductPricingCadence.Weekly or
            ProductPricingCadence.Fortnightly or
            ProductPricingCadence.Monthly or
            ProductPricingCadence.TwoMonths or
            ProductPricingCadence.Quarterly or
            ProductPricingCadence.FourMonths or
            ProductPricingCadence.FiveMonths or
            ProductPricingCadence.SixMonths or
            ProductPricingCadence.Yearly;

    /// <summary>
    ///     Validates that a pricing option cancellation policy is internally consistent.
    ///     We store the richer policy structure directly on the pricing option so future refund
    ///     workflows can rely on the same source of truth instead of reinterpreting ad-hoc fields.
    /// </summary>
    private static void ValidateCancellationPolicy(ProductPricing pricing)
    {
        switch (pricing.CancellationPolicyType)
        {
            case ProductPricingCancellationPolicyType.NoCancellation:
                if (pricing.CancellationRefundRules.Count != 0)
                {
                    throw new ProductPricingCancellationPolicyInvalid();
                }

                return;

            case ProductPricingCancellationPolicyType.FullRefundBeforeCutoff:
                {
                    if (pricing.CancellationRefundRules.Count == 0)
                    {
                        return;
                    }

                    if (pricing.CancellationRefundRules.Count != 1)
                    {
                        throw new ProductPricingCancellationPolicyInvalid();
                    }

                    var rule = pricing.CancellationRefundRules.Single();
                    if (rule.MinutesBefore < 0 || rule.RefundPercentage != 100)
                    {
                        throw new ProductPricingCancellationPolicyInvalid();
                    }

                    return;
                }

            case ProductPricingCancellationPolicyType.TieredRefund:
                {
                    if (pricing.CancellationRefundRules.Count == 0)
                    {
                        throw new ProductPricingCancellationPolicyInvalid();
                    }

                    var orderedRules = pricing.CancellationRefundRules.OrderByDescending(item => item.MinutesBefore).ToList();
                    if (orderedRules.Any(item => item.MinutesBefore < 0 || item.RefundPercentage is < 0 or > 100))
                    {
                        throw new ProductPricingCancellationPolicyInvalid();
                    }

                    if (orderedRules.Select(item => item.MinutesBefore).Distinct().Count() != orderedRules.Count)
                    {
                        throw new ProductPricingCancellationPolicyInvalid();
                    }

                    return;
                }

            default:
                throw new ProductPricingCancellationPolicyInvalid();
        }
    }
}
