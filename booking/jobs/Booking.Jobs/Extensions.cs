using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Booking.Jobs.Services;
using Enterprise.Shared.GraphQL;

namespace Booking.Jobs;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services;

        public IServiceCollection AddServices() =>
            services
                .AddSingleton<IDomainGraphQlTopicEventSender, DomainGraphQlTopicEventSender>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddInDomainClients(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

            services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureBooking);

            return services
                .AddSingleton(bookingConfiguration);
        }
    }
}
