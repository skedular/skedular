using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Context;

public interface IContext
{
    PropertyBag PropertyBag { get; }
    void SetPropertyBag(PropertyBag propertyBag);
    PropertyBag GetPropertyBag();
}

public class Context(IHttpContextAccessor httpContextAccessor) : IContext
{
    private const string PropertyBagKey = "PropertyBag";

    public void SetPropertyBag(PropertyBag propertyBag) => GetHttpContext().Items[PropertyBagKey] = propertyBag;

    public PropertyBag GetPropertyBag()
    {
        if (!GetHttpContext().Items.TryGetValue(PropertyBagKey, out var propertyBag))
        {
            return new PropertyBag();
        }

        var propertyBagValue = propertyBag as PropertyBag;
        ArgumentNullException.ThrowIfNull(propertyBagValue);
        return propertyBagValue;
    }

    public PropertyBag PropertyBag => GetPropertyBag();

    private HttpContext GetHttpContext()
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

        return httpContextAccessor.HttpContext;
    }
}
