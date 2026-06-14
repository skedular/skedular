using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Offering;
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
            services
                .AddSingleton<IGraphQlMapper, GraphQlMapper>()
                .AddSingleton<IGrpcMapper, GrpcMapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<IPricingEntitlementEvaluator, PricingEntitlementEvaluator>()
                .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
                .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
                .AddScoped<IBookingPaymentService, BookingPaymentService>()
                .AddScoped<IRecurringBookingPaymentService, RecurringBookingPaymentService>()
                .AddScoped<IBookingService, BookingService>()
                .AddScoped<IRecurringBookingService, RecurringBookingService>()
                .AddScoped<IMarketplaceRefundReadService, MarketplaceRefundReadService>()
                .AddScoped<IMarketplaceRefundPreviewService, MarketplaceRefundPreviewService>()
                .AddScoped<IMarketplaceRefundAdminService, MarketplaceRefundAdminService>()
                .AddScoped<IPrivateBookingService, PrivateBookingService>()
                .AddScoped<IPrivateRecurringBookingService, PrivateRecurringBookingService>()
                .AddScoped<IMarketplaceBookingService, MarketplaceBookingService>()
                .AddScoped<IMarketplaceBookingSubscriptionService, MarketplaceBookingSubscriptionService>()
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
