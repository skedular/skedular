using Microsoft.Extensions.Configuration;
using Path = System.IO.Path;

namespace Enterprise.Shared.Configurations;

public static class Extensions
{
    public static IConfigurationRoot BuildConfig<TProgram>(this IConfigurationBuilder builder, string? environmentName = null, string[]? args = null)
        where TProgram : class
    {
        ArgumentNullException.ThrowIfNull(builder);

        using var embeddedAppSettingsStream =
            typeof(TProgram).Assembly.GetManifestResourceStream($"{typeof(TProgram).Namespace}.appsettings.json");

        if (embeddedAppSettingsStream is not null)
        {
            using var streamReader = new StreamReader(embeddedAppSettingsStream);
            var content = streamReader.ReadToEnd();
            var filename = Path.GetTempFileName();

            File.WriteAllText(filename, content);
            builder.AddJsonFile(filename, true);
        }

        return builder.BuildConfig(environmentName, args);
    }

    public static IConfigurationRoot BuildConfig(this IConfigurationBuilder builder, string? environmentName = null, string[]? args = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true);

        if (!string.IsNullOrWhiteSpace(environmentName))
        {
            builder.AddJsonFile($"appsettings.{environmentName}.json", true);
        }

        builder.AddEnvironmentVariables();

        if (args is not null)
        {
            builder.AddCommandLine(args);
        }

        return builder.Build();
    }
}
