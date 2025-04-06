using Enterprise.Shared.Database;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Sso.Models;
using Organization.Api.Mappers;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationSsoService
{
    Task<Shared.Models.Organization> UpdateSsoSettingsAsync(OrganizationSsoSetting ssoSetting, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> RemoveSsoSettingsAsync(string organizationId, CancellationToken cancellationToken);
    Task<string> SsoLoginAsync(string id, string redirectUrl, CancellationToken cancellationToken);
    Task ProcessSsoResponseAsync(HttpResponse httpResponse, string rawSamlResponse, CancellationToken cancellationToken);
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
    IRandomHelper randomHelper) : IOrganizationSsoService
{
    public async Task<Shared.Models.Organization> UpdateSsoSettingsAsync(OrganizationSsoSetting ssoSetting, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ssoSetting.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(ssoSetting.Organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(ssoSetting.Organization.Id, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new Unauthorized();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationSsoSettings is null)
        {
            ssoSetting.Id = randomHelper.Generate();
            repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapToEntity(ssoSetting, organization));
        }
        else
        {
            ssoSetting.Id = organization.OrganizationSsoSettings.Id;
            repositoryFactory.OrganizationSsoSettingRepository.Update(
                mapper.MergeToEntity(ssoSetting, organization.OrganizationSsoSettings, organization));
        }

        await organizationOutboxPublisher.PublishOrganizationsAsync(
            [mapper.MapTo(organization)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<Shared.Models.Organization> RemoveSsoSettingsAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(organization, customer))
        {
            throw new Unauthorized();
        }

        if (organization.OrganizationSsoSettings is null)
        {
            return mapper.MapTo(organization);
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationSsoSettingRepository.Remove(organization.OrganizationSsoSettings);
        organization.OrganizationSsoSettings = null;
        
        await organizationOutboxPublisher.PublishOrganizationsAsync(
            [mapper.MapTo(organization)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(organization);
    }

    public async Task<string> SsoLoginAsync(string id, string redirectUrl, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var existingOrganizationSsoSetting = await repositoryFactory.OrganizationSsoSettingRepository.GetByOrganizationIdAsync(id, cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
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
        var response = samlAssertionConsumerService.ExtractSamlResponse(decodedSaml);
        var existingOrganizationSsoSetting = await repositoryFactory.OrganizationSsoSettingRepository.GetByOrganizationIdAsync(
            ExtractSamlOriginalId(response.InResponseTo),
            cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
        }

        var certificate = await samlAssertionConsumerService.GetSigningCertificateFromMetadataAsync(
            existingOrganizationSsoSetting.AppFederationMetadataUrl,
            cancellationToken);
        var isSignatureValid = samlAssertionConsumerService.ValidateSamlResponseSignature(decodedSaml, certificate);
        if (!isSignatureValid)
        {
            throw new Unauthorized();
        }

        samlAssertionConsumerService.StoreSamlResponseInCookie(httpResponse, response, existingOrganizationSsoSetting.Organization.Id);
    }

    // Split and return the original ID without the prefix
    private static string ExtractSamlOriginalId(string prefixedId) =>
        prefixedId.StartsWith(Constants.SamlIdPrefix) ? prefixedId[Constants.SamlIdPrefix.Length..] : prefixedId;
}
