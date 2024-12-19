namespace Enterprise.Shared.Database;

public class PostgresConfigurationOptions
{
    public const string Key = "Postgres";

    public string DefaultConnection { get; set; } = string.Empty;
    public string? Server { get; set; }
    public int? Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}
