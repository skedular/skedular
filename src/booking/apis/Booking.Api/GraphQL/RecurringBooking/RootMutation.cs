using Api.Shared.Services;
using Booking.Api.GraphQL.Booking;
using Booking.Api.Mappers;
using Booking.Api.Models;
using Booking.Api.Services;
using HotChocolate;
using HotChocolate.Types;

namespace Booking.Api.GraphQL.RecurringBooking;

[MutationType]
public class RootMutation(IGraphQlMapper graphQlMapper)
{
    [UseResolverScope]
    public async Task<RecurringBookingPayload> AddPrivateRecurringBookingAsync(
        AddPrivateRecurringBookingInput input,
        [Service]
        IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var recurringBooking = await privateRecurringBookingService.AddAsync(graphQlMapper.MapTo(input), cancellationToken);
            return new RecurringBookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
            };
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> UpdatePrivateRecurringBookingAsync(
        UpdatePrivateRecurringBookingInput input,
        [Service]
        IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        try
        {
            var recurringBooking = await privateRecurringBookingService.UpdateAsync(
                new PrivateRecurringBookingPatchRequest(graphQlMapper.MapTo(input), input.FieldsToUpdate),
                cancellationToken);
            return new RecurringBookingPayload
            {
                ClientMutationId = input.ClientMutationId,
                RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
            };
        }
        catch (SpacesAccessDenied exception)
        {
            return ToAccessErrorPayload(input.ClientMutationId, exception);
        }
    }

    [UseResolverScope]
    public async Task<RecurringBookingPayload> DeletePrivateRecurringBookingAsync(
        DeletePrivateRecurringBookingInput input,
        [Service]
        IPrivateRecurringBookingService privateRecurringBookingService,
        CancellationToken cancellationToken)
    {
        var recurringBooking = await privateRecurringBookingService.DeleteAsync(input.Id, cancellationToken);
        return new RecurringBookingPayload
        {
            ClientMutationId = input.ClientMutationId,
            RecurringBooking = graphQlMapper.MapTo(recurringBooking)!,
        };
    }

    private static RecurringBookingPayload ToAccessErrorPayload(string? clientMutationId, SpacesAccessDenied exception) =>
        new()
        {
            ClientMutationId = clientMutationId,
            AccessError = new SpacesAccessErrorDetails
            {
                ErrorCode = exception.ErrorCode,
                Status = exception.Status,
                ReasonCode = exception.ReasonCode,
                UpgradeRequired = exception.UpgradeRequired,
                Message = exception.Message,
            },
        };
}
