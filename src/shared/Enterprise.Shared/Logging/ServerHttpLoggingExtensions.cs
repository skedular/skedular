using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Enterprise.Shared.Logging;

public sealed class ServerHttpLoggingOptions
{
    public const string Key = "HttpLogging";

    private static readonly string[] s_defaultRequestHeaders =
    [
        "Accept",
        "Content-Length",
        "Content-Type",
        "Traceparent",
        "X-Correlation-ID",
        "X-Request-ID",
    ];

    private static readonly string[] s_defaultResponseHeaders =
    [
        "Content-Length",
        "Content-Type",
        "Traceparent",
    ];

    private static readonly string[] s_defaultMediaTypes =
    [
        "application/graphql-response+json",
        "application/json",
        "application/problem+json",
        "application/x-www-form-urlencoded",
        "application/xml",
        "text/plain",
        "text/xml",
    ];

    public bool Enabled { get; set; }
    public HttpLoggingFields LoggingFields { get; set; } = HttpLoggingFields.All;
    public int RequestBodyLogLimit { get; set; } = 64 * 1024;
    public int ResponseBodyLogLimit { get; set; } = 64 * 1024;
    public bool CombineLogs { get; set; }
    public string[]? RequestHeaders { get; set; }
    public string[]? ResponseHeaders { get; set; }
    public string[]? MediaTypes { get; set; }

    public void Apply(HttpLoggingOptions options)
    {
        options.LoggingFields = LoggingFields;
        options.RequestBodyLogLimit = RequestBodyLogLimit;
        options.ResponseBodyLogLimit = ResponseBodyLogLimit;
        options.CombineLogs = CombineLogs;
        options.RequestHeaders.Clear();
        foreach (var header in (RequestHeaders ?? s_defaultRequestHeaders).Where(header => !string.IsNullOrWhiteSpace(header)))
        {
            options.RequestHeaders.Add(header);
        }

        options.ResponseHeaders.Clear();
        foreach (var header in (ResponseHeaders ?? s_defaultResponseHeaders).Where(header => !string.IsNullOrWhiteSpace(header)))
        {
            options.ResponseHeaders.Add(header);
        }

        options.MediaTypeOptions.Clear();
        foreach (var mediaType in (MediaTypes ?? s_defaultMediaTypes).Where(mediaType => !string.IsNullOrWhiteSpace(mediaType)))
        {
            options.MediaTypeOptions.AddText(mediaType, Encoding.UTF8);
        }
    }
}

public static class ServerHttpLoggingExtensions
{
    public static WebApplicationBuilder AddServerHttpLogging(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(ServerHttpLoggingOptions.Key).Get<ServerHttpLoggingOptions>() ??
                      new ServerHttpLoggingOptions();
        if (options.RequestBodyLogLimit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.RequestBodyLogLimit));
        }

        if (options.ResponseBodyLogLimit < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options.ResponseBodyLogLimit));
        }

        builder.Services.AddSingleton(options);
        if (options.Enabled)
        {
            builder.Services.AddHttpLogging(options.Apply);
        }

        return builder;
    }

    public static WebApplication UseServerHttpLogging(this WebApplication app)
    {
        if (app.Services.GetService<ServerHttpLoggingOptions>()?.Enabled == true)
        {
            app.UseHttpLogging();
        }

        return app;
    }
}
