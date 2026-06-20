using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Enterprise.Shared.Outbox.Temporal;
using Location.Shared.Configurations;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Location.Shared.Services;
using Location.Shared.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BookingService = Api.Shared.Grpc.Skedular.Booking.Core.V1.BookingService;
using LocationGraphqlService = Api.Shared.Grpc.Skedular.Location.Graphql.V1.LocationGraphqlService;
using MarketplaceService = Api.Shared.Grpc.Skedular.Marketplace.Core.V1.MarketplaceService;
using OrganizationTagsService = Api.Shared.Grpc.Skedular.Organization.Tags.V1.OrganizationTagsService;

namespace Location.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration)
        {
            var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
            ArgumentNullException.ThrowIfNull(emailConfiguration);

            return services.AddSingleton(emailConfiguration);
        }

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IEntityMapper, EntityMapper>()
                .AddSingleton<IEventMapper, EventMapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<IWorkflowIdService, WorkflowIdService>()
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddScoped<IAutoResourceService, AutoResourceService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedLocationService, CachedLocationService>()
                .AddScoped<ICachedResourceService, CachedResourceService>()
                .AddScoped<ICachedLocationBookingAccessService, CachedLocationBookingAccessService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<ILocationPhysicalAddressRepository, LocationPhysicalAddressRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IDailyDeskCountRecordingRepository, DailyDeskCountRecordingRepository>()
                .AddScoped<IDailyRoomCountRecordingRepository, DailyRoomCountRecordingRepository>()
                .AddScoped<IResourceRepository, ResourceRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
                .AddScoped<IFloorPlanRepository, FloorPlanRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IProductVersionRepository, ProductVersionRepository>()
                .AddScoped<IPrecomputedLocationProductRepository, PrecomputedLocationProductRepository>()
                .AddScoped<ILocationRestrictedInformationRepository, LocationRestrictedInformationRepository>()
                .AddScoped<ILocationBookingAccessRepository, LocationBookingAccessRepository>()
                .AddScoped<ILocationBookingRecordingRepository, LocationBookingRecordingRepository>()
                .AddScoped<IDailyResourceAvailabilitySnapshotRepository, DailyResourceAvailabilitySnapshotRepository>()
                .AddScoped<IResourcePositionRepository, ResourcePositionRepository>()
                .AddScoped<IDailyBookingCountRecordingRepository, DailyBookingCountRecordingRepository>()
                .AddScoped<IDailyDeskBookingCountRecordingRepository, DailyDeskBookingCountRecordingRepository>()
                .AddScoped<IDailyRoomBookingCountRecordingRepository, DailyRoomBookingCountRecordingRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddSingleton<ILocationPublisher, LocationPublisher>()
                .AddSingleton<ICustomerReadinessPublisher, CustomerReadinessPublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<ILocationOutboxPublisher, LocationOutboxPublisher>();

        public IServiceCollection AddSharedCrossDomainClients(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

            services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureBooking);

            var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
            ArgumentNullException.ThrowIfNull(organizationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);
            services.AddGrpcClient<OrganizationTagsService.OrganizationTagsServiceClient>(GrpcClients.ConfigureOrganization);

            var marketplaceConfiguration = configuration.GetSection(MarketplaceConfiguration.Key).Get<MarketplaceConfiguration>();
            ArgumentNullException.ThrowIfNull(marketplaceConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(marketplaceConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(marketplaceConfiguration.GrpcUrl);
            services.AddGrpcClient<MarketplaceService.MarketplaceServiceClient>(GrpcClients.ConfigureMarketplace);

            var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
            ArgumentNullException.ThrowIfNull(locationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);
            services.AddGrpcClient<LocationGraphqlService.LocationGraphqlServiceClient>(GrpcClients.ConfigureLocation);

            return services
                .AddSingleton(bookingConfiguration)
                .AddSingleton(organizationConfiguration)
                .AddSingleton(marketplaceConfiguration)
                .AddSingleton(locationConfiguration);
        }
    }
}
