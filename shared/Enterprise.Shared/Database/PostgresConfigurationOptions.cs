namespace Enterprise.Shared.Database;

public class PostgresConfigurationOptions
{
    public const string Key = "Postgres";

    public string DefaultConnection { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
