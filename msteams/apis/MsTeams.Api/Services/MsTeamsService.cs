using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.EntityFrameworkCore;
using MsTeams.Shared.Configurations;
using MsTeams.Shared.Database.Entities;
using MsTeams.Shared.Repositories;
using Tenant = MsTeams.Shared.Database.Entities.Tenant;

namespace MsTeams.Api.Services;

public interface IMsTeamsService
{
    Task<string> GenerateAdminConsentUrl(
        string tenantId,
        string currentUri,
        CancellationToken cancellationToken);

    Task OnBoardTenant(
        string tenantId,
        string? error,
        string? errorMessage,
        bool adminConsent,
        string state,
        CancellationToken cancellationToken);
}

public class MsTeamsService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    AzureEntraConfiguration azureEntraConfiguration) : IMsTeamsService
{
    public async Task<string> GenerateAdminConsentUrl(
        string tenantId,
        string currentUri,
        CancellationToken cancellationToken)
    {
        var authorizedTenant = new TemporaryAuthorizationCode
        {
            Id = randomHelper.Generate(), CreatedAt = timeProvider.GetUtcNow()
        };

        var clientId = Uri.EscapeDataString(azureEntraConfiguration.ClientId);
        var redirectUri = Uri.EscapeDataString(currentUri + "msteams/api/v1/onboard-tenant");
        var state = authorizedTenant.Id;
        var scope = Uri.EscapeDataString("User.ReadBasic.All");
        var authorizationRequest =
            $"https://login.microsoftonline.com/{tenantId}/adminconsent?client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}";

        repositoryFactory.TemporaryAuthorizationCodeRepository.Add(authorizedTenant);
        await repositoryFactory.TemporaryAuthorizationCodeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return authorizationRequest;
    }

    public async Task OnBoardTenant(
        string tenantId,
        string? error,
        string? errorMessage,
        bool adminConsent,
        string state,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException(
                $"onboarding went wrong with error {error} and message {errorMessage}.");
        }

        if (!adminConsent)
        {
            throw new InvalidOperationException("Admin consent is required for the onboarding process.");
        }

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("no tenant Id provided.", nameof(tenantId));
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            throw new ArgumentException("temporary authorization code is empty.", nameof(state));
        }

        var existingTenantQuery = await repositoryFactory.TenantRepository
            .Query(new Specification<Tenant> { Criteria = query => query.Id == tenantId }
                .ApplyOrderBy(query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingTenantQuery is not null)
        {
            return;
        }

        var existingAuthorizationCodeQuery = await repositoryFactory.TemporaryAuthorizationCodeRepository
            .Query(new Specification<TemporaryAuthorizationCode> { Criteria = query => query.Id == state }.ApplyOrderBy(
                query => query.Id))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingAuthorizationCodeQuery is null)
        {
            throw new ArgumentException("The authorization code provided is not valid.");
        }

        var newTenant = new Tenant { Id = tenantId, CreatedAt = timeProvider.GetUtcNow() };

        repositoryFactory.TenantRepository.Add(newTenant);
        await repositoryFactory.TenantRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
