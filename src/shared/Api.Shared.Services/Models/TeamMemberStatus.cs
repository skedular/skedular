namespace Api.Shared.Services.Models;

public enum TeamMemberStatus
{
    Active,
    Inactive
}

public static class TeamMemberStatusConstants
{
    public const string Active = "ACTIVE";
    public const string Inactive = "INACTIVE";
}

public static class TeamMemberStatusExtensions
{
    extension(string src)
    {
        public TeamMemberStatus ToTeamMemberStatus() =>
            src switch
            {
                TeamMemberStatusConstants.Active => TeamMemberStatus.Active,
                TeamMemberStatusConstants.Inactive => TeamMemberStatus.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }

    extension(TeamMemberStatus src)
    {
        public string ToTeamMemberStatus() =>
            src switch
            {
                TeamMemberStatus.Active => TeamMemberStatusConstants.Active,
                TeamMemberStatus.Inactive => TeamMemberStatusConstants.Inactive,
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };

        public string ToTeamMemberStatusName() =>
            src switch
            {
                TeamMemberStatus.Active => "Active",
                TeamMemberStatus.Inactive => "Inactive",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src,
                    $"Unexpected value for {nameof(src)}: {src}. Update enum mapping or caller input.")
            };
    }
}
