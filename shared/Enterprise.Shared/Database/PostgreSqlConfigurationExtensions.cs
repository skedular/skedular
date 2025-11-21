using System.Collections.Concurrent;
using Npgsql;

namespace Enterprise.Shared.Database;

public static class PostgreSqlConfigurationExtensions
{
    private static readonly ConcurrentDictionary<string, NpgsqlDataSource> s_dataSources = new();

    extension(string connectionString)
    {
        internal NpgsqlDataSource BuildDataSource(bool isPostgisEnabled)
        {
            try
            {
                if (s_dataSources.TryGetValue(connectionString, out var dataSource))
                {
                    return dataSource;
                }

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                if (isPostgisEnabled)
                {
                    dataSourceBuilder = dataSourceBuilder.UseNetTopologySuite();
                }

                s_dataSources[connectionString] = dataSourceBuilder.EnableDynamicJson().Build();
                return s_dataSources[connectionString];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to build npgsql datasource {ex.Message}");
            }
        }
    }
}
