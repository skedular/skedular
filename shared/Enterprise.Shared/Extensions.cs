using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Azure.Graph;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.HealthCheck;
using Enterprise.Shared.Logging;
using Enterprise.Shared.Random;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Configurations;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Version;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkOS;

namespace Enterprise.Shared;

public static class Extensions
{
    public static string ToFullName(this Type type) => type.FullName ?? type.Name;
    public static ICollection<T> ToSafeCollection<T>(this IEnumerable<T>? list) => list is null ? [] : list.ToList();
    public static int ToNullInt(this int? value) => value ?? -1;
    public static int? FromNullInt(this int value) => value == -1 ? null : value;
    public static string ToRoundedPrice(this decimal price) => price.ToString("0.##", CultureInfo.InvariantCulture);
    public static decimal FromRoundedPrice(this string price) => decimal.Parse(price);
    public static string ToSafeString(this string? str) => string.IsNullOrWhiteSpace(str) ? string.Empty : str;

    public static async Task ForEachAsync<T>(this IEnumerable<T> list, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var value in list)
        {
            await action(value, cancellationToken);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(list);
        ArgumentNullException.ThrowIfNull(action);

        foreach (var item in list)
        {
            action(item);
        }
    }

    public static string Truncate(this string? content, int length)
    {
        var str = string.IsNullOrWhiteSpace(content) ? string.Empty : content;

        return string.IsNullOrWhiteSpace(str) || str.Length <= length ? str : str[..length];
    }

    public static WebApplicationBuilder AddDefaultServices<TProgram>(this WebApplicationBuilder builder) where TProgram : class
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var appName = GetAppName<TProgram>(builder.Environment);

        AppDomain.CurrentDomain.UnhandledException += RecordExceptionOnActivity;

        configuration.AddEnvironmentVariables("ASPNETCORE");
        configuration.BuildConfig<TProgram>();

        var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
        ArgumentNullException.ThrowIfNull(applicationConfiguration);
        services.AddSingleton(applicationConfiguration);

        var identityProvidersConfiguration = configuration.GetSection(IdentityProvidersConfiguration.Key).Get<IdentityProvidersConfiguration>();
        if (identityProvidersConfiguration is not null)
        {
            services.AddSingleton(identityProvidersConfiguration);

            if (identityProvidersConfiguration.WorkOS is not null)
            {
                services
                    .AddSingleton(new WorkOSClient(new WorkOSOptions { ApiKey = identityProvidersConfiguration.WorkOS.ApiKey }))
                    .AddSingleton<IWorkOSTokenService, WorkOSTokenService>();
            }

            if (identityProvidersConfiguration.Cognito is not null)
            {
                services.AddSingleton<ICognitoTokenService, CognitoTokenService>();
            }

            if (identityProvidersConfiguration.Google is not null)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(identityProvidersConfiguration.Google.Issuer);

                services.AddSingleton<IGoogleTokenService, GoogleTokenService>();
            }
        }

        var azureEntraConfiguration = configuration.GetSection(AzureEntraConfiguration.Key).Get<AzureEntraConfiguration>();
        if (azureEntraConfiguration is not null)
        {
            services
                .AddSingleton(azureEntraConfiguration)
                .AddSingleton<IGraphServiceClientFactory, GraphServiceClientFactory>()
                .AddSingleton<IAzureEntraTokenService, AzureEntraTokenService>();
        }

        services.ConfigureOpenTelemetry(configuration, appName);

        if (builder.Environment.IsDevelopment())
        {
            services.AddSwaggerDocument();
        }

        services
            .AddServiceDiscovery()
            .ConfigureHttpClientDefaults(http =>
            {
                // Turn on resilience by default
                http.AddStandardResilienceHandler();

                // Turn on service discovery by default
                http.AddServiceDiscovery();
            });

        services.AddAuthentication();
        services.AddAuthorization();

        var cookieConfiguration = configuration.GetSection(CookieConfiguration.Key).Get<CookieConfiguration>();
        if (cookieConfiguration is not null)
        {
            services
                .AddSingleton(cookieConfiguration)
                .AddSingleton<ICookieHelper, CookieHelper>();
        }

        services
            .AddCors()
            .AddProblemDetails()
            .AddHttpContextAccessor()
            .AddMemoryCache()
            .AddSingleton<IVersionService, VersionService<TProgram>>()
            .AddSingleton<IContext, Context.Context>()
            .AddSingleton(new System.Random())
            .AddSingleton<IRandomHelper, RandomHelper>()
            .TryAddSingleton(TimeProvider.System);

        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), [Constants.LivenessTag]);

        services
            .AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonOptions.JsonSerializerOptions.WriteIndented = true;
            })
            .ConfigureApplicationPartManager(partManager => partManager.ApplicationParts.Add(new AssemblyPart(typeof(TProgram).Assembly)))
            .ConfigureApiBehaviorOptions(behaviorOptions =>
            {
                // this was shamelessly lifted from here
                // https://github.com/KevinDockx/BuildingRESTfulAPIAspNetCore3/blob/master/Finished%20sample/CourseLibrary/CourseLibrary.API/Startup.cs

                behaviorOptions.InvalidModelStateResponseFactory = context =>
                {
                    // create a problem details object
                    var problemDetailsFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext, context.ModelState);

                    // add additional info not added by default
                    problemDetails.Detail = "See the errors field for details.";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    // find out which status code to use
                    var actionExecutingContext = context as ActionExecutingContext;

                    // only validation errors should be here
                    if (context.ModelState.ErrorCount > 0 && (context is ControllerContext ||
                                                              actionExecutingContext?.ActionArguments.Count ==
                                                              context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://myapi.com/path/to/modelrequirements";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occurred.";

                        return new UnprocessableEntityObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
                    }

                    // if one of the keys wasn't correctly found / couldn't be parsed
                    // we're dealing with null/unparsable input
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occurred.";
                    return new BadRequestObjectResult(problemDetails) { ContentTypes = { "application/problem+json" } };
                };
            });

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddEventSourceLogger();
            });
        builder.Host.UseSerilogCustom(appName);

        return builder;
    }

    public static WebApplication UseWebApplicationDefaults<TProgram>(this WebApplication app) where TProgram : class
    {
        var appName = GetAppName<TProgram>(app.Environment);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                });
            });
        }

        app.UseCors(corsPolicyBuilder => corsPolicyBuilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

        if (app.Environment.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi();

            // redirect root to health
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", Constants.ReadinessPath));
        }

        app.UseRouting();

        // UseAuthentication must appear between UseRouting and UseEndpoints
        app.UseAuthentication();

        // UseAuthorization must appear between UseRouting and UseEndpoints
        app.UseAuthorization();

        // Health checks must go before any middleware
        app.UseHealthChecks(
            Constants.LivenessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(Constants.LivenessTag) || registration.Name.Contains("self")
            });

        app.UseHealthChecks(
            Constants.ReadinessPath,
            new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(Constants.ReadinessTag) || registration.Name.Contains("services")
            });

        app.UseMiddleware<ContextEnricherMiddleware>();
        app.MapGraphqlEndpoints(app.Configuration);
        app.MapControllers();

        var logger = app.Services.GetRequiredService<ILogger<TProgram>>();

        logger.LogInformation("EnvironmentName = {EnvironmentName}", app.Environment.EnvironmentName);
        logger.LogInformation("AppName = {appName}", appName);

        return app;
    }

    private static string GetAppName<TProgram>(IWebHostEnvironment environment) where TProgram : class =>
        typeof(TProgram).Assembly.GetName().Name ?? environment.ApplicationName;

    private static void RecordExceptionOnActivity(object sender, UnhandledExceptionEventArgs e)
    {
        if (Activity.Current is { } activity && e.ExceptionObject is Exception ex)
        {
            activity.AddException(ex);
        }
    }
}
