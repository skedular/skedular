using Api.Shared.Services.Models;
using Team.Api.GraphQL.Invitation;
using Team.Api.Mappers;

namespace Team.Api.UnitTests.GraphQL.Invitation.RootQueryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TeamInvitationStatusesShould
{
    [Fact]
    public void Return_All_Invitation_Statuses()
    {
        var sut = new RootQuery(A.Fake<IMapper>());

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
