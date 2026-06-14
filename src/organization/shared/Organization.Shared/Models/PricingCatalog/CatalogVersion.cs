namespace Organization.Shared.Models.PricingCatalog;

public enum CatalogVersion
{
    TeamsV1 = 0
}

public static class CatalogVersionConstants
{
    public const string TeamsV1 = "TEAMS_V1";
}

public static class CatalogVersionCodeExtensions
{
    extension(CatalogVersion? src)
    {
        public string? ToNullableCatalogVersionCode() =>
            src is null
                ? null
                : src switch
                {
                    CatalogVersion.TeamsV1 => CatalogVersionConstants.TeamsV1,
                    _ => throw new ArgumentOutOfRangeException()
                };
    }

    extension(CatalogVersion src)
    {
        public string ToCatalogVersion() =>
            src switch
            {
                CatalogVersion.TeamsV1 => CatalogVersionConstants.TeamsV1,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToCatalogVersionName() =>
            src switch
            {
                CatalogVersion.TeamsV1 => "Teams V1",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public CatalogVersion ToCatalogVersion() =>
            src switch
            {
                CatalogVersionConstants.TeamsV1 => CatalogVersion.TeamsV1,
                _ => throw new ArgumentOutOfRangeException()
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
                    _ => throw new ArgumentOutOfRangeException()
                };
    }
}
