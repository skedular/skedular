namespace Unityhubctl.GraphQL.Generator.Base;

public enum ArgumentSeparatorType
{
    Paraneteses,
    EqualSign
}

public class DirectiveArgument(string key, IReadOnlyCollection<string> values)
{
    private static bool IsArrayTypeKey(string keyName) =>
        keyName switch
        {
            "pattern" => false,
            "maxLength" => false,
            "policy" => false,
            "roles" => true,
            _ => false
        };

    private static string ConvertDirectiveKeyNameToClrTypeName(string keyName) =>
        keyName switch
        {
            "pattern" => "RegularExpression",
            "maxLength" => "StringLength",
            "policy" => "Policy",
            "roles" => "Roles",
            _ => keyName
        };

    public string ToString(ArgumentSeparatorType argumentSeparator)
    {
        var isArrayTypeKey = IsArrayTypeKey(key);
        var convertedKey = ConvertDirectiveKeyNameToClrTypeName(key);
        var convertedValue = string.Empty;

        if (values.Count != 0 && isArrayTypeKey)
        {
            convertedValue = $"new[] {"{" + string.Join(",", values.Select(value => $"\"{value}\"")) + "}"}";
        }
        else if (values.Count == 1)
        {
            var flattedValues = values.First();
            convertedValue = int.TryParse(flattedValues, out _) ? flattedValues : $"\"{flattedValues}\"";
        }
        else if (values.Count > 1)
        {
            convertedValue = $"new[] {"{" + string.Join(",", values.Select(value => $"\"{value}\"")) + "}"}";
        }

        return ToKeyValueText(convertedKey, convertedValue, argumentSeparator);
    }

    private string ToKeyValueText(string convertedKey, string convertedValue,
        ArgumentSeparatorType argumentSeparator) =>
        argumentSeparator == ArgumentSeparatorType.Paraneteses
            ? $"{convertedKey}({convertedValue})"
            : $"{convertedKey} = {convertedValue}";
}
