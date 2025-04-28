using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enterprise.Shared.Database;

public class NullableDateTimeOffsetToUtcConverter()
    : ValueConverter<DateTimeOffset?, DateTimeOffset?>(ToProvider, FromProvider)
{
    private static readonly Expression<Func<DateTimeOffset?, DateTimeOffset?>> ToProvider = offset => ConvertIfNeeded(offset);

    private static readonly Expression<Func<DateTimeOffset?, DateTimeOffset?>> FromProvider = offset => offset;

    private static DateTimeOffset? ConvertIfNeeded(DateTimeOffset? dateTimeOffset) =>
        dateTimeOffset switch
        {
            { } dto when dto.Offset == TimeSpan.Zero => dto,
            { } dto => dto.ToUniversalTime(),
            _ => null
        };
}
