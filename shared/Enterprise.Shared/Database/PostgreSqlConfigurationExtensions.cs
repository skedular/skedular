using System.Collections.Concurrent;
using Npgsql;

namespace Enterprise.Shared.Database;

public static class PostgreSqlConfigurationExtensions
{
    private static readonly ConcurrentDictionary<string, NpgsqlDataSource> s_dataSources = new();

    internal static NpgsqlDataSource BuildDataSource(this string connectionString)
    {
        try
        {
            if (s_dataSources.TryGetValue(connectionString, out var dataSource))
            {
                return dataSource;
            }

            s_dataSources[connectionString] = new NpgsqlDataSourceBuilder(connectionString).EnableDynamicJson().Build();
            return s_dataSources[connectionString];
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Failed to build npgsql datasource {message}", ex.Message);
        }
    }
}
