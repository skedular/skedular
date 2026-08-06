using Api.Shared.Services;
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

public interface IOrganizationBankAccountService
{
    Task<OrganizationBankAccount> AddAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> UpdatePatchAsync(OrganizationBankAccountPatchRequest request, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> DeleteAsync(string id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrganizationBankAccount>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken);
    Task<OrganizationBankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationBankAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationBankAccountOrder> orderByFields,
        bool ignoreAuthorizationCheck,
        CancellationToken cancellationToken);
}

public class OrganizationBankAccountService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    ICustomerService customerService,
    ICachedCustomerService cachedCustomerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher) : IOrganizationBankAccountService
{
    public async Task<OrganizationBankAccount> AddAsync(OrganizationBankAccount organizationBankAccount, CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(organizationBankAccount.Id))
        {
            var existingOrganizationBankAccount =
                await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(organizationBankAccount.Id, cancellationToken);
            if (existingOrganizationBankAccount is not null)
            {
                return await UpdateInternalAsync(organizationBankAccount, existingOrganizationBankAccount, customer, cancellationToken);
            }
        }
        else
        {
            organizationBankAccount.Id = randomHelper.Generate();
        }

        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationBankAccount.Organization.Id,
                                       organizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationBankAccount.IsDefault = true;

        var mappedResource =
            graphQlMapper.MapTo(
                repositoryFactory.OrganizationBankAccountRepository.Add(graphQlMapper.MapTo(organizationBankAccount, existingOrganization)));
        PublishOrganization(existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return mappedResource;
    }

    public async Task<OrganizationBankAccount> UpdatePatchAsync(OrganizationBankAccountPatchRequest request, CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount =
            await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(request.Id, cancellationToken) ??
            throw new OrganizationBankAccountNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (!ApplyPatch(request, existingOrganizationBankAccount))
        {
            return graphQlMapper.MapTo(existingOrganizationBankAccount);
        }

        var organizationBankAccount =
            graphQlMapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Update(existingOrganizationBankAccount));
        PublishOrganization(existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationBankAccount;
    }

    public async Task<OrganizationBankAccount> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount = await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ??
                                              throw new ResourceNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var deletedResource = graphQlMapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Remove(existingOrganizationBankAccount));
        PublishOrganization(existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return deletedResource;
    }

    public async Task<IReadOnlyList<OrganizationBankAccount>> DeleteAsync(IReadOnlyList<string> ids, CancellationToken cancellationToken)
    {
        if (ids.Count == 0)
        {
            return [];
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var resources = await repositoryFactory.OrganizationBankAccountRepository.GetByIdsAsync(ids, cancellationToken);
        var organizationIds = resources.Select(item => item.Organization.Id).ToList();
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

        repositoryFactory.OrganizationBankAccountRepository.RemoveRange(resources);

        var deletedOrganizationBankAccounts = resources.Select(graphQlMapper.MapTo).ToList();
        PublishOrganizations(existingOrganizations);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return deletedOrganizationBankAccounts;
    }

    public async Task<OrganizationBankAccount> SetAsDefaultAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganizationBankAccount = await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ??
                                              throw new OrganizationBankAccountNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        foreach (var item in existingOrganization.OrganizationBankAccounts.Where(item => item.Id != id))
        {
            item.IsDefault = false;
            repositoryFactory.OrganizationBankAccountRepository.Update(item);
        }

        existingOrganizationBankAccount.IsDefault = true;
        var organizationBankAccount =
            graphQlMapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Update(existingOrganizationBankAccount));
        PublishOrganization(existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationBankAccount;
    }

    public async Task<OrganizationBankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var customerId = await cachedCustomerService.GetIdAsync(cancellationToken);
        var existingOrganizationBankAccount =
            await repositoryFactory.OrganizationBankAccountRepository.GetByIdAsync(id, cancellationToken) ?? throw new ResourceNotFound();
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanViewAsync(existingOrganization, customerId, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return graphQlMapper.MapTo(existingOrganizationBankAccount);
    }

    public async Task<(PaginatedInfo, IReadOnlyList<Edge<OrganizationBankAccount>>, int)> GetPaginatedAccountsAsync(
        PaginationInputParam paginationInputParam,
        OrganizationBankAccountSearchCriteria searchCriteria,
        IEnumerable<OrganizationBankAccountOrder> orderByFields,
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

        var (paginatedInfo, edges, totalCount) =
            await repositoryFactory.OrganizationBankAccountRepository.GetPaginatedBankAccountsAsync(
                paginationInputParam,
                searchCriteria,
                orderByFields,
                cancellationToken);

        return (paginatedInfo, edges.Select(item => new Edge<OrganizationBankAccount>(graphQlMapper.MapTo(item.Node), item.Cursor)).ToList(),
            totalCount);
    }

    private async Task<OrganizationBankAccount> UpdateInternalAsync(
        OrganizationBankAccount organizationBankAccount,
        Shared.Database.Entities.OrganizationBankAccount existingOrganizationBankAccount,
        Customer? customer,
        CancellationToken cancellationToken)
    {
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       existingOrganizationBankAccount.Organization.Id,
                                       existingOrganizationBankAccount.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (customer is not null && !await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationBankAccount.IsDefault = existingOrganizationBankAccount.IsDefault;
        existingOrganizationBankAccount = graphQlMapper.MergeTo(organizationBankAccount, existingOrganizationBankAccount, existingOrganization);

        organizationBankAccount = graphQlMapper.MapTo(repositoryFactory.OrganizationBankAccountRepository.Update(existingOrganizationBankAccount));
        PublishOrganization(existingOrganization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return organizationBankAccount;
    }

    private void PublishOrganization(Shared.Database.Entities.Organization organization) =>
        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

    private void PublishOrganizations(IEnumerable<Shared.Database.Entities.Organization> organizations) =>
        organizationOutboxPublisher.PublishOrganizations(
            organizations.Select(organization =>
                graphQlMapper.MapTo(organization,
                    organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))).ToList(),
            repositoryFactory.UnitOfWork);

    private static void ValidatePatchRequest(OrganizationBankAccountPatchRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Id);

        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation bank account field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation bank account patch field is not supported.");
            }

            var value = field switch
            {
                OrganizationBankAccountPatchField.Name => request.Name,
                OrganizationBankAccountPatchField.BankName => request.BankName,
                OrganizationBankAccountPatchField.AccountHolderName => request.AccountHolderName,
                OrganizationBankAccountPatchField.AccountNumber => request.AccountNumber,
                OrganizationBankAccountPatchField.Country => request.Country,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation bank account patch field is not supported."),
            };
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Selected organisation bank account patch fields are required.", nameof(request));
            }
        }
    }

    private static bool ApplyPatch(
        OrganizationBankAccountPatchRequest request,
        Shared.Database.Entities.OrganizationBankAccount bankAccount)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            changed = field switch
            {
                OrganizationBankAccountPatchField.Name => ApplyValue(request.Name!, bankAccount.Name, value => bankAccount.Name = value) || changed,
                OrganizationBankAccountPatchField.BankName => ApplyValue(request.BankName!, bankAccount.BankName,
                    value => bankAccount.BankName = value) || changed,
                OrganizationBankAccountPatchField.AccountHolderName => ApplyValue(request.AccountHolderName!, bankAccount.AccountHolderName,
                    value => bankAccount.AccountHolderName = value) || changed,
                OrganizationBankAccountPatchField.AccountNumber => ApplyValue(request.AccountNumber!, bankAccount.AccountNumber,
                    value => bankAccount.AccountNumber = value) || changed,
                OrganizationBankAccountPatchField.Country =>
                    ApplyValue(request.Country!, bankAccount.Country, value => bankAccount.Country = value) || changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation bank account patch field is not supported."),
            };
        }

        return changed;
    }

    private static bool ApplyValue(string value, string currentValue, Action<string> apply)
    {
        if (value == currentValue)
        {
            return false;
        }

        apply(value);
        return true;
    }
}
