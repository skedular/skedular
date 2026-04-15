using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterprise.Shared.Azure.Configurations;
using Enterprise.Shared.Azure.Graph;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Cookie;
using Enterprise.Shared.Cookie.Configurations;
using Enterprise.Shared.Email;
using Enterprise.Shared.Encryption;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Helpers;
using Enterprise.Shared.IdentityProviders.Azure;
using Enterprise.Shared.IdentityProviders.Cognito;
using Enterprise.Shared.IdentityProviders.Configurations;
using Enterprise.Shared.IdentityProviders.Google;
using Enterprise.Shared.IdentityProviders.WorkOS;
using Enterprise.Shared.Image;
using Enterprise.Shared.IO;
using Enterprise.Shared.Logging;
using Enterprise.Shared.Random;
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
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTopologySuite.IO.Converters;
using QuestPDF;
using QuestPDF.Infrastructure;
using WorkOS;

namespace Enterprise.Shared;

public static class Extensions
{
    private static string GetAppName<TProgram>(IWebHostEnvironment environment) where TProgram : class =>
        typeof(TProgram).Assembly.GetName().Name ?? environment.ApplicationName;

    private static void RecordExceptionOnActivity(object sender, UnhandledExceptionEventArgs e)
    {
        if (Activity.Current is { } activity && e.ExceptionObject is Exception ex)
        {
            activity.AddException(ex);
        }
    }

    private static void ApplyCoreMiddleware(WebApplication app)
    {
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
            app.MapOpenApi();
            app.UseOpenApi();
            app.UseSwaggerUi();

            // redirect root to health
            app.UseRewriter(new RewriteOptions().AddRedirect("^$", HealthCheck.Constants.ReadinessPath));
        }

        app.UseRouting();

        // UseAuthentication must appear between UseRouting and UseEndpoints
        app.UseAuthentication();

        // UseAuthorization must appear between UseRouting and UseEndpoints
        app.UseAuthorization();

        // Health checks must go before any middleware
        app.UseHealthChecks(
            HealthCheck.Constants.LivenessPath,
            new HealthCheckOptions
            {
                Predicate = registration =>
                    registration.Tags.Contains(HealthCheck.Constants.LivenessTag) || registration.Name.Contains("self")
            });

        app.UseHealthChecks(
            HealthCheck.Constants.ReadinessPath,
            new HealthCheckOptions
            {
                Predicate = registration =>
                    registration.Tags.Contains(HealthCheck.Constants.ReadinessTag) || registration.Name.Contains("services")
            });

        app.UseMiddleware<ContextEnricherMiddleware>();
    }

    extension(decimal? value)
    {
        public double ToNullDouble() => value is null ? double.MinValue : Convert.ToDouble(value);
    }

    extension(double value)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        public decimal? FromNullDouble() => value == double.MinValue ? null : Convert.ToDecimal(value);
    }

    extension(decimal price)
    {
        public string ToRoundedPrice() => price.ToString("0.00", CultureInfo.InvariantCulture);
        public decimal RoundedDecimal() => Math.Round(price, 2);
        public string ToRoundedDecimal() => price.ToString("0.00", CultureInfo.InvariantCulture);
    }

    extension(string price)
    {
        public decimal FromRoundedPrice() => decimal.Parse(price);
        public decimal FromRoundedDecimal() => decimal.Parse(price);
    }

    extension(string? str)
    {
        public string ToSafeString() => string.IsNullOrWhiteSpace(str) ? string.Empty : str;

        public string Truncate(int length)
        {
            var str1 = string.IsNullOrWhiteSpace(str) ? string.Empty : str;

            return string.IsNullOrWhiteSpace(str1) || str1.Length <= length ? str1 : str1[..length];
        }
    }

    extension(WebApplicationBuilder builder)
    {
        /// <summary>
        ///     Minimum foundation required by all Enterprise.Shared-based applications.
        ///     Registers: configuration loading, <see cref="Configurations.ApplicationConfiguration" />,
        ///     OpenTelemetry, service discovery, HTTP resilience (optional), authentication/authorization,
        ///     CORS, ProblemDetails, HttpContextAccessor, essential singletons
        ///     (<see cref="IStringEncryptionAlgorithm" />, <see cref="IVersionService" />,
        ///     <see cref="IImageHelper" />, <see cref="Context.Context" />,
        ///     <see cref="IRandomHelper" />, <see cref="IO.IDirectoryService" />,
        ///     <see cref="Email.IEmailService" />, <see cref="TimeProvider" />),
        ///     and the "self" liveness health check.
        /// </summary>
        public WebApplicationBuilder AddCoreServices<TProgram>(bool enableHttpResilience = false)
            where TProgram : class
        {
            var services = builder.Services;
            var configuration = builder.Configuration;
            var appName = GetAppName<TProgram>(builder.Environment);

            Settings.License = LicenseType.Community;
            AppDomain.CurrentDomain.UnhandledException += RecordExceptionOnActivity;

            configuration.AddEnvironmentVariables("ASPNETCORE");
            configuration.BuildConfig<TProgram>();

            var applicationConfiguration = configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>();
            ArgumentNullException.ThrowIfNull(applicationConfiguration);
            services.AddSingleton(applicationConfiguration);

            services.ConfigureOpenTelemetry(configuration, appName);

            services
                .AddServiceDiscovery()
                .ConfigureHttpClientDefaults(httpClientBuilder =>
                {
                    if (enableHttpResilience)
                    {
                        httpClientBuilder.ConfigureHttpClient(httpClient => httpClient.Timeout = Timeout.InfiniteTimeSpan);
                        httpClientBuilder.AddStandardResilienceHandler(options => options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30));
                    }
                    else
                    {
                        httpClientBuilder.ConfigureHttpClient(httpClient => httpClient.Timeout = TimeSpan.FromSeconds(30));
                    }

                    // Turn on service discovery by default
                    httpClientBuilder.AddServiceDiscovery();
                });

            services.AddAuthentication();
            services.AddAuthorization();

            services
                .AddCors()
                .AddProblemDetails()
                .AddHttpContextAccessor()
                .AddSingleton<IStringEncryptionAlgorithm, StringEncryptionAlgorithm>()
                .AddSingleton<IVersionService, VersionService<TProgram>>()
                .AddSingleton<IImageHelper, ImageHelper>()
                .AddSingleton<IContext, Context.Context>()
                .AddSingleton<IRandomHelper, RandomHelper>()
                .AddSingleton<IDirectoryService, DirectoryService>()
                .AddSingleton<IPortFinder, PortFinder>()
                .AddSingleton<IEmailService, EmailService>();

            services.TryAddSingleton(TimeProvider.System);

            services
                .AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), [HealthCheck.Constants.LivenessTag]);

            return builder;
        }

        /// <summary>
        ///     Registers identity token provider services based on configuration presence.
        ///     Conditionally wires up: WorkOS (<see cref="IWorkOSTokenService" />),
        ///     Cognito (<see cref="ICognitoTokenService" />),
        ///     Google (<see cref="IGoogleTokenService" />),
        ///     Azure Entra (<see cref="Azure.Graph.IGraphServiceClientFactory" />,
        ///     <see cref="IAzureEntraTokenService" />), and cookie encryption
        ///     Each provider is registered only when its corresponding configuration section exists.
        /// </summary>
        public WebApplicationBuilder AddIdentityTokenProviders()
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

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

                if (identityProvidersConfiguration.Cognito is not null && identityProvidersConfiguration.Cognito.JwksUri is not null)
                {
                    services.AddSingleton<ICognitoTokenService, CognitoTokenService>();
                }

                if (identityProvidersConfiguration.Google is not null && !string.IsNullOrWhiteSpace(identityProvidersConfiguration.Google.Issuer))
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

            services
                .AddSingleton<IEnumerable<ITokenService>>(sp =>
                {
                    var tokenServices = new List<ITokenService>();

                    var workOsTokenService = sp.GetService<IWorkOSTokenService>();
                    if (workOsTokenService is not null)
                    {
                        tokenServices.Add(workOsTokenService);
                    }

                    var cognitoTokenService = sp.GetService<ICognitoTokenService>();
                    if (cognitoTokenService is not null)
                    {
                        tokenServices.Add(cognitoTokenService);
                    }

                    var googleTokenService = sp.GetService<IGoogleTokenService>();
                    if (googleTokenService is not null)
                    {
                        tokenServices.Add(googleTokenService);
                    }

                    var azureEntraTokenService = sp.GetService<IAzureEntraTokenService>();
                    if (azureEntraTokenService is not null)
                    {
                        tokenServices.Add(azureEntraTokenService);
                    }

                    return tokenServices;
                });

            return builder;
        }

        public WebApplicationBuilder AddCookieServices()
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            var cookieConfiguration = configuration.GetSection(CookieConfiguration.Key).Get<CookieConfiguration>();
            if (cookieConfiguration is not null)
            {
                services
                    .AddSingleton(cookieConfiguration)
                    .AddSingleton<ICookieEncryptionService, CookieEncryptionService>();
            }

            return builder;
        }

        /// <summary>
        ///     Registers StackExchange.Redis-backed <see cref="Microsoft.Extensions.Caching.Hybrid.HybridCache" />
        ///     with GeoJSON-aware JSON serializer options keyed to <c>IHybridCacheSerializer&lt;&gt;</c>.
        ///     Redis must be registered separately via <see cref="Cache.Extensions.AddRedis" />.
        /// </summary>
        public WebApplicationBuilder AddHybridCaching()
        {
            var services = builder.Services;

            services.AddHybridCache(options =>
            {
                options.MaximumKeyLength = 1024 * 1024;
                options.MaximumPayloadBytes = 16 * 1024 * 1024;
            });

            services
                .AddKeyedSingleton<JsonSerializerOptions>(
                    typeof(IHybridCacheSerializer<>),
                    new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.IgnoreCycles, Converters = { new GeoJsonConverterFactory() } });

            return builder;
        }

        /// <summary>
        ///     Registers MVC controllers with opinionated JSON options (camelCase, indented),
        ///     the <typeparamref name="TProgram" /> assembly part, and standard API validation error handling
        ///     (validation failures → 422, parse failures → 400).
        ///     Also registers EndpointsApiExplorer and SwaggerGen. In development mode, an NSwag document is added.
        /// </summary>
        public WebApplicationBuilder AddApiControllers<TProgram>()
            where TProgram : class
        {
            var services = builder.Services;

            if (builder.Environment.IsDevelopment())
            {
                services.AddSwaggerDocument();
            }

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

            return builder;
        }

        /// <summary>
        ///     Configures Serilog as the application's log provider, clears default providers,
        ///     and sets up the Serilog host integration. Reads Serilog settings from appsettings.
        ///     See <see cref="Logging.SerilogExtensions.UseSerilogCustom" /> for details on enrichers,
        ///     sinks, and health-check log filtering.
        /// </summary>
        public WebApplicationBuilder AddSerilogLogging<TProgram>()
            where TProgram : class
        {
            var services = builder.Services;
            var appName = GetAppName<TProgram>(builder.Environment);

            services
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddEventSourceLogger();
                });
            builder.Host.UseSerilogCustom(appName);

            return builder;
        }

        /// <summary>
        ///     Skedular opinionated bundle: calls <see cref="AddCoreServices{TProgram}" />,
        ///     <see cref="AddIdentityTokenProviders" />, <see cref="AddHybridCaching" />,
        ///     <see cref="AddApiControllers{TProgram}" />, and <see cref="AddSerilogLogging{TProgram}" />
        ///     in the expected order.
        ///     <para>
        ///         External consumers of Enterprise.Shared should compose only the modules they need
        ///         rather than calling this method, so that unused infrastructure (e.g. HybridCache when
        ///         Redis is not available, or MVC controllers for a minimal-API service) is not registered.
        ///     </para>
        /// </summary>
        public WebApplicationBuilder AddDefaultServices<TProgram>(bool enableHttpResilience = false)
            where TProgram : class =>
            builder
                .AddCoreServices<TProgram>(enableHttpResilience)
                .AddIdentityTokenProviders()
                .AddCookieServices()
                .AddHybridCaching()
                .AddApiControllers<TProgram>()
                .AddSerilogLogging<TProgram>();
    }

    extension(WebApplication app)
    {
        /// <summary>
        ///     Configures the core ASP.NET Core middleware pipeline without mapping GraphQL endpoints.
        ///     Applies: exception handling, CORS, routing, authentication, authorization,
        ///     health check endpoints (liveness + readiness), <see cref="ContextEnricherMiddleware" />,
        ///     and <c>MapControllers()</c>. In development mode also enables Swagger/OpenAPI UI.
        ///     <para>
        ///         Use this method for apps that do not use GraphQL or that map GraphQL endpoints explicitly.
        ///         Skedular internal apps should use <see cref="UseWebApplicationDefaults{TProgram}" /> which
        ///         also maps GraphQL, unless they intentionally opt out.
        ///     </para>
        /// </summary>
        public WebApplication UseApplicationCore<TProgram>() where TProgram : class
        {
            var appName = GetAppName<TProgram>(app.Environment);
            ApplyCoreMiddleware(app);
            app.MapControllers();
            var logger = app.Services.GetRequiredService<ILogger<TProgram>>();
            logger.LogInformation("EnvironmentName = {EnvironmentName}", app.Environment.EnvironmentName);
            logger.LogInformation("AppName = {appName}", appName);
            return app;
        }

        /// <summary>
        ///     Skedular opinionated bundle: applies the full middleware pipeline including GraphQL endpoint
        ///     mapping. Internally calls <see cref="UseApplicationCore{TProgram}" />'s shared setup and also
        ///     maps GraphQL endpoints via <see cref="GraphQL.GraphqlExtensions.MapGraphqlEndpoints" />.
        ///     <para>
        ///         GraphQL endpoint mapping is a no-op when the <c>GraphQL</c> configuration section is absent,
        ///         so this is safe to call from non-GraphQL hosts (processors, jobs).
        ///         External consumers who do not use GraphQL should prefer <see cref="UseApplicationCore{TProgram}" />
        ///         to avoid the unnecessary GraphQL routing pass.
        ///     </para>
        /// </summary>
        public WebApplication UseWebApplicationDefaults<TProgram>() where TProgram : class
        {
            var appName = GetAppName<TProgram>(app.Environment);
            ApplyCoreMiddleware(app);
            app.MapGraphqlEndpoints(app.Configuration);
            app.MapControllers();
            var logger = app.Services.GetRequiredService<ILogger<TProgram>>();
            logger.LogInformation("EnvironmentName = {EnvironmentName}", app.Environment.EnvironmentName);
            logger.LogInformation("AppName = {appName}", appName);
            return app;
        }
    }

    extension(Type type)
    {
        public string ToFullName() => type.FullName ?? type.Name;
    }

    extension(int? value)
    {
        public int ToNullInt() => value ?? int.MinValue;
    }

    extension(int value)
    {
        public int? FromNullInt() => value == int.MinValue ? null : value;
    }

    extension<T>(IEnumerable<T>? list)
    {
        public ICollection<T> ToSafeCollection() => list is null ? [] : list.ToList();
    }

    extension<T>(IEnumerable<T> list)
    {
        public async Task ForEachAsync(Func<T, CancellationToken, Task> action, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var value in list)
            {
                await action(value, cancellationToken);
            }
        }

        public void ForEach(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(action);

            foreach (var item in list)
            {
                action(item);
            }
        }
    }
}
