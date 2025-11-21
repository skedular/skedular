using Api.Shared.Services.Configurations.Grpc;
using Core.Api.Mappers;
using Core.Api.Services;

namespace Core.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IFileUploaderService, FileUploaderService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var coreConfiguration = configuration.GetSection(CoreConfiguration.Key).Get<CoreConfiguration>();
            ArgumentNullException.ThrowIfNull(coreConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(coreConfiguration.ApiKey);

            return services
                .AddSingleton(coreConfiguration);
        }
    }
}
