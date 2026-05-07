using Slack.Processors.Mappers;

namespace Slack.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IEventMapper, EventMapper>();

        public IServiceCollection AddJobs() =>
            services;
    }
}
