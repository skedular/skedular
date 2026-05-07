using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Grpc.Skedular.Customer.Admin.V1;
using Enterprise.Shared.Outbox.Temporal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Team.Shared.Configurations;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services;
using Team.Shared.Services.Cache;
using CustomerService = Api.Shared.Grpc.Skedular.Customer.Core.V1.CustomerService;

namespace Team.Shared;

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
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedTeamService, CachedTeamService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<IJoinInvitationRepository, JoinInvitationRepository>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<ILocationRepository, LocationRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddSingleton<ITeamPublisher, TeamPublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<ITeamOutboxPublisher, TeamOutboxPublisher>();

        public IServiceCollection AddSharedCrossDomainClients(IConfiguration configuration)
        {
            var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
            ArgumentNullException.ThrowIfNull(customerConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

            services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);
            services.AddGrpcClient<CustomerAdminService.CustomerAdminServiceClient>(GrpcClients.ConfigureCustomer);

            return services.AddSingleton(customerConfiguration);
        }
    }
}
