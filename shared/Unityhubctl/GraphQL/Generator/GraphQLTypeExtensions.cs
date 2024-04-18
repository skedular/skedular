using GraphQLParser.AST;

namespace Unityhubctl.GraphQL.Generator;

// ReSharper disable once InconsistentNaming
public static class GraphQLTypeExtensions
{
    public static bool IsArrayType(this GraphQLType graphqlType) =>
        graphqlType.Kind switch
        {
            ASTNodeKind.NonNullType => IsArrayType(((GraphQLNonNullType)graphqlType).Type),
            ASTNodeKind.ListType => true,
            _ => false
        };
}
