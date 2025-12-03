using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit3;

namespace Testing.Shared;

public interface IFixtureCustomizer
{
    void Customize(IFixture fixture);
}

public static class AutoFakeItEasyCustomizers
{
    internal static IList<Type> GlobalCustomizerTypes { get; } = new List<Type>();
    public static void RegisterGlobalCustomizer<T>() where T : IFixtureCustomizer => GlobalCustomizerTypes.Add(typeof(T));
}

public class AutoFakeItEasyDataAttribute(Type[]? fixtureCustomizers = null, bool skipGlobalCustomizers = false) : AutoDataAttribute(() =>
{
    var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

    fixture.Behaviors.Add(new OmitOnRecursionBehavior());

    fixture.Customizations.Add(new ExceptionContextGenerator());
    fixture.Customizations.Add(new CancellationTokenGenerator());
    fixture.Customizations.Add(new CoordinateGenerator());

    if (!skipGlobalCustomizers)
    {
        ApplyGlobalCustomizers(fixture);
    }

    if (fixtureCustomizers is null)
    {
        return fixture;
    }

    foreach (var fixtureCustomizerType in fixtureCustomizers.Where(fixtureCustomizer =>
                 fixtureCustomizer.GetInterfaces().Any(implementedInterface => implementedInterface == typeof(IFixtureCustomizer))))
    {
        ApplyCustomizationsFromType(fixture, fixtureCustomizerType);
    }

    return fixture;
})
{
    private static void ApplyGlobalCustomizers(IFixture fixture)
    {
        foreach (var fixtureCustomizerType in AutoFakeItEasyCustomizers.GlobalCustomizerTypes)
        {
            ApplyCustomizationsFromType(fixture, fixtureCustomizerType);
        }
    }

    private static void ApplyCustomizationsFromType(IFixture fixture, Type fixtureCustomizerType)
    {
        var fixtureCustomizer = Activator.CreateInstance(fixtureCustomizerType) as IFixtureCustomizer;

        fixtureCustomizer?.Customize(fixture);
    }
}
