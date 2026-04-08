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
            return services.AddSingleton(emailConfiguration);
        }

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<IWorkflowIdService, WorkflowIdService>()
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddSingleton<IInvoicePaymentTermsService, InvoicePaymentTermsService>()
                .AddScoped<IAccountingInvoiceCancellationService, AccountingInvoiceCancellationService>()
                .AddSingleton<IRecurringInvoiceBillingScheduleService, RecurringInvoiceBillingScheduleService>()
                .AddSingleton<IRecurringBookingScheduleService, RecurringBookingScheduleService>()
                .AddSingleton<MarketplaceRefundPolicyService>()
                .AddSingleton<IXeroRepeatingInvoiceScheduleService, XeroRepeatingInvoiceScheduleService>()
                .AddSingleton<IXeroRecurringInvoiceTransitionService, XeroRecurringInvoiceTransitionService>()
                .AddSingleton<IProductVersionHelperService, ProductVersionHelperService>()
                .AddSingleton<IOrganizationArrearsInvoiceService, OrganizationArrearsInvoiceService>()
                .AddSingleton<IOrganizationArrearsChargeSegmentService, OrganizationArrearsChargeSegmentService>()
                .AddSingleton<IOrganizationArrearsBillingPlannerService, OrganizationArrearsBillingPlannerService>()
                .AddScoped<IMarketplaceBookingOpeningHoursService, MarketplaceBookingOpeningHoursService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedTeamService, CachedTeamService>()
                .AddScoped<ICachedBookingService, CachedBookingService>()
                .AddScoped<ICachedRecurringBookingService, CachedRecurringBookingService>()
                .AddScoped<ICachedMarketplaceBookingSubscriptionService, CachedMarketplaceBookingSubscriptionService>()
                .AddScoped<ILocationResourceBookingSlotsHelperService, LocationResourceBookingSlotsHelperService>()
                .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
                .AddScoped<IMarketplaceRefundEventService, MarketplaceRefundEventService>()
                .AddScoped<IMarketplaceRefundService, MarketplaceRefundService>()
                .AddScoped<IMarketplaceRefundAutomationService, MarketplaceRefundAutomationService>()
                .AddScoped<IMarketplaceRefundNotificationService, MarketplaceRefundNotificationService>()
                .AddScoped<IStripeProductPricingService, StripeProductPricingService>()
                .AddScoped<IStripeCustomerService, StripeCustomerService>()
                .AddScoped<IXeroWebhookService, XeroWebhookService>()
                .AddScoped<IBookingInvoiceService, BookingInvoiceService>()
                .AddScoped<IOrganizationInvoiceCounterService, OrganizationInvoiceCounterService>()
                .AddScoped<IResourceService, ResourceService>()
                .AddScoped<IMarketplaceEventResourceService, MarketplaceEventResourceService>()
                .AddScoped<IPrivateBookingPreferenceService, PrivateBookingPreferenceService>()
                .AddScoped<IMarketplaceBookingPreferenceService, MarketplaceBookingPreferenceService>()
                .AddScoped<IPrivateBookingService, PrivateBookingService>()
                .AddScoped<IPrivateRecurringBookingService, PrivateRecurringBookingService>()
                .AddScoped<IMarketplaceBookingSubscriptionService, MarketplaceBookingSubscriptionService>()
                .AddScoped<IMarketplaceBookingService, MarketplaceBookingService>()
                .AddScoped<ISkedularInvoiceService, SkedularInvoiceService>()
                .AddScoped<IXeroInvoiceService, XeroInvoiceService>()
                .AddScoped<IXeroRefundService, XeroRefundService>();

        public IServiceCollection AddRepositoryFactory() =>
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<IAccountingContactLinkRepository, AccountingContactLinkRepository>()
                .AddScoped<IAccountingInvoiceExportLinkRepository, AccountingInvoiceExportLinkRepository>()
                .AddScoped<IAccountingInvoiceInstanceRepository, AccountingInvoiceInstanceRepository>()
                .AddScoped<IAccountingPaymentEventRepository, AccountingPaymentEventRepository>()
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IMarketplaceBookingRepository, MarketplaceBookingRepository>()
                .AddScoped<IMarketplaceBookingSubscriptionRepository, MarketplaceBookingSubscriptionRepository>()
                .AddScoped<IMarketplaceRefundEventRepository, MarketplaceRefundEventRepository>()
                .AddScoped<IMarketplaceRefundRepository, MarketplaceRefundRepository>()
                .AddScoped<IOrganizationArrearsInvoiceRepository, OrganizationArrearsInvoiceRepository>()
                .AddScoped<IOrganizationInvoiceCounterRepository, OrganizationInvoiceCounterRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IProductVersionRepository, ProductVersionRepository>()
                .AddScoped<IRecurringBookingRepository, RecurringBookingRepository>()
                .AddScoped<IResourceBookingSlotRepository, ResourceBookingSlotRepository>()
                .AddScoped<IResourceRepository, ResourceRepository>()
                .AddScoped<IStripeCheckoutSessionRepository, StripeCheckoutSessionRepository>()
                .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
                .AddScoped<IStripePriceRepository, StripePriceRepository>()
                .AddScoped<IStripeProductRepository, StripeProductRepository>()
                .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
                .AddScoped<ITeamRepository, TeamRepository>();

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
