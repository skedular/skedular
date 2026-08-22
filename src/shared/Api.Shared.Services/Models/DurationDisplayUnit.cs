using System.Text.Json;
using System.Text.Json.Serialization;

namespace Api.Shared.Services.Models;

public enum DurationDisplayUnit
{
    Minutes,
    Hours,
}

public static class DurationDisplayUnitConstants
{
    public const string Minutes = "MINUTES";
    public const string Hours = "HOURS";
}

public static class DurationDisplayUnitExtensions
{
    public static string ToDurationDisplayUnit(this DurationDisplayUnit value) => value switch
    {
        DurationDisplayUnit.Minutes => DurationDisplayUnitConstants.Minutes,
        DurationDisplayUnit.Hours => DurationDisplayUnitConstants.Hours,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static DurationDisplayUnit ToDurationDisplayUnit(this string? value) => value switch
    {
        null or "" => DurationDisplayUnit.Hours,
        DurationDisplayUnitConstants.Minutes => DurationDisplayUnit.Minutes,
        DurationDisplayUnitConstants.Hours => DurationDisplayUnit.Hours,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported duration display unit."),
    };
}

public sealed class DurationDisplayUnitJsonConverter : JsonConverter<DurationDisplayUnit?>
{
    public override DurationDisplayUnit? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null ? null : reader.GetString().ToDurationDisplayUnit();

    public override void Write(Utf8JsonWriter writer, DurationDisplayUnit? value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value?.ToDurationDisplayUnit());
}
