using Api.Shared.Services.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Shared.Services;

public static class Extensions
{
    public static IServiceCollection AddRootLevelSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<IGenericCustomerCacheService, GenericCustomerCacheService>();
}
