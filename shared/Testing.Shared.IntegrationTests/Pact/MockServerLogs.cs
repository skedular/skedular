namespace Testing.Shared.IntegrationTests.Pact;

/// <summary>
///     Parsed MockServerLogs
/// </summary>
/// <param name="FullLogs"></param>
/// <param name="Lines"></param>
public record MockServerLogs(string FullLogs, string[] Lines)
{
    public void Save(PactSettings pactSettings)
    {
        var logFile = Path.Combine(pactSettings.TempPactDirectory, "mockserver.log");
        File.WriteAllText(logFile, FullLogs);
    }

    /// <summary>
    ///     Find the first line that matches predicate in Lines from the "fromIndex" element, and return the index of the line.
    /// </summary>
    /// <param name="predicate">condition to check if a line matches</param>
    /// <param name="fromIndex">the starting index to find</param>
    /// <returns>the index of the line, -1 if not found</returns>
    public int FindFirstLine(Func<string, bool> predicate, int fromIndex = 0)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fromIndex, Lines.Length);

        for (var i = fromIndex; i < Lines.Length; i++)
        {
            if (predicate(Lines[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Find the last line that matches predicate in Lines from the "fromIndex" element, and return the index of the line.
    /// </summary>
    /// <param name="predicate">condition to check if a line matches</param>
    /// <param name="fromIndex">the starting index to find</param>
    /// <returns>the index of the line, -1 if not found</returns>
    public int FindLastLine(Func<string, bool> predicate, int fromIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(fromIndex, Lines.Length);

        for (var i = fromIndex; i >= 0; i--)
        {
            var line = Lines[i];
            if (predicate(line))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Find the last line that matches predicate in Lines, and return the index of the line.
    /// </summary>
    /// <param name="predicate">condition to check if a line matches</param>
    /// <returns>the index of the line, -1 if not found</returns>
    public int FindLastLine(Func<string, bool> predicate) => FindLastLine(predicate, Lines.Length - 1);
}
