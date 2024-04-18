using FakeItEasy;
using FakeItEasy.Configuration;
using Microsoft.Extensions.Logging;

namespace Testing.Shared.Assertions;

public static class LogAssertions
{
    public static IAnyCallConfigurationWithNoReturnTypeSpecified ACallToLog(ILogger logger, LogLevel level) =>
        A.CallTo(logger)
            .Where(call =>
                call.Method.Name == "Log" &&
                call.GetArgument<LogLevel>(0) == level);

    public static IAnyCallConfigurationWithNoReturnTypeSpecified ACallToLogInfo(ILogger logger) =>
        ACallToLog(logger, LogLevel.Information);

    public static IAnyCallConfigurationWithNoReturnTypeSpecified ACallToLogError(ILogger logger) =>
        ACallToLog(logger, LogLevel.Error);

    // This is private to ensure it is only called on the result of ACallToLog 
    private static IAnyCallConfigurationWithNoReturnTypeSpecified Containing(
        this IAnyCallConfigurationWithNoReturnTypeSpecified logCall,
        string part) =>
        logCall.Where(call =>
            call.GetArgument<IReadOnlyList<KeyValuePair<string, object>>>(2)!.ToString()!.Contains(part));

    public static IAnyCallConfigurationWithNoReturnTypeSpecified
        ACallToLogInfoContaining(ILogger logger, string part) =>
        ACallToLogInfo(logger).Containing(part);

    public static IAnyCallConfigurationWithNoReturnTypeSpecified
        ACallToLogErrorContaining(ILogger logger, string part) =>
        ACallToLogError(logger).Containing(part);
}
