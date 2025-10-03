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
                 .Select(line =>
                 {
                     var idx = line.IndexOf("=", StringComparison.InvariantCultureIgnoreCase);
                     return idx == -1 ? Array.Empty<string>() : [line[..idx], line[(idx + 1)..]];
                 })
                 .Where(parts => parts.Length == 2))
        {
            if (parts.Last().StartsWith('"') && parts.Last().EndsWith('"'))
            {
                Environment.SetEnvironmentVariable(parts.First(), parts.Last().Trim('"'));
            }
            else
            {
                Environment.SetEnvironmentVariable(parts.First(), parts.Last());
            }
        }
    }
}
