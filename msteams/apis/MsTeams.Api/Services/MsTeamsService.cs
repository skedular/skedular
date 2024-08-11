using Ardalis.GuardClauses;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Repositories;
using Tenant = MsTeams.Shared.Database.Entities.Tenant;

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
    TimeProvider timeProvider,
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

        var clientId = Uri.EscapeDataString(msTeamsAzureEntraConfiguration.ClientId);
        var redirectUri = Uri.EscapeDataString(currentUri + "msteams/api/v1/onboard-tenant");
        var scope = Uri.EscapeDataString("User.ReadBasic.All");
        var authorizationRequest =
            $"https://login.microsoftonline.com/{context.PropertyBag.AzureTenantId}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

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

        var existingTenantQuery = await repositoryFactory.TenantRepository
            .Query(new Specification<Tenant> { Criteria = query => query.Id == tenantId }
                .ApplyOrderBy(query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingTenantQuery is not null)
        {
            return;
        }

        var newTenant = new Tenant { Id = tenantId, CreatedAt = timeProvider.GetUtcNow() };

        repositoryFactory.TenantRepository.Add(newTenant);
        await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
