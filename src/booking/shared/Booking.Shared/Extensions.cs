using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Api.Shared.Grpc.Skedular.Organization.Billing.V1;
using Api.Shared.Grpc.Skedular.Organization.Core.V1;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Booking.Shared.Services.Entitlements;
using Enterprise.Shared.Outbox.Temporal;
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
                .AddSingleton<EntityMapper>()
                .AddSingleton<IEntityMapper>(sp => sp.GetRequiredService<EntityMapper>())
                .AddSingleton<IEntitlementModelMapper>(sp => sp.GetRequiredService<EntityMapper>())
                .AddSingleton<IEntitlementPurchaseModelMapper>(sp => sp.GetRequiredService<EntityMapper>())
                .AddSingleton<IEventMapper, EventMapper>();

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
                .AddSingleton<IMarketplaceBookingAvailableDaysService, MarketplaceBookingAvailableDaysService>()
                .AddSingleton<IMarketplaceBookingWeeklyDaySelectionService, MarketplaceBookingWeeklyDaySelectionService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedTeamService, CachedTeamService>()
                .AddScoped<ICachedBookingService, CachedBookingService>()
                .AddScoped<ICachedRecurringBookingService, CachedRecurringBookingService>()
                .AddScoped<ICachedMarketplaceBookingSubscriptionService, CachedMarketplaceBookingSubscriptionService>()
                .AddScoped<ILocationResourceBookingSlotsHelperService, LocationResourceBookingSlotsHelperService>()
                .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
                .AddScoped<IMarketplaceRefundEventService, MarketplaceRefundEventService>()
                .AddScoped<IMarketplaceRefundTransitionService, MarketplaceRefundTransitionService>()
                .AddScoped<IMarketplacePurchaseHistoryEventService, MarketplacePurchaseHistoryEventService>()
                .AddScoped<IMarketplaceRefundService, MarketplaceRefundService>()
                .AddScoped<IEntitlementService, EntitlementService>()
                .AddScoped<IEntitlementPurchaseService, EntitlementPurchaseService>()
                .AddScoped<IEntitlementPurchaseCheckoutService, EntitlementPurchaseCheckoutService>()
                .AddScoped<IEntitlementPurchaseBankTransferService, EntitlementPurchaseBankTransferService>()
                .AddScoped<IEntitlementPurchasePaymentCancellationService, EntitlementPurchasePaymentCancellationService>()
                .AddScoped<IEntitlementPurchasePaymentReconciliationService, EntitlementPurchasePaymentReconciliationService>()
                .AddScoped<IEntitlementInvoiceService, EntitlementInvoiceService>()
                .AddScoped<IEntitlementRenewalService, EntitlementRenewalService>()
                .AddScoped<IEntitlementAdjustmentService, EntitlementAdjustmentService>()
                .AddScoped<IHostCommissionService, HostCommissionService>()
                .AddScoped<IHostStripeApplicationFeeService, HostStripeApplicationFeeService>()
                .AddScoped<IStripeHostRefundService, StripeHostRefundService>()
                .AddScoped<IStripeHostRefundClient, StripeHostRefundClient>()
                .AddScoped<IStripePayoutReconciliationService, StripePayoutReconciliationService>()
                .AddScoped<IMarketplaceRefundAutomationService, MarketplaceRefundAutomationService>()
                .AddScoped<IMarketplaceRefundExhaustionService, MarketplaceRefundExhaustionService>()
                .AddScoped<IMarketplaceRefundNotificationService, MarketplaceRefundNotificationService>()
                .AddScoped<IMarketplaceRefundReconciliationService, MarketplaceRefundReconciliationService>()
                .AddScoped<IMarketplaceRefundOperationsService, MarketplaceRefundOperationsService>()
                .AddScoped<IMarketplaceBookingFailureService, MarketplaceBookingFailureService>()
                .AddScoped<IMarketplaceBookingCleanupReconciliationService, MarketplaceBookingCleanupReconciliationService>()
                .AddScoped<IMarketplaceBookingAccountingCleanupService, MarketplaceBookingAccountingCleanupService>()
                .AddScoped<IMarketplacePartialBookingResolutionService, MarketplacePartialBookingResolutionService>()
                .AddSingleton<IMarketplaceRefundOwnershipService, MarketplaceRefundOwnershipService>()
                .AddScoped<IMarketplaceBookingFailureNotificationService, MarketplaceBookingFailureNotificationService>()
                .AddScoped<IMarketplaceBookingModificationNotificationService, MarketplaceBookingModificationNotificationService>()
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
                .AddSingleton<ISpacesBookingInstanceCounter, SpacesBookingInstanceCounter>()
                .AddScoped<ISpacesBookingQuotaService, SpacesBookingQuotaService>()
                .AddScoped<ISpacesBookingUsageRolloverService, SpacesBookingUsageRolloverService>()
                .AddScoped<IMarketplaceBookingSubscriptionService, MarketplaceBookingSubscriptionService>()
                .AddScoped<IMarketplaceBookingService, MarketplaceBookingService>()
                .AddScoped<ICreditLedgerService, CreditLedgerService>()
                .AddScoped<IEntitlementEligibilityService, EntitlementEligibilityService>()
                .AddScoped<IEntitlementBookingService, EntitlementBookingService>()
                .AddScoped<IEntitlementCancellationService, EntitlementCancellationService>()
                .AddScoped<IEntitlementExpiryService, EntitlementExpiryService>()
                .AddScoped<ISkedularInvoiceService, SkedularInvoiceService>()
                .AddScoped<IXeroInvoiceService, XeroInvoiceService>()
                .AddScoped<IXeroRefundService, XeroRefundService>()
                .AddSingleton<IResourceAvailabilityClassifier, ResourceAvailabilityClassifier>()
                .AddSingleton<IResourceDayViewBookingVisibilityFilter, ResourceDayViewBookingVisibilityFilter>()
                .AddSingleton<ISubscriptionKeyService, SubscriptionKeyService>()
                .AddScoped<IResourceAvailabilityDayViewService, ResourceAvailabilityDayViewService>()
                .AddOptions<ResourceAvailabilityOptions>()
                .Services;

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
                .AddScoped<IEntitlementRepository, EntitlementRepository>()
                .AddScoped<IMarketplaceBookingFailureRepository, MarketplaceBookingFailureRepository>()
                .AddScoped<IMarketplaceBookingFailureEventRepository, MarketplaceBookingFailureEventRepository>()
                .AddScoped<IMarketplaceBookingFailureDeliveryRepository, MarketplaceBookingFailureDeliveryRepository>()
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
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ISpacesBookingUsageRepository, SpacesBookingUsageRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddSingleton<IBookingInternalPublisher, BookingInternalPublisher>()
                .AddSingleton<IBookingPublisher, BookingPublisher>()
                .AddSingleton<ICustomerReadinessPublisher, CustomerReadinessPublisher>();

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
            services.AddGrpcClient<OrganizationBillingService.OrganizationBillingServiceClient>(GrpcClients.ConfigureOrganization);

            return services
                .AddSingleton(coreConfiguration)
                .AddSingleton(organizationConfiguration);
        }
    }
}
