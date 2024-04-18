using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Exceptions;

public static class DirectiveExtensions
{
    public static string ToDirectiveAttributes(this IEnumerable<Directive?>? directives) =>
        directives is null
            ? string.Empty
            : directives.Aggregate(string.Empty,
                (current, directive) => directive is null ? current : current + directive).Trim();
}
