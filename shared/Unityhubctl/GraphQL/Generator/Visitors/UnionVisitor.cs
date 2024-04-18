using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Visitors;

/// <summary>
///     This visitor will extract GraphQL unions.
/// </summary>
/// <example>
///     GraphQL schema:
///     <code>
/// union Identity = EmailIdentity | PhoneNumberIdentity
/// 
/// type EmailIdentity {
///     value: String!
/// }
/// 
/// type PhoneNumberIdentity {
///     value: Float!
/// }
/// </code>
///     Extracted interfaces and class:
///     <code>
/// public class Identity
/// {
/// 	public EmailIdentity? EmailIdentity { get; set; }
/// 	public PhoneNumberIdentity? PhoneNumberIdentity { get; set; }
/// }
/// 
/// public class EmailIdentity
/// {
/// 	public string Value { get; set; }
/// }
/// 
/// public class PhoneNumberIdentity
/// {
/// 	public float Value { get; set; }
/// }
/// </code>
/// </example>
public class UnionVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var classes = new HashSet<IGeneratableType>();

        foreach (var graphType in graphTypes.Where(type => type is UnionGraphType))
        {
            var unionGraphType = (UnionGraphType)graphType;
            var @class = new Class(unionGraphType.Name, graphType.GetFieldDirectives().Select(GetDirective));

            foreach (var iObjectGraphType in unionGraphType.PossibleTypes)
            {
                var typeReference = (GraphQLTypeReference)iObjectGraphType;
                var propertyName = typeReference.TypeName;
                var propertyType = typeReference.TypeName;

                @class.Properties.Add(new Property(propertyName, propertyType, false, true));
            }

            classes.Add(@class);
        }

        return classes;
    }
}
