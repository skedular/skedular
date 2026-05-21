using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationTaxDetailsService
{
    Task<Shared.Models.Organization> UpdatePatchAsync(OrganizationTaxDetailsPatchRequest request, CancellationToken cancellationToken);

    Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);
}

public class OrganizationTaxDetailsService(
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper) : IOrganizationTaxDetailsService
{
    public async Task<Shared.Models.Organization> UpdatePatchAsync(OrganizationTaxDetailsPatchRequest request, CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               request.OrganizationId,
                               request.OrganizationCustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var changed = false;
        if (organization.OrganizationTaxDetails is null)
        {
            if (string.IsNullOrWhiteSpace(request.TaxId) || request.TaxRatePercentage is null)
            {
                throw new ArgumentException("Tax ID and tax rate are required when creating organisation tax details.", nameof(request));
            }

            var taxDetails = new Shared.Models.OrganizationTaxDetails
            {
                Id = randomHelper.Generate(), TaxId = request.TaxId, TaxRatePercentage = request.TaxRatePercentage.Value
            };
            organization.OrganizationTaxDetails =
                repositoryFactory.OrganizationTaxDetailsRepository.Add(graphQlMapper.MapToEntity(taxDetails, organization));
            changed = true;
        }
        else
        {
            changed = ApplyPatch(request, organization.OrganizationTaxDetails);
            if (changed)
            {
                repositoryFactory.OrganizationTaxDetailsRepository.Update(organization.OrganizationTaxDetails);
            }
        }

        var mappedOrganization = graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        if (!changed)
        {
            return mappedOrganization;
        }

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               organizationCustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationTaxDetails is null)
        {
            return graphQlMapper.MapTo(organization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationTaxDetailsRepository.Remove(organization.OrganizationTaxDetails);
        organization.OrganizationTaxDetails = null;

        var mappedOrganization = graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    private static void ValidatePatchRequest(OrganizationTaxDetailsPatchRequest request)
    {
        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation tax details field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation tax details patch field is not supported.");
            }
        }

        if (request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.TaxId) && string.IsNullOrWhiteSpace(request.TaxId))
        {
            throw new ArgumentException("Organisation tax ID is required.", nameof(request));
        }

        if (request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.TaxRatePercentage) && request.TaxRatePercentage is null)
        {
            throw new ArgumentException("Organisation tax rate is required.", nameof(request));
        }
    }

    private static bool ApplyPatch(
        OrganizationTaxDetailsPatchRequest request,
        OrganizationTaxDetails taxDetails)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            changed = field switch
            {
                OrganizationTaxDetailsPatchField.TaxId => ApplyTaxIdPatch(request.TaxId!, taxDetails) || changed,
                OrganizationTaxDetailsPatchField.TaxRatePercentage => ApplyTaxRatePercentagePatch(request.TaxRatePercentage!.Value, taxDetails) ||
                                                                      changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation tax details patch field is not supported.")
            };
        }

        return changed;
    }

    private static bool ApplyTaxIdPatch(string taxId, OrganizationTaxDetails taxDetails)
    {
        if (taxDetails.TaxId == taxId)
        {
            return false;
        }

        taxDetails.TaxId = taxId;
        return true;
    }

    private static bool ApplyTaxRatePercentagePatch(decimal taxRatePercentage, OrganizationTaxDetails taxDetails)
    {
        if (taxDetails.TaxRatePercentage == taxRatePercentage)
        {
            return false;
        }

        taxDetails.TaxRatePercentage = taxRatePercentage;
        return true;
    }
}
