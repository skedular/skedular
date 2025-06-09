using System.Text.Json;
using Booking.Shared.Workflows;
using Enterprise.Shared.Outbox;
using Temporalio.Client;
using Temporalio.Exceptions;

namespace Booking.Jobs.Services;

public class TemporalOutboxExecutorService(ITemporalClient temporalClient) : ITemporalOutboxExecutor
{
    private static readonly string s_bookingPaidThroughStripeType = typeof(BookingPaidThroughStripe).ToWorkflowType();

    public async Task StartWorkflowAsync(string workflowType, string? executionArgs, WorkflowOptions workflowOptions)
    {
        await temporalClient.Connection.ConnectAsync();

        if (workflowType == s_bookingPaidThroughStripeType)
        {
            try
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(executionArgs);
                var input = JsonSerializer.Deserialize<BookingPaidThroughStripeInput>(executionArgs);
                ArgumentNullException.ThrowIfNull(input);

                _ = await temporalClient.StartWorkflowAsync(
                    (BookingPaidThroughStripe workflow) => workflow.ExecuteAsync(input),
                    workflowOptions);
            }
            catch (WorkflowAlreadyStartedException)
            {
            }
        }
    }
}
