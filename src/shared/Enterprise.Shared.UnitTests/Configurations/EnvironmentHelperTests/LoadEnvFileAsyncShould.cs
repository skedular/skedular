using Enterprise.Shared.Configurations;

namespace Enterprise.Shared.UnitTests.Configurations.EnvironmentHelperTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class LoadEnvFileAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_variables_from_env_file(string key, string value, CancellationToken cancellationToken)
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, $"{key}={value}", cancellationToken);

        await EnvironmentHelper.LoadEnvFileAsync(path, cancellationToken);

        Environment.GetEnvironmentVariable(key).ShouldBe(value);

        File.Delete(path);
        Environment.SetEnvironmentVariable(key, null);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Load_quoted_variables_stripping_quotes(string key, string value, CancellationToken cancellationToken)
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, $"{key}=\"{value}\"", cancellationToken);

        await EnvironmentHelper.LoadEnvFileAsync(path, cancellationToken);

        Environment.GetEnvironmentVariable(key).ShouldBe(value);

        File.Delete(path);
        Environment.SetEnvironmentVariable(key, null);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Skip_lines_without_equals(string uniqueId, CancellationToken cancellationToken)
    {
        var path = Path.GetTempFileName();
        await File.WriteAllTextAsync(path, $"# comment-{uniqueId}", cancellationToken);

        // Should not throw
        await EnvironmentHelper.LoadEnvFileAsync(path, cancellationToken);

        File.Delete(path);
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_immediately_when_file_does_not_exist(string nonExistentPath, CancellationToken cancellationToken) =>
        // Should not throw
        await EnvironmentHelper.LoadEnvFileAsync(nonExistentPath, cancellationToken);
}
