using Ardalis.GuardClauses;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Repositories;

namespace MsTeams.Api.Services;

public interface ITenantService
{
    Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken);
    string GenerateAdminConsentUrl();
}

public class TenantService(
    IRepositoryFactory repositoryFactory,
    IContext context,
    IMemoryCache memoryCache,
    MsTeamsAzureEntraConfiguration msTeamsAzureEntraConfiguration,
    IHttpContextAccessor httpContextAccessor,
    IRandomHelper randomHelper) : ITenantService
{
    public async Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);

        var tenantId = context.PropertyBag.AzureTenantId;
        var key = $"tenant-exists-{tenantId}";
        if (memoryCache.Get<bool>(key))
        {
            return true;
        }

        memoryCache.Remove(key);
        return await memoryCache.GetOrCreateAsync(
            key,
            async cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromHours(1);

                return await repositoryFactory.TenantRepository.Query(
                    new Specification<Tenant>
                    {
                        Criteria = query => !query.DeletedAt.HasValue && query.Id == tenantId.ToString()
                    }).AsNoTracking().AnyAsync(cancellationToken);
            });
    }

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
}
