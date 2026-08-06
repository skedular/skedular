using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services;

public interface IOrganizationTaxDetailsService
{
    Task<Shared.Models.Organization> UpdatePatchAsync(OrganizationTaxDetailsPatchRequest request, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> RemoveAsync(string? organizationId, string? organizationCustomDomain, CancellationToken cancellationToken);
}

public class OrganizationTaxDetailsService(
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IGraphQlMapper graphQlMapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationTaxDetailsService
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

        bool changed;
        if (organization.OrganizationTaxDetails is null)
        {
            ValidateEffectiveRegistrationState(request, null);

            var taxDetails = new Shared.Models.OrganizationTaxDetails
            {
                Id = randomHelper.Generate(),
                IsRegistered = request.IsRegistered ?? false,
                TaxId = request.TaxId ?? string.Empty,
                TaxRatePercentage = request.TaxRatePercentage ?? 0,
            };
            organization.OrganizationTaxDetails =
                repositoryFactory.OrganizationTaxDetailsRepository.Add(graphQlMapper.MapToEntity(taxDetails, organization));
            changed = true;
        }
        else
        {
            ValidateEffectiveRegistrationState(request, organization.OrganizationTaxDetails);
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
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

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
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

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

        if (request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.IsRegistered) && request.IsRegistered is null)
        {
            throw new ArgumentException("Organisation tax registration state is required.", nameof(request));
        }
    }

    private static void ValidateEffectiveRegistrationState(
        OrganizationTaxDetailsPatchRequest request,
        OrganizationTaxDetails? currentTaxDetails)
    {
        var isRegistered = request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.IsRegistered)
            ? request.IsRegistered!.Value
            : currentTaxDetails?.IsRegistered ?? false;
        if (!isRegistered)
        {
            return;
        }

        var taxId = request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.TaxId)
            ? request.TaxId
            : currentTaxDetails?.TaxId;
        var taxRatePercentage = request.FieldsToUpdate.Contains(OrganizationTaxDetailsPatchField.TaxRatePercentage)
            ? request.TaxRatePercentage
            : currentTaxDetails?.TaxRatePercentage;

        if (string.IsNullOrWhiteSpace(taxId) || taxRatePercentage is null or <= 0)
        {
            throw new ArgumentException("Tax ID and tax rate are required when organisation tax details are registered.", nameof(request));
        }
    }

    private static bool ApplyPatch(OrganizationTaxDetailsPatchRequest request, OrganizationTaxDetails taxDetails) =>
        request.FieldsToUpdate.Aggregate(false, (current, field) => field switch
        {
            OrganizationTaxDetailsPatchField.IsRegistered => ApplyIsRegisteredPatch(request.IsRegistered!.Value, taxDetails) || current,
            OrganizationTaxDetailsPatchField.TaxId => ApplyTaxIdPatch(request.TaxId ?? string.Empty, taxDetails) || current,
            OrganizationTaxDetailsPatchField.TaxRatePercentage =>
                ApplyTaxRatePercentagePatch(request.TaxRatePercentage ?? 0, taxDetails) || current,
            _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation tax details patch field is not supported."),
        });

    private static bool ApplyIsRegisteredPatch(bool isRegistered, OrganizationTaxDetails taxDetails)
    {
        if (taxDetails.IsRegistered == isRegistered)
        {
            return false;
        }

        taxDetails.IsRegistered = isRegistered;
        return true;
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
