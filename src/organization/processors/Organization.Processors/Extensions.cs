using Organization.Processors.Mappers;

namespace Organization.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IEventMapper, EventMapper>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddServices() =>
            services;
    }
}
