using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Enterprise.Shared.Database;

public class DateTimeOffsetToUtcConverter() : ValueConverter<DateTimeOffset, DateTimeOffset>(ToProvider, FromProvider)
{
    public static readonly Expression<Func<DateTimeOffset, DateTimeOffset>> ToProvider = dateTimeOffset => ConvertIfNeeded(dateTimeOffset);

    public static readonly Expression<Func<DateTimeOffset, DateTimeOffset>> FromProvider = dateTimeOffset => dateTimeOffset;

    public static DateTimeOffset ConvertIfNeeded(DateTimeOffset dateTimeOffset) =>
        dateTimeOffset.Offset == TimeSpan.Zero ? dateTimeOffset : dateTimeOffset.ToUniversalTime();
}
