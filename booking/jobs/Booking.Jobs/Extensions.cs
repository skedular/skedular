using Booking.Jobs.Jobs;
using Booking.Jobs.Services;
using Enterprise.Shared.Outbox;

namespace Booking.Jobs;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ITemporalOutboxExecutor, TemporalOutboxExecutorService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<GenerateResourceBookingSlotJob>()
            .AddHostedService<ReleaseUnpaidBookingsJob>();
}
