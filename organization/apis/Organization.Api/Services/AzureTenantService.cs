using Ardalis.GuardClauses;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using Organization.Api.Mappers;
using Organization.Shared.Database.Entities;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Api.Services;

public interface IAzureTenantService
{
    Task<bool> DoesTenantExistAsync(CancellationToken cancellationToken);
    Task<string> GenerateAdminConsentUrlAsync(CancellationToken cancellationToken);
    Task<Uri> InstallAsync(string tenantId, string state, CancellationToken cancellationToken);
    Task<Shared.Models.Organization?> GetAttachedOrganizationAsync(CancellationToken cancellationToken);
}

public class AzureTenantService(
    ApplicationConfiguration applicationConfiguration,
    IDbTransactionBuilder transactionBuilder,
    IRepositoryFactory repositoryFactory,
    IContext context,
    IMemoryCache memoryCache,
    IRandomHelper randomHelper,
    AzureEntraConfiguration azureEntraConfiguration,
    IHttpContextAccessor httpContextAccessor,
    IAzureTenantOnboardingService azureTenantOnboardingService,
    IOrganizationInternalOutboxPublisher organizationInternalOutboxPublisher,
    IMapper mapper) : IAzureTenantService
{
    private static readonly string[] s_userProfilePermissions =
        ["User.ReadBasic.All", "ProfilePhoto.Read.All", "email", "offline_access", "openid"];

    private static readonly string[] s_teamPermissions =
    [
        "Team.ReadBasic.All" // List teams
    ];

    private static readonly string[] s_channelPermissions =
    [
        "Group.ReadWrite.All", // Maintain channel
        "ChannelSettings.ReadWrite.All", // Archive Channel
        "Teamwork.Migrate.All" // Send chatMessage in channel
    ];

    private static readonly string[] s_allPermissions =
        s_userProfilePermissions.Concat(s_teamPermissions).Concat(s_channelPermissions).ToArray();

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

                return await repositoryFactory.AzureTenantRepository.Query(
                    new Specification<AzureTenant>
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

        var currentUri = string.IsNullOrWhiteSpace(applicationConfiguration.ApiBaseDomain)
            ? UriHelper.BuildAbsolute(
                httpContextAccessor.HttpContext.Request.Scheme,
                httpContextAccessor.HttpContext.Request.Host,
                httpContextAccessor.HttpContext.Request.PathBase)
            : applicationConfiguration.ApiBaseDomain;

        var installStateUserIdLookup = repositoryFactory.AzureInstallStateUserIdLookupRepository.Add(
            new AzureInstallStateUserIdLookup
            {
                Id = randomHelper.Generate(), InstalledByUserId = context.PropertyBag.VerifiableToken
            });

        var tenantId = context.PropertyBag.AzureTenantId;
        var clientId = Uri.EscapeDataString(azureEntraConfiguration.ClientId);
        var redirectUri = Uri.EscapeDataString(new Uri(new Uri(currentUri), "organization/api/v1/onboard-azure-tenant")
            .OriginalString);
        var scope = Uri.EscapeDataString(Strings.Join(s_allPermissions)!);
        var authorizationRequest =
            $"https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}&scope={scope}&state={installStateUserIdLookup.Id}";

        await repositoryFactory.AzureInstallStateUserIdLookupRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return authorizationRequest;
    }

    public async Task<Uri> InstallAsync(string tenantId, string state, CancellationToken cancellationToken)
    {
        var installStateUserIdLookup =
            await repositoryFactory.AzureInstallStateUserIdLookupRepository.GetByIdAsync(state, cancellationToken);
        ArgumentNullException.ThrowIfNull(installStateUserIdLookup);

        ArgumentException.ThrowIfNullOrWhiteSpace(tenantId);
        var organization =
            await repositoryFactory.OrganizationRepository.GetByAzureTenantIdAsync(tenantId, cancellationToken);

        if (organization is null)
        {
            await azureTenantOnboardingService.OnboardAsync(
                tenantId,
                installStateUserIdLookup,
                cancellationToken);
        }
        else
        {
            await using var transaction =
                await transactionBuilder.BeginTransactionAsync(
                    repositoryFactory.AzureTenantRepository.UnitOfWork,
                    cancellationToken);

            repositoryFactory.AzureInstallStateUserIdLookupRepository.Remove(installStateUserIdLookup);

            var tenant =
                await repositoryFactory.AzureTenantRepository.GetByIdAsync(tenantId, cancellationToken);
            ArgumentNullException.ThrowIfNull(tenant);
            tenant = repositoryFactory.AzureTenantRepository.Update(tenant);
            await organizationInternalOutboxPublisher.PublishRefreshAzureTenantMembersAsync(
                [tenant.Id],
                repositoryFactory.AzureTenantRepository.UnitOfWork,
                cancellationToken);
            await repositoryFactory.AzureTenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        return new Uri("https://teams.microsoft.com/v2/");
    }

    public async Task<Shared.Models.Organization?> GetAttachedOrganizationAsync(CancellationToken cancellationToken)
    {
        Guard.Against.NullOrEmpty(context.PropertyBag.AzureTenantId);

        var tenantId = context.PropertyBag.AzureTenantId;
        var tenant = await repositoryFactory.AzureTenantRepository.Query(
            new Specification<AzureTenant>
            {
                Criteria = query => !query.DeletedAt.HasValue && query.Id == tenantId.ToString()
            }).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        return tenant is null ? null : mapper.MapTo(tenant.Organization);
    }
}
