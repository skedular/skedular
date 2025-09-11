using Api.Shared.Services;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security.Sso;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Constants = Enterprise.Shared.Security.Sso.Models.Constants;
using OrganizationSsoValidationResult = Organization.Shared.Configurations.OrganizationSsoValidationResult;

namespace Organization.Api.Services;

public interface IOrganizationSsoService
{
    Task<bool> IsSsoTokenValidAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task<string> SsoLoginAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        string redirectUrl,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdateAsync(OrganizationSsoSettings ssoSettings, CancellationToken cancellationToken);

    Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);

    Task ProcessSsoResponseAsync(HttpResponse httpResponse, string rawSamlResponse, CancellationToken cancellationToken);
}

public class OrganizationSsoService(
    IRepositoryFactory repositoryFactory,
    ISamlLoginRequestFactory samlLoginRequestFactory,
    ISamlAssertionConsumerService samlAssertionConsumerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IMapper mapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IContext context) : IOrganizationSsoService
{
    public async Task<bool> IsSsoTokenValidAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationUniqueAlphanumericName);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationUniqueAlphanumericNameAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken);
        if (ssoSettings is null || !ssoSettings.IsActive)
        {
            return true;
        }

        var userSsoContext = context.GetUserSsoContext(organizationUniqueAlphanumericName);
        if (userSsoContext is null)
        {
            return false;
        }

        return customer.Identities.Any(item =>
            !string.IsNullOrWhiteSpace(item.Email) && item.Email.Equals(userSsoContext.Email, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<string> SsoLoginAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        string redirectUrl,
        CancellationToken cancellationToken)
    {
        var existingOrganizationSsoSetting = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationUniqueAlphanumericNameAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(organizationId))
        {
            return samlLoginRequestFactory.GenerateSamlLoginRequest(
                $"id{organizationId}",
                redirectUrl,
                existingOrganizationSsoSetting.EntityId,
                existingOrganizationSsoSetting.LoginUrl);
        }

        if (!string.IsNullOrWhiteSpace(organizationUniqueAlphanumericName))
        {
            return samlLoginRequestFactory.GenerateSamlLoginRequest(
                $"uniquename{organizationUniqueAlphanumericName}",
                redirectUrl,
                existingOrganizationSsoSetting.EntityId,
                existingOrganizationSsoSetting.LoginUrl);
        }

        throw new InvalidOperationException("Either id or uniqueAlphanumericName must be provided.");
    }

    public async Task<Shared.Models.Organization> UpdateAsync(OrganizationSsoSettings ssoSettings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ssoSettings.Organization);

        // Validate SSO settings first
        var validationResult = await ValidateSsoConfigurationAsync(ssoSettings, cancellationToken);
        if (!validationResult.IsMetadataValid || !validationResult.IsCertificateValid)
        {
            throw new InvalidSsoConfiguration();
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               ssoSettings.Organization.Id,
                               ssoSettings.Organization.UniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationSsoSettings is null)
        {
            ssoSettings.Id = randomHelper.Generate();
            var organizationSsoSettingsEntity = mapper.MapToEntity(ssoSettings, organization);
            organizationSsoSettingsEntity.IsActive = true;
            repositoryFactory.OrganizationSsoSettingsRepository.Add(organizationSsoSettingsEntity);
        }
        else
        {
            ssoSettings.Id = organization.OrganizationSsoSettings.Id;
            var organizationSsoSettingsEntity = mapper.MergeToEntity(ssoSettings, organization.OrganizationSsoSettings, organization);
            organizationSsoSettingsEntity.IsActive = true;
            repositoryFactory.OrganizationSsoSettingsRepository.Update(organizationSsoSettingsEntity);
        }

        var mappedOrganization = mapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrUniqueAlphanumericNameAsync(
                               organizationId,
                               organizationUniqueAlphanumericName,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationSsoSettings is null)
        {
            return mapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.OrganizationSsoSettings.IsActive = false;
        repositoryFactory.OrganizationSsoSettingsRepository.Update(organization.OrganizationSsoSettings);

        var mappedOrganization = mapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mappedOrganization;
    }

    public async Task ProcessSsoResponseAsync(HttpResponse httpResponse, string rawSamlResponse, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(rawSamlResponse);

        var decodedSaml = samlAssertionConsumerService.VerifyAndDecodeSamlResponse(rawSamlResponse);
        var samlResponse = samlAssertionConsumerService.ExtractSamlResponse(decodedSaml);
        if (samlResponse.StatusCode != "urn:oasis:names:tc:SAML:2.0:status:Success" || samlResponse.SessionNotOnOrAfter <= timeProvider.GetUtcNow())
        {
            throw new UnauthorizedAccessException();
        }

        var samlOriginId = samlResponse.InResponseTo.StartsWith(Constants.SamlIdPrefix)
            ? samlResponse.InResponseTo[Constants.SamlIdPrefix.Length..]
            : samlResponse.InResponseTo;

        Shared.Database.Entities.OrganizationSsoSettings ssoSettings;
        if (samlOriginId.StartsWith("id"))
        {
            ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationUniqueAlphanumericNameAsync(
                              samlOriginId["id".Length..],
                              null,
                              cancellationToken) ??
                          throw new OrganizationSsoIsNotYetSetup();
        }
        else if (samlOriginId.StartsWith("uniquename"))
        {
            ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationUniqueAlphanumericNameAsync(
                              null,
                              samlOriginId["uniquename".Length..],
                              cancellationToken) ??
                          throw new OrganizationSsoIsNotYetSetup();
        }
        else
        {
            throw new ArgumentException(nameof(samlOriginId));
        }

        var isSignatureValid = await samlAssertionConsumerService.ValidateSamlResponseSignatureAsync(
            decodedSaml,
            ssoSettings.AppFederationMetadataUrl,
            cancellationToken);
        if (!isSignatureValid)
        {
            throw new UnauthorizedAccessException();
        }

        samlAssertionConsumerService.StoreSamlResponseInCookie(httpResponse, ssoSettings.Organization.Id, samlResponse);
    }

    private async Task<OrganizationSsoValidationResult> ValidateSsoConfigurationAsync(
        OrganizationSsoSettings ssoSettings,
        CancellationToken cancellationToken)
    {
        var result = new OrganizationSsoValidationResult();

        try
        {
            // Test metadata
            result.IsMetadataValid = await samlAssertionConsumerService.ValidateMetadataAsync(
                ssoSettings.AppFederationMetadataUrl,
                cancellationToken);

            // Test certificate from metadata
            result.IsCertificateValid = await samlAssertionConsumerService.ValidateCertificateAsync(
                ssoSettings.AppFederationMetadataUrl,
                cancellationToken);
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
        }

        return result;
    }
}
