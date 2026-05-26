using Api.Shared.Services;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using NetTopologySuite.Geometries;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services;

public interface IOrganizationBillingService
{
    Task<OrganizationBillingDetails?> GetAsync(string? organizationId, string? organizationCustomDomain, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> AddAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> UpdatePatchAsync(OrganizationBillingDetailsPatchRequest request, CancellationToken cancellationToken);
}

public class OrganizationBillingService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IRandomHelper randomHelper,
    IGraphQlMapper graphQlMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationBillingService
{
    public async Task<OrganizationBillingDetails?> GetAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationId,
                                       organizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanViewAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        return graphQlMapper.MapTo(existingOrganization.BillingDetails);
    }

    public async Task<Shared.Models.Organization> AddAsync(OrganizationBillingDetails organizationBillingDetails, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(organizationBillingDetails.Organization);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       organizationBillingDetails.Organization.Id,
                                       organizationBillingDetails.Organization.CustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        if (!string.IsNullOrWhiteSpace(organizationBillingDetails.Id))
        {
            var existingOrganizationBillingDetails = await repositoryFactory.OrganizationBillingDetailsRepository.GetByIdAsync(
                organizationBillingDetails.Id,
                cancellationToken);
            if (existingOrganizationBillingDetails is not null)
            {
                if (existingOrganizationBillingDetails.Organization.Id != existingOrganization.Id)
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdateInternalAsync(
                    organizationBillingDetails,
                    existingOrganizationBillingDetails,
                    existingOrganization,
                    cancellationToken);
            }
        }
        else
        {
            organizationBillingDetails.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var organizationBillingDetailsEntity = graphQlMapper.MapTo(organizationBillingDetails, existingOrganization);
        repositoryFactory.OrganizationBillingDetailsRepository.Add(organizationBillingDetailsEntity);

        existingOrganization.BillingDetails = organizationBillingDetailsEntity;
        var mappedOrganization = graphQlMapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(existingOrganization.Id, existingOrganization.CustomDomain, cancellationToken);
        var memberCustomerIds = existingOrganization.OrganizationMembers.Select(m => m.CustomerId).ToList();
        if (memberCustomerIds.Count == 0)
        {
            memberCustomerIds.Add(customer.Id);
        }

        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(memberCustomerIds, cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> UpdatePatchAsync(
        OrganizationBillingDetailsPatchRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       request.OrganizationId,
                                       request.OrganizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

        if (!await organizationAuthorizationService.CanModifyAsync(existingOrganization, customer.Id, cancellationToken))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var changed = false;
        if (existingOrganization.BillingDetails is null)
        {
            ValidateCreatePatchRequest(request);

            var billingDetails = CreateBillingDetails(request, existingOrganization);
            existingOrganization.BillingDetails = repositoryFactory.OrganizationBillingDetailsRepository.Add(billingDetails);
            changed = true;
        }
        else
        {
            changed = ApplyPatch(request, existingOrganization.BillingDetails);
            if (changed)
            {
                repositoryFactory.OrganizationBillingDetailsRepository.Update(existingOrganization.BillingDetails);
            }
        }

        var mappedOrganization = graphQlMapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));
        if (!changed)
        {
            return mappedOrganization;
        }

        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(existingOrganization.Id, existingOrganization.CustomDomain, cancellationToken);
        var patchMemberCustomerIds = existingOrganization.OrganizationMembers.Select(m => m.CustomerId).ToList();
        if (patchMemberCustomerIds.Count == 0)
        {
            patchMemberCustomerIds.Add(customer.Id);
        }

        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(patchMemberCustomerIds, cancellationToken);

        return mappedOrganization;
    }

    private async Task<Shared.Models.Organization> UpdateInternalAsync(
        OrganizationBillingDetails organizationBillingDetails,
        Shared.Database.Entities.OrganizationBillingDetails existingOrganizationBillingDetails,
        Shared.Database.Entities.Organization existingOrganization,
        CancellationToken cancellationToken)
    {
        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        existingOrganizationBillingDetails =
            graphQlMapper.MergeToEntity(organizationBillingDetails, existingOrganizationBillingDetails, existingOrganization);
        repositoryFactory.OrganizationBillingDetailsRepository.Update(existingOrganizationBillingDetails);

        existingOrganization.BillingDetails = existingOrganizationBillingDetails;

        var mappedOrganization = graphQlMapper.MapTo(
            existingOrganization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(existingOrganization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await cachedOrganizationService.UpdateByIdOrCustomDomainAsync(existingOrganization.Id, existingOrganization.CustomDomain, cancellationToken);
        await cachedOrganizationService.RemoveMyOrganizationsByCustomerIdsAsync(
            existingOrganization.OrganizationMembers.Select(m => m.CustomerId).ToList(),
            cancellationToken);

        return mappedOrganization;
    }

    private static void ValidatePatchRequest(OrganizationBillingDetailsPatchRequest request)
    {
        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation billing details field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation billing details patch field is not supported.");
            }
        }

        if (request.FieldsToUpdate.Contains(OrganizationBillingDetailsPatchField.Email) && string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Organisation billing email is required.", nameof(request));
        }

        if (request.FieldsToUpdate.Contains(OrganizationBillingDetailsPatchField.BillingAddress))
        {
            ValidateBillingAddress(request);
        }
    }

    private static void ValidateCreatePatchRequest(OrganizationBillingDetailsPatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Organisation billing email is required when creating billing details.", nameof(request));
        }

        ValidateBillingAddress(request);
    }

    private static void ValidateBillingAddress(OrganizationBillingDetailsPatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AddressLine1) ||
            string.IsNullOrWhiteSpace(request.Zipcode) ||
            string.IsNullOrWhiteSpace(request.Country))
        {
            throw new ArgumentException("Organisation billing address line 1, zipcode, and country are required.", nameof(request));
        }
    }

    private Shared.Database.Entities.OrganizationBillingDetails CreateBillingDetails(
        OrganizationBillingDetailsPatchRequest request,
        Shared.Database.Entities.Organization organization) =>
        new()
        {
            Id = randomHelper.Generate(),
            CompanyName = request.CompanyName,
            Email = request.Email!,
            Organization = organization,
            OrganizationId = organization.Id,
            OsmType = request.OsmType,
            OsmId = request.OsmId,
            PlaceId = request.PlaceId,
            Coordinates = CreateCoordinates(request),
            FormattedAddress = request.FormattedAddress,
            AddressLine1 = request.AddressLine1!,
            AddressLine2 = request.AddressLine2,
            Suburb = request.Suburb,
            City = request.City,
            Province = request.Province,
            Zipcode = request.Zipcode!,
            Country = request.Country!,
            CountryCode = request.CountryCode
        };

    private static bool ApplyPatch(
        OrganizationBillingDetailsPatchRequest request,
        Shared.Database.Entities.OrganizationBillingDetails billingDetails)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            changed = field switch
            {
                OrganizationBillingDetailsPatchField.CompanyName => ApplyValue(request.CompanyName, billingDetails.CompanyName,
                    value => billingDetails.CompanyName = value) || changed,
                OrganizationBillingDetailsPatchField.Email =>
                    ApplyValue(request.Email!, billingDetails.Email, value => billingDetails.Email = value) || changed,
                OrganizationBillingDetailsPatchField.BillingAddress => ApplyBillingAddressPatch(request, billingDetails) || changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation billing details patch field is not supported.")
            };
        }

        return changed;
    }

    private static bool ApplyBillingAddressPatch(
        OrganizationBillingDetailsPatchRequest request,
        Shared.Database.Entities.OrganizationBillingDetails billingDetails)
    {
        var changed = false;
        changed = ApplyValue(request.OsmType, billingDetails.OsmType, value => billingDetails.OsmType = value) || changed;
        changed = ApplyValue(request.OsmId, billingDetails.OsmId, value => billingDetails.OsmId = value) || changed;
        changed = ApplyValue(request.PlaceId, billingDetails.PlaceId, value => billingDetails.PlaceId = value) || changed;
        changed = ApplyValue(CreateCoordinates(request), billingDetails.Coordinates, value => billingDetails.Coordinates = value) || changed;
        changed = ApplyValue(request.FormattedAddress, billingDetails.FormattedAddress, value => billingDetails.FormattedAddress = value) || changed;
        changed = ApplyValue(request.AddressLine1!, billingDetails.AddressLine1, value => billingDetails.AddressLine1 = value) || changed;
        changed = ApplyValue(request.AddressLine2, billingDetails.AddressLine2, value => billingDetails.AddressLine2 = value) || changed;
        changed = ApplyValue(request.Suburb, billingDetails.Suburb, value => billingDetails.Suburb = value) || changed;
        changed = ApplyValue(request.City, billingDetails.City, value => billingDetails.City = value) || changed;
        changed = ApplyValue(request.Province, billingDetails.Province, value => billingDetails.Province = value) || changed;
        changed = ApplyValue(request.Zipcode!, billingDetails.Zipcode, value => billingDetails.Zipcode = value) || changed;
        changed = ApplyValue(request.Country!, billingDetails.Country, value => billingDetails.Country = value) || changed;
        changed = ApplyValue(request.CountryCode, billingDetails.CountryCode, value => billingDetails.CountryCode = value) || changed;
        return changed;
    }

    private static Point? CreateCoordinates(OrganizationBillingDetailsPatchRequest request) =>
        request.Longitude is null || request.Latitude is null
            ? null
            : new Point(new Coordinate(request.Longitude.Value, request.Latitude.Value));

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
