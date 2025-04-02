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
    Task<OrganizationSsoSetting> UpdateSsoSettingsAsync(OrganizationSsoSetting ssoSetting, CancellationToken cancellationToken);
    Task<string> SsoLoginAsync(string organizationId, string redirectUrl, CancellationToken cancellationToken);
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
    public async Task<OrganizationSsoSetting> UpdateSsoSettingsAsync(OrganizationSsoSetting ssoSetting, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ssoSetting.Organization);
        ArgumentException.ThrowIfNullOrWhiteSpace(ssoSetting.Organization.Id);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var existingOrganization = await repositoryFactory.OrganizationRepository.GetByIdAsync(ssoSetting.Organization.Id, cancellationToken);
        if (existingOrganization is null)
        {
            throw new OrganizationNotFound();
        }

        if (!organizationAuthorizationService.CanModify(existingOrganization, customer))
        {
            throw new Unauthorized();
        }

        if (!string.IsNullOrWhiteSpace(ssoSetting.Id))
        {
            var existingOrganizationSsoSettings =
                await repositoryFactory.OrganizationSsoSettingRepository.GetByIdAsync(ssoSetting.Id, cancellationToken);
            if (existingOrganizationSsoSettings is not null && existingOrganization.Id != existingOrganizationSsoSettings.Organization.Id)
            {
                throw new Unauthorized();
            }
        }
        else
        {
            ssoSetting.Id = randomHelper.Generate();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var ssoSettingsEntity = existingOrganization.OrganizationSsoSettings is null
            ? repositoryFactory.OrganizationSsoSettingRepository.Add(mapper.MapToEntity(ssoSetting, existingOrganization))
            : repositoryFactory.OrganizationSsoSettingRepository.Update(
                mapper.MergeToEntity(ssoSetting, existingOrganization.OrganizationSsoSettings, existingOrganization));

        await organizationOutboxPublisher.PublishOrganizationsAsync(
            [mapper.MapTo(existingOrganization)],
            repositoryFactory.UnitOfWork,
            cancellationToken);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return mapper.MapTo(ssoSettingsEntity)!;
    }

    public async Task<string> SsoLoginAsync(string organizationId, string redirectUrl, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var existingOrganizationSsoSetting =
            await repositoryFactory.OrganizationSsoSettingRepository.GetByOrganizationIdAsync(organizationId, cancellationToken);
        if (existingOrganizationSsoSetting is null)
        {
            throw new OrganizationSsoIsNotYetSetup();
        }

        return samlLoginRequestFactory.GenerateSamlLoginRequest(
            organizationId,
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

        samlAssertionConsumerService.StoreSamlResponseInCookie(httpResponse, response, existingOrganizationSsoSetting.Id);
    }

    // Split and return the original ID without the prefix
    private static string ExtractSamlOriginalId(string prefixedId) =>
        prefixedId.StartsWith(Constants.SamlIdPrefix) ? prefixedId[Constants.SamlIdPrefix.Length..] : prefixedId;
}
