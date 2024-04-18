using System.Runtime.InteropServices;

namespace Testing.Shared.IntegrationTests.Pact;

public static class PactMockServerLogsHelper
{
    [DllImport("pact_ffi", EntryPoint = "pactffi_mock_server_logs")]
    private static extern IntPtr MockServerLogs(int mockServerPort);

    /// <summary>
    ///     Read logs of Pact mockserver into a MockServerLogs object,
    ///     once it's called, the logs buffer will be cleared.
    /// </summary>
    /// <param name="mockServerPort"></param>
    /// <returns></returns>
    public static MockServerLogs ReadMockServerLogs(int mockServerPort)
    {
        var fullLogs = DumpMockServerLogs(mockServerPort);
        var lines = fullLogs.Split(Environment.NewLine);
        return new MockServerLogs(fullLogs, lines);
    }

    [DllImport("pact_ffi", EntryPoint = "pactffi_mock_server_mismatches")]
    private static extern IntPtr MockServerMismatches(int mockServerPort);

    /// <summary>
    ///     Read the matching rules of Pact
    /// </summary>
    /// <param name="mockServerPort"></param>
    /// <returns></returns>
    public static string? ReadMockServerMismatches(int mockServerPort)
    {
        var intPtr = MockServerMismatches(mockServerPort);

        return intPtr != IntPtr.Zero ? Marshal.PtrToStringAnsi(intPtr) : string.Empty;
    }

    /// <summary>
    ///     Dump logs of Pact mockserver into a string,
    ///     once it's called, the logs buffer will be cleared.
    /// </summary>
    /// <param name="mockServerPort"></param>
    /// <returns></returns>
    private static string DumpMockServerLogs(int mockServerPort)
    {
        //Pact writes all logs into output in one go at the end of pact's lifetime, so we can not wait to read the output,
        //therefore, we have to read the log buffer using unmanaged API directly.
        //It's ugly, but it works.
        var logsPtr = MockServerLogs(mockServerPort);
        var fullLogs = logsPtr == IntPtr.Zero
            ? "ERROR: Unable to retrieve mock server logs. It may not be ready yet."
            : Marshal.PtrToStringAnsi(logsPtr);

        ArgumentNullException.ThrowIfNull(fullLogs);

        return fullLogs;
    }
}
