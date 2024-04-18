using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;
using Enum = System.Enum;
using Directive = Unityhubctl.GraphQL.Generator.Base.Directive;
using DirectiveArgument = Unityhubctl.GraphQL.Generator.Base.DirectiveArgument;

namespace Unityhubctl.GraphQL.Generator.Visitors;

public abstract class GeneratableTypeVisitor
{
    /// <summary>
    ///     This method will visit and extract specified generatable types from GraphQL types.
    /// </summary>
    /// <param name="graphTypes">GraphQL types to visit.</param>
    /// <returns>Extracted generatable types.</returns>
    public abstract IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes);

    protected static Directive? GetDirective(AppliedDirective appliedDirective)
    {
        if (!Enum.TryParse(appliedDirective.Name, true, out DirectiveType directiveType))
        {
            return null;
        }

        var directiveArguments = appliedDirective.Select(directiveArgument =>
            {
                IReadOnlyCollection<string> values;

                if (directiveArgument.Value is null)
                {
                    values = [];
                }
                else
                {
                    if (directiveArgument.Value.GetType().IsArray)
                    {
                        var objectValues = directiveArgument.Value as object?[];
                        values = objectValues is null
                            ? []
                            : objectValues.Select(val => val?.ToString() ?? string.Empty)
                                .Where(val => !string.IsNullOrWhiteSpace(val)).ToList();
                    }
                    else
                    {
                        values = [directiveArgument.Value.ToString() ?? string.Empty];
                    }
                }

                return new DirectiveArgument(directiveArgument.Name, values);
            })
            .ToList();

        return new Directive(directiveArguments, directiveType);
    }
}
