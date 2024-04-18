using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;

namespace Unityhubctl.GraphQL.Generator;

public static class QueryArgumentExtensions
{
    public static bool IsArray(this QueryArgument queryArgument)
    {
        var fieldMetaData =
            queryArgument.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.IsArrayType();
    }

    public static bool IsNullable(this QueryArgument queryArgument)
    {
        var fieldMetaData =
            queryArgument.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.Kind != ASTNodeKind.NonNullType;
    }

    public static string GetTypeName(this QueryArgument queryArgument)
    {
        ArgumentNullException.ThrowIfNull(queryArgument);
        ArgumentNullException.ThrowIfNull(queryArgument.ResolvedType);

        return queryArgument.ResolvedType.GetNamedType() is GraphQLTypeReference typeReference
            ? typeReference.TypeName
            : ((GraphType)queryArgument.ResolvedType.GetNamedType()).Name;
    }
}
