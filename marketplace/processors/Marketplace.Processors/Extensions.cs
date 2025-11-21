using Marketplace.Processors.Mappers;

namespace Marketplace.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddServices() =>
            services;

        public IServiceCollection AddJobs() =>
            services;
    }
}
