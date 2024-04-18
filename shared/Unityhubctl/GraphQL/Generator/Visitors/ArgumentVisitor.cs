using GraphQL;
using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Visitors;

/// <summary>
///     This visitor will extract GraphQL field arguments as a class.
/// </summary>
/// <example>
///     GraphQL schema:
///     <code>
/// type Query {
///     order(id: Int!): Order!
/// }
/// </code>
///     Extracted class:
///     <code>
/// public class Query_Order_Arguments
/// {
///     public int Id { get; set; }
/// }
/// </code>
/// </example>
public class ArgumentVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var classes = new HashSet<IGeneratableType>();

        foreach (var graphType in graphTypes.Where(type => type is ObjectGraphType))
        {
            var objectGraphType = (ObjectGraphType)graphType;
            var className = objectGraphType.Name; // ex: Query

            foreach (var fieldType in objectGraphType.Fields)
            {
                var fieldName = fieldType.Name.ToPascalCase(); // ex: Order

                if (fieldType.Arguments!.Count == 0)
                {
                    continue;
                }

                var @class =
                    new Class(
                        $"{className}_{fieldName}_Arguments",
                        fieldType.GetFieldDirectives().Select(GetDirective)); // ex: Query_Order_Arguments

                foreach (var argument in fieldType.Arguments!)
                {
                    @class.Properties
                        .Add(new Property(
                            argument.Name,
                            argument.GetTypeName(),
                            argument.IsArray(),
                            argument.IsNullable(),
                            fieldType.GetFieldDirectives().Select(GetDirective)));

                    classes.Add(@class);
                }
            }
        }

        return classes;
    }
}
