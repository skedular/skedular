namespace Enterprise.Shared.Database;

public class DatabaseSetupContext<T>(DatabaseSetup databaseSetup)
    : DatabaseSetup(databaseSetup.ServiceCollection, databaseSetup.NpgsqlDataSource);
