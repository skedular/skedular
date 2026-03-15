using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration)
        {
            var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
            ArgumentNullException.ThrowIfNull(emailConfiguration);
            services.AddSingleton(emailConfiguration);

            return services;
        }

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddSingleton<IRecurringBookingScheduleService, RecurringBookingScheduleService>()
                .AddSingleton<IProductVersionHelperService, ProductVersionHelperService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedTeamService, CachedTeamService>()
                .AddScoped<ICachedBookingService, CachedBookingService>()
                .AddScoped<ICachedRecurringBookingService, CachedRecurringBookingService>()
                .AddScoped<ILocationResourceBookingSlotsHelperService, LocationResourceBookingSlotsHelperService>()
                .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
                .AddScoped<IStripeProductPricingService, StripeProductPricingService>()
                .AddScoped<IStripeCustomerService, StripeCustomerService>()
                .AddScoped<IBookingInvoiceService, BookingInvoiceService>()
                .AddScoped<IOrganizationInvoiceCounterService, OrganizationInvoiceCounterService>()
                .AddScoped<IResourceService, ResourceService>()
                .AddScoped<IPrivateBookingPreferenceService, PrivateBookingPreferenceService>()
                .AddScoped<IMarketplaceBookingPreferenceService, MarketplaceBookingPreferenceService>()
                .AddScoped<IPrivateBookingService, PrivateBookingService>()
                .AddScoped<IPrivateRecurringBookingService, PrivateRecurringBookingService>()
                .AddScoped<IMarketplaceRecurringBookingService, MarketplaceRecurringBookingService>()
                .AddScoped<IMarketplaceBookingService, MarketplaceBookingService>();

        public IServiceCollection AddRepositoryFactory() =>
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<IRecurringBookingRepository, RecurringBookingRepository>()
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<IMarketplaceBookingRepository, MarketplaceBookingRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IResourceRepository, ResourceRepository>()
                .AddScoped<IResourceBookingSlotRepository, ResourceBookingSlotRepository>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
                .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IProductVersionRepository, ProductVersionRepository>()
                .AddScoped<IStripeProductRepository, StripeProductRepository>()
                .AddScoped<IStripePriceRepository, StripePriceRepository>()
                .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
                .AddScoped<IStripeCheckoutSessionRepository, StripeCheckoutSessionRepository>()
                .AddScoped<IOrganizationInvoiceCounterRepository, OrganizationInvoiceCounterRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddSingleton<IBookingInternalPublisher, BookingInternalPublisher>()
                .AddSingleton<IBookingPublisher, BookingPublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<IBookingOutboxPublisher, BookingOutboxPublisher>();

        public IServiceCollection AddSharedCrossDomainClients(IConfiguration configuration)
        {
            var coreConfiguration = configuration.GetSection(CoreConfiguration.Key).Get<CoreConfiguration>();
            ArgumentNullException.ThrowIfNull(coreConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(coreConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(coreConfiguration.GrpcUrl);

            var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
            ArgumentNullException.ThrowIfNull(organizationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

            services.AddGrpcClient<CoreService.CoreServiceClient>(GrpcClients.ConfigureCore);
            services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);

            return services
                .AddSingleton(coreConfiguration)
                .AddSingleton(organizationConfiguration);
        }
    }
}
