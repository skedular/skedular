namespace Unityhubctl.GraphQL.Generator.Base;

public class Interface(string graphName) : IGeneratableType
{
    private string InterfaceName { get; } = graphName;
    public string GraphName { get; } = graphName;
    public HashSet<IMember> Properties { get; } = new();
    public override int GetHashCode() => (GraphName + InterfaceName).GetHashCode();

    public override string ToString()
    {
        var separator = $"{Environment.NewLine}            ";
        var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));

        return $@"
        [HotChocolate.GraphQLName(""{GraphName}"")]
        public interface {InterfaceName}
        {{
            {properties}
        }}
        ";
    }
}
