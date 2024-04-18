using GraphQL;
using Unityhubctl.GraphQL.Generator.Exceptions;

namespace Unityhubctl.GraphQL.Generator.Base;

public class Method(
    string name,
    string type,
    bool isArray,
    bool isNullable,
    IEnumerable<Directive?>? directives = null)
    : IMember
{
    private readonly string _name = name.ToPascalCase();
    private readonly string _type = type.ToGraphqlScalarNameToClrTypeName();
    public HashSet<IMember> Arguments { get; } = [];
    public override int GetHashCode() => HashCode.Combine(_name, _type);

    public override string ToString()
    {
        var props = Arguments.Reverse().Select(prop => prop.ToString())
            .Append("[HotChocolate.Service] IServiceProvider serviceProvider")
            .Append("CancellationToken cancellationToken");
        var arguments = string.Join(", ", props);
        var directiveAttributes = directives.ToDirectiveAttributes();

        return
            $@"
                {directiveAttributes}
                public abstract Task<{_type}{(isArray ? "[]" : "")}{(isNullable ? "?" : "")}> {_name}Async({arguments});
            ";
    }
}
