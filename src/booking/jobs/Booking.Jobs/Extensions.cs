using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Grpc.Skedular.Booking.Graphql.V1;
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
                .AddSingleton<IGraphQlTopicEventSender, GraphQlTopicEventSender>();

        public IServiceCollection AddJobs() =>
            services
                .AddHostedService<SpacesBookingUsageRolloverWorkflowHostedService>()
                .AddHostedService<MarketplaceRefundReconciliationHostedService>();

        public IServiceCollection AddInDomainClients(IConfiguration configuration)
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
