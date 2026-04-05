using Api.Shared.Services;
using AutoFixture.Xunit3;
using Booking.Api.Services.Authorization;
using Booking.Shared.Database.Entities;
using Booking.Shared.Repositories;
using FakeItEasy;

namespace Booking.Api.UnitTests.Services.Authorization.TeamAuthorizationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class GetBookingInvolvedTeamAndValidatePermissionsAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Empty_When_No_Team_Ids_Are_Provided(
        TeamAuthorizationService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await sut.GetBookingInvolvedTeamAndValidatePermissionsAsync([], customerId, false, cancellationToken);

        result.ShouldBeEmpty();
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_A_Team_Is_Not_Found(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamRepository teamRepository,
        TeamAuthorizationService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var teamIds = new[] { "team-1" };

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => teamRepository.GetByIdsAsync(teamIds, false, cancellationToken)).Returns([]);

        await Should.ThrowAsync<TeamNotFound>(() =>
            sut.GetBookingInvolvedTeamAndValidatePermissionsAsync(teamIds, customerId, false, cancellationToken));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Throw_When_Add_Permission_Is_Denied(
        [Frozen] IRepositoryFactory repositoryFactory,
        [Frozen] ITeamRepository teamRepository,
        [Frozen] IOrganizationAuthorizationService organizationAuthorizationService,
        TeamAuthorizationService sut,
        string customerId,
        CancellationToken cancellationToken)
    {
        var team = new Team { Id = "team-1", Organization = new Organization { Id = "org-1" } };

        A.CallTo(() => repositoryFactory.TeamRepository).Returns(teamRepository);
        A.CallTo(() => teamRepository.GetByIdsAsync(A<ICollection<string>>._, false, cancellationToken)).Returns([team]);
        A.CallTo(() => organizationAuthorizationService.CanAddBookingAsync("org-1", customerId, cancellationToken)).Returns(false);

        await Should.ThrowAsync<UnauthorizedAccessException>(() =>
            sut.GetBookingInvolvedTeamAndValidatePermissionsAsync([team.Id], customerId, false, cancellationToken));
    }
}
