using Customer.Shared.Configurations;
using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Shared;

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
                .AddSingleton<IWorkflowIdService, WorkflowIdService>()
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<ICustomerFeedbackRepository, CustomerFeedbackRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IResourceRepository, ResourceRepository>()
                .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
                .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
                .AddScoped<IStripePaymentIntentRepository, StripePaymentIntentRepository>()
                .AddScoped<IStripePaymentMethodRepository, StripePaymentMethodRepository>()
                .AddScoped<ICustomerBillingDetailsRepository, CustomerBillingDetailsRepository>();

        public IServiceCollection AddPublishers() =>
            services.AddSingleton<ICustomerPublisher, CustomerPublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<ICustomerOutboxPublisher, CustomerOutboxPublisher>();
    }
}
