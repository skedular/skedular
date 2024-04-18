namespace Unityhubctl.GraphQL.Generator.Base;

public class EnumValue(string graphName) : IMember
{
    private string GraphName { get; } = graphName;
    public override int GetHashCode() => GraphName.GetHashCode();
    public override string ToString() => GraphName;
}
