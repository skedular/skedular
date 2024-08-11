using Ardalis.GuardClauses;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Random;
using Microsoft.AspNetCore.Http.Extensions;
using MsTeams.Shared.Repositories;

namespace MsTeams.Api.Services;

public interface IMsTeamsService
{
    string GenerateAdminConsentUrl();

    Task OnBoardTenant(
        string tenantId,
        string? error,
        string? errorMessage,
        CancellationToken cancellationToken);
}

public class MsTeamsService(
    MsTeamsAzureEntraConfiguration msTeamsAzureEntraConfiguration,
    IHttpContextAccessor httpContextAccessor,
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    IContext context) : IMsTeamsService
{
    public string GenerateAdminConsentUrl()
    {
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        var currentUri = UriHelper.BuildAbsolute(
            httpContextAccessor.HttpContext.Request.Scheme,
            httpContextAccessor.HttpContext.Request.Host,
            httpContextAccessor.HttpContext.Request.PathBase);

        var tenantId = context.PropertyBag.AzureTenantId;
        var clientId = Uri.EscapeDataString(msTeamsAzureEntraConfiguration.ClientId);
        var redirectUri = Uri.EscapeDataString(currentUri + "msteams/api/v1/onboard-tenant");
        var scope = Uri.EscapeDataString("User.ReadBasic.All");
        var state = randomHelper.Generate();
        var authorizationRequest =
            $"https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&state={state}";

        return authorizationRequest;
    }

    public async Task OnBoardTenant(
        string tenantId,
        string? error,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException(
                $"onboarding went wrong with error {error} and message {errorMessage}.");
        }

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("no tenant Id provided.", nameof(tenantId));
        }

        await repositoryFactory.TenantRepository.UpsertNakedAsync(tenantId, cancellationToken);
        await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
