using Api.Shared.Services.Offering;
using Booking.Api.GraphQL.Booking;
using Booking.Shared.Services;
using Enterprise.Shared.Metrics;
using GreenDonut;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Api.UnitTests.GraphQL.Booking;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class SpacesQuotaStatusDataLoaderShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Unavailable_For_Missing_Decision(
        ISpacesBookingQuotaService quotaService,
        IOpenTelemetryInstrumentation instrumentation,
        CancellationToken cancellationToken)
    {
        A.CallTo(() => quotaService.GetQuotaStatusesAsync(
                A<IReadOnlyCollection<string>>._,
                cancellationToken))
            .Returns(new Dictionary<string, SpacesQuotaDecision>());
        await using var serviceProvider = new ServiceCollection()
            .AddSingleton(quotaService)
            .BuildServiceProvider();
        var scheduler = new ManualBatchScheduler();
        var sut = new SpacesQuotaStatusDataLoader(
            scheduler,
            new DataLoaderOptions(),
            serviceProvider.GetRequiredService<IServiceScopeFactory>(),
            instrumentation);

        var resultTask = sut.LoadAsync(string.Empty, cancellationToken);
        await scheduler.DispatchAsync();

        (await resultTask)!.Available.ShouldBeFalse();
    }

    private sealed class ManualBatchScheduler : IBatchScheduler
    {
        private Batch? batch;

        public void Schedule(Batch scheduledBatch) => batch = scheduledBatch;

        public Task DispatchAsync() => batch?.DispatchAsync() ??
                                       throw new InvalidOperationException("No batch was scheduled.");
    }
}
