using GraphQL.Types;
using Unityhubctl.GraphQL.Generator.Base;

namespace Unityhubctl.GraphQL.Generator.Visitors;

public class AbstractClassVisitor : GeneratableTypeVisitor
{
    public override IEnumerable<IGeneratableType> Visit(IEnumerable<IGraphType> graphTypes)
    {
        var classes = new HashSet<IGeneratableType>();

        foreach (var graphType in graphTypes.Where(type => type is ObjectGraphType &&
                                                           (type.Name.Contains("query",
                                                                StringComparison.InvariantCultureIgnoreCase) ||
                                                            type.Name.Contains("mutation",
                                                                StringComparison.InvariantCultureIgnoreCase))))
        {
            var objectGraphType = (ObjectGraphType)graphType;
            var className = objectGraphType.Name;
            var @class = new AbstractClass(className, graphType.GetFieldDirectives().Select(GetDirective));

            foreach (var fieldType in objectGraphType.Fields)
            {
                var method = new Method(
                    fieldType.Name,
                    fieldType.GetTypeName(),
                    fieldType.IsArray(),
                    fieldType.IsNullable(),
                    fieldType.GetFieldDirectives().Select(GetDirective));

                var queryArguments = fieldType.Arguments ?? [];

                foreach (var queryArgument in queryArguments.Reverse())
                {
                    method.Arguments.Add(
                        new Argument(queryArgument.Name,
                            queryArgument.GetTypeName(),
                            queryArgument.IsArray(),
                            queryArgument.IsNullable()));
                }

                @class.Methods.Add(method);
            }

            ExtractImplementedInterfaces(objectGraphType, @class);

            classes.Add(@class);
        }

        return classes;
    }

    private static void ExtractImplementedInterfaces(
        IImplementInterfaces objectGraphType,
        AbstractClass @class)
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
