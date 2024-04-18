namespace Enterprise.Shared.Context;

public interface IContext
{
    PropertyBag PropertyBag { get; set; }
}

public class Context : IContext
{
    public PropertyBag PropertyBag { get; set; } = new();
}
