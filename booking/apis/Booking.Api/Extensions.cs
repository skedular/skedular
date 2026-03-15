using Api.Shared.Services.Configurations.Grpc;
using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;
using Enterprise.Shared.GraphQL;

namespace Booking.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
                .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
                .AddScoped<IBookingPaymentService, BookingPaymentService>()
                .AddScoped<IBookingService, BookingService>()
                .AddScoped<IRecurringBookingService, RecurringBookingService>()
                .AddScoped<IPrivateBookingService, PrivateBookingService>()
                .AddScoped<IPrivateRecurringBookingService, PrivateRecurringBookingService>()
                .AddScoped<IMarketplaceBookingService, MarketplaceBookingService>()
                .AddScoped<IMarketplaceBookingSubscriptionService, MarketplaceBookingSubscriptionService>()
                .AddScoped<IMarketplaceRecurringBookingService, MarketplaceRecurringBookingService>()
                .AddScoped<IResourceService, ResourceService>()
                .AddScoped<IGraphQlTopicEventSender, GraphQlTopicEventSender>()
                .AddScoped<IWorkaroundService, WorkaroundService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);

            return services
                .AddSingleton(bookingConfiguration);
        }
    }
}
