namespace Enterprise.Shared.Configurations;

public class ApplicationConfiguration
{
    public const string Key = "Application";

    public string Environment { get; set; } = string.Empty;
    public string DomainSource { get; set; } = string.Empty;
    public string AppSource { get; set; } = string.Empty;
    public string PublicWebSiteBaseDomain { get; set; } = string.Empty;
    public string WebAppBaseDomain { get; set; } = string.Empty;
    public bool EnableSchemaRegistry { get; set; }
    public IdentityProviders IdentityProviders { get; set; } = new();
    public string GetSource() => $"{DomainSource}::{AppSource}";
}

public class IdentityProviders
{
    public Cognito? Cognito { get; set; }
    public Google? Google { get; set; }
}

public class Cognito
{
    public Uri? JwksUri { get; set; }
    public string? Issuer { get; set; }
    public string? Audiences { get; set; }
}

public class Google
{
    public string? ApplicationId { get; set; }
    public string? Issuer { get; set; }
}
