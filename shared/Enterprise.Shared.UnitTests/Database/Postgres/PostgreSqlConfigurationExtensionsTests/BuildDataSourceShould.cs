using Enterprise.Shared.Database.Postgres;

namespace Enterprise.Shared.UnitTests.Database.Postgres.PostgreSqlConfigurationExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BuildDataSourceShould
{
    [Fact]
    public void Cache_data_source_per_connection_string()
    {
        var connectionString = $"Host=localhost;Database=test_{Guid.NewGuid():N};Username=test;Password=test";

        var dataSource = connectionString.BuildDataSource(false);
        var cachedDataSource = connectionString.BuildDataSource(false);

        ReferenceEquals(dataSource, cachedDataSource).ShouldBeTrue();
    }

    [Fact]
    public void Throw_argument_exception_for_invalid_connection_string()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            "Host=localhost;Port=not-a-number;Database=test;Username=test;Password=test".BuildDataSource(false));

        exception.Message.ShouldContain("Failed to build npgsql datasource");
    }
}
