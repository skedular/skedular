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
    Task<string> GenerateTemporaryAuthorizationCode(string currentUri, CancellationToken cancellationToken);

    Task OnBoardTenant(string tenantId, string? error, string? errorMessage, bool adminConsent,
        string temporaryAuthorizationCode, CancellationToken cancellationToken);
}

public class MsTeamsService(
    IRepositoryFactory repositoryFactory,
    IRandomHelper randomHelper,
    TimeProvider timeProvider,
    AzureAdConfiguration azureAdConfiguration,
    GraphApiConfiguration graphApiConfiguration) : IMsTeamsService
{
    public async Task<string> GenerateTemporaryAuthorizationCode(string currentUri, CancellationToken cancellationToken)
    {
        var authorizedTenant = new TemporaryAuthorizationCode
        {
            Id = randomHelper.Generate(), CreatedAt = timeProvider.GetUtcNow()
        };

        var authorizationRequest = string.Format(
            "{0}organizations/v2.0/adminconsent?client_id={1}&redirect_uri={2}&state={3}&scope={4}",
            azureAdConfiguration.Instance,
            Uri.EscapeDataString(azureAdConfiguration.ClientId),
            Uri.EscapeDataString(currentUri + "msteams/api/v1/onboard-tenant"),
            Uri.EscapeDataString(authorizedTenant.Id),
            Uri.EscapeDataString(graphApiConfiguration.Scopes));

        repositoryFactory.TemporaryAuthorizationCodeRepository.Add(authorizedTenant);
        await repositoryFactory.TemporaryAuthorizationCodeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return authorizationRequest;
    }

    public async Task OnBoardTenant(string tenantId, string? error, string? errorMessage, bool adminConsent,
        string temporaryAuthorizationCode, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(temporaryAuthorizationCode))
        {
            throw new ArgumentException("temporary authorization code is empty.", nameof(temporaryAuthorizationCode));
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
            .Query(new Specification<TemporaryAuthorizationCode>
            {
                Criteria = query => query.Id == temporaryAuthorizationCode
            }.ApplyOrderBy(query => query.Id))
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
