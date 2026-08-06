using Api.Shared.Services;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security.Sso;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services.Cache;
using Polly;
using Constants = Enterprise.Shared.Security.Sso.Models.Constants;
using OrganizationSsoValidationResult = Organization.Shared.Configurations.OrganizationSsoValidationResult;

namespace Organization.Api.Services;

public interface IOrganizationSsoService
{
    Task<bool> IsSsoTokenValidAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken);

    Task<string> SsoLoginAsync(
        string? organizationId,
        string? organizationCustomDomain,
        string redirectUrl,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdatePatchAsync(
        OrganizationSsoSettingsPatchRequest request,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> RemoveAsync(
        string? organizationId,
        string? organizationCustomDomain,
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
    IGraphQlMapper graphQlMapper,
    IDbTransactionBuilder transactionBuilder,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    IContext context,
    ICachedOrganizationService cachedOrganizationService) : IOrganizationSsoService
{
    private const int MaxPatchConcurrencyRetryCount = 1;
    private const string PostgresUniqueViolationSqlState = "23505";

    public async Task<bool> IsSsoTokenValidAsync(
        string? organizationId,
        string? organizationCustomDomain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationCustomDomain);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationCustomDomainAsync(
            organizationId,
            organizationCustomDomain,
            cancellationToken);
        if (ssoSettings is null || !ssoSettings.IsActive)
        {
            return true;
        }

        var userSsoContext = context.GetUserSsoContext(organizationCustomDomain);
        if (userSsoContext is null)
        {
            return false;
        }

        return customer.Identities.Any(item =>
            !string.IsNullOrWhiteSpace(item.Email) && item.Email.Equals(userSsoContext.Email, StringComparison.InvariantCultureIgnoreCase));
    }

    public async Task<string> SsoLoginAsync(
        string? organizationId,
        string? organizationCustomDomain,
        string redirectUrl,
        CancellationToken cancellationToken)
    {
        var existingOrganizationSsoSetting = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationCustomDomainAsync(
            organizationId,
            organizationCustomDomain,
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

        if (!string.IsNullOrWhiteSpace(organizationCustomDomain))
        {
            return samlLoginRequestFactory.GenerateSamlLoginRequest(
                $"uniquename{organizationCustomDomain}",
                redirectUrl,
                existingOrganizationSsoSetting.EntityId,
                existingOrganizationSsoSetting.LoginUrl);
        }

        throw new InvalidOperationException("Either id or customDomain must be provided.");
    }

    public async Task<Shared.Models.Organization> UpdatePatchAsync(
        OrganizationSsoSettingsPatchRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePatchRequest(request);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);

        return await Policy
            .Handle<DbUpdateConcurrencyException>()
            .Or<DbUpdateException>(IsUniqueViolation)
            .WaitAndRetryAsync(
                MaxPatchConcurrencyRetryCount,
                _ => TimeSpan.Zero,
                (_, _, _, _) =>
                {
                    repositoryFactory.OrganizationRepository.ClearTrackedEntities();
                })
            .ExecuteAsync(async () =>
            {
                var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                                       request.OrganizationId,
                                       request.OrganizationCustomDomain,
                                       cancellationToken) ??
                                   throw new OrganizationNotFound();

                if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
                {
                    throw new UnauthorizedAccessException();
                }

                return await UpdatePatchInternalAsync(request.SsoSettings, organization, cancellationToken);
            });
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

        if (organization.OrganizationSsoSettings is null)
        {
            return graphQlMapper.MapTo(organization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organization.OrganizationSsoSettings.IsActive = false;
        repositoryFactory.OrganizationSsoSettingsRepository.Update(organization.OrganizationSsoSettings);

        var mappedOrganization = graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

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
            ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationCustomDomainAsync(
                              samlOriginId["id".Length..],
                              null,
                              cancellationToken) ??
                          throw new OrganizationSsoIsNotYetSetup();
        }
        else if (samlOriginId.StartsWith("uniquename"))
        {
            ssoSettings = await repositoryFactory.OrganizationSsoSettingsRepository.GetByOrganizationCustomDomainAsync(
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

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresUniqueViolationSqlState };

    private async Task<Shared.Models.Organization> UpdatePatchInternalAsync(
        OrganizationSsoSettings ssoSettings,
        Shared.Database.Entities.Organization organization,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(ssoSettings.Organization);

        // Validate active SSO settings before enabling sign-in.
        var validationResult = ssoSettings.IsActive
            ? await ValidateSsoConfigurationAsync(ssoSettings, cancellationToken)
            : new OrganizationSsoValidationResult
            {
                IsMetadataValid = true,
                IsCertificateValid = true,
            };
        if (!validationResult.IsMetadataValid || !validationResult.IsCertificateValid)
        {
            throw new InvalidSsoConfiguration();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationSsoSettings is null)
        {
            ssoSettings.Id = randomHelper.Generate();
            var organizationSsoSettingsEntity = graphQlMapper.MapToEntity(ssoSettings, organization);
            repositoryFactory.OrganizationSsoSettingsRepository.Add(organizationSsoSettingsEntity);
            organization.OrganizationSsoSettings = organizationSsoSettingsEntity;
        }
        else
        {
            ssoSettings.Id = organization.OrganizationSsoSettings.Id;
            var organizationSsoSettingsEntity = graphQlMapper.MergeToEntity(ssoSettings, organization.OrganizationSsoSettings, organization);
            repositoryFactory.OrganizationSsoSettingsRepository.Update(organizationSsoSettingsEntity);
        }

        var mappedOrganization = graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        organizationOutboxPublisher.PublishOrganizations([mappedOrganization], repositoryFactory.UnitOfWork);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return mappedOrganization;
    }

    private static void ValidatePatchRequest(OrganizationSsoSettingsPatchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.SsoSettings);

        if (string.IsNullOrWhiteSpace(request.OrganizationId) && string.IsNullOrWhiteSpace(request.OrganizationCustomDomain))
        {
            throw new ArgumentException("Either organisation id or custom domain must be provided.", nameof(request));
        }

        if (!request.FieldsToUpdate.SetEquals([OrganizationSsoSettingsPatchField.SsoSettings]))
        {
            throw new ArgumentException("Only SSO settings can be patched by this request.", nameof(request));
        }
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
