using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Enterprise.Shared.Database;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Enterprise.Shared.Pagination;

public sealed record KeysetPaginationField<T>(
    string CursorKey,
    LambdaExpression Selector,
    Func<T, object?> ValueAccessor,
    Type ValueType,
    OrderDirection Direction)
{
    public static KeysetPaginationField<T> Create<TValue>(
        string cursorKey,
        Expression<Func<T, TValue>> selector,
        OrderDirection direction) =>
        new(cursorKey, selector, item => selector.Compile()(item), typeof(TValue), direction);

    public KeysetPaginationField<T> Reverse() => this with
    {
        Direction = Direction == OrderDirection.Ascending ? OrderDirection.Descending : OrderDirection.Ascending
    };
}

internal sealed record KeysetCursorPayload(string Id, IReadOnlyDictionary<string, string?> Values);

public static class PaginationExtensions
{
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, ICollection<KeysetPaginationField<T>> fields)
    {
        ArgumentNullException.ThrowIfNull(fields);
        if (fields.Count == 0)
        {
            throw new ArgumentException("At least one pagination field is required.", nameof(fields));
        }

        IOrderedQueryable<T>? orderedQuery = null;
        foreach (var field in fields)
        {
            orderedQuery = ApplyOrderingInternal(orderedQuery ?? query, field, orderedQuery is null);
        }

        return orderedQuery!;
    }

    public static async Task<(PaginatedInfo, ICollection<Edge<T>>, int)> ToPaginatedAsync<T>(
        this IQueryable<T> query,
        PaginationInputParam paginationInputParam,
        ICollection<KeysetPaginationField<T>> fields,
        CancellationToken cancellationToken) where T : EntityBase
    {
        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return (new PaginatedInfo(false, false, null, null), [], 0);
        }

        var normalizedFields = AddStableIdField(fields);
        var isBackward = paginationInputParam.Last is not null || !string.IsNullOrWhiteSpace(paginationInputParam.Before);
        var effectiveFields = isBackward ? normalizedFields.Select(field => field.Reverse()).ToList() : normalizedFields.ToList();
        var cursor = !string.IsNullOrWhiteSpace(paginationInputParam.After)
            ? DecodeKeysetCursor(paginationInputParam.After)
            : !string.IsNullOrWhiteSpace(paginationInputParam.Before)
                ? DecodeKeysetCursor(paginationInputParam.Before)
                : null;

        var pagedQuery = query;
        if (cursor is not null)
        {
            var predicate = BuildSeekPredicate(effectiveFields, cursor);
            pagedQuery = pagedQuery.Where(predicate);
        }

        var requestedCount = paginationInputParam.First ?? paginationInputParam.Last ?? totalCount;
        var items = await pagedQuery.ApplyOrdering(effectiveFields).Take(requestedCount + 1).ToListAsync(cancellationToken);

        var hasExtraItem = items.Count > requestedCount;
        if (hasExtraItem)
        {
            items = items.Take(requestedCount).ToList();
        }

        if (isBackward)
        {
            items.Reverse();
        }

        var edges = items.Select(item => new Edge<T>(item, EncodeKeysetCursor(item, normalizedFields))).ToList();

        var paginatedInfo = isBackward
            ? new PaginatedInfo(
                !string.IsNullOrWhiteSpace(paginationInputParam.Before),
                hasExtraItem,
                edges.FirstOrDefault()?.Cursor,
                edges.LastOrDefault()?.Cursor)
            : new PaginatedInfo(
                hasExtraItem,
                !string.IsNullOrWhiteSpace(paginationInputParam.After),
                edges.FirstOrDefault()?.Cursor,
                edges.LastOrDefault()?.Cursor);

        return (paginatedInfo, edges, totalCount);
    }

    private static IOrderedQueryable<T> ApplyOrderingInternal<T>(
        IQueryable<T> query,
        KeysetPaginationField<T> field,
        bool isFirst)
    {
        var methodName = isFirst
            ? field.Direction == OrderDirection.Ascending ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending)
            : field.Direction == OrderDirection.Ascending
                ? nameof(Queryable.ThenBy)
                : nameof(Queryable.ThenByDescending);

        var method = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(item =>
                item.Name == methodName &&
                item.GetParameters().Length == 2);

        var genericMethod = method.MakeGenericMethod(typeof(T), field.ValueType);
        return (IOrderedQueryable<T>)genericMethod.Invoke(null, [query, field.Selector])!;
    }

    private static Expression<Func<T, bool>> BuildSeekPredicate<T>(
        IReadOnlyList<KeysetPaginationField<T>> fields,
        KeysetCursorPayload cursor)
    {
        var parameter = Expression.Parameter(typeof(T), "item");
        Expression? predicate = null;
        Expression prefixEquals = Expression.Constant(true);

        foreach (var field in fields)
        {
            var member = ReplaceParameter(field.Selector.Body, field.Selector.Parameters[0], parameter);
            var cursorValue = DeserializeCursorValue(cursor, field);
            var greaterThan = BuildComparisonExpression(member, cursorValue, field.Direction);
            predicate = predicate is null
                ? Expression.AndAlso(prefixEquals, greaterThan)
                : Expression.OrElse(predicate, Expression.AndAlso(prefixEquals, greaterThan));
            prefixEquals = Expression.AndAlso(prefixEquals, BuildEqualityExpression(member, cursorValue));
        }

        return Expression.Lambda<Func<T, bool>>(predicate ?? Expression.Constant(false), parameter);
    }

    private static Expression BuildComparisonExpression(
        Expression member,
        object? cursorValue,
        OrderDirection direction)
    {
        var type = member.Type;
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        var isNullable = Nullable.GetUnderlyingType(type) is not null || !type.IsValueType;

        if (underlyingType == typeof(string))
        {
            return BuildStringComparisonExpression(member, cursorValue as string, direction, isNullable);
        }

        if (underlyingType.IsEnum)
        {
            var convertedMember = Expression.Convert(member, typeof(int));
            var convertedValue = cursorValue is null ? (int?)null : Convert.ToInt32(cursorValue);
            return BuildComparableComparisonExpression(convertedMember, convertedValue, direction, isNullable);
        }

        return BuildComparableComparisonExpression(member, cursorValue, direction, isNullable);
    }

    private static Expression BuildStringComparisonExpression(
        Expression member,
        string? cursorValue,
        OrderDirection direction,
        bool isNullable)
    {
        var nullConstant = Expression.Constant(null, typeof(string));
        var compareMethod = typeof(string).GetMethod(nameof(string.Compare), [typeof(string), typeof(string)])!;

        if (cursorValue is null)
        {
            if (!isNullable)
            {
                return Expression.Constant(false);
            }

            return direction == OrderDirection.Ascending ? Expression.Constant(false) : Expression.NotEqual(member, nullConstant);
        }

        var constant = Expression.Constant(cursorValue, typeof(string));
        var compareExpression = Expression.Call(compareMethod, member, constant);
        var zero = Expression.Constant(0);

        var orderedComparison = direction == OrderDirection.Ascending
            ? Expression.GreaterThan(compareExpression, zero)
            : Expression.LessThan(compareExpression, zero);

        if (!isNullable)
        {
            return orderedComparison;
        }

        return direction == OrderDirection.Ascending
            ? Expression.OrElse(
                Expression.AndAlso(Expression.NotEqual(member, nullConstant), orderedComparison),
                Expression.Equal(member, nullConstant))
            : Expression.AndAlso(Expression.NotEqual(member, nullConstant), orderedComparison);
    }

    private static Expression BuildComparableComparisonExpression(
        Expression member,
        object? cursorValue,
        OrderDirection direction,
        bool isNullable)
    {
        var type = member.Type;
        var constant = CreateTypedConstant(member, cursorValue);
        var nullConstant = Expression.Constant(null, type);

        if (cursorValue is null)
        {
            if (!isNullable)
            {
                return Expression.Constant(false);
            }

            return direction == OrderDirection.Ascending ? Expression.Constant(false) : Expression.NotEqual(member, nullConstant);
        }

        var orderedComparison = direction == OrderDirection.Ascending
            ? Expression.GreaterThan(member, constant)
            : Expression.LessThan(member, constant);

        if (!isNullable)
        {
            return orderedComparison;
        }

        return direction == OrderDirection.Ascending
            ? Expression.OrElse(
                Expression.AndAlso(Expression.NotEqual(member, nullConstant), orderedComparison),
                Expression.Equal(member, nullConstant))
            : Expression.AndAlso(Expression.NotEqual(member, nullConstant), orderedComparison);
    }

    private static BinaryExpression BuildEqualityExpression(Expression member, object? cursorValue)
    {
        var type = member.Type;
        if (cursorValue is null)
        {
            return Expression.Equal(member, Expression.Constant(null, member.Type));
        }

        return (Nullable.GetUnderlyingType(type) ?? type).IsEnum
            ? Expression.Equal(Expression.Convert(member, typeof(int)), Expression.Constant(Convert.ToInt32(cursorValue)))
            : Expression.Equal(member, CreateTypedConstant(member, cursorValue));
    }

    private static object? DeserializeCursorValue<T>(KeysetCursorPayload cursor, KeysetPaginationField<T> field)
    {
        if (field.CursorKey == nameof(EntityBase.Id))
        {
            return cursor.Id;
        }

        cursor.Values.TryGetValue(field.CursorKey, out var rawValue);
        return rawValue is null ? null : JsonSerializer.Deserialize(rawValue, field.ValueType);
    }

    private static string EncodeKeysetCursor<T>(T item, List<KeysetPaginationField<T>> fields) where T : EntityBase
    {
        var payload = new KeysetCursorPayload(
            item.Id,
            fields.ToDictionary(
                field => field.CursorKey,
                field => field.CursorKey == nameof(EntityBase.Id)
                    ? null
                    : field.ValueAccessor(item) is { } value
                        ? JsonSerializer.Serialize(value, field.ValueType)
                        : null));

        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private static KeysetCursorPayload DecodeKeysetCursor(string cursor)
    {
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(cursor));
        return JsonSerializer.Deserialize<KeysetCursorPayload>(json) ??
               throw new ArgumentException("Cursor is not a valid keyset cursor.", nameof(cursor));
    }

    private static Expression ReplaceParameter(Expression expression, ParameterExpression source, ParameterExpression target) =>
        new ReplaceParameterVisitor(source, target).Visit(expression);

    private static List<KeysetPaginationField<T>> AddStableIdField<T>(ICollection<KeysetPaginationField<T>> fields) where T : EntityBase
    {
        if (fields.Any(field => field.CursorKey == nameof(EntityBase.Id)))
        {
            return fields.ToList();
        }

        var direction = fields.LastOrDefault()?.Direction ?? OrderDirection.Ascending;
        return fields.Append(KeysetPaginationField<T>.Create(nameof(EntityBase.Id), query => query.Id, direction)).ToList();
    }

    private static Expression CreateTypedConstant(Expression member, object? value)
    {
        if (value is null)
        {
            return Expression.Constant(null, member.Type);
        }

        var memberType = member.Type;
        var underlyingType = Nullable.GetUnderlyingType(memberType);
        if (underlyingType is not null)
        {
            return Expression.Convert(Expression.Constant(value, underlyingType), memberType);
        }

        return Expression.Constant(value, memberType);
    }

    private sealed class ReplaceParameterVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : base.VisitParameter(node);
    }
}
