using GraphQL;
using Unityhubctl.GraphQL.Generator.Exceptions;

namespace Unityhubctl.GraphQL.Generator.Base;

public class Class(
    string name,
    IEnumerable<Directive?>? directives) : IGeneratableType
{
    private readonly string _className = name.ToPascalCase();
    private bool HasInterface => Interfaces.Count != 0;

    public HashSet<string> Interfaces { get; } = [];
    public string GraphName { get; } = name;
    public HashSet<IMember> Properties { get; } = [];

    public override int GetHashCode() => GraphName.GetHashCode();

    public override string ToString()
    {
        var separator = $"{Environment.NewLine}            ";
        var properties = string.Join(separator, Properties.Select(prop => prop.ToString()));
        var directiveAttributes = directives.ToDirectiveAttributes();

        if (HasInterface)
        {
            var interfaces = string.Join(", ", Interfaces);

            return $@"
        {directiveAttributes}
        [HotChocolate.GraphQLName(""{GraphName}"")]
        public class {_className} : {interfaces}
        {{
            {properties}
        }}
        ";
        }

        return $@"
        {directiveAttributes}
        [HotChocolate.GraphQLName(""{GraphName}"")]
        public class {_className}
        {{
            {properties}
        }}
        ";
    }
}
