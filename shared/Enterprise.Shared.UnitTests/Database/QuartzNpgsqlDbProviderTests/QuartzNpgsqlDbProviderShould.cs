using Enterprise.Shared.Database;
using FluentAssertions;
using Npgsql;
using NpgsqlTypes;
using Xunit;

namespace Enterprise.Shared.UnitTests.Database.QuartzNpgsqlDbProviderTests;

public class QuartzNpgsqlDbProviderTests
{
    [Fact]
    public void Initialize_Metadata()
    {
        var dataSource = NpgsqlDataSource.Create("Host=123");

        var dbProvider = new QuartzNpgsqlDbProvider(dataSource);

        var metadata = dbProvider.Metadata;

        metadata.AssemblyName.Should().StartWith("Npgsql.NpgsqlConnection");
        metadata.BindByName.Should().BeTrue();
        metadata.CommandType.Should().BeAssignableTo<NpgsqlCommand>();
        metadata.ConnectionType.Should().BeAssignableTo<NpgsqlConnection>();
        metadata.DbBinaryType.Should().Be(NpgsqlDbType.Bytea);
        metadata.ExceptionType.Should().BeAssignableTo<NpgsqlException>();
        metadata.ParameterDbType.Should().BeAssignableTo<NpgsqlDbType>();
        metadata.ParameterDbTypeProperty!.PropertyType.Should().BeAssignableTo<NpgsqlDbType>();
        metadata.ParameterNamePrefix.Should().Be(":");
        metadata.ParameterType.Should().BeAssignableTo<NpgsqlParameter>();
        metadata.UseParameterNamePrefixInParameterCollection.Should().BeTrue();
    }

    [Fact]
    public void Return_NpgsqlCommand()
    {
        var dataSource = NpgsqlDataSource.Create("Host=123");

        var dbProvider = new QuartzNpgsqlDbProvider(dataSource);

        dbProvider.CreateCommand().Should().BeEquivalentTo(new NpgsqlCommand());
    }

    [Fact]
    public void Return_NpgsqlConnection()
    {
        const string ConnectionString = "Host=123";
        var dataSource = NpgsqlDataSource.Create(ConnectionString);

        var dbProvider = new QuartzNpgsqlDbProvider(dataSource);

        var dbConnection = dbProvider.CreateConnection();

        dbConnection.Should().BeAssignableTo<NpgsqlConnection>();
        dbConnection.ConnectionString.Should().Be(ConnectionString);
    }
}
