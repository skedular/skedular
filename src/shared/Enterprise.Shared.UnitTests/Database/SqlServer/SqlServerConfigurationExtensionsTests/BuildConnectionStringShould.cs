using Enterprise.Shared.Database.SqlServer;

namespace Enterprise.Shared.UnitTests.Database.SqlServer.SqlServerConfigurationExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class BuildConnectionStringShould
{
    [Fact]
    public void Return_normalized_connection_string_for_valid_input()
    {
        var connectionString = "Database=test;Server=localhost;TrustServerCertificate=True;User Id=sa;Password=Password123!";

        var result = connectionString.BuildConnectionString();

        result.ShouldContain("Data Source=localhost");
        result.ShouldContain("Initial Catalog=test");
        result.ShouldContain("Trust Server Certificate=True");
    }

    [Fact]
    public void Throw_argument_exception_for_invalid_input()
    {
        var exception = Should.Throw<ArgumentException>(() => "not-a-valid-connection-string".BuildConnectionString());

        exception.Message.ShouldContain("Failed to build SQL Server connection string");
    }
}
