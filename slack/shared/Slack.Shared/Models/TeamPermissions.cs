namespace Slack.Shared.Models;

public class TeamPermissions
{
    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
    public bool CanInvitePeople { get; set; }
    public bool CanCancelPeopleExistingInvitations { get; set; }
}
