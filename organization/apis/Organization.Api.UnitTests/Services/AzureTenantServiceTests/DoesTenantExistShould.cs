using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database;
using Enterprise.Shared.Random;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Services;
using Organization.Shared.Repositories;
using Organization.Shared.Services;

namespace Organization.Api.UnitTests.Services.AzureTenantServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class DoesTenantExistShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_False_When_There_Is_No_Tenant_In_Context(
        [Frozen] IContext context,
        [Frozen] IRepositoryFactory repositoryFactory,
        IRandomHelper randomHelper,
        IAzureTenantOnboardingService azureTenantOnboardingService,
        ITemporalOutboxService temporalOutboxService,
        CancellationToken cancellationToken)
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());

        A.CallTo(() => context.GetAzureTenantId()).Returns(Guid.Empty);

        var sut = new AzureTenantService(
            new ApplicationConfiguration(),
            A.Fake<IDbTransactionBuilder>(),
            repositoryFactory,
            context,
            memoryCache,
            randomHelper,
            new AzureEntraConfiguration(),
            A.Fake<IHttpContextAccessor>(),
            azureTenantOnboardingService,
            temporalOutboxService,
            TimeProvider.System);

        var result = await sut.DoesTenantExistAsync(cancellationToken);

        result.ShouldBeFalse();
        A.CallTo(() => repositoryFactory.AzureTenantRepository).MustNotHaveHappened();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Cache_Positive_Tenant_Existence_Lookups(
        [Frozen] IContext context,
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] IAzureTenantRepository azureTenantRepository,
        IRandomHelper randomHelper,
        IAzureTenantOnboardingService azureTenantOnboardingService,
        ITemporalOutboxService temporalOutboxService,
        CancellationToken cancellationToken)
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        A.CallTo(() => context.GetAzureTenantId()).Returns(tenantId);
        A.CallTo(() => repositoryFactory.AzureTenantRepository).Returns(azureTenantRepository);
        A.CallTo(() => azureTenantRepository.ExistsActiveByIdAsync(tenantId.ToString(), cancellationToken)).Returns(true);

        var sut = new AzureTenantService(
            new ApplicationConfiguration(),
            A.Fake<IDbTransactionBuilder>(),
            repositoryFactory,
            context,
            memoryCache,
            randomHelper,
            new AzureEntraConfiguration(),
            A.Fake<IHttpContextAccessor>(),
            azureTenantOnboardingService,
            temporalOutboxService,
            TimeProvider.System);

        var first = await sut.DoesTenantExistAsync(cancellationToken);
        var second = await sut.DoesTenantExistAsync(cancellationToken);

        first.ShouldBeTrue();
        second.ShouldBeTrue();
        A.CallTo(() => azureTenantRepository.ExistsActiveByIdAsync(tenantId.ToString(), cancellationToken)).MustHaveHappenedOnceExactly();
    }
}
