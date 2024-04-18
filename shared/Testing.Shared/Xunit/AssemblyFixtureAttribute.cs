namespace Testing.Shared.Xunit;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class AssemblyFixtureAttribute(Type fixtureType) : Attribute
{
    public Type FixtureType { get; } = fixtureType;
}
