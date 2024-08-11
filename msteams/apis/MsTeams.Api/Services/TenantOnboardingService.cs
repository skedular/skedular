using MsTeams.Shared.Repositories;

namespace MsTeams.Api.Services;

public interface ITenantOnboardingService
{
    Task OnBoardTenantAsync(
        string tenantId,
        string? error,
        string? errorMessage,
        CancellationToken cancellationToken);
}

public class TenantOnboardingService(IRepositoryFactory repositoryFactory) : ITenantOnboardingService
{
    public async Task OnBoardTenantAsync(
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
