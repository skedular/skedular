namespace Enterprise.Shared.Configurations;

public static class EnvironmentHelper
{
    public static async Task LoadEnvFileAsync(string envFilePath, CancellationToken cancellationToken)
    {
        if (!File.Exists(envFilePath))
        {
            return;
        }

        foreach (var parts in (await File.ReadAllLinesAsync(envFilePath, cancellationToken))
                 .Select(line => line.Split("=", StringSplitOptions.RemoveEmptyEntries))
                 .Where(parts => parts.Length == 2))
        {
            Environment.SetEnvironmentVariable(parts[0], parts[1]);
        }
    }
}
