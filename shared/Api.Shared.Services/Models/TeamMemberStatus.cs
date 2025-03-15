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
    public static TeamMemberStatus ToTeamMemberStatus(this string src) =>
        src switch
        {
            TeamMemberStatusConstants.Active => TeamMemberStatus.Active,
            TeamMemberStatusConstants.Inactive => TeamMemberStatus.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static string ToTeamMemberStatus(this TeamMemberStatus src) =>
        src switch
        {
            TeamMemberStatus.Active => TeamMemberStatusConstants.Active,
            TeamMemberStatus.Inactive => TeamMemberStatusConstants.Inactive,
            _ => throw new ArgumentOutOfRangeException()
        };
}
