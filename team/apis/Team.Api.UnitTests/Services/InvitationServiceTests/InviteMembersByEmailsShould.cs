using Microsoft.Extensions.Logging;
using Team.Api.Services;

namespace Team.Api.UnitTests.Services.InvitationServiceTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class InviteMembersByEmailsShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Log_Information_When_Email_List_Is_Empty(
        [Frozen] ILogger<InvitationService> logger,
        InvitationService sut,
        CancellationToken cancellationToken)
    {
        var result = await sut.InviteMembersByEmailsAsync("team-1", [], cancellationToken);

        result.ShouldBeEmpty();
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == nameof(ILogger.Log) &&
                call.GetArgument<LogLevel>(0) == LogLevel.Information)
            .MustHaveHappened();
    }
}
