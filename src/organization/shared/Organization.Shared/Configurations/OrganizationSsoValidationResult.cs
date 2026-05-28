namespace Organization.Shared.Configurations;

public class OrganizationSsoValidationResult
{
    public bool IsMetadataValid { get; set; }
    public bool IsCertificateValid { get; set; }
    public string? Error { get; set; }
}
