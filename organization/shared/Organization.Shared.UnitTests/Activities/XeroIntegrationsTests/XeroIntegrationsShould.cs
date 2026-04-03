using Api.Shared.Services.Models;
using AutoFixture.Xunit3;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Database;
using FakeItEasy;
using Organization.Shared.Activities;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;
using Temporalio.Testing;

namespace Organization.Shared.UnitTests.Activities.XeroIntegrationsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class XeroIntegrationsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_Connection_Has_No_Refresh_Token(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IXeroTokenRefreshService xeroTokenRefreshService,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        XeroIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var organization = new Database.Entities.Organization
        {
            Id = "org-1",
            Name = "Org 1",
            OrganizationXeroConnection = new OrganizationXeroConnection
            {
                Id = "xero-1", OrganizationId = "org-1", BillingMode = XeroBillingModeConstants.Disabled, RefreshTokenEncrypted = string.Empty
            }
        };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, environment.CancellationTokenSource.Token))
            .Returns(organization);

        var result = await environment.RunAsync(() =>
            sut.RefreshOrganizationXeroConnectionAsync(new RefreshOrganizationXeroConnectionInput("org-1")));

        result.ShouldBe(new RefreshOrganizationXeroConnectionResult(false, null));
        A.CallTo(() => xeroTokenRefreshService.RefreshAsync(A<OrganizationXeroConnection>._, A<CancellationToken>._))
            .MustNotHaveHappened();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync(A<string?>._, A<string?>._, A<CancellationToken>._))
            .MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Deactivate_Connection_When_Refresh_Token_Is_Expired(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        XeroIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 3, 31, 10, 0, 0, TimeSpan.Zero);
        var connection = new OrganizationXeroConnection
        {
            Id = "xero-1",
            OrganizationId = "org-1",
            BillingMode = XeroBillingModeConstants.Enabled,
            RefreshTokenEncrypted = "refresh-token",
            RefreshTokenExpiresAt = now.AddMinutes(-1),
            IsActive = true
        };
        var organization = new Database.Entities.Organization { Id = "org-1", Name = "Org 1", OrganizationXeroConnection = connection };

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, environment.CancellationTokenSource.Token))
            .Returns(organization);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);

        var result = await environment.RunAsync(() =>
            sut.RefreshOrganizationXeroConnectionAsync(new RefreshOrganizationXeroConnectionInput("org-1")));

        result.ShouldBe(new RefreshOrganizationXeroConnectionResult(false, null));
        connection.IsActive.ShouldBeFalse();
        connection.LastError.ShouldBe("Xero refresh token expired. Reconnect required.");
        A.CallTo(() => organizationXeroConnectionRepository.Update(connection)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync("org-1", null, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Next_Maintenance_After_Successful_Refresh(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IOrganizationRepository organizationRepository,
        [Frozen] IOrganizationXeroConnectionRepository organizationXeroConnectionRepository,
        [Frozen] IUnitOfWork unitOfWork,
        [Frozen] IXeroTokenRefreshService xeroTokenRefreshService,
        [Frozen] TimeProvider timeProvider,
        [Frozen] ICachedOrganizationService cachedOrganizationService,
        XeroIntegrations sut)
    {
        var environment = new ActivityEnvironment();
        var now = new DateTimeOffset(2026, 3, 31, 10, 0, 0, TimeSpan.Zero);
        var nextRefreshAt = now.AddDays(40);
        var nextMaintenanceAt = now.AddDays(35);
        var connection = new OrganizationXeroConnection
        {
            Id = "xero-1",
            OrganizationId = "org-1",
            BillingMode = XeroBillingModeConstants.Enabled,
            RefreshTokenEncrypted = "old-refresh-token",
            RefreshTokenExpiresAt = now.AddDays(30),
            LastError = "old-error"
        };
        var organization = new Database.Entities.Organization { Id = "org-1", Name = "Org 1", OrganizationXeroConnection = connection };
        var refreshResult = new XeroTokenRefreshResult(
            true,
            false,
            "new-access-token",
            "new-refresh-token",
            now.AddHours(1),
            nextRefreshAt,
            null);

        A.CallTo(() => repositoryFactory.OrganizationRepository).Returns(organizationRepository);
        A.CallTo(() => repositoryFactory.OrganizationXeroConnectionRepository).Returns(organizationXeroConnectionRepository);
        A.CallTo(() => repositoryFactory.UnitOfWork).Returns(unitOfWork);
        A.CallTo(() => organizationRepository.GetByIdOrCustomDomainAsync("org-1", null, environment.CancellationTokenSource.Token))
            .Returns(organization);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(now);
        A.CallTo(() => xeroTokenRefreshService.RefreshAsync(connection, environment.CancellationTokenSource.Token)).Returns(refreshResult);
        A.CallTo(() => xeroTokenRefreshService.GetNextMaintenanceAt(nextRefreshAt)).Returns(nextMaintenanceAt);

        var result = await environment.RunAsync(() =>
            sut.RefreshOrganizationXeroConnectionAsync(new RefreshOrganizationXeroConnectionInput("org-1")));

        result.ShouldBe(new RefreshOrganizationXeroConnectionResult(true, nextMaintenanceAt));
        connection.AccessTokenEncrypted.ShouldBe("new-access-token");
        connection.RefreshTokenEncrypted.ShouldBe("new-refresh-token");
        connection.AccessTokenExpiresAt.ShouldBe(now.AddHours(1));
        connection.RefreshTokenExpiresAt.ShouldBe(nextRefreshAt);
        connection.LastSuccessfulSyncAt.ShouldBe(now);
        connection.LastError.ShouldBeNull();
        A.CallTo(() => organizationXeroConnectionRepository.Update(connection)).MustHaveHappenedOnceExactly();
        A.CallTo(() => unitOfWork.SaveChangesAsync(environment.CancellationTokenSource.Token)).MustHaveHappenedOnceExactly();
        A.CallTo(() => cachedOrganizationService.RemoveByIdOrCustomDomainAsync("org-1", null, environment.CancellationTokenSource.Token))
            .MustHaveHappenedOnceExactly();
    }
}
