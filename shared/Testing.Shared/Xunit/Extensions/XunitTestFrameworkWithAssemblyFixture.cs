using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Testing.Shared.Xunit.Extensions;

public class XunitTestFrameworkWithAssemblyFixture(IMessageSink messageSink) : XunitTestFramework(messageSink)
{
    protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
        new XunitTestFrameworkExecutorWithAssemblyFixture(assemblyName,
            SourceInformationProvider, DiagnosticMessageSink);
}
