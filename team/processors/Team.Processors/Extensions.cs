using Team.Processors.Mappers;

namespace Team.Processors;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IEventMapper, EventMapper>();
    }
}
