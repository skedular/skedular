using Enterprise.Shared.Database.PostgreSql;
using Npgsql;

namespace Enterprise.Shared.UnitTests.Database.PostgreSql.PostgreSqlConfigurationExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BuildDataSourceShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Cache_data_source_per_connection_string(string uniqueId)
    {
        var connectionString = $"Host=localhost;Database=local.test_{uniqueId};Username=test;Password=test";

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

    [Theory]
    [AutoFakeItEasyData]
    public void Reuse_the_data_source_for_the_same_connection_string_and_postgis_mode(string databaseName)
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Database = databaseName,
        }.ConnectionString;

        var standard = connectionString.BuildDataSource(false);
        var spatial = connectionString.BuildDataSource(true);

        connectionString.BuildDataSource(false).ShouldBeSameAs(standard);
        connectionString.BuildDataSource(true).ShouldBeSameAs(spatial);
        spatial.ShouldNotBeSameAs(standard);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void Keep_standard_mapping_separate_when_spatial_mapping_is_registered_first(string databaseName)
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Database = databaseName,
        }.ConnectionString;

        var spatial = connectionString.BuildDataSource(true);
        var standard = connectionString.BuildDataSource(false);

        standard.ShouldNotBeSameAs(spatial);
        connectionString.BuildDataSource(true).ShouldBeSameAs(spatial);
        connectionString.BuildDataSource(false).ShouldBeSameAs(standard);
    }
}
