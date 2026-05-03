namespace Enterprise.Shared.IdentityProviders.Configurations;

public class IdentityProvidersConfiguration
{
    public const string Key = "IdentityProviders";

    public Cognito? Cognito { get; set; }
    public Google? Google { get; set; }

// ReSharper disable once InconsistentNaming
    public WorkOS? WorkOS { get; set; }
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

// ReSharper disable once InconsistentNaming
public class WorkOS
{
    public Uri? JwksUri { get; set; }
    public string? Issuer { get; set; }
    public string? ApiKey { get; set; }
    public IReadOnlyList<Uri> OtherIssuers { get; set; } = [];
}
