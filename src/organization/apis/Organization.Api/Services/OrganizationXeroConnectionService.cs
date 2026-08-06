using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.OpenApi.Skedular.Organization.Core.V1;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Accounting.Configurations;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Flurl;
using Microsoft.AspNetCore.Mvc;
using Organization.Api.Mappers;
using Organization.Api.Models;
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Organization.Shared.Workflows;
using Xero.NetStandard.OAuth2.Model.Identity;

namespace Organization.Api.Services;

public interface IOrganizationXeroConnectionService
{
    Task<Uri> GetAuthorizeUrlAsync(string? organizationId, string? organizationCustomDomain, CancellationToken cancellationToken);
    Task<Uri> ConnectAsync(string code, string state, CancellationToken cancellationToken);

    Task<OrganizationXeroConnection?> RefreshTokensAsync(
        string organizationId,
        string accessTokenEncrypted,
        string refreshTokenEncrypted,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset refreshTokenExpiresAt,
        CancellationToken cancellationToken);

    Task<Shared.Models.Organization> UpdatePatchAsync(OrganizationXeroConnectionPatchRequest request, CancellationToken cancellationToken);
    Task<Shared.Models.Organization> RemoveAsync(string? organizationId, string? organizationCustomDomain, CancellationToken cancellationToken);
}

public class OrganizationXeroConnectionService(
    ApplicationConfiguration applicationConfiguration,
    XeroConfiguration xeroConfiguration,
    IRepositoryFactory repositoryFactory,
    ICustomerService customerService,
    IOrganizationAuthorizationService organizationAuthorizationService,
    IOrganizationStripeConnectAccountService organizationStripeConnectAccountService,
    IXeroTokenRefreshService xeroTokenRefreshService,
    IXeroTokenEncryptionService xeroTokenEncryptionService,
    IXeroSdkClientFactory xeroSdkClientFactory,
    ICachedOrganizationService cachedOrganizationService,
    ITemporalOutboxService temporalOutboxService,
    IGraphQlMapper graphQlMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    IDbTransactionBuilder transactionBuilder,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IOrganizationXeroConnectionService
{
    private static readonly HashSet<OrganizationXeroBillingMode> s_allowedBillingModes =
    [
        OrganizationXeroBillingMode.Disabled,
        OrganizationXeroBillingMode.Enabled,
        OrganizationXeroBillingMode.RepeatingInvoices,
    ];

    private static readonly Lazy<string> s_xeroOAuthCallbackBaseUrl = new(() =>
    {
        var method = typeof(OrganizationCoreControllerBase).GetMethod(nameof(OrganizationCoreControllerBase.XeroOAuthCallback));
        ArgumentNullException.ThrowIfNull(method);

        var routeAttribute = method.GetCustomAttributes(typeof(RouteAttribute), true).Cast<RouteAttribute>().First();
        ArgumentNullException.ThrowIfNull(routeAttribute);

        return routeAttribute.Template;
    });

    public async Task<Uri> GetAuthorizeUrlAsync(
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
            throw new OrganizationXeroConnectionUnauthorizedException();
        }

        var state = xeroTokenEncryptionService.Encrypt(JsonSerializer.Serialize(new XeroAuthorizeState(organization.Id)));
        var redirectUri = Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), s_xeroOAuthCallbackBaseUrl.Value);

        return new Uri(
            xeroConfiguration.AuthorizeEndpoint
                .SetQueryParam("response_type", "code")
                .SetQueryParam("client_id", xeroConfiguration.ClientId)
                .SetQueryParam("redirect_uri", redirectUri)
                .SetQueryParam("scope", xeroConfiguration.Scopes)
                .SetQueryParam("state", state));
    }

    public async Task<Uri> ConnectAsync(string code, string state, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);

        var authorizeState = JsonSerializer.Deserialize<XeroAuthorizeState>(xeroTokenEncryptionService.Decrypt(state)) ??
                             throw new InvalidXeroAuthorizeStateException();
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               authorizeState.OrganizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();

        var redirectUri = new Uri(Url.Combine(applicationConfiguration.ApiBaseDomain.ToString(), s_xeroOAuthCallbackBaseUrl.Value));
        var tokenResponse = await xeroSdkClientFactory.CreateClient(redirectUri).RequestAccessTokenAsync(code) ??
                            throw new EmptyXeroTokenResponseException();

        var identityApi = xeroSdkClientFactory.CreateIdentityApi();
        var connections = await identityApi.GetConnectionsAsync(tokenResponse.AccessToken, null, cancellationToken);
        var (connection, iReadOnlyCollection) = SelectTenantConnection(organization.OrganizationXeroConnection?.TenantId, connections);

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var now = timeProvider.GetUtcNow();
        var organizationXeroConnection = organization.OrganizationXeroConnection ?? new Shared.Database.Entities.OrganizationXeroConnection
        {
            Id = randomHelper.Generate(),
            Organization = organization,
            BillingMode = XeroBillingModeConstants.Disabled,
            SendInvoicesViaXero = true,
            AutoReconcilePayments = true,
        };
        organizationXeroConnection.Scopes = xeroConfiguration.Scopes;
        organizationXeroConnection.AccessTokenEncrypted = xeroTokenEncryptionService.Encrypt(tokenResponse.AccessToken);
        organizationXeroConnection.RefreshTokenEncrypted = xeroTokenEncryptionService.Encrypt(tokenResponse.RefreshToken);
        organizationXeroConnection.AccessTokenExpiresAt = tokenResponse.ExpiresAtUtc;
        organizationXeroConnection.RefreshTokenExpiresAt = now.AddDays(60);
        organizationXeroConnection.LastError = null;
        organizationXeroConnection.LastSuccessfulSyncAt = now;

        if (connection is null)
        {
            organizationXeroConnection.TenantId = string.Empty;
            organizationXeroConnection.TenantName = string.Empty;
            organizationXeroConnection.IsActive = false;
            organization.OrganizationXeroConnection = organization.OrganizationXeroConnection is null
                ? repositoryFactory.OrganizationXeroConnectionRepository.Add(organizationXeroConnection)
                : repositoryFactory.OrganizationXeroConnectionRepository.Update(organizationXeroConnection);

            temporalOutboxService.StartWorkflowMaintainOrganizationXeroConnection(
                new MaintainOrganizationXeroConnectionInput(
                    organization.Id,
                    xeroTokenRefreshService.GetNextMaintenanceAt(organization.OrganizationXeroConnection.RefreshTokenExpiresAt!.Value)),
                repositoryFactory.UnitOfWork);
            PublishOrganization(organization);

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

            return BuildMarketplaceSetupUri(
                organization.CustomDomain ?? throw new OrganizationNotFound(),
                organization.Type,
                iReadOnlyCollection,
                "Choose the Xero tenant you want to use, then save the settings to finish the connection.");
        }

        organizationXeroConnection.TenantId = connection.TenantId?.ToString() ?? string.Empty;
        organizationXeroConnection.TenantName = connection.TenantName ?? string.Empty;
        organizationXeroConnection.IsActive = true;

        organization.OrganizationXeroConnection = organization.OrganizationXeroConnection is null
            ? repositoryFactory.OrganizationXeroConnectionRepository.Add(organizationXeroConnection)
            : repositoryFactory.OrganizationXeroConnectionRepository.Update(organizationXeroConnection);

        temporalOutboxService.StartWorkflowMaintainOrganizationXeroConnection(
            new MaintainOrganizationXeroConnectionInput(
                organization.Id,
                xeroTokenRefreshService.GetNextMaintenanceAt(organization.OrganizationXeroConnection.RefreshTokenExpiresAt!.Value)),
            repositoryFactory.UnitOfWork);
        PublishOrganization(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return BuildMarketplaceSetupUri(
            organization.CustomDomain ?? throw new OrganizationNotFound(),
            organization.Type);
    }

    public async Task<OrganizationXeroConnection?> RefreshTokensAsync(
        string organizationId,
        string accessTokenEncrypted,
        string refreshTokenEncrypted,
        DateTimeOffset accessTokenExpiresAt,
        DateTimeOffset refreshTokenExpiresAt,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               organizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var organizationXeroConnection = organization.OrganizationXeroConnection;
        if (organizationXeroConnection is null)
        {
            return null;
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        organizationXeroConnection.AccessTokenEncrypted = accessTokenEncrypted;
        organizationXeroConnection.RefreshTokenEncrypted = refreshTokenEncrypted;
        organizationXeroConnection.AccessTokenExpiresAt = accessTokenExpiresAt;
        organizationXeroConnection.RefreshTokenExpiresAt = refreshTokenExpiresAt;
        organizationXeroConnection.LastSuccessfulSyncAt = timeProvider.GetUtcNow();
        organizationXeroConnection.LastError = null;

        organization.OrganizationXeroConnection =
            repositoryFactory.OrganizationXeroConnectionRepository.Update(organizationXeroConnection);

        temporalOutboxService.StartWorkflowMaintainOrganizationXeroConnection(
            new MaintainOrganizationXeroConnectionInput(
                organization.Id,
                xeroTokenRefreshService.GetNextMaintenanceAt(refreshTokenExpiresAt)),
            repositoryFactory.UnitOfWork);
        PublishOrganization(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return new OrganizationXeroConnection
        {
            Id = organizationXeroConnection.Id,
            Organization =
                new Shared.Models.Organization
                {
                    Id = organization.Id,
                    CustomDomain = organization.CustomDomain,
                    Name = organization.Name,
                },
            TenantId = organizationXeroConnection.TenantId,
            TenantName = organizationXeroConnection.TenantName,
            BillingMode = organizationXeroConnection.BillingMode.ToOrganizationXeroBillingMode(),
            Scopes = organizationXeroConnection.Scopes,
            IsActive = organizationXeroConnection.IsActive,
            SendInvoicesViaXero = organizationXeroConnection.SendInvoicesViaXero,
            AutoReconcilePayments = organizationXeroConnection.AutoReconcilePayments,
            DefaultSalesAccountCode = organizationXeroConnection.DefaultSalesAccountCode,
            DefaultReceivablesAccountCode = organizationXeroConnection.DefaultReceivablesAccountCode,
            DefaultTrackingCategory1 = organizationXeroConnection.DefaultTrackingCategory1,
            DefaultTrackingCategory2 = organizationXeroConnection.DefaultTrackingCategory2,
            DefaultBrandingThemeId = organizationXeroConnection.DefaultBrandingThemeId,
            DefaultReferencePrefix = organizationXeroConnection.DefaultReferencePrefix,
            AccessTokenEncrypted = organizationXeroConnection.AccessTokenEncrypted,
            RefreshTokenEncrypted = organizationXeroConnection.RefreshTokenEncrypted,
            AccessTokenExpiresAt = organizationXeroConnection.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = organizationXeroConnection.RefreshTokenExpiresAt,
            LastSuccessfulSyncAt = organizationXeroConnection.LastSuccessfulSyncAt,
            LastError = organizationXeroConnection.LastError,
        };
    }

    public async Task<Shared.Models.Organization> UpdatePatchAsync(
        OrganizationXeroConnectionPatchRequest request,
        CancellationToken cancellationToken)
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
            throw new OrganizationXeroConnectionUnauthorizedException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        var xeroConnection = organization.OrganizationXeroConnection;
        var created = xeroConnection is null;
        if (xeroConnection is null)
        {
            xeroConnection = new Shared.Database.Entities.OrganizationXeroConnection
            {
                Id = randomHelper.Generate(),
                Organization = organization,
                BillingMode = XeroBillingModeConstants.Disabled,
                SendInvoicesViaXero = true,
                AutoReconcilePayments = true,
            };
        }

        if (!ApplyPatch(request, xeroConnection))
        {
            return graphQlMapper.MapTo(
                organization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        ValidateConnectionState(xeroConnection);
        if (!string.IsNullOrWhiteSpace(xeroConnection.TenantId) &&
            !string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted))
        {
            await ValidateTenantSelectionAsync(xeroConnection, xeroConnection.TenantId, cancellationToken);
        }

        organization.OrganizationXeroConnection = created
            ? repositoryFactory.OrganizationXeroConnectionRepository.Add(xeroConnection)
            : repositoryFactory.OrganizationXeroConnectionRepository.Update(xeroConnection);

        if (!string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted) &&
            xeroConnection.RefreshTokenExpiresAt is not null)
        {
            temporalOutboxService.StartWorkflowMaintainOrganizationXeroConnection(
                new MaintainOrganizationXeroConnectionInput(
                    organization.Id,
                    xeroTokenRefreshService.GetNextMaintenanceAt(xeroConnection.RefreshTokenExpiresAt.Value)),
                repositoryFactory.UnitOfWork);
        }

        PublishOrganization(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
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
            throw new OrganizationXeroConnectionUnauthorizedException();
        }

        if (organization.OrganizationXeroConnection is null)
        {
            return graphQlMapper.MapTo(
                organization,
                organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        _ = repositoryFactory.OrganizationXeroConnectionRepository.Remove(organization.OrganizationXeroConnection);
        organization.OrganizationXeroConnection = null;
        PublishOrganization(organization);

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return graphQlMapper.MapTo(
            organization,
            organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id));
    }

    private static void ValidateBillingMode(OrganizationXeroBillingMode billingMode)
    {
        if (!s_allowedBillingModes.Contains(billingMode))
        {
            throw new UnsupportedXeroBillingModeException(billingMode.ToString());
        }
    }

    private void PublishOrganization(Shared.Database.Entities.Organization organization) =>
        organizationOutboxPublisher.PublishOrganizations(
            [graphQlMapper.MapTo(organization, organizationStripeConnectAccountService.GetStripeAuthorizeExistingConnectAccountUrl(organization.Id))],
            repositoryFactory.UnitOfWork);

    private static void ValidatePatchRequest(OrganizationXeroConnectionPatchRequest request)
    {
        if (request.FieldsToUpdate.Count == 0)
        {
            throw new ArgumentException("Choose at least one organisation Xero connection field to update.", nameof(request));
        }

        foreach (var field in request.FieldsToUpdate)
        {
            if (!Enum.IsDefined(field))
            {
                throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation Xero connection patch field is not supported.");
            }

            if (field == OrganizationXeroConnectionPatchField.BillingMode)
            {
                if (request.BillingMode is null)
                {
                    throw new ArgumentException("Organisation Xero billing mode is required.", nameof(request));
                }

                ValidateBillingMode(request.BillingMode.Value);
            }

            if (field is OrganizationXeroConnectionPatchField.IsActive or
                    OrganizationXeroConnectionPatchField.SendInvoicesViaXero or
                    OrganizationXeroConnectionPatchField.AutoReconcilePayments &&
                GetBooleanValue(request, field) is null)
            {
                throw new ArgumentException("Selected organisation Xero connection patch fields are required.", nameof(request));
            }
        }
    }

    private static bool ApplyPatch(
        OrganizationXeroConnectionPatchRequest request,
        Shared.Database.Entities.OrganizationXeroConnection xeroConnection)
    {
        var changed = false;
        foreach (var field in request.FieldsToUpdate)
        {
            if (field == OrganizationXeroConnectionPatchField.TenantId &&
                !string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted) &&
                !string.IsNullOrWhiteSpace(xeroConnection.TenantId) &&
                !string.Equals(xeroConnection.TenantId, request.TenantId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new XeroTenantReconnectRequiredException();
            }

            changed = field switch
            {
                OrganizationXeroConnectionPatchField.TenantId => ApplyValue(request.TenantId ?? string.Empty, xeroConnection.TenantId,
                    value => xeroConnection.TenantId = value) || changed,
                OrganizationXeroConnectionPatchField.TenantName => ApplyValue(request.TenantName ?? string.Empty, xeroConnection.TenantName,
                    value => xeroConnection.TenantName = value) || changed,
                OrganizationXeroConnectionPatchField.BillingMode =>
                    ApplyValue(request.BillingMode!.Value.ToOrganizationXeroBillingMode(), xeroConnection.BillingMode,
                        value => xeroConnection.BillingMode = value) || changed,
                OrganizationXeroConnectionPatchField.Scopes => ApplyValue(request.Scopes, xeroConnection.Scopes,
                    value => xeroConnection.Scopes = value) || changed,
                OrganizationXeroConnectionPatchField.IsActive => ApplyValue(request.IsActive!.Value, xeroConnection.IsActive,
                    value => xeroConnection.IsActive = value) || changed,
                OrganizationXeroConnectionPatchField.SendInvoicesViaXero =>
                    ApplyValue(request.SendInvoicesViaXero!.Value, xeroConnection.SendInvoicesViaXero,
                        value => xeroConnection.SendInvoicesViaXero = value) || changed,
                OrganizationXeroConnectionPatchField.AutoReconcilePayments =>
                    ApplyValue(request.AutoReconcilePayments!.Value, xeroConnection.AutoReconcilePayments,
                        value => xeroConnection.AutoReconcilePayments = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultSalesAccountCode =>
                    ApplyValue(request.DefaultSalesAccountCode, xeroConnection.DefaultSalesAccountCode,
                        value => xeroConnection.DefaultSalesAccountCode = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultReceivablesAccountCode =>
                    ApplyValue(request.DefaultReceivablesAccountCode, xeroConnection.DefaultReceivablesAccountCode,
                        value => xeroConnection.DefaultReceivablesAccountCode = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultTrackingCategory1 =>
                    ApplyValue(request.DefaultTrackingCategory1, xeroConnection.DefaultTrackingCategory1,
                        value => xeroConnection.DefaultTrackingCategory1 = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultTrackingCategory2 =>
                    ApplyValue(request.DefaultTrackingCategory2, xeroConnection.DefaultTrackingCategory2,
                        value => xeroConnection.DefaultTrackingCategory2 = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultBrandingThemeId =>
                    ApplyValue(request.DefaultBrandingThemeId, xeroConnection.DefaultBrandingThemeId,
                        value => xeroConnection.DefaultBrandingThemeId = value) || changed,
                OrganizationXeroConnectionPatchField.DefaultReferencePrefix =>
                    ApplyValue(request.DefaultReferencePrefix, xeroConnection.DefaultReferencePrefix,
                        value => xeroConnection.DefaultReferencePrefix = value) || changed,
                _ => throw new ArgumentOutOfRangeException(nameof(request), field, "This organisation Xero connection patch field is not supported."),
            };
        }

        return changed;
    }

    private static void ValidateConnectionState(Shared.Database.Entities.OrganizationXeroConnection xeroConnection)
    {
        if (xeroConnection.IsActive && string.IsNullOrWhiteSpace(xeroConnection.RefreshTokenEncrypted))
        {
            throw new XeroActivationRequiresConnectionException();
        }

        if (xeroConnection.IsActive && string.IsNullOrWhiteSpace(xeroConnection.TenantId))
        {
            throw new XeroActivationRequiresTenantSelectionException();
        }
    }

    private static bool? GetBooleanValue(OrganizationXeroConnectionPatchRequest request, OrganizationXeroConnectionPatchField field) =>
        field switch
        {
            OrganizationXeroConnectionPatchField.IsActive => request.IsActive,
            OrganizationXeroConnectionPatchField.SendInvoicesViaXero => request.SendInvoicesViaXero,
            OrganizationXeroConnectionPatchField.AutoReconcilePayments => request.AutoReconcilePayments,
            _ => null,
        };

    private static bool ApplyValue<T>(T value, T currentValue, Action<T> apply)
    {
        if (EqualityComparer<T>.Default.Equals(value, currentValue))
        {
            return false;
        }

        apply(value);
        return true;
    }

    private Uri BuildMarketplaceSetupUri(
        string organizationCustomDomain,
        string organizationType,
        IReadOnlyList<XeroTenantOption>? tenantOptions = null,
        string? message = null)
    {
        var webAppBaseDomain = organizationType switch
        {
            OrganizationTypeConstants.Marketplace => applicationConfiguration.SpacesWebAppBaseDomain,
            OrganizationTypeConstants.Host => applicationConfiguration.HostWebAppBaseDomain,
            _ => applicationConfiguration.WebAppBaseDomain,
        };
        var setupUrl = Url.Combine(
                webAppBaseDomain.ToString(),
                "organizations",
                organizationCustomDomain,
                "setup-marketplace")
            .SetQueryParam("section", "xero-setup");
        if (tenantOptions is { Count: > 0 })
        {
            // These options are only consumed after redirect on the web setup page.
            // They are serialized into the callback query string so the UI can render
            // tenant-choice buttons and prefill tenantId/tenantName before the user saves.
            var serializedOptions = JsonSerializer.Serialize(tenantOptions);
            setupUrl = setupUrl.SetQueryParam("xeroTenantOptions", Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedOptions)));
        }

        if (!string.IsNullOrWhiteSpace(message))
        {
            setupUrl = setupUrl.SetQueryParam("xeroMessage", message);
        }

        return new Uri(setupUrl);
    }

    private async Task ValidateTenantSelectionAsync(
        Shared.Database.Entities.OrganizationXeroConnection organizationXeroConnection,
        string tenantId,
        CancellationToken cancellationToken)
    {
        var accessToken = await EnsureValidAccessTokenAsync(organizationXeroConnection, cancellationToken);
        var connections = await xeroSdkClientFactory.CreateIdentityApi().GetConnectionsAsync(accessToken, null, cancellationToken);
        var matchingConnection = connections
            .Where(item => string.Equals(item.TenantType, "ORGANISATION", StringComparison.InvariantCultureIgnoreCase))
            .FirstOrDefault(item => string.Equals(item.TenantId?.ToString(), tenantId, StringComparison.InvariantCultureIgnoreCase));
        if (matchingConnection is null)
        {
            throw new UnavailableXeroTenantSelectionException();
        }

        organizationXeroConnection.TenantName = matchingConnection.TenantName ?? organizationXeroConnection.TenantName;
    }

    private async Task<string> EnsureValidAccessTokenAsync(
        Shared.Database.Entities.OrganizationXeroConnection organizationXeroConnection,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(organizationXeroConnection.AccessTokenEncrypted) &&
            organizationXeroConnection.AccessTokenExpiresAt is not null &&
            organizationXeroConnection.AccessTokenExpiresAt > timeProvider.GetUtcNow().AddMinutes(1))
        {
            return xeroTokenEncryptionService.Decrypt(organizationXeroConnection.AccessTokenEncrypted);
        }

        var refreshResult = await xeroTokenRefreshService.RefreshAsync(organizationXeroConnection, cancellationToken);
        if (!refreshResult.IsSuccessful ||
            string.IsNullOrWhiteSpace(refreshResult.AccessTokenEncrypted) ||
            string.IsNullOrWhiteSpace(refreshResult.RefreshTokenEncrypted) ||
            refreshResult.AccessTokenExpiresAt is null ||
            refreshResult.RefreshTokenExpiresAt is null)
        {
            throw new XeroTokenRefreshFailedException(refreshResult.Error ?? "Unable to refresh Xero tokens.");
        }

        organizationXeroConnection.AccessTokenEncrypted = refreshResult.AccessTokenEncrypted;
        organizationXeroConnection.RefreshTokenEncrypted = refreshResult.RefreshTokenEncrypted;
        organizationXeroConnection.AccessTokenExpiresAt = refreshResult.AccessTokenExpiresAt.Value;
        organizationXeroConnection.RefreshTokenExpiresAt = refreshResult.RefreshTokenExpiresAt.Value;
        organizationXeroConnection.LastSuccessfulSyncAt = timeProvider.GetUtcNow();
        organizationXeroConnection.LastError = null;

        return xeroTokenEncryptionService.Decrypt(refreshResult.AccessTokenEncrypted);
    }

    private static TenantConnectionSelection SelectTenantConnection(string? existingTenantId, IReadOnlyList<Connection> connections)
    {
        var organizationConnections = connections
            .Where(item => string.Equals(item.TenantType, "ORGANISATION", StringComparison.InvariantCultureIgnoreCase))
            .ToList();
        var tenantOptions = organizationConnections
            .Where(item => item.TenantId.HasValue)
            .Select(item => new XeroTenantOption(item.TenantId!.Value.ToString(), item.TenantName ?? string.Empty))
            .ToList();

        if (!string.IsNullOrWhiteSpace(existingTenantId))
        {
            var existingConnection = organizationConnections.FirstOrDefault(item =>
                string.Equals(item.TenantId.ToString(), existingTenantId, StringComparison.InvariantCultureIgnoreCase));
            if (existingConnection is not null)
            {
                return new TenantConnectionSelection(existingConnection, []);
            }
        }

        return organizationConnections.Count switch
        {
            1 => new TenantConnectionSelection(organizationConnections[0], []),
            > 1 => new TenantConnectionSelection(null, tenantOptions),
            _ => throw new NoXeroOrganizationTenantConnectionsException(),
        };
    }

    private sealed record XeroAuthorizeState(string OrganizationId);

    // This payload is intentionally backend-to-UI only. The web marketplace setup page
    // reads these serialized values from the callback redirect and uses them to render
    // tenant suggestions for multi-tenant Xero connections.
    private sealed record XeroTenantOption(
        [property: JsonPropertyName("tenantId")]
        // ReSharper disable once NotAccessedPositionalProperty.Local
        string TenantId,
        [property: JsonPropertyName("tenantName")]
        // ReSharper disable once NotAccessedPositionalProperty.Local
        string TenantName);

    private sealed record TenantConnectionSelection(Connection? Connection, IReadOnlyList<XeroTenantOption> TenantOptions);
}
