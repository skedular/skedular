using System.Text;
using System.Text.Json;
using Enterprise.Shared.Context;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Telemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Application.WebHostService;

public static class UnityHubWebHostExtensions
{
    private const string ReadinessPath = "/health/readiness";
    private const string LivenessPath = "/health/liveness";

    public static void UseApplicationBuilderDefaults(
        this IApplicationBuilder app,
        IWebHostEnvironment env,
        IConfiguration configuration,
        Action? middleAction = null,
        Action<IEndpointRouteBuilder>? configureEndpointRouteBuilder = null)
    {
        app.UseCors(corsPolicyBuilder => corsPolicyBuilder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin()
        );

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi();

            // redirect root to health
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", ReadinessPath));
        }

        app.UseRouting();

        // UseAuthentication must appear between UseRouting and UseEndpoints
        app.UseAuthentication();

        // UseAuthorization must appear between UseRouting and UseEndpoints
        app.UseAuthorization();

        app.UseHealthChecks(
            LivenessPath,
            new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains(HealthCheckTags.Liveness) || r.Name.Contains("self"),
                ResponseWriter = WriteResponseAsync
            });

        app.UseHealthChecks(
            ReadinessPath,
            new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains(HealthCheckTags.Readiness) || r.Name.Contains("services"),
                ResponseWriter = WriteResponseAsync
            });

        // Health checks must go before any middleware
        middleAction?.Invoke();

        app
            .UseMiddleware<TelemetryMiddleware>()
            .UseMiddleware<SecurityContextEnricherMiddleware>()
            .UseMiddleware<ContextEnricherMiddleware>();

        app.UseEndpoints(endpointRouteBuilder =>
        {
            endpointRouteBuilder.MapGraphqlEndpoints(configuration);
            endpointRouteBuilder.MapControllers();
            configureEndpointRouteBuilder?.Invoke(endpointRouteBuilder);
        });
    }

    private static Task WriteResponseAsync(HttpContext context, HealthReport healthReport)
    {
        if (healthReport.Status is not HealthStatus.Healthy)
        {
            // Exporting full health report object to log when unhealthy 
            context.RequestServices.GetRequiredService<ILogger<HealthReport>>()
                .LogError("Health check status: {Status}. {@Report}",
                    healthReport.Status,
                    healthReport);
        }

        return WriteJsonResponseAsync(context, healthReport);
    }

    /// <summary>
    ///     Response writer taken from
    ///     https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-6.0#customize-output
    /// </summary>
    /// <param name="context"></param>
    /// <param name="healthReport"></param>
    /// <returns></returns>
    private static Task WriteJsonResponseAsync(HttpContext context, HealthReport healthReport)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonWriterOptions { Indented = true };

        using var memoryStream = new MemoryStream();
        using var jsonWriter = new Utf8JsonWriter(memoryStream, options);

        jsonWriter.WriteStartObject();
        jsonWriter.WriteString("status", healthReport.Status.ToString());
        jsonWriter.WriteStartObject("results");

        foreach (var healthReportEntry in healthReport.Entries)
        {
            jsonWriter.WriteStartObject(healthReportEntry.Key);
            jsonWriter.WriteString("status", healthReportEntry.Value.Status.ToString());
            jsonWriter.WriteString("description", healthReportEntry.Value.Description);
            jsonWriter.WriteStartObject("data");

            foreach (var item in healthReportEntry.Value.Data)
            {
                jsonWriter.WritePropertyName(item.Key);

                JsonSerializer.Serialize(jsonWriter, item.Value, item.Value.GetType());
            }

            jsonWriter.WriteEndObject();
            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndObject();
        jsonWriter.WriteEndObject();


        return context.Response.WriteAsync(Encoding.UTF8.GetString(memoryStream.ToArray()));
    }
}
