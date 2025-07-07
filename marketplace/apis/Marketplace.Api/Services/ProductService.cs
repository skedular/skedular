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
using Microsoft.EntityFrameworkCore;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Services;

public interface IProductService
{
    Task<Product> AddAsync(string? productId, string organizationId, ProductVersion productVersion, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(string productId, ProductVersion productVersion, CancellationToken cancellationToken);
    Task<ICollection<Product>> DeleteAsync(ICollection<string> productIds, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken);
    Task<ICollection<Product>> ActivateAsync(ICollection<string> ids, CancellationToken cancellationToken);
    Task<ICollection<Product>> DeactivateAsync(ICollection<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Product>>, int )> GetPaginatedProductsAsync(
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
    IMapper mapper) : IProductService
{
    public async Task<Product> AddAsync(string? productId, string organizationId, ProductVersion productVersion, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);
        Validate(productVersion);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(productId))
        {
            var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
            if (existingProduct is not null)
            {
                if (existingProduct.OrganizationId != organizationId)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
            }
        }
        else
        {
            productId = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModifyProduct(existingOrganization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var productTagIds = productVersion.ProductTags.Select(item => item.Id).ToList();
        var locationTagIds = productVersion.LocationTags.Select(item => item.Id).ToList();
        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    productTagIds.Concat(locationTagIds).Contains(query.Id) &&
                                    query.Organization.Id == organizationId &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var productTags = organizationTags.Where(item => productTagIds.Contains(item.Id)).ToList();
        var locationTags = organizationTags.Where(item => locationTagIds.Contains(item.Id)).ToList();
        var productEntity = mapper.MapTo(
            new Product { Id = productId, Inactive = true },
            productVersion,
            existingOrganization,
            productTags,
            locationTags);

        var productVersionEntity = mapper.MapTo(productVersion, productEntity, productTags, locationTags);
        productEntity.ProductVersions.Add(productVersionEntity);
        repositoryFactory.ProductVersionRepository.Add(productVersionEntity);

        var product = mapper.MapTo(repositoryFactory.ProductRepository.Add(productEntity));

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return product;
    }

    public async Task<Product> UpdateAsync(string productId, ProductVersion productVersion, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        Validate(productVersion);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken) ?? throw new ProductNotFound();

        return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
    }

    public async Task<ICollection<Product>> DeleteAsync(ICollection<string> productIds, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProducts = await repositoryFactory.ProductRepository.GetByIdsAsync(productIds, cancellationToken) ?? throw new ProductNotFound();
        if (existingProducts.Any(existingProduct => !organizationAuthorizationService.CanModifyProduct(existingProduct.Organization, customer)))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.ProductRepository.RemoveRange(existingProducts);
        var deletedProducts = existingProducts.Select(mapper.MapTo).ToList();

        marketplaceOutboxPublisher.PublishProducts(deletedProducts, repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedProducts;
    }

    public async Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);

        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
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
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);
        if (existingOrganizations.Any(item => !organizationAuthorizationService.CanModifyProduct(item, customer)))
        {
            throw new UnauthorizedAccessException();
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
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);
        if (existingOrganizations.Any(item => !organizationAuthorizationService.CanModifyProduct(item, customer)))
        {
            throw new UnauthorizedAccessException();
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

        return updatedProducts;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Product>>, int)> GetPaginatedProductsAsync(
        PaginationInputParam paginationInputParam,
        ProductSearchCriteria searchCriteria,
        ICollection<ProductOrder> orderByFields,
        CancellationToken cancellationToken)
    {
        var (paginatedInfo, edges, totalCount) = await repositoryFactory.ProductRepository.GetPaginatedProductsAsync(
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
        if (customer is not null && !organizationAuthorizationService.CanModifyProduct(existingProduct.Organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        productVersion.Id = randomHelper.Generate();
        var productTagIds = productVersion.ProductTags.Select(item => item.Id).ToList();
        var locationTagIds = productVersion.LocationTags.Select(item => item.Id).ToList();
        var existingProductRef = existingProduct;
        var organizationTags = await repositoryFactory.OrganizationTagRepository.Query(
            new Specification<OrganizationTag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    productTagIds.Concat(locationTagIds).Contains(query.Id) &&
                                    query.Organization.Id == existingProductRef.Organization.Id &&
                                    !query.Organization.DeletedAt.HasValue
            }).ToListAsync(cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var productTags = organizationTags.Where(item => productTagIds.Contains(item.Id)).ToList();
        var locationTags = organizationTags.Where(item => locationTagIds.Contains(item.Id)).ToList();

        _ = repositoryFactory.ProductVersionRepository.Add(mapper.MapTo(productVersion, existingProduct, productTags, locationTags));
        existingProduct = mapper.MergeTo(productVersion, existingProduct, existingProduct.Organization, productTags, locationTags);

        var product = mapper.MapTo(repositoryFactory.ProductRepository.Update(existingProduct));

        marketplaceOutboxPublisher.PublishProducts([product], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return product;
    }

    private static void Validate(ProductVersion productVersion)
    {
        if (productVersion.AcceptedBookingPaymentMethods.Count <= 0)
        {
            throw new ArgumentException("At least one accepted booking payment method must be selected",
                nameof(productVersion.AcceptedBookingPaymentMethods));
        }

        if (productVersion.RecurrenceWindowDays <= 0)
        {
            throw new ArgumentException("RecurrenceWindowDays must be greater than 0", nameof(productVersion.RecurrenceWindowDays));
        }

        if (productVersion is { RequireConsecutiveDays: true, MaxBookingSpreadDays: not null })
        {
            throw new ArgumentException(
                "MaxBookingSpreadDays can't be set if ForceContinuousSlots is set",
                nameof(productVersion.MaxBookingSpreadDays));
        }

        if (productVersion.MinDurationMinutes is not null && productVersion.MaxDurationMinutes is not null)
        {
            if (productVersion.MinDurationMinutes <= 0)
            {
                throw new ArgumentException("MinDurationMinutes must be greater than 0", nameof(productVersion.MinDurationMinutes));
            }

            if (productVersion.MinDurationMinutes % OpeningHoursDetails.OpeningHoursSlotSizeInMinutes != 0)
            {
                throw new ArgumentException(
                    $"MinDurationMinutes must be must be in {OpeningHoursDetails.OpeningHoursSlotSizeInMinutes}-minute increments",
                    nameof(productVersion.MinDurationMinutes));
            }

            if (productVersion.MaxDurationMinutes <= 0)
            {
                throw new ArgumentException("MaxDurationMinutes must be greater than 0", nameof(productVersion.MaxDurationMinutes));
            }

            if (productVersion.MaxDurationMinutes % OpeningHoursDetails.OpeningHoursSlotSizeInMinutes != 0)
            {
                throw new ArgumentException(
                    $"MaxDurationMinutes must be must be in {OpeningHoursDetails.OpeningHoursSlotSizeInMinutes}-minute increments",
                    nameof(productVersion.MaxDurationMinutes));
            }

            if (productVersion.MaxDurationMinutes < productVersion.MinDurationMinutes)
            {
                throw new ArgumentException(
                    "MaxDurationMinutes must be greater or equal than productVersion.MinDurationMinutes",
                    nameof(productVersion.MaxDurationMinutes));
            }
        }
        else if (productVersion.MinDurationMinutes is not null && productVersion.MaxDurationMinutes is null)
        {
            if (productVersion.MinDurationMinutes <= 0)
            {
                throw new ArgumentException("MinDurationMinutes must be greater than 0", nameof(productVersion.MinDurationMinutes));
            }

            if (productVersion.MinDurationMinutes % OpeningHoursDetails.OpeningHoursSlotSizeInMinutes != 0)
            {
                throw new ArgumentException(
                    $"MinDurationMinutes must be must be in {OpeningHoursDetails.OpeningHoursSlotSizeInMinutes}-minute increments",
                    nameof(productVersion.MinDurationMinutes));
            }
        }
        else if (productVersion.MinDurationMinutes is null && productVersion.MaxDurationMinutes is not null)
        {
            if (productVersion.MaxDurationMinutes <= 0)
            {
                throw new ArgumentException("MaxDurationMinutes must be greater than 0", nameof(productVersion.MaxDurationMinutes));
            }

            if (productVersion.MaxDurationMinutes % OpeningHoursDetails.OpeningHoursSlotSizeInMinutes != 0)
            {
                throw new ArgumentException(
                    $"MaxDurationMinutes must be must be in {OpeningHoursDetails.OpeningHoursSlotSizeInMinutes}-minute increments",
                    nameof(productVersion.MaxDurationMinutes));
            }
        }
    }
}
