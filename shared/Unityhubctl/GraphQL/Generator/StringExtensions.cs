namespace Unityhubctl.GraphQL.Generator;

public static class StringExtensions
{
    public static string ToGraphqlScalarNameToClrTypeName(this string propertyType) =>
        propertyType switch
        {
            "Int" => "int",
            "Float" => "float",
            "String" => "string",
            "Boolean" => "bool",
            "ID" => "string",
            "Decimal" => "decimal",
            "DateTime" => "DateTimeOffset",
            "Cursor" => "string",
            _ => propertyType
        };
}
