using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Location.Api.Mappers;
using Location.Api.Services.Authorization;
using Location.Shared.Models;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Location.Api.Services;

public interface ITagService
{
    Task<Tag> GetAsync(string tagId, CancellationToken cancellationToken);

    Task<Tag> AddAsync(
        Tag tag,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken);

    Task<(PaginatedInfo, ICollection<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        ICollection<TagOrder> orderByFields,
        CancellationToken cancellationToken);
}

public class TagService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    ILocationAuthorizationService locationAuthorizationService,
    IOrganizationOfferingService organizationOfferingService,
    IMapper mapper,
    ILocationOutboxPublisher locationOutboxPublisher) : ITagService
{
    public async Task<Tag> GetAsync(string tagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var tag =
            await repositoryFactory.TagRepository.GetByIdAsync(tagId, cancellationToken);
        if (tag is null)
        {
            throw new LocationTagNotFound();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(tag.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        return mapper.MapTo(tag);
    }

    public async Task<Tag> AddAsync(
        Tag tag,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag.Location.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(tag.Id))
        {
            var existingTag =
                await repositoryFactory.TagRepository.GetByIdAsync(tag.Id, cancellationToken);
            if (existingTag is not null)
            {
                return await UpdateInternalAsync(
                    tag,
                    existingTag,
                    customer,
                    cancellationToken);
            }
        }
        else
        {
            tag.Id = randomHelper.Generate();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(tag.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var matchingTagFound = await repositoryFactory.TagRepository
            .Query(new Specification<Shared.Database.Entities.Tag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Location.Id == tag.Location.Id &&
                                    EF.Functions.ILike(query.Name, tag.Name)
            }).AnyAsync(cancellationToken);
        if (matchingTagFound)
        {
            throw new LocationTagWithSameNameExist();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.LocationRepository.UnitOfWork,
                cancellationToken);

        var tagEntity = mapper.MapTo(tag, existingLocation);
        _ = repositoryFactory.TagRepository.Add(tagEntity);

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return tag;
    }

    public async Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tag.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTag =
            await repositoryFactory.TagRepository.GetByIdAsync(tag.Id, cancellationToken);
        if (existingTag is null)
        {
            throw new LocationTagNotFound();
        }

        return await UpdateInternalAsync(tag, existingTag, customer, cancellationToken);
    }

    public async Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var tag =
            await repositoryFactory.TagRepository.GetByIdAsync(tagId, cancellationToken);
        if (tag is null)
        {
            throw new LocationTagNotFound();
        }

        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(tag.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (!locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TagRepository.UnitOfWork,
                cancellationToken);

        var deletedTag = mapper.MapTo(repositoryFactory.TagRepository.Remove(tag));

        var mappedLocation = mapper.MapTo(existingLocation);
        mappedLocation.Tags = mappedLocation.Tags.Where(item => item.Id != tagId).ToList();

        await locationOutboxPublisher.PublishLocationAsync(
            [mappedLocation],
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedTag;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Tag>>, int)>
        GetPaginatedTagsAsync(
            PaginationInputParam paginationInputParam,
            TagSearchCriteria searchCriteria,
            ICollection<TagOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location =
            await repositoryFactory.LocationRepository.GetByIdAsync(searchCriteria.LocationId,
                cancellationToken);
        if (location is null)
        {
            throw new LocationNotFound();
        }

        if (!locationAuthorizationService.CanView(location, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.TagRepository.GetPaginatedTagsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(location)).ToList(), totalCount);
    }

    private async Task<Tag> UpdateInternalAsync(
        Tag tag,
        Shared.Database.Entities.Tag existingTag,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingLocation =
            await repositoryFactory.LocationRepository.GetByIdAsync(existingTag.Location.Id, cancellationToken);
        if (existingLocation is null)
        {
            throw new LocationNotFound();
        }

        if (customer is not null && existingLocation.Organization is not null &&
            !organizationOfferingService.IsMoreInteractionAllowed(existingLocation.Organization, customer))
        {
            throw new NoMoreInteractionAllowed();
        }

        if (customer is not null && !locationAuthorizationService.CanModify(existingLocation, customer))
        {
            throw new Unauthorized();
        }

        var tagId = tag.Id;
        var tagName = tag.Name;
        var locationId = existingTag.Location.Id;
        var matchingDeskFound = await repositoryFactory.TagRepository
            .Query(new Specification<Shared.Database.Entities.Tag>
            {
                Criteria = query =>
                    !query.DeletedAt.HasValue &&
                    query.Location.Id == locationId &&
                    EF.Functions.ILike(query.Name, tagName) &&
                    query.Id != tagId
            }).AnyAsync(cancellationToken);
        if (matchingDeskFound)
        {
            throw new LocationTagWithSameNameExist();
        }

        await using var transaction =
            await transactionBuilder.BeginTransactionAsync(repositoryFactory.TagRepository.UnitOfWork,
                cancellationToken);

        tag =
            mapper.MapTo(
                repositoryFactory.TagRepository.Update(mapper.MergeTo(tag, existingTag, existingLocation)));

        await locationOutboxPublisher.PublishLocationAsync(
            [mapper.MapTo(existingLocation)],
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return tag;
    }
}
