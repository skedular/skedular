using GraphQL;
using Unityhubctl.GraphQL.Generator.Exceptions;

namespace Unityhubctl.GraphQL.Generator.Base;

public class Property(
    string graphName,
    string type,
    bool isArray,
    bool isNullable,
    IEnumerable<Directive?>? directives = null)
    : IMember
{
    private readonly string _className = graphName.ToPascalCase();
    private readonly string _classType = type.ToGraphqlScalarNameToClrTypeName();
    public override int GetHashCode() => HashCode.Combine(_className, _classType);

    public override string ToString()
    {
        var directiveAttributes = directives.ToDirectiveAttributes();

        if (!string.IsNullOrWhiteSpace(directiveAttributes))
        {
            directiveAttributes += Environment.NewLine;
        }

        return type == "ID"
            ? $"{directiveAttributes}[HotChocolate.GraphQLName(\"{graphName}\")][ID]\n            public {_classType}{(isArray ? "[]" : "")}{(isNullable ? "?" : "")} {_className} {{ get; set; }}\n"
            : $"{directiveAttributes}[HotChocolate.GraphQLName(\"{graphName}\")]\n            public {_classType}{(isArray ? "[]" : "")}{(isNullable ? "?" : "")} {_className} {{ get; set; }}\n";
    }
}
