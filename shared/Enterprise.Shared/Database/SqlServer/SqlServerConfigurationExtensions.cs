using Microsoft.Data.SqlClient;

namespace Enterprise.Shared.Database.SqlServer;

public static class SqlServerConfigurationExtensions
{
    extension(string connectionString)
    {
        public string BuildConnectionString()
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
}
