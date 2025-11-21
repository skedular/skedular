using Microsoft.Extensions.DependencyInjection;

namespace Api.Shared.Services;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRootLevelSharedServices() =>
            services;
    }
}
