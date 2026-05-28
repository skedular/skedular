using Location.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Location.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Location.Api")]
public class LocationMutationContractShould(ILocationMutationContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Field_Selection(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var data = result.Data.ShouldNotBeNull();
        ShouldHaveFieldSelection(data.UpdateLocationInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateLocationOpeningHoursInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateLocationPhysicalAddressInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateLocationRestrictedInformationInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateFloorPlanInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateResourcePositionsInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateResourceInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateLocationResourceAvailableHoursInput?.InputFields?.Select(field => field.Name));
    }

    private static void ShouldHaveFieldSelection(IEnumerable<string>? fields)
    {
        fields.ShouldNotBeNull();
        fields.ShouldContain("fieldsToUpdate");
    }
}
