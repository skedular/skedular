namespace Unityhubctl.GraphQL.Generator.Base;

public interface IGeneratableType
{
    string GraphName { get; }
    HashSet<IMember> Properties { get; }
}
