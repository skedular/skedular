using Slack.Processors.Mappers;

namespace Slack.Processors;

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
