using System.Diagnostics;
using Enterprise.Shared.Telemetry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.UnitTests.Telemetry.ProcessTelemetryExportRegistryTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class TryClaimShould
{
    [Theory]
    [AutoData]
    public void Allow_Only_One_Exporter_Per_Process_Signal_And_Destination(string signalName, string destination)
    {
        using var firstClaim = ProcessTelemetryExportRegistry.TryClaim(signalName, destination);

        firstClaim.ShouldNotBeNull();
        ProcessTelemetryExportRegistry.TryClaim(signalName, destination).ShouldBeNull();
    }

    [Fact]
    public void Export_Once_Across_Two_Configured_OpenTelemetry_Providers_And_Release_The_Claim()
    {
        var sourceName = Guid.CreateVersion7().ToString();
        var operationName = Guid.CreateVersion7().ToString();
        var configuration = CreateConsoleConfiguration();
        var firstServices = new ServiceCollection();
        var secondServices = new ServiceCollection();
        firstServices.ConfigureOpenTelemetry(configuration, "first", false);
        firstServices.ConfigureOpenTelemetryTracerProvider(builder => builder.AddSource(sourceName));
        secondServices.ConfigureOpenTelemetry(configuration, "second", false);
        secondServices.ConfigureOpenTelemetryTracerProvider(builder => builder.AddSource(sourceName));
        using var output = new StringWriter();
        var originalOutput = Console.Out;

        try
        {
            Console.SetOut(output);
            var firstProvider = firstServices.BuildServiceProvider();
            var secondProvider = secondServices.BuildServiceProvider();
            using var activitySource = new ActivitySource(sourceName);
            var firstTracerProvider = firstProvider.GetRequiredService<TracerProvider>();
            var secondTracerProvider = secondProvider.GetRequiredService<TracerProvider>();

            using (activitySource.StartActivity(operationName))
            {
            }

            firstTracerProvider.ForceFlush().ShouldBeTrue();
            secondTracerProvider.ForceFlush().ShouldBeTrue();
            var exportedOutput = output.ToString();
            CountOccurrences(exportedOutput, operationName).ShouldBe(1);
            exportedOutput.ShouldContain("service.name: first");

            firstProvider.Dispose();
            secondProvider.Dispose();
            using var replacementClaim = ProcessTelemetryExportRegistry.TryClaim("TRACES:CONSOLE", "console");
            replacementClaim.ShouldNotBeNull();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }

    [Theory]
    [AutoData]
    public void Claim_Different_Signals_And_Destinations_Independently(
        string firstSignalName,
        string firstDestination,
        string secondSignalName,
        string secondDestination)
    {
        using var firstClaim = ProcessTelemetryExportRegistry.TryClaim(firstSignalName, firstDestination);
        using var secondClaim = ProcessTelemetryExportRegistry.TryClaim(secondSignalName, secondDestination);

        firstClaim.ShouldNotBeNull();
        secondClaim.ShouldNotBeNull();
    }

    [Fact]
    public void Export_One_Activity_Once_Across_Two_Providers()
    {
        using var activitySource = new ActivitySource(Guid.CreateVersion7().ToString());
        using var exporter = new RecordingExporter();
        using var firstClaim = ProcessTelemetryExportRegistry.TryClaim("TRACES", exporter.Destination);
        using var firstProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(activitySource.Name)
            .AddProcessor(new SimpleActivityExportProcessor(exporter))
            .Build();
        using var secondClaim = ProcessTelemetryExportRegistry.TryClaim("TRACES", exporter.Destination);
        using var secondProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(activitySource.Name)
            .Build();

        using (activitySource.StartActivity("operation"))
        {
        }

        secondClaim.ShouldBeNull();
        exporter.ExportedActivities.Count.ShouldBe(1);
    }

    private static IConfiguration CreateConsoleConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenTelemetry:ConsoleEnabled"] = "true",
                ["OpenTelemetry:MetricsIngestEnabled"] = "false",
                ["OpenTelemetry:EntityFrameworkEnabled"] = "false",
                ["OpenTelemetry:ExcludeOutboxTelemetry"] = "false",
                ["OpenTelemetry:MeterProviderName"] = "test",
            })
            .Build();

    private static int CountOccurrences(string value, string expected) =>
        value.Split(expected).Length - 1;

    private sealed class RecordingExporter : BaseExporter<Activity>
    {
        public string Destination { get; } = Guid.CreateVersion7().ToString();
        public List<Activity> ExportedActivities { get; } = [];

        public override ExportResult Export(in Batch<Activity> batch)
        {
            foreach (var activity in batch)
            {
                ExportedActivities.Add(activity);
            }

            return ExportResult.Success;
        }
    }
}
