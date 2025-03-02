using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Models;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface ITagService
{
    Task<Tag?> GetByIdAsync(string tagId, CancellationToken cancellationToken);

    Task<Tag> AddAsync(
        Tag tag,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);

    Task<Tag> UpdateAsync(Tag tag, CancellationToken cancellationToken);
    Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken);
    Task<ICollection<Tag>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken);

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
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : ITagService
{
    public async Task<Tag?> GetByIdAsync(string tagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var tag = await repositoryFactory.TagRepository.GetByIdAsync(tagId, cancellationToken);
        if (tag is null)
        {
            throw new OrganizationTagNotFound();
        }

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(tag.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
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
        ArgumentException.ThrowIfNullOrWhiteSpace(tag.Organization.Id);

        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(tag.Id))
        {
            var existingTag = await repositoryFactory.TagRepository.GetByIdAsync(tag.Id, cancellationToken);
            if (existingTag is not null)
            {
                return await UpdateInternalAsync(tag, existingTag, customer, cancellationToken);
            }
        }
        else
        {
            tag.Id = randomHelper.Generate();
        }

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(tag.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        var tagType = tag.Type switch
        {
            OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
            OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
            _ => throw new ArgumentOutOfRangeException()
        };

        var matchingTagFound = await repositoryFactory.TagRepository
            .Query(new Specification<Shared.Database.Entities.Tag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Organization.Id == tag.Organization.Id &&
                                    query.Type == tagType &&
                                    EF.Functions.ILike(query.Name, tag.Name)
            }).AnyAsync(cancellationToken);
        if (matchingTagFound)
        {
            if (tag.Type == OrganizationTagType.Custom)
            {
                throw new CustomTagWithSameNameExist();
            }

            if (tag.Type == OrganizationTagType.Zone)
            {
                throw new ZoneWithSameNameExist();
            }

            throw new OrganizationTagWithSameNameExist();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.OrganizationRepository.UnitOfWork,
            cancellationToken);

        var tagEntity = mapper.MapTo(tag, existingOrganization);
        _ = repositoryFactory.TagRepository.Add(tagEntity);

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(existingOrganization)],
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
        var existingTag = await repositoryFactory.TagRepository.GetByIdAsync(tag.Id, cancellationToken);
        if (existingTag is null)
        {
            throw new OrganizationTagNotFound();
        }

        return await UpdateInternalAsync(tag, existingTag, customer, cancellationToken);
    }

    public async Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var tag = await repositoryFactory.TagRepository.GetByIdAsync(tagId, cancellationToken);
        if (tag is null)
        {
            throw new OrganizationTagNotFound();
        }

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(tag.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);

        var deletedTag = mapper.MapTo(repositoryFactory.TagRepository.Remove(tag));

        var mappedOrganization = mapper.MapTo(existingOrganization);
        mappedOrganization.Tags = mappedOrganization.Tags.Where(item => item.Id != tagId).ToList();

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mappedOrganization],
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedTag;
    }

    public async Task<ICollection<Tag>> DeleteAsync(ICollection<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var tags = await repositoryFactory.TagRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = tags.Select(item => item.Organization.Id).ToList();
        var existingOrganizations =
            await repositoryFactory.OrganizationRepository.GetByIdsAsync(organizationIds, cancellationToken);

        if (existingOrganizations.Any(existingOrganization =>
                !organizationAuthorizationService.CanModify(existingOrganization, customer)))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);

        repositoryFactory.TagRepository.RemoveRange(tags);
        var deletedTags = tags.Select(mapper.MapTo).ToList();

        var mappedOrganizations = existingOrganizations.Select(mapper.MapTo).ToList();
        foreach (var mappedOrganization in mappedOrganizations)
        {
            mappedOrganization.Tags = mappedOrganization.Tags.Where(item => !ids.Contains(item.Id)).ToList();
        }

        await organizationOutboxPublisher.PublishOrganizationAsync(
            mappedOrganizations,
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedTags;
    }

    public async Task<(PaginatedInfo, ICollection<Edge<Tag>>, int)>
        GetPaginatedTagsAsync(
            PaginationInputParam paginationInputParam,
            TagSearchCriteria searchCriteria,
            ICollection<TagOrder> orderByFields,
            CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(searchCriteria.OrganizationId,
                cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanView(organization, customer))
        {
            throw new Unauthorized();
        }

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.TagRepository.GetPaginatedTagsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, mapper.MapTo(edges, mapper.MapTo(organization)).ToList(), totalCount);
    }

    private async Task<Tag> UpdateInternalAsync(
        Tag tag,
        Shared.Database.Entities.Tag existingTag,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(existingTag.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (customer is not null && !organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        var tagId = tag.Id;
        var tagName = tag.Name;
        var tagType = tag.Type switch
        {
            OrganizationTagType.Custom => OrganizationTagTypeConstants.Custom,
            OrganizationTagType.Zone => OrganizationTagTypeConstants.Zone,
            _ => throw new ArgumentOutOfRangeException()
        };
        var organizationId = existingTag.Organization.Id;
        var matchingTagFound = await repositoryFactory.TagRepository
            .Query(new Specification<Shared.Database.Entities.Tag>
            {
                Criteria = query => !query.DeletedAt.HasValue &&
                                    query.Organization.Id == organizationId &&
                                    query.Type == tagType &&
                                    EF.Functions.ILike(query.Name, tagName) &&
                                    query.Id != tagId
            }).AnyAsync(cancellationToken);
        if (matchingTagFound)
        {
            if (tag.Type == OrganizationTagType.Custom)
            {
                throw new CustomTagWithSameNameExist();
            }

            if (tag.Type == OrganizationTagType.Zone)
            {
                throw new ZoneWithSameNameExist();
            }

            throw new OrganizationTagWithSameNameExist();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);

        tag =
            mapper.MapTo(
                repositoryFactory.TagRepository.Update(mapper.MergeTo(tag, existingTag, existingOrganization)));

        await organizationOutboxPublisher.PublishOrganizationAsync(
            [mapper.MapTo(existingOrganization)],
            repositoryFactory.TagRepository.UnitOfWork,
            cancellationToken);
        await repositoryFactory.TagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return tag;
    }
}
