namespace Enterprise.Shared.UnitTests.DomainAppHostEnvironmentVariablesTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class IsFakeDependenciesEnabledShould
{
    [Fact]
    public void Return_false_when_env_variable_not_set()
    {
        Environment.SetEnvironmentVariable(DomainAppHostEnvironmentVariables.UseFakeDependencies, null);

        DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled().ShouldBeFalse();
    }

    [Fact]
    public void Return_true_when_env_variable_is_true()
    {
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);

        DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled().ShouldBeTrue();

        Environment.SetEnvironmentVariable(DomainAppHostEnvironmentVariables.UseFakeDependencies, null);
    }

    [Fact]
    public void Return_false_when_env_variable_is_false()
    {
        DomainAppHostEnvironmentVariables.SetFakeDependencies(false);

        DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled().ShouldBeFalse();

        Environment.SetEnvironmentVariable(DomainAppHostEnvironmentVariables.UseFakeDependencies, null);
    }
}
