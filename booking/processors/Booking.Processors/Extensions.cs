using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Booking.Processors.Mappers;

namespace Booking.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddGrpcClients(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

            services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureCore);

            return services
                .AddSingleton(bookingConfiguration);
        }
    }
}
