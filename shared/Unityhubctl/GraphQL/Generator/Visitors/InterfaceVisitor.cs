using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Visitors;

/// <summary>
///     This visitor will extract GraphQL interfaces.
/// </summary>
/// <example>
///     GraphQL schema:
///     <code>
/// interface ICharacter {
///   id: Int!
///   name: String!
/// }
/// 
/// type Human implements ICharacter {
///   id: Int!
///   name: String!
///   totalCredits: Int
/// }
/// </code>
///     Extracted interfaces and class:
///     <code>
/// public interface ICharacter
/// {
/// 	public int Id { get; set; }
/// 	public string Name { get; set; }
/// }
/// 
/// public class Human : ICharacter
/// {
/// 	public int Id { get; set; }
/// 	public string Name { get; set; }
/// 	public int? TotalCredits { get; set; }
/// }
/// </code>
/// </example>
public class InterfaceVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var interfaces = new HashSet<IGeneratableType>();

        foreach (var iGraphType in graphTypes.Where(type =>
                     type is InterfaceGraphType))
        {
            var interfaceGraphType = (InterfaceGraphType)iGraphType;
            var interfaceName = interfaceGraphType.Name;
            var @interface = new Interface(interfaceName);

            foreach (var fieldType in interfaceGraphType.Fields)
            {
                @interface.Properties
                    .Add(new Property(fieldType.Name,
                        fieldType.GetTypeName(),
                        fieldType.IsArray(),
                        fieldType.IsNullable(),
                        fieldType.GetFieldDirectives().Select(GetDirective)));
            }

            interfaces.Add(@interface);
        }

        return interfaces;
    }
}
