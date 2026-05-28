namespace Api.Shared.Services.Models;

public enum OrganizationMemberStatus
{
    Active,
    Inactive
}

public static class OrganizationMemberStatusConstants
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";
}

public static class OrganizationMemberStatusExtensions
{
    extension(string src)
    {
        public OrganizationMemberStatus ToOrganizationMemberStatus() =>
            src switch
            {
                OrganizationMemberStatusConstants.Active => OrganizationMemberStatus.Active,
                OrganizationMemberStatusConstants.Inactive => OrganizationMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(OrganizationMemberStatus src)
    {
        public string ToOrganizationMemberStatus() =>
            src switch
            {
                OrganizationMemberStatus.Active => OrganizationMemberStatusConstants.Active,
                OrganizationMemberStatus.Inactive => OrganizationMemberStatusConstants.Inactive,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToOrganizationMemberStatusName() =>
            src switch
            {
                OrganizationMemberStatus.Active => "Active",
                OrganizationMemberStatus.Inactive => "Inactive",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
