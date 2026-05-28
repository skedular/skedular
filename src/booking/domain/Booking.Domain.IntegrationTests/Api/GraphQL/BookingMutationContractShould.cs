using Booking.Domain.IntegrationTests.Skedular.GraphQL.V1;

namespace Booking.Domain.IntegrationTests.Api.GraphQL;

[Trait(CategoryNames.Key, CategoryNames.Integration)]
[Collection("Booking.Api")]
public class BookingMutationContractShould(IBookingMutationContractQuery query)
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Require_Field_Selection(CancellationToken cancellationToken)
    {
        var result = await query.ExecuteAsync(cancellationToken);

        result.Errors.Select(error => error.Message).ShouldBeEmpty();
        var data = result.Data.ShouldNotBeNull();
        ShouldHaveFieldSelection(data.UpdatePrivateBookingInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdateMarketplaceBookingInput?.InputFields?.Select(field => field.Name));
        ShouldHaveFieldSelection(data.UpdatePrivateRecurringBookingInput?.InputFields?.Select(field => field.Name));
    }

    private static void ShouldHaveFieldSelection(IEnumerable<string>? fields)
    {
        fields.ShouldNotBeNull();
        fields.ShouldContain("fieldsToUpdate");
    }
}
