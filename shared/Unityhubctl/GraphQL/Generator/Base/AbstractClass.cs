using Unityhubctl.GraphQL.Generator.Exceptions;

namespace Unityhubctl.GraphQL.Generator.Base;

public class AbstractClass(
    string graphName,
    IEnumerable<Directive?>? directives = null) : IGeneratableType
{
    public HashSet<IMember> Methods { get; } = [];
    public HashSet<string> Interfaces { get; } = [];
    private bool HasInterface => Interfaces.Count != 0;
    public string GraphName { get; } = graphName;
    public HashSet<IMember> Properties { get; } = [];
    public override int GetHashCode() => GraphName.GetHashCode();

    public override string ToString()
    {
        var properties = string.Join(Environment.NewLine, Properties.Select(prop => prop.ToString())).Trim();
        var methods = string.Join(Environment.NewLine, Methods.Select(prop => prop.ToString())).Trim();
        var directiveAttributes = directives.ToDirectiveAttributes();

        if (HasInterface)
        {
            var interfaces = string.Join(", ", Interfaces);

            return $@"
        {directiveAttributes}
        public abstract class {GraphName} : {interfaces}
        {{
            {properties}
            {methods}
        }}
        ";
        }

        return $@"
        {directiveAttributes}
        public abstract class {GraphName}
        {{
            {properties}
            {methods}
        }}
        ";
    }
}
