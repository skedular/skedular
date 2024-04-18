using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;

namespace Unityhubctl.GraphQL.Generator;

public static class FieldTypeExtensions
{
    public static bool IsArray(this FieldType fieldType)
    {
        var fieldMetaData = fieldType.GetMetadata<GraphQLFieldDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.IsArrayType();
    }

    public static bool IsNullable(this FieldType fieldType)
    {
        var fieldMetaData = fieldType.GetMetadata<GraphQLFieldDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.Kind != ASTNodeKind.NonNullType;
    }

    public static bool InputFieldIsArray(this FieldType fieldType)
    {
        var fieldMetaData =
            fieldType.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.IsArrayType();
    }

    public static AppliedDirectives GetFieldDirectives(this FieldType fieldType) =>
        fieldType.GetMetadata<AppliedDirectives?>("__APPLIED__DIRECTIVES__") ?? [];

    public static AppliedDirectives GetFieldDirectives(this IGraphType graphType) =>
        graphType.GetMetadata<AppliedDirectives?>("__APPLIED__DIRECTIVES__") ?? [];

    public static bool InputFieldIsNullable(this FieldType fieldType)
    {
        var fieldMetaData =
            fieldType.GetMetadata<GraphQLInputValueDefinition>("__AST_MetaField__");

        return fieldMetaData.Type.Kind != ASTNodeKind.NonNullType;
    }

    public static string GetTypeName(this FieldType fieldType)
    {
        ArgumentNullException.ThrowIfNull(fieldType);
        ArgumentNullException.ThrowIfNull(fieldType.ResolvedType);

        return fieldType.ResolvedType.GetNamedType() is GraphQLTypeReference reference
            ? reference.TypeName
            : ((GraphType)fieldType.ResolvedType.GetNamedType()).Name;
    }
}
