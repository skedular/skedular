using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enterprise.Shared.Outbox;

public static class OutboxJsonValueConverter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static ValueConverter<T, string> CreateConverter<T>() =>
        new(
            value => JsonSerializer.Serialize(value, SerializerOptions),
            value => JsonSerializer.Deserialize<T>(value, SerializerOptions)!);

    public static ValueComparer<T> CreateComparer<T>() =>
        new(
            (left, right) => JsonSerializer.Serialize(left, SerializerOptions) == JsonSerializer.Serialize(right, SerializerOptions),
            value => JsonSerializer.Serialize(value, SerializerOptions).GetHashCode(),
            value => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, SerializerOptions), SerializerOptions)!);
}
