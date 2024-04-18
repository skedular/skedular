namespace Slack.Shared.Models;

public class LocationPermissions
{
    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
    public bool CanInvitePeople { get; set; }
    public bool CanCancelPeopleExistingInvitations { get; set; }
    public bool CanViewAnalytics { get; set; }
}
