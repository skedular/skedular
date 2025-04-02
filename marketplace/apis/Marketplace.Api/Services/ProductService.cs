using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Marketplace.Api.Mappers;
using Marketplace.Api.Services.Authorization;
using Marketplace.Shared.Models;
using Marketplace.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using OrganizationTag = Marketplace.Shared.Database.Entities.OrganizationTag;

namespace Marketplace.Api.Services;

public interface IProductService
{
    Task<Product> AddAsync(string? productId, string organizationId, ProductVersion productVersion, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(string productId, ProductVersion productVersion, CancellationToken cancellationToken);
    Task<Product> DeleteAsync(string productId, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken);
}

public class ProductService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
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
                    throw new Unauthorized();
                }

                return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
            }
        }
        else
        {
            productId = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModifyProduct(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

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

        var productEntity = mapper.MapTo(new Product { Id = productId }, existingOrganization);

        productEntity.ProductVersions.Add(
            mapper.MapTo(
                productVersion,
                productEntity,
                organizationTags.Where(item => productTagIds.Contains(item.Id)).ToList(),
                organizationTags.Where(item => locationTagIds.Contains(item.Id)).ToList()));
        var product = mapper.MapTo(repositoryFactory.ProductRepository.Add(productEntity));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return product;
    }

    public async Task<Product> UpdateAsync(string productId, ProductVersion productVersion, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        Validate(productVersion);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
        if (existingProduct is null)
        {
            throw new ProductNotFound();
        }

        return await UpdateInternalAsync(productVersion, existingProduct, customer, cancellationToken);
    }

    public async Task<Product> DeleteAsync(string productId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
        if (existingProduct is null)
        {
            throw new ProductNotFound();
        }

        if (!organizationAuthorizationService.CanModifyProduct(existingProduct.Organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedProduct = mapper.MapTo(repositoryFactory.ProductRepository.Remove(existingProduct));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedProduct;
    }

    public async Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);

        var existingProduct = await repositoryFactory.ProductRepository.GetByIdAsync(productId, cancellationToken);
        if (existingProduct is null)
        {
            return null;
        }

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        if (!organizationAuthorizationService.CanViewProducts(existingProduct.Organization, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(existingProduct);
    }

    private async Task<Product> UpdateInternalAsync(
        ProductVersion productVersion,
        Shared.Database.Entities.Product existingProduct,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        if (customer is not null && !organizationAuthorizationService.CanModifyProduct(existingProduct.Organization, customer))
        {
            throw new Unauthorized();
        }

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

        _ = repositoryFactory.ProductVersionRepository.Add(
            mapper.MapTo(
                productVersion,
                existingProduct,
                organizationTags.Where(item => productTagIds.Contains(item.Id)).ToList(),
                organizationTags.Where(item => locationTagIds.Contains(item.Id)).ToList()));

        var product = mapper.MapTo(repositoryFactory.ProductRepository.Update(existingProduct));

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return product;
    }

    private static void Validate(ProductVersion productVersion)
    {
        if (productVersion.RecurrenceIntervalDays <= 0)
        {
            throw new ArgumentException("RecurrenceIntervalDays must be greater than 0", nameof(productVersion.RecurrenceIntervalDays));
        }

        if (productVersion is { ForceContinuousSlots: true, MaxSpreadDays: not null })
        {
            throw new ArgumentException("MaxSpreadDays can't be set if ForceContinuousSlots is set", nameof(productVersion.MaxSpreadDays));
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
