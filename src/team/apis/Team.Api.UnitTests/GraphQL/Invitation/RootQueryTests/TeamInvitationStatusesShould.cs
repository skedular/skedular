using Api.Shared.Services.Models;
using Team.Api.GraphQL.Invitation;

namespace Team.Api.UnitTests.GraphQL.Invitation.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TeamInvitationStatusesShould
{
    [Theory]
    [AutoFakeItEasyData]
    public void Return_All_Invitation_Statuses(RootQuery sut)
    {
        var result = sut.TeamInvitationStatuses().ToList();

        result.Count.ShouldBe(5);
        result.ShouldContain(item =>
            item.Type == InvitationStatus.Pending &&
            item.Name == InvitationStatus.Pending.ToInvitationStatusName());
        result.ShouldContain(item =>
            item.Type == InvitationStatus.Accepted &&
            item.Name == InvitationStatus.Accepted.ToInvitationStatusName());
        result.ShouldContain(item =>
            item.Type == InvitationStatus.Rejected &&
            item.Name == InvitationStatus.Rejected.ToInvitationStatusName());
        result.ShouldContain(item =>
            item.Type == InvitationStatus.Cancelled &&
            item.Name == InvitationStatus.Cancelled.ToInvitationStatusName());
        result.ShouldContain(item =>
            item.Type == InvitationStatus.Expired &&
            item.Name == InvitationStatus.Expired.ToInvitationStatusName());
    }
}
