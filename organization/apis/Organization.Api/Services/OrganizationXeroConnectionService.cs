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
using Organization.Api.Services.Authorization;
using Organization.Shared.Models;
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

    Task<Shared.Models.Organization> UpdateAsync(OrganizationXeroConnection xeroConnection, CancellationToken cancellationToken);
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
    IDbTransactionBuilder transactionBuilder,
    IRandomHelper randomHelper,
    TimeProvider timeProvider) : IOrganizationXeroConnectionService
{
    private static readonly HashSet<OrganizationXeroBillingMode> s_allowedBillingModes =
    [
        OrganizationXeroBillingMode.Disabled,
        OrganizationXeroBillingMode.Enabled,
        OrganizationXeroBillingMode.RepeatingInvoices
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
            AutoReconcilePayments = true
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

            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

            return BuildMarketplaceSetupUri(
                organization.CustomDomain ?? throw new OrganizationNotFound(),
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

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return BuildMarketplaceSetupUri(organization.CustomDomain ?? throw new OrganizationNotFound());
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

        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return new OrganizationXeroConnection
        {
            Id = organizationXeroConnection.Id,
            Organization =
                new Shared.Models.Organization { Id = organization.Id, CustomDomain = organization.CustomDomain, Name = organization.Name },
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
            LastError = organizationXeroConnection.LastError
        };
    }

    public async Task<Shared.Models.Organization> UpdateAsync(OrganizationXeroConnection xeroConnection, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(xeroConnection.Organization);

        ValidateBillingMode(xeroConnection.BillingMode);

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               xeroConnection.Organization.Id,
                               xeroConnection.Organization.CustomDomain,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        if (!await organizationAuthorizationService.CanModifyAsync(organization, customer.Id, cancellationToken))
        {
            throw new OrganizationXeroConnectionUnauthorizedException();
        }

        await using var transaction = await transactionBuilder.BeginTransactionAsync(repositoryFactory.UnitOfWork, cancellationToken);

        if (organization.OrganizationXeroConnection is null)
        {
            xeroConnection.Id = randomHelper.Generate();
            organization.OrganizationXeroConnection =
                repositoryFactory.OrganizationXeroConnectionRepository.Add(graphQlMapper.MapToEntity(xeroConnection, organization));
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.RefreshTokenEncrypted) &&
                !string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.TenantId) &&
                !string.Equals(organization.OrganizationXeroConnection.TenantId, xeroConnection.TenantId, StringComparison.OrdinalIgnoreCase))
            {
                throw new XeroTenantReconnectRequiredException();
            }

            xeroConnection.Id = organization.OrganizationXeroConnection.Id;
            organization.OrganizationXeroConnection = repositoryFactory.OrganizationXeroConnectionRepository.Update(
                graphQlMapper.MergeToEntity(xeroConnection, organization.OrganizationXeroConnection, organization));
        }

        if (organization.OrganizationXeroConnection is not null &&
            organization.OrganizationXeroConnection.IsActive &&
            string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.RefreshTokenEncrypted))
        {
            throw new XeroActivationRequiresConnectionException();
        }

        if (organization.OrganizationXeroConnection is not null &&
            organization.OrganizationXeroConnection.IsActive &&
            string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.TenantId))
        {
            throw new XeroActivationRequiresTenantSelectionException();
        }

        if (organization.OrganizationXeroConnection is not null &&
            !string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.TenantId) &&
            !string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.RefreshTokenEncrypted))
        {
            await ValidateTenantSelectionAsync(organization.OrganizationXeroConnection, organization.OrganizationXeroConnection.TenantId,
                cancellationToken);
        }

        if (organization.OrganizationXeroConnection is not null &&
            !string.IsNullOrWhiteSpace(organization.OrganizationXeroConnection.RefreshTokenEncrypted) &&
            organization.OrganizationXeroConnection.RefreshTokenExpiresAt is not null)
        {
            temporalOutboxService.StartWorkflowMaintainOrganizationXeroConnection(
                new MaintainOrganizationXeroConnectionInput(
                    organization.Id,
                    xeroTokenRefreshService.GetNextMaintenanceAt(organization.OrganizationXeroConnection.RefreshTokenExpiresAt.Value)),
                repositoryFactory.UnitOfWork);
        }

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

    private Uri BuildMarketplaceSetupUri(string organizationCustomDomain, IReadOnlyList<XeroTenantOption>? tenantOptions = null,
        string? message = null)
    {
        var setupUrl = Url.Combine(
                applicationConfiguration.WebAppBaseDomain.ToString(),
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
            .Where(item => string.Equals(item.TenantType, "ORGANISATION", StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault(item => string.Equals(item.TenantId?.ToString(), tenantId, StringComparison.OrdinalIgnoreCase));
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
            .Where(item => string.Equals(item.TenantType, "ORGANISATION", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var tenantOptions = organizationConnections
            .Where(item => item.TenantId.HasValue)
            .Select(item => new XeroTenantOption(item.TenantId!.Value.ToString(), item.TenantName ?? string.Empty))
            .ToList();

        if (!string.IsNullOrWhiteSpace(existingTenantId))
        {
            var existingConnection = organizationConnections.FirstOrDefault(item =>
                string.Equals(item.TenantId.ToString(), existingTenantId, StringComparison.OrdinalIgnoreCase));
            if (existingConnection is not null)
            {
                return new TenantConnectionSelection(existingConnection, []);
            }
        }

        return organizationConnections.Count switch
        {
            1 => new TenantConnectionSelection(organizationConnections[0], []),
            > 1 => new TenantConnectionSelection(null, tenantOptions),
            _ => throw new NoXeroOrganizationTenantConnectionsException()
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
