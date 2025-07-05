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
    Task<bool> IsSsoLoginRequiredAsync(string id, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> UpdateSsoSettingsAsync(OrganizationSsoSettings ssoSettings, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> RemoveSsoSettingsAsync(string organizationId, CancellationToken cancellationToken);
    Task<string> SsoLoginAsync(string id, string redirectUrl, CancellationToken cancellationToken);
    Task ProcessSsoResponseAsync(HttpResponse httpResponse, string rawSamlResponse, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> ToggleSsoSettingsAsync(string organizationId, bool isActive, CancellationToken cancellationToken);
}

public class OrganizationSsoService(
    IRepositoryFactory repositoryFactory,
    ISamlLoginRequestFactory samlLoginRequestFactory,
    ISamlAssertionConsumerService samlAssertionConsumerService,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IMapper mapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IContext context) : IOrganizationSsoService
{
    public async Task<bool> IsSsoLoginRequiredAsync(string id, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationIdAsync(id, cancellationToken);
        if (ssoSettings is null || !ssoSettings.IsActive)
        {
            return false;
        }

        var userSsoContext = context.GetUserSsoContext(id);
        if (userSsoContext is null)
        {
            return true;
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        return !customer.Identities.Any(item =>
            !string.IsNullOrWhiteSpace(item.Email) && item.Email.Equals(userSsoContext.Email, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<Shared.Models.Organization> UpdateSsoSettingsAsync(OrganizationSsoSettings ssoSettings, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ssoSettings.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(ssoSettings.Organization.Id);

        // Validate SSO settings first
        var validationResult = await ValidateSsoConfigurationAsync(ssoSettings, cancellationToken);
        if (!validationResult.IsMetadataValid || !validationResult.IsCertificateValid)
        {
            throw new InvalidSsoConfiguration();
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(ssoSettings.Organization.Id, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationSsoSettings is null)
        {
            ssoSettings.Id = randomHelper.Generate();
            repositoryFactory.OrganizationSsoSettingsRepository.Add(mapper.MapToEntity(ssoSettings, organization));
        }
        else
        {
            ssoSettings.Id = organization.OrganizationSsoSettings.Id;
            repositoryFactory.OrganizationSsoSettingsRepository.Update(
                mapper.MergeToEntity(ssoSettings, organization.OrganizationSsoSettings, organization));
        }

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<Shared.Models.Organization> RemoveSsoSettingsAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationSsoSettings is null)
        {
            return mapper.MapTo(organization);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationSsoSettingsRepository.Remove(organization.OrganizationSsoSettings);
        organization.OrganizationSsoSettings = null;

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<string> SsoLoginAsync(string id, string redirectUrl, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existingOrganizationSsoSetting =
            await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationIdAsync(id, cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            return string.Empty;
        }

        return samlLoginRequestFactory.GenerateSamlLoginRequest(
            id,
            redirectUrl,
            existingOrganizationSsoSetting.EntityId,
            existingOrganizationSsoSetting.LoginUrl);
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
        var ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationIdAsync(samlOriginId, cancellationToken);
        if (ssoSettings is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
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

    public async Task<Shared.Models.Organization> ToggleSsoSettingsAsync(string organizationId, bool isActive, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new UnauthorizedAccessException();
        }

        if (organization.OrganizationSsoSettings is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.OrganizationSsoSettings.IsActive = isActive;
        repositoryFactory.OrganizationSsoSettingsRepository.Update(organization.OrganizationSsoSettings);

        organizationOutboxPublisher.PublishOrganizations([mapper.MapTo(organization)], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<OrganizationSsoValidationResult> ValidateSsoConfigurationAsync(
        OrganizationSsoSettings ssoSettingses,
        CancellationToken cancellationToken)
    {
        var result = new OrganizationSsoValidationResult();

        try
        {
            // Test metadata
            result.IsMetadataValid = await samlAssertionConsumerService.ValidateMetadataAsync(
                ssoSettingses.AppFederationMetadataUrl,
                cancellationToken);

            // Test certificate from metadata
            result.IsCertificateValid = await samlAssertionConsumerService.ValidateCertificateAsync(
                ssoSettingses.AppFederationMetadataUrl,
                cancellationToken);
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
        }

        return result;
    }
}
