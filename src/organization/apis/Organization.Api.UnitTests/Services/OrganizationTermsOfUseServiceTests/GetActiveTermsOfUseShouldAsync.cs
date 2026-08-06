using Microsoft.Extensions.Caching.Memory;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Shared.Database.Entities;
using Organization.Shared.Repositories;

namespace Organization.Api.UnitTests.Services.OrganizationTermsOfUseServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetActiveTermsOfUseShouldAsync
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Cache_The_Mapped_Active_Terms_Of_Use(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ITermsOfUseRepository termsOfUseRepository,
        [Frozen]
        IGraphQlMapper graphQlMapper,
        [Frozen]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var entity = new TermsOfUse
        {
            Id = "terms-1",
            Active = true,
            Terms = "terms",
        };
        var mapped = new Shared.Models.TermsOfUse
        {
            Id = entity.Id,
            Active = true,
            Terms = entity.Terms,
        };

        A.CallTo(() => repositoryFactory.TermsOfUseRepository).Returns(termsOfUseRepository);
        A.CallTo(() => timeProvider.GetUtcNow()).Returns(new DateTimeOffset(2026, 4, 19, 10, 0, 0, TimeSpan.Zero));
        A.CallTo(() => termsOfUseRepository.GetActiveUntrackedAsync(cancellationToken)).Returns(entity);
        A.CallTo(() => graphQlMapper.MapTo(entity)).Returns(mapped);

        var sut = new OrganizationTermsOfUseService(repositoryFactory, graphQlMapper, memoryCache, timeProvider);

        var first = await sut.GetActiveTermsOfUseAsync(cancellationToken);
        var second = await sut.GetActiveTermsOfUseAsync(cancellationToken);

        first.ShouldBe(mapped);
        second.ShouldBe(mapped);
        A.CallTo(() => termsOfUseRepository.GetActiveUntrackedAsync(cancellationToken)).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_The_Active_Terms_Entity(
        [Frozen]
        IRepositoryFactory repositoryFactory,
        [Frozen]
        ITermsOfUseRepository termsOfUseRepository,
        IGraphQlMapper graphQlMapper,
        [Frozen]
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var entity = new TermsOfUse
        {
            Id = "terms-1",
            Active = true,
            Terms = "terms",
        };

        A.CallTo(() => repositoryFactory.TermsOfUseRepository).Returns(termsOfUseRepository);
        A.CallTo(() => termsOfUseRepository.GetActiveAsync(cancellationToken)).Returns(entity);

        var sut = new OrganizationTermsOfUseService(repositoryFactory, graphQlMapper, memoryCache, timeProvider);

        var result = await sut.GetActiveTermsOfUseEntityAsync(cancellationToken);

        result.ShouldBe(entity);
    }
}
