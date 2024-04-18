using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Context;

public static class Extensions
{
    public static AsyncServiceScope CreateScopeAndSetContent(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var scopedContext = serviceProvider.GetRequiredService<IContext>();
        var context = scope.ServiceProvider.GetRequiredService<IContext>();
        context.PropertyBag = scopedContext.PropertyBag;

        return scope;
    }
}
