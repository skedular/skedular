using Enterprise.Shared.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Enterprise.Shared.UnitTests.Cache.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddRedisShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Reuse_the_registered_multiplexer_without_connecting_when_resolving_options(
        IDatabase database,
        IConnectionMultiplexer multiplexer,
        string connectionName,
        string key)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{connectionName}"] = "localhost:6379",
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddRedis(configuration, connectionName).ShouldBeSameAs(services);

        var resolutions = 0;
        services.Replace(ServiceDescriptor.Singleton<IConnectionMultiplexer>(_ =>
        {
            resolutions++;
            return multiplexer;
        }));
        using var provider = services.BuildServiceProvider();

        var cache = provider.GetRequiredService<IDistributedCache>();
        cache.ShouldBeAssignableTo<RedisCache>();
        var options = provider.GetRequiredService<IOptions<RedisCacheOptions>>().Value;
        resolutions.ShouldBe(0);
        var factory = options.ConnectionMultiplexerFactory.ShouldNotBeNull();

        (await factory()).ShouldBeSameAs(multiplexer);
        (await factory()).ShouldBeSameAs(provider.GetRequiredService<IConnectionMultiplexer>());
        resolutions.ShouldBe(1);

        A.CallTo(() => multiplexer.GetDatabase(A<int>._, A<object>._)).Returns(database);
        await cache.RemoveAsync(key, TestContext.Current.CancellationToken);

        A.CallTo(() => database.KeyDeleteAsync((RedisKey)key, CommandFlags.None)).MustHaveHappenedOnceExactly();
        resolutions.ShouldBe(1);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Reject_missing_or_blank_connection_strings_before_registering_services(string? connectionString)
    {
        const string connectionName = "redis";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"ConnectionStrings:{connectionName}"] = connectionString,
            })
            .Build();
        var services = new ServiceCollection();

        Should.Throw<ArgumentException>(() => services.AddRedis(configuration, connectionName));
        services.ShouldBeEmpty();
    }
}
