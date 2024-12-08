using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Sso.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IOrganizationSsoService
{
    Task<string> SsoLoginAsync(
        string organizationId,
        CancellationToken cancellationToken);

    Task<string> ProcessSsoResponseAsync(
        HttpResponse httpResponse,
        string rawSamlResponse,
        CancellationToken cancellationToken);
}

public class OrganizationSsoService(
    IRepositoryFactory repositoryFactory,
    ISamlLoginRequestFactory samlLoginRequestFactory,
    ISamlAssertionConsumerService samlAssertionConsumerService) : IOrganizationSsoService
{
    public async Task<string> SsoLoginAsync(string organizationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var existingOrganizationSsoSetting =
            await repositoryFactory.OrganizationSsoSettingRepository.GetByOrganizationIdAsync(
                organizationId,
                cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
        }

        return samlLoginRequestFactory.GenerateSamlLoginRequest(
            organizationId,
            existingOrganizationSsoSetting.EntityId,
            existingOrganizationSsoSetting.LoginUrl);
    }

    public async Task<string> ProcessSsoResponseAsync(
        HttpResponse httpResponse,
        string rawSamlResponse,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(rawSamlResponse);

        var decodedSaml = samlAssertionConsumerService.VerifyAndDecodeSamlResponse(rawSamlResponse);
        var response = samlAssertionConsumerService.ExtractSamlResponse(decodedSaml);
        var existingOrganizationSsoSetting =
            await repositoryFactory.OrganizationSsoSettingRepository.GetByOrganizationIdAsync(
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

        samlAssertionConsumerService.StoreSamlResponseInCookie(httpResponse, response);
        return existingOrganizationSsoSetting.RedirectUrl;
    }
    
    // Split and return the original ID without the prefix
    private static string ExtractSamlOriginalId(string prefixedId) =>
        prefixedId.StartsWith(Constants.SamlIdPrefix) ? prefixedId[Constants.SamlIdPrefix.Length..] : prefixedId;
}
