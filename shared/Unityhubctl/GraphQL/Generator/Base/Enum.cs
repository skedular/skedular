namespace Unityhubctl.GraphQL.Generator.Base;

public class Enum(string name) : IGeneratableType
{
    public string GraphName { get; } = name;

    public HashSet<IMember> Properties { get; } = [];

    public override int GetHashCode() => GraphName.GetHashCode();

    public override string ToString()
    {
        var separator = $",{Environment.NewLine}            ";
        var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));

        return $@"
        public enum {GraphName}
        {{
            {properties}
        }}
        ";
    }
}
