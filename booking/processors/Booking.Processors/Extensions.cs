using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Grpc.Skedular.Booking.Graphql.V1;
using Booking.Processors.Mappers;
using Booking.Processors.Services;
using Enterprise.Shared.GraphQL;

namespace Booking.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddServices() =>
            services
                .AddSingleton<IGraphQlTopicEventSender, GraphQlTopicEventSender>();

        public IServiceCollection AddCrossDomainClients(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

            services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureBooking);
            services.AddGrpcClient<BookingGraphqlService.BookingGraphqlServiceClient>(GrpcClients.ConfigureBooking);

            return services
                .AddSingleton(bookingConfiguration);
        }
    }
}
