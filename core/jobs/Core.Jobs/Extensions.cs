namespace Core.Jobs;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services;

        public IServiceCollection AddServices() =>
            services;

        public IServiceCollection AddJobs() =>
            services;
    }
}
