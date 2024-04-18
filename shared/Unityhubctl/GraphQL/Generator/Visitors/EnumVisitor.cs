using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;
using Enum = Unityhubctl.GraphQL.Generator.Base.Enum;

namespace Unityhubctl.GraphQL.Generator.Visitors;

/// <summary>
///     This visitor will extract GraphQL enums.
/// </summary>
/// <example>
///     GraphQL schema:
///     <code>
/// enum Color {
///   RED
///   GREEN
///   BLUE
/// }
/// </code>
///     Extracted enum:
///     <code>
/// public enum Color
/// {
/// 	RED,
///     GREEN,
///     BLUE
/// }
/// </code>
/// </example>
public class EnumVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var enums = new HashSet<IGeneratableType>();

        foreach (var iGraphType in graphTypes.Where(type => type is EnumerationGraphType))
        {
            var enumGraphType = (EnumerationGraphType)iGraphType;
            var @enum = new Enum(enumGraphType.Name);

            foreach (var enumValueDefinition in enumGraphType.Values)
            {
                var value = enumValueDefinition.Name;

                @enum.Properties.Add(new EnumValue(value));
            }

            enums.Add(@enum);
        }

        return enums;
    }
}
