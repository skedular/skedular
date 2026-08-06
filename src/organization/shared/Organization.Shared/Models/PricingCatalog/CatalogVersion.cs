namespace Organization.Shared.Models.PricingCatalog;

public enum CatalogVersion
{
    TeamsV1 = 0,
    SpacesV1 = 1,
    HostV1 = 2,
}

public static class CatalogVersionConstants
{
    public const string TeamsV1 = "TEAMS_V1";
    public const string SpacesV1 = "SPACES_V1";
    public const string HostV1 = "HOST_V1";
}

public static class CatalogVersionCodeExtensions
{
    extension(CatalogVersion src)
    {
        public string ToCatalogVersionName() =>
            src switch
            {
                CatalogVersion.TeamsV1 => "Teams V1",
                CatalogVersion.SpacesV1 => "Spaces V1",
                CatalogVersion.HostV1 => "Host V1",
                _ => throw new ArgumentOutOfRangeException(null,
                    "Unexpected value encountered. Update enum mapping or caller input to include this case."),
            };
    }

    extension(string? src)
    {
        public CatalogVersion? ToNullableCatalogVersion() =>
            src is null
                ? null
                : src switch
                {
                    CatalogVersionConstants.TeamsV1 => CatalogVersion.TeamsV1,
                    CatalogVersionConstants.SpacesV1 => CatalogVersion.SpacesV1,
                    CatalogVersionConstants.HostV1 => CatalogVersion.HostV1,
                    _ => throw new ArgumentOutOfRangeException(null,
                        "Unexpected value encountered. Update enum mapping or caller input to include this case."),
                };
    }
}
