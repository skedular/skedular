using Api.Shared.Services;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Temporalio.Activities;

namespace Organization.Shared.Activities;

public record RefreshOrganizationXeroConnectionInput(string OrganizationId);

public record RefreshOrganizationXeroConnectionResult(bool ShouldContinue, DateTimeOffset? NextRefreshAt);

public class XeroIntegrations(
    IRepositoryFactory repositoryFactory,
    IXeroTokenRefreshService xeroTokenRefreshService,
    ICachedOrganizationService cachedOrganizationService,
    IEntityMapper entityMapper,
    IOrganizationOutboxPublisher organizationOutboxPublisher,
    TimeProvider timeProvider)
{
    [Activity]
    public async Task<RefreshOrganizationXeroConnectionResult> RefreshOrganizationXeroConnectionAsync(RefreshOrganizationXeroConnectionInput input)
    {
        var cancellationToken = ActivityExecutionContext.Current.CancellationToken;
        var organization = await repositoryFactory.OrganizationRepository.GetByIdOrCustomDomainAsync(
                               input.OrganizationId,
                               null,
                               cancellationToken) ??
                           throw new OrganizationNotFound();
        var connection = organization.OrganizationXeroConnection;
        if (connection is null || string.IsNullOrWhiteSpace(connection.RefreshTokenEncrypted))
        {
            return new RefreshOrganizationXeroConnectionResult(false, null);
        }

        if (connection.RefreshTokenExpiresAt is null)
        {
            connection.LastError ??= "Xero refresh token expiry is unknown. Reconnect required.";
            connection.IsActive = false;
            repositoryFactory.OrganizationXeroConnectionRepository.Update(connection);
            PublishOrganization(organization);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
            return new RefreshOrganizationXeroConnectionResult(false, null);
        }

        var now = timeProvider.GetUtcNow();
        if (connection.RefreshTokenExpiresAt <= now)
        {
            connection.IsActive = false;
            connection.LastError = "Xero refresh token expired. Reconnect required.";
            repositoryFactory.OrganizationXeroConnectionRepository.Update(connection);
            PublishOrganization(organization);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
            return new RefreshOrganizationXeroConnectionResult(false, null);
        }

        var refreshResult = await xeroTokenRefreshService.RefreshAsync(connection, cancellationToken);
        if (!refreshResult.IsSuccessful)
        {
            connection.LastError = refreshResult.Error;
            if (refreshResult.NeedsReconnect)
            {
                connection.IsActive = false;
            }

            repositoryFactory.OrganizationXeroConnectionRepository.Update(connection);
            PublishOrganization(organization);
            await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
            await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);
            return new RefreshOrganizationXeroConnectionResult(
                !refreshResult.NeedsReconnect,
                refreshResult.NeedsReconnect ? null : xeroTokenRefreshService.GetRetryMaintenanceAt());
        }

        connection.AccessTokenEncrypted = refreshResult.AccessTokenEncrypted;
        connection.RefreshTokenEncrypted = refreshResult.RefreshTokenEncrypted;
        connection.AccessTokenExpiresAt = refreshResult.AccessTokenExpiresAt;
        connection.RefreshTokenExpiresAt = refreshResult.RefreshTokenExpiresAt;
        connection.LastSuccessfulSyncAt = now;
        connection.LastError = null;

        repositoryFactory.OrganizationXeroConnectionRepository.Update(connection);
        PublishOrganization(organization);
        await repositoryFactory.UnitOfWork.SaveChangesAsync(cancellationToken);
        await cachedOrganizationService.RemoveByIdOrCustomDomainAsync(organization.Id, organization.CustomDomain, cancellationToken);

        return new RefreshOrganizationXeroConnectionResult(
            true,
            refreshResult.RefreshTokenExpiresAt is null
                ? null
                : xeroTokenRefreshService.GetNextMaintenanceAt(refreshResult.RefreshTokenExpiresAt.Value));
    }

    private void PublishOrganization(Database.Entities.Organization organization) =>
        organizationOutboxPublisher.PublishOrganizations([entityMapper.MapTo(organization)], repositoryFactory.UnitOfWork);
}
