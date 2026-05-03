using System.Text.Json;
using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Location.Api.UnitTests.Services.LocationAnalyticsServiceTests;

// The InMemory provider cannot map complex types (e.g. CdnImageFile, OpeningHours) that
// production stores as PostgreSQL `jsonb` columns through Npgsql's automatic serialisation.
// This test-only context applies a JSON string converter to every property declared as
// `jsonb` so the model validates under the InMemory provider used by these unit tests.
internal sealed class TestLocationDbContext(
    DbContextOptions<LocationDbContext> options,
    CustomDbContextOptions<LocationDbContext> customDbContextOptions)
    : LocationDbContext(options, customDbContextOptions)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties().ToList())
            {
                if (!string.Equals(property.GetColumnType(), "jsonb", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var converterType = typeof(JsonStringConverter<>).MakeGenericType(property.ClrType);
                var converter = (ValueConverter)Activator.CreateInstance(converterType)!;
                property.SetValueConverter(converter);
                property.SetColumnType(null);
            }
        }
    }

    private sealed class JsonStringConverter<T>() : ValueConverter<T, string>(
        value => JsonSerializer.Serialize(value, SerializerOptions),
        value => JsonSerializer.Deserialize<T>(value, SerializerOptions)!);
}
