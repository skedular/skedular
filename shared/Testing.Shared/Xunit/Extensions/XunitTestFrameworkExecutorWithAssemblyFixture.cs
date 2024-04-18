using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Testing.Shared.Xunit.Extensions;

public class XunitTestFrameworkExecutorWithAssemblyFixture(
    AssemblyName assemblyName,
    ISourceInformationProvider sourceInformationProvider,
    IMessageSink diagnosticMessageSink)
    : XunitTestFrameworkExecutor(assemblyName, sourceInformationProvider, diagnosticMessageSink)
{
#pragma warning disable VSTHRD100
    protected override async void RunTestCases(
#pragma warning restore VSTHRD100
        IEnumerable<IXunitTestCase> testCases,
        IMessageSink executionMessageSink,
        ITestFrameworkExecutionOptions executionOptions)
    {
        using var assemblyRunner = new XunitTestAssemblyRunnerWithAssemblyFixture(TestAssembly,
            testCases, DiagnosticMessageSink, executionMessageSink,
            executionOptions);

        await assemblyRunner.RunAsync();
    }
}
