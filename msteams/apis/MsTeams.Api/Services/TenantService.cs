using Ardalis.GuardClauses;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MsTeams.Api.Mappers;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;
using Organization = MsTeams.Shared.Models.Organization;

namespace MsTeams.Api.Services;

public interface ITenantService
{
    Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken);
    Task<string> GenerateAdminConsentUrlAsync(CancellationToken cancellationToken);
    Task<Uri> InstallAsync(string tenantId, string state, CancellationToken cancellationToken);
    Task<Organization?> GetAttachedOrganizationAsync(CancellationToken cancellationToken);
}

public class TenantService(
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IContext context,
    IMemoryCache memoryCache,
    IRandomHelper randomHelper,
    MsTeamsAzureEntraConfiguration msTeamsAzureEntraConfiguration,
    IHttpContextAccessor httpContextAccessor,
    ITenantOnboardingService tenantOnboardingService,
    IMsTeamsInternalOutboxPublisher msTeamsInternalOutboxPublisher,
    IMapper mapper) : ITenantService
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

    public async Task<string> GenerateAdminConsentUrlAsync(CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(context.PropertyBag.VerifiableToken);
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        var currentUri = UriHelper.BuildAbsolute(
            httpContextAccessor.HttpContext.Request.Scheme,
            httpContextAccessor.HttpContext.Request.Host,
            httpContextAccessor.HttpContext.Request.PathBase);

        var installStateUserIdLookup = repositoryFactory.InstallStateUserIdLookupRepository.Add(
            new InstallStateUserIdLookup
            {
                Id = randomHelper.Generate(), InstalledByUserId = context.PropertyBag.VerifiableToken
            });

        var tenantId = context.PropertyBag.AzureTenantId;
        var clientId = Uri.EscapeDataString(msTeamsAzureEntraConfiguration.ClientId);
        var redirectUri = Uri.EscapeDataString(currentUri + "msteams/api/v1/onboard-tenant");
        var scope = Uri.EscapeDataString("User.ReadBasic.All");
        var authorizationRequest =
            $"https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&state={installStateUserIdLookup.Id}";

        await repositoryFactory.InstallStateUserIdLookupRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return authorizationRequest;
    }

    public async Task<Uri> InstallAsync(string tenantId, string state, CancellationToken cancellationToken)
    {
        var installStateUserIdLookup =
            await repositoryFactory.InstallStateUserIdLookupRepository.GetByIdAsync(state, cancellationToken);
        ArgumentNullException.ThrowIfNull(installStateUserIdLookup);

        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByTenantIdAsync(tenantId, cancellationToken);

        if (organization is null)
        {
            await tenantOnboardingService.OnboardAsync(
                tenantId,
                installStateUserIdLookup,
                cancellationToken);
        }
        else
        {
            await using var transaction =
                await transactionBuilder.BeginTransactionAsync(
                    repositoryFactory.TenantRepository.UnitOfWork,
                    cancellationToken);

            repositoryFactory.InstallStateUserIdLookupRepository.Remove(installStateUserIdLookup);

            var tenant =
                await repositoryFactory.TenantRepository.GetByIdAsync(tenantId, cancellationToken);
            ArgumentNullException.ThrowIfNull(tenant);
            tenant = repositoryFactory.TenantRepository.Update(tenant);
            await msTeamsInternalOutboxPublisher.PublishRefreshTenantMembersAsync(
                [tenant.Id],
                repositoryFactory.TenantRepository.UnitOfWork,
                cancellationToken);
            await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return new Uri("https://teams.microsoft.com/v2/");
    }

    public async Task<Organization?> GetAttachedOrganizationAsync(CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);

        var tenantId = context.PropertyBag.AzureTenantId;
        var tenant = await repositoryFactory.TenantRepository.Query(
            new Specification<Tenant>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Id == tenantId.ToString()
            }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        return tenant is null ? null : mapper.MapTo(tenant.Organization);
    }
}
