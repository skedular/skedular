using Location.Processors.Mappers;

namespace Location.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddJobs() =>
            services;
    }
}
