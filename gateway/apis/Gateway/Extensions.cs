namespace Gateway;

public static class Extensions
{
    public static string ToGraphQlSchema(this string resourceName)
    {
        using var embeddedStream =
            typeof(Program).Assembly.GetManifestResourceStream(
                $"{typeof(Program).Namespace}.schemas.{resourceName}.graphql");

        ArgumentNullException.ThrowIfNull(embeddedStream);

        using var streamReader = new StreamReader(embeddedStream);
        return streamReader.ReadToEnd();
    }
}
