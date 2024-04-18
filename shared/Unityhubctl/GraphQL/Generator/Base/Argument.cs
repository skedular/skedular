using GraphQL;

namespace Unityhubctl.GraphQL.Generator.Base;

public class Argument(string graphName, string graphType, bool isArray, bool isNullable) : IMember
{
    private readonly string _classType = graphType.ToGraphqlScalarNameToClrTypeName();
    private readonly string _parameterName = graphName.ToCamelCase();
    public override int GetHashCode() => HashCode.Combine(graphName, _classType);

    public override string ToString() =>
        graphType == "ID"
            ? $"[HotChocolate.GraphQLName(\"{graphName}\")][ID] {_classType}{(isArray ? "[]" : "")}{(isNullable ? "?" : "")} {_parameterName}"
            : $"[HotChocolate.GraphQLName(\"{graphName}\")] {_classType}{(isArray ? "[]" : "")}{(isNullable ? "?" : "")} {_parameterName}";
}
