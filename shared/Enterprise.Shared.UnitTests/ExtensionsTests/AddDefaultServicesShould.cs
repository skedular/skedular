using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;

namespace Enterprise.Shared.UnitTests.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddDefaultServicesShould
{
    [Fact]
    public void Register_open_telemetry_logger_provider_after_serilog_logging()
    {
        var builder = CreateBuilder();

        builder.AddDefaultServices<AddDefaultServicesShould>();

        using var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider
            .GetServices<ILoggerProvider>()
            .Select(provider => provider.GetType())
            .ShouldContain(typeof(OpenTelemetryLoggerProvider));
    }

    private static WebApplicationBuilder CreateBuilder()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(AddDefaultServicesShould).Assembly.GetName().Name,
            EnvironmentName = Environments.Development
        });

        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Application:Environment"] = "local",
            ["Application:Domain"] = "Enterprise.Shared",
            ["Application:DomainSource"] = "urn::skedular::enterprise-shared",
            ["Application:AppSource"] = "enterprise-shared-unit-tests",
            ["OpenTelemetry:ConsoleEnabled"] = "false",
            ["OpenTelemetry:ExcludeOutboxTelemetry"] = "true",
            ["OpenTelemetry:MetricsIngestEnabled"] = "true",
            ["OpenTelemetry:EntityFrameworkEnabled"] = "false",
            ["OpenTelemetry:MeterProviderName"] = "Skedular",
            ["OTEL_EXPORTER_OTLP_ENDPOINT"] = "http://localhost:18889"
        });

        return builder;
    }
}
