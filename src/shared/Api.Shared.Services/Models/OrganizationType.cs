namespace Api.Shared.Services.Models;

public enum OrganizationType
{
    Private,
    Marketplace,
    Host
}

public static class OrganizationTypeConstants
{
    public const string Private = "PRIVATE";
    public const string Marketplace = "MARKETPLACE";
    public const string Host = "HOST";
}

public static class OrganizationTypeExtensions
{
    extension(OrganizationType src)
    {
        public string ToOrganizationType() =>
            src switch
            {
                OrganizationType.Private => OrganizationTypeConstants.Private,
                OrganizationType.Marketplace => OrganizationTypeConstants.Marketplace,
                OrganizationType.Host => OrganizationTypeConstants.Host,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public string ToOrganizationTypeName() =>
            src switch
            {
                OrganizationType.Private => "Private",
                OrganizationType.Marketplace => "Marketplace",
                OrganizationType.Host => "Host",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }

    extension(string src)
    {
        public string ToOrganizationTypeName() =>
            src switch
            {
                OrganizationTypeConstants.Private => "Private",
                OrganizationTypeConstants.Marketplace => "Marketplace",
                OrganizationTypeConstants.Host => "Host",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public OrganizationType ToOrganizationType() =>
            src switch
            {
                OrganizationTypeConstants.Private => OrganizationType.Private,
                OrganizationTypeConstants.Marketplace => OrganizationType.Marketplace,
                OrganizationTypeConstants.Host => OrganizationType.Host,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
