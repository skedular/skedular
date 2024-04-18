using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Visitors;

/// <summary>
///     This visitor will extract GraphQL fields as classes.
/// </summary>
/// <example>
///     GraphQL schema:
///     <code>
///  type Simple {
///      int32: Int!
///      float: Float
///      string: String!
///      bool: Boolean
///      id: ID!
///  }
/// </code>
///     Extracted class:
///     <code>
/// public class Simple
/// {
///     public int Int32 { get; set; }
///     public float? Float { get; set; }
///     public string String { get; set; }
///     public bool? Bool { get; set; }
///     public Guid Id { get; set; }
/// }
/// </code>
/// </example>
public class ClassVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var classes = new HashSet<IGeneratableType>();
        var enumeratedGraphTypes = graphTypes.ToList();

        foreach (var graphType in enumeratedGraphTypes.Where(type =>
                     type is ObjectGraphType &&
                     !(type.Name.Contains("query", StringComparison.InvariantCultureIgnoreCase) ||
                       type.Name.Contains("mutation",
                           StringComparison.InvariantCultureIgnoreCase))))
        {
            var objectGraphType = (ObjectGraphType)graphType;
            var className = objectGraphType.Name;
            var parsedClass = new Class(className, graphType.GetFieldDirectives().Select(GetDirective));

            foreach (var fieldType in objectGraphType.Fields)
            {
                parsedClass.Properties
                    .Add(new Property(fieldType.Name,
                        fieldType.GetTypeName(),
                        fieldType.IsArray(),
                        fieldType.IsNullable()));
            }

            ExtractImplementedInterfaces(objectGraphType, parsedClass);

            classes.Add(parsedClass);
        }

        foreach (var graphType in enumeratedGraphTypes.Where(type =>
                     type is InputObjectGraphType &&
                     !type.Name.Equals("query", StringComparison.InvariantCultureIgnoreCase) &&
                     !type.Name.Equals("mutation",
                         StringComparison.InvariantCultureIgnoreCase)))
        {
            var objectGraphType = (InputObjectGraphType)graphType;
            var className = objectGraphType.Name;
            var parsedClass = new Class(className, graphType.GetFieldDirectives().Select(GetDirective));

            foreach (var fieldType in objectGraphType.Fields)
            {
                parsedClass.Properties
                    .Add(new Property(
                        fieldType.Name,
                        fieldType.GetTypeName(),
                        fieldType.InputFieldIsArray(),
                        fieldType.InputFieldIsNullable(),
                        fieldType.GetFieldDirectives().Select(GetDirective)));
            }

            classes.Add(parsedClass);
        }

        return classes;
    }

    private static void ExtractImplementedInterfaces(
        IImplementInterfaces objectGraphType,
        Class @class)
    {
        if (objectGraphType.ResolvedInterfaces.Count == 0)
        {
            return;
        }

        foreach (var interfaceGraphType in objectGraphType.ResolvedInterfaces)
        {
            @class.Interfaces.Add(((GraphQLTypeReference)interfaceGraphType).TypeName);
        }
    }
}
