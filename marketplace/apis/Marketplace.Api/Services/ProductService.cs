using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Models;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Marketplace.Shared.Services.Cache;
using Microsoft.EntityFrameworkCore;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Services;

public interface IProductService
{
    Task<Product> AddAsync(
        string? id,
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        ProductVersion productVersion,
        CancellationToken cancellationToken);

    Task<Product> UpdateAsync(string id, ProductVersion productVersion, CancellationToken cancellationToken);
    Task<ICollection<Product>> DeleteAsync(ICollection<string> productIds, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<ICollection<Product>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Product>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class ProductService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMarketplaceOutboxPublisher marketplaceOutboxPublisher,
    IMapper mapper,
    ICachedProductService cachedProductService) : IProductService
{
    public async Task<Product> AddAsync(
        string? id,
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        ProductVersion productVersion,
        CancellationToken cancellationToken)
    {
        Validate(productVersion.PricingOptions);

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
                else if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
                {
                    if (existingProduct.Organization.UniqueAlphanumericName != organizationUniqueAlphanumericName)
                    {
                        throw new UnauthorizedAccessException();
                    }
                }
                else
                {
                    throw new InvalidOperationException("Either organizationId or organizationUniqueAlphanumericName must be provided.");
                }

                return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
            }
        }
        else
        {
            id = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                                       organizationId,
                                       organizationUniqueAlphanumericName,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyProductAsync(existingOrganization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var tagIds = productVersion.OrganizationTags.Select(item => item.Id).ToList();
        var tags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    tagIds.Contains(query.Id) &&
                                    (query.Organization.Id == organizationId || (query.Organization.UniqueAlphanumericName != null &&
                                                                                 query.Organization.UniqueAlphanumericName ==
                                                                                 organizationUniqueAlphanumericName)) &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationTags = tags.Where(item => tagIds.Contains(item.Id)).ToList();
        var productEntity = mapper.MapTo(new Product { Id = id, Inactive = true }, existingOrganization);

        var productVersionEntity = mapper.MapTo(productVersion, productEntity, organizationTags);
        productEntity.ProductVersions.Add(productVersionEntity);
        repositoryFactory.ProductVersionRepository.Add(productVersionEntity);

        var product = mapper.MapTo(repositoryFactory.ProductRepository.Add(productEntity));

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedProductService.UpdateByIdAsync(product.Id, cancellationToken);

        return product;
    }

    public async Task<Product> UpdateAsync(string id, ProductVersion productVersion, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Validate(productVersion.PricingOptions);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(id, cancellationToken) ?? throw new ProductNotFound();

        return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
    }

    public async Task<ICollection<Product>> DeleteAsync(ICollection<string> productIds, CancellationToken cancellationToken)
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
        var deletedProducts = existingProducts.Select(mapper.MapTo).ToList();

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
        return existingProduct is null ? null : mapper.MapTo(existingProduct);
    }

    public async Task<ICollection<Product>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var products = await repositoryFactory.ProductRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = products.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
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
            product.Inactive = false;
            repositoryFactory.ProductRepository.Update(product);
        }

        var updatedProducts = products
            .Select(product => mapper.MapTo(product, mapper.MapTo(existingOrganizations.Single(item => item.Id == product.Organization.Id))))
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

    public async Task<ICollection<Product>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var products = await repositoryFactory.ProductRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = products.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrUniqueAlphanumericNamesAsync(
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
            .Select(product => mapper.MapTo(product, mapper.MapTo(existingOrganizations.Single(item => item.Id == product.Organization.Id))))
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

    public async Task<(PaginatedInfo, ICollection<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await repositoryFactory.ProductRepository.GetPaginatedProductsUntrackedAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo, edges.Select(item => new Edge<Product>(mapper.MapTo(item.Node), item.Cursor)).ToList(), totalCount);
    }

    private async Task<Product> UpdateInternalAsync(
        ProductVersion productVersion,
        Shared.Database.Entities.Product existingProduct,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null &&
            !await organizationAuthorizationService.CanModifyProductAsync(existingProduct.Organization.Id, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var tagIds = productVersion.OrganizationTags.Select(item => item.Id).ToList();
        var existingProductRef = existingProduct;
        var tags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    tagIds.Contains(query.Id) &&
                                    query.Organization.Id == existingProductRef.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationTags = tags.Where(item => tagIds.Contains(item.Id)).ToList();

        _ = repositoryFactory.ProductVersionRepository.Add(mapper.MapTo(productVersion, existingProduct, organizationTags));
        existingProduct = mapper.MergeTo(existingProduct, existingProduct.Organization);

        var product = mapper.MapTo(repositoryFactory.ProductRepository.Update(existingProduct));

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedProductService.UpdateByIdAsync(product.Id, cancellationToken);

        return product;
    }

    private static void Validate(ICollection<ProductPricing> options)
    {
        foreach (var option in options)
        {
            Validate(option);
        }
    }

    private static (int DurationStepMinutes, string DurationStepLabel) GetDurationStepDetails(ProductPricingCadence cadence) =>
        cadence switch
        {
            ProductPricingCadence.Per15Minutes => (15, "15-minute"),
            ProductPricingCadence.Per30Minutes => (30, "30-minute"),
            ProductPricingCadence.PerHour => (60, "60-minute"),
            _ => (OpeningHoursDetails.BookingSlotSizeInMinutes, $"{OpeningHoursDetails.BookingSlotSizeInMinutes}-minute")
        };

    private static bool HasInvalidAcceptedBillingScheduleCombination(ICollection<ProductPricingBillingSchedule> acceptedBillingSchedules) =>
        acceptedBillingSchedules.Any(item =>
            item.Mode == ProductPricingBillingMode.Upfront &&
            (item.Interval == ProductPricingBillingInterval.Weekly ||
             item.Interval == ProductPricingBillingInterval.Fortnightly ||
             item.Interval == ProductPricingBillingInterval.Monthly));

    private static void Validate(ProductPricing pricing)
    {
        if (pricing.AcceptedPaymentMethods.Count <= 0)
        {
            throw new ArgumentException("At least one accepted booking payment method must be selected", nameof(pricing.AcceptedPaymentMethods));
        }

        var acceptedBillingSchedules = pricing.AcceptedBillingSchedules;
        if (acceptedBillingSchedules.Count <= 0)
        {
            throw new ProductPricingAcceptedBillingSchedulesRequired();
        }

        if (acceptedBillingSchedules.Any(item =>
                item.Mode == ProductPricingBillingMode.NotSet || item.Interval == ProductPricingBillingInterval.NotSet))
        {
            throw new ProductPricingAcceptedBillingSchedulesCannotContainNotSet();
        }

        if (acceptedBillingSchedules.Distinct().Count() != acceptedBillingSchedules.Count)
        {
            throw new ProductPricingAcceptedBillingSchedulesCannotContainDuplicates();
        }

        if (HasInvalidAcceptedBillingScheduleCombination(acceptedBillingSchedules))
        {
            throw new ProductPricingAcceptedBillingSchedulesContainInvalidCombination();
        }

        var (durationStepMinutes, durationStepLabel) = GetDurationStepDetails(pricing.Cadence);

        if (pricing.MinDurationMinutes is not null && pricing.MaxDurationMinutes is not null)
        {
            if (pricing.MinDurationMinutes <= 0)
            {
                throw new ArgumentException("MinDurationMinutes must be greater than 0", nameof(pricing.MinDurationMinutes));
            }

            if (pricing.MinDurationMinutes % durationStepMinutes != 0)
            {
                throw new ArgumentException($"MinDurationMinutes must be in {durationStepLabel} increments", nameof(pricing.MinDurationMinutes));
            }

            if (pricing.MaxDurationMinutes <= 0)
            {
                throw new ArgumentException("MaxDurationMinutes must be greater than 0", nameof(pricing.MaxDurationMinutes));
            }

            if (pricing.MaxDurationMinutes % durationStepMinutes != 0)
            {
                throw new ArgumentException($"MaxDurationMinutes must be in {durationStepLabel} increments", nameof(pricing.MaxDurationMinutes));
            }

            if (pricing.MaxDurationMinutes < pricing.MinDurationMinutes)
            {
                throw new ArgumentException(
                    "MaxDurationMinutes must be greater or equal than productVersion.MinDurationMinutes",
                    nameof(pricing.MaxDurationMinutes));
            }
        }
        else if (pricing.MinDurationMinutes is not null && pricing.MaxDurationMinutes is null)
        {
            if (pricing.MinDurationMinutes <= 0)
            {
                throw new ArgumentException("MinDurationMinutes must be greater than 0", nameof(pricing.MinDurationMinutes));
            }

            if (pricing.MinDurationMinutes % durationStepMinutes != 0)
            {
                throw new ArgumentException($"MinDurationMinutes must be in {durationStepLabel} increments", nameof(pricing.MinDurationMinutes));
            }
        }
        else if (pricing.MinDurationMinutes is null && pricing.MaxDurationMinutes is not null)
        {
            if (pricing.MaxDurationMinutes <= 0)
            {
                throw new ArgumentException("MaxDurationMinutes must be greater than 0", nameof(pricing.MaxDurationMinutes));
            }

            if (pricing.MaxDurationMinutes % durationStepMinutes != 0)
            {
                throw new ArgumentException($"MaxDurationMinutes must be in {durationStepLabel} increments", nameof(pricing.MaxDurationMinutes));
            }
        }
    }
}
