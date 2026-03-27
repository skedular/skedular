using Microsoft.Data.SqlClient;

namespace Enterprise.Shared.Database.SqlServer;

public static class SqlServerConfigurationExtensions
{
    public static string BuildConnectionString(this string connectionString)
    {
        try
        {
            // Validate connection string
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.ConnectionString;
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"Failed to build SQL Server connection string: {ex.Message}");
        }
    }
}
