namespace Enterprise.Shared.Security.Configurations;

public class AuthenticationConfiguration
{
    public const string Key = "Authentication";

    public JwtAuthenticationConfiguration Jwt { get; set; } = new();
}

public class JwtAuthenticationConfiguration
{
    public string Issuer { get; set; } = string.Empty;
}
