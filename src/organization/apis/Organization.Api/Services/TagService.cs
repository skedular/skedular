using Api.Shared.Services;
using Api.Shared.Services.Models;
using Enterprise.Shared.Database;
using Enterprise.Shared.Pagination;
using Enterprise.Shared.Random;
using HotChocolate.Types.Pagination;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services;

public interface ITagService
{
    Task<Tag> GetByIdAsync(string tagId, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Tag> AddAsync(Tag tag, bool ignoreAuthorizationCheck, CancellationToken cancellationToken);
    Task<Tag> UpdatePatchAsync(OrganizationTagPatchRequest request, CancellationToken cancellationToken);
    Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Tag>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        IReadOnlyList<TagOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class TagService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICachedCustomerService cachedCustomerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ICachedTagService cachedTagService,
    ICachedOrganizationService cachedOrganizationService) : ITagService
{
    public async Task<Tag> GetByIdAsync(string tagId, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);


        var tag = await cachedTagService.GetByIdAsync(tagId, cancellationToken) ?? throw new OrganizationTagNotFound();
        var existingOrganization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(
                                       tag.Organization.Id,
                                       null,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!ignoreAuthorizationCheck)
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            if (!await organizationAuthorizationService.CanViewAsync(existingOrganization, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        return graphQlMapper.MapTo(tag);
    }

    public async Task<Tag> AddAsync(Tag tag, bool ignoreAuthorizationCheck, CancellationToken cancellationToken)
    {
        Customer? customer = null;
        if (!ignoreAuthorizationCheck)
        {
            (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(tag.Id) && HostLocationSystemIds.IsProductTag(tag.Id))
            {
                throw new UnauthorizedAccessException("Host Location Product tags are system managed.");
            }
        }

        if (string.IsNullOrWhiteSpace(tag.Id))
        {
            tag.Id = randomHelper.Generate();
        }
        else
        {
            var existingTag = await repositoryFactory.TagRepository.GetByIdAsync(tag.Id, cancellationToken);
            if (existingTag is not null)
            {
                return await UpdateInternalAsync(tag, existingTag, customer, cancellationToken);
            }
        }

        var existingOrganization =
            await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                tag.Organization.Id,
                tag.Organization.CustomDomain,
                cancellationToken) ??
            throw new OrganizationNotFound();

        if (customer is not null && !await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var tagType = tag.Type.ToOrganizationTagType();

        var matchingTagFound = await repositoryFactory.TagRepository
            .ExistsActiveWithNameAsync(tag.Organization.Id, tagType, tag.Name, null, cancellationToken);
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

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var tagEntity = graphQlMapper.MapTo(tag, existingOrganization);
        _ = repositoryFactory.TagRepository.Add(tagEntity);

        organizationOutboxPublisher.PublishOrganizations(
        [
            graphQlMapper.MapTo(
                existingOrganization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id)),
        ], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTagService.UpdateByIdAsync(tag.Id, cancellationToken);

        return tag;
    }

    public async Task<Tag> UpdatePatchAsync(OrganizationTagPatchRequest request, CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);
        EnsureUserManagedTag(request.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingTag = await repositoryFactory.TagRepository.GetByIdAsync(request.Id, cancellationToken) ?? throw new OrganizationTagNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingTag.Organization.Id,
                                       null,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        var existingType = existingTag.Type.ToOrganizationTagType();
        if (existingType != request.Type)
        {
            throw new OrganizationTagNotFound();
        }

        if (request.FieldsToUpdate.Contains(OrganizationTagPatchField.Name) && existingTag.Name != request.Name)
        {
            await ValidateNameAsync(request.Id, request.Type, request.Name!, existingOrganization.Id, cancellationToken);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (!ApplyPatch(request, existingTag))
        {
            return graphQlMapper.MapTo(existingTag);
        }

        var tag = graphQlMapper.MapTo(repositoryFactory.TagRepository.Update(existingTag));

        organizationOutboxPublisher.PublishOrganizations(
        [
            graphQlMapper.MapTo(
                existingOrganization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id)),
        ], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTagService.UpdateByIdAsync(tag.Id, cancellationToken);

        return tag;
    }

    public async Task<Tag> DeleteAsync(string tagId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(tagId);
        EnsureUserManagedTag(tagId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var tag = await repositoryFactory.TagRepository.GetByIdAsync(tagId, cancellationToken) ?? throw new OrganizationTagNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       tag.Organization.Id,
                                       null,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedTag = graphQlMapper.MapTo(repositoryFactory.TagRepository.Remove(tag));

        var mappedOrganization = graphQlMapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));
        mappedOrganization.Tags = mappedOrganization.Tags.Where(item => item.Id != tagId).ToList();

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTagService.RemoveByIdAsync(deletedTag.Id, cancellationToken);

        return deletedTag;
    }

    public async Task<IReadOnlyList<Tag>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        foreach (var id in ids)
        {
            EnsureUserManagedTag(id);
        }

        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var tags = await repositoryFactory.TagRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = tags.Select(item => item.Organization.Id).ToList();
        var existingOrganizations = await repositoryFactory.OrganizationRepository.GetByIdsOrCustomDomainsAsync(
            organizationIds,
            null,
            cancellationToken);

        foreach (var existingOrganization in existingOrganizations)
        {
            if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        repositoryFactory.TagRepository.RemoveRange(tags);
        var deletedTags = tags.Select(graphQlMapper.MapTo).ToList();

        var mappedOrganizations = existingOrganizations.Select(item =>
            graphQlMapper.MapTo(item, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(item.Id))).ToList();
        foreach (var mappedOrganization in mappedOrganizations)
        {
            mappedOrganization.Tags = mappedOrganization.Tags.Where(item => !ids.Contains(item.Id)).ToList();
        }

        organizationOutboxPublisher.PublishOrganizations(mappedOrganizations, repositoryFactory.UnitOfWork);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        foreach (var deletedTag in deletedTags)
        {
            await cachedTagService.RemoveByIdAsync(deletedTag.Id, cancellationToken);
        }

        return deletedTags;
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<Tag>>, int)> GetPaginatedTagsAsync(
        PaginationInputParam paginationInputParam,
        TagSearchCriteria searchCriteria,
        IReadOnlyList<TagOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               searchCriteria.OrganizationId,
                               searchCriteria.OrganizationCustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!ignoreAuthorizationCheck)
        {
            var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
            if (!await organizationAuthorizationService.CanViewAsync(organization, customerId, cancellationToken))
            {
                throw new UnauthorizedAccessException();
            }
        }

        var (paginatedInfo, edges, totalCount) = await repositoryFactory.TagRepository.GetPaginatedTagsAsync(
            paginationInputParam,
            searchCriteria,
            orderByFields,
            cancellationToken);

        return (paginatedInfo,
            graphQlMapper.MapTo(
                    edges,
                    graphQlMapper.MapTo(organization,
                        organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id)))
                .ToList(),
            totalCount);
    }

    private static void EnsureUserManagedTag(string id)
    {
        if (HostLocationSystemIds.IsProductTag(id))
        {
            throw new UnauthorizedAccessException("Host Location Product tags are system managed.");
        }
    }

    private async Task<Tag> UpdateInternalAsync(
        Tag tag,
        Shared.Database.Entities.Tag existingTag,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingTag.Organization.Id,
                                       null,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (customer is not null && !await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await ValidateNameAsync(tag.Id, tag.Type, tag.Name, existingTag.Organization.Id, cancellationToken);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        tag = graphQlMapper.MapTo(repositoryFactory.TagRepository.Update(graphQlMapper.MergeTo(tag, existingTag, existingOrganization)));

        organizationOutboxPublisher.PublishOrganizations(
        [
            graphQlMapper.MapTo(
                existingOrganization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id)),
        ], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedTagService.UpdateByIdAsync(tag.Id, cancellationToken);

        return tag;
    }

    private async Task ValidateNameAsync(
        string tagId,
        OrganizationTagType tagType,
        string tagName,
        string organizationId,
        CancellationToken cancellationToken)
    {
        var matchingTagFound = await repositoryFactory.TagRepository
            .ExistsActiveWithNameAsync(organizationId, tagType.ToOrganizationTagType(), tagName, tagId, cancellationToken);
        if (!matchingTagFound)
        {
            return;
        }

        if (tagType == OrganizationTagType.Custom)
        {
            throw new CustomTagWithSameNameExist();
        }

        if (tagType == OrganizationTagType.Zone)
        {
            throw new ZoneWithSameNameExist();
        }

        throw new OrganizationTagWithSameNameExist();
    }

    private static void ValidatePatchRequest(OrganizationTagPatchRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation tag field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation tag patch field is not supported.");
            }
        }

        if (request.FieldsToUpdate.Contains(OrganizationTagPatchField.Name) && string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Organisation tag name is required.", nameof(request));
        }
    }

    private static bool ApplyPatch(OrganizationTagPatchRequest request, Shared.Database.Entities.Tag tag)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            changed = field switch
            {
                OrganizationTagPatchField.Name => ApplyValue(request.Name!, tag.Name, value => tag.Name = value) || changed,
                OrganizationTagPatchField.Description => ApplyValue(request.Description, tag.Description, value => tag.Description = value) ||
                                                         changed,
                OrganizationTagPatchField.Color => ApplyValue(request.Color, tag.Color, value => tag.Color = value) || changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation tag patch field is not supported."),
            };
        }

        return changed;
    }

    private static bool ApplyValue<T>(T value, T currentValue, Action<T> apply)
    {
        if (EqualityComparer<T>.Default.Equals(value, currentValue))
        {
            return false;
        }

        apply(value);
        return true;
    }
}
