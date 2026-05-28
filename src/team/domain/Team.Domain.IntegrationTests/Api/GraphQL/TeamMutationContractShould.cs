using Team.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Team.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Team.Api")]
public class TeamMutationContractShould(ITeamMutationContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Field_Selection(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var data = result.Data.ShouldNotBeNull();
        ShouldHaveFieldSelection(data.UpdateTeamInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateTeamAndTeamMembersInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateTeamMembersInput?.InputFields?.Select(field => field.Name));
    }

    private static void ShouldHaveFieldSelection(IEnumerable<string>? fields)
    {
        fields.ShouldNotBeNull();
        fields.ShouldContain("fieldsToUpdate");
    }
}
