using Booking.Domain.FakeDependencies.Fakes;

namespace Booking.Domain.FakeDependencies;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddFakeDependencyServices() =>
            services
                .AddSingleton<FakeCoreGrpcState>()
                .AddSingleton<FakeOrganizationGrpcState>();
    }
}
