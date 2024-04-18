using System.Data.Common;
using Npgsql;
using NpgsqlTypes;
using Quartz.Impl.AdoJobStore.Common;

namespace Enterprise.Shared.Database;

public class QuartzNpgsqlDbProvider : IDbProvider
{
    private readonly NpgsqlDataSource _dataSource;

    public QuartzNpgsqlDbProvider(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;

        Metadata = new DbMetadata
        {
            AssemblyName = typeof(NpgsqlConnection).AssemblyQualifiedName,
            BindByName = true,
            CommandType = typeof(NpgsqlCommand),
            ConnectionType = typeof(NpgsqlConnection),
            DbBinaryTypeName = nameof(NpgsqlDbType.Bytea),
            ExceptionType = typeof(NpgsqlException),
            ParameterDbType = typeof(NpgsqlDbType),
            ParameterDbTypePropertyName = nameof(NpgsqlDbType),
            ParameterNamePrefix = ":",
            ParameterType = typeof(NpgsqlParameter),
            UseParameterNamePrefixInParameterCollection = true
        };
        Metadata.Init();
    }

    public string ConnectionString { get; set; } = string.Empty;

    public DbMetadata Metadata { get; }

    public void Initialize() { }

    public DbCommand CreateCommand() => new NpgsqlCommand();

    public DbConnection CreateConnection() => _dataSource.CreateConnection();

    public void Shutdown() { }
}
