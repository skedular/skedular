using Unityhubctl.GraphQL.Generator.Exceptions;

namespace Unityhubctl.GraphQL.Generator.Visitors;

public static class VisitorFactory
{
    /// <summary>
    ///     This method creates a single visitor based on the name provided.
    /// </summary>
    /// <param name="visitorName">Visitor name to create.</param>
    public static GeneratableTypeVisitor Create(string visitorName)
    {
        visitorName = $"{visitorName}Visitor";

        var visitorType = typeof(GeneratableTypeVisitor).Assembly
            .GetExportedTypes()
            .Where(type => typeof(GeneratableTypeVisitor).IsAssignableFrom(type))
            .SingleOrDefault(visitorType => visitorType.Name.Equals(visitorName, StringComparison.OrdinalIgnoreCase));

        if (visitorType is null)
        {
            throw new VisitorNotFoundException($"No visitor named {visitorName} found.");
        }

        return (GeneratableTypeVisitor)Activator.CreateInstance(visitorType)!;
    }

    /// <summary>
    ///     This method creates and returns all available visitors in project.
    /// </summary>
    public static GeneratableTypeVisitor[] CreateAll()
    {
        var visitors = typeof(GeneratableTypeVisitor)
            .Assembly
            .GetExportedTypes()
            .Where(type => typeof(GeneratableTypeVisitor).IsAssignableFrom(type))
            .ToArray();

        if (visitors is null || visitors.Any() is false)
        {
            throw new VisitorNotFoundException("No visitor found!");
        }

        return visitors
            .Select(visitor => (GeneratableTypeVisitor)Activator.CreateInstance(visitor)!)
            .ToArray();
    }
}
