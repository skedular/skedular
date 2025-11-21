namespace Api.Shared.Services.Models;

public enum OrganizationType
{
    Private,
    Marketplace,
    Individual
}

public static class OrganizationTypeConstants
{
    public const string Private = "PRIVATE";
    public const string Marketplace = "MARKETPLACE";
    public const string Individual = "INDIVIDUAL";
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
                OrganizationType.Individual => OrganizationTypeConstants.Individual,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToOrganizationTypeName() =>
            src switch
            {
                OrganizationType.Private => "Private",
                OrganizationType.Marketplace => "Marketplace",
                OrganizationType.Individual => "Individual",
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(string src)
    {
        public string ToOrganizationTypeName() =>
            src switch
            {
                OrganizationTypeConstants.Private => "Private",
                OrganizationTypeConstants.Marketplace => "Marketplace",
                OrganizationTypeConstants.Individual => "Individual",
                _ => throw new ArgumentOutOfRangeException()
            };

        public OrganizationType ToOrganizationType() =>
            src switch
            {
                OrganizationTypeConstants.Private => OrganizationType.Private,
                OrganizationTypeConstants.Marketplace => OrganizationType.Marketplace,
                OrganizationTypeConstants.Individual => OrganizationType.Individual,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
