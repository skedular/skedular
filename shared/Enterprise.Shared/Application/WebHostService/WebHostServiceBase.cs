using System.Diagnostics;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Enterprise.Shared.Infrastructure.Filters;
using Enterprise.Shared.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace Enterprise.Shared.Application.WebHostService;

public abstract class WebHostServiceBase<TProgram>
    where TProgram : class
{
    private static readonly string s_appName = typeof(TProgram).Assembly.GetName().Name!;

    protected static IHostBuilder CreateHostBuilder<TStartup>(string[] args)
        where TStartup : class
    {
        var currentDomain = AppDomain.CurrentDomain;
        currentDomain.UnhandledException += RecordExceptionOnActivity;

        return Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(config => config.AddEnvironmentVariables("ASPNETCORE"))
            .ConfigureAppConfiguration((host, builder) =>
            {
                Console.WriteLine(
                    $"EnvironmentName={host.HostingEnvironment.EnvironmentName}");

                Console.WriteLine($"AppName={s_appName}");

                host.Configuration =
                    builder.BuildConfig<TProgram>(host.HostingEnvironment.EnvironmentName, args);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .ConfigureServices((_, services) =>
                    {
                        services.AddCors();

                        services.AddLogging(builder =>
                        {
                            builder.ClearProviders();
                            builder.AddEventSourceLogger();
                        });

                        services.AddHealthChecks()
                            .AddCheck("self", () => HealthCheckResult.Healthy());

                        services.AddScoped<IGlobalHttpExceptionHandler, GlobalHttpExceptionHandler>();

                        services.AddControllers(options =>
                            {
                                options.Filters.Add(typeof(HttpGlobalExceptionFilter));

                                if (services.Any(descriptor => descriptor.ServiceType == typeof(TraceSettings)))
                                {
                                    options.Filters.Add<TraceIdAsyncActionFilter>();
                                }
                            })
                            .AddNewtonsoftJson();

                        services
                            .AddEndpointsApiExplorer()
                            .AddSwaggerGen();
                    })
                    .UseStartup<TStartup>()
                    .ConfigureKestrel(o =>
                    {
                        o.ConfigureHttpsDefaults(options =>
                            options.ClientCertificateMode =
                                ClientCertificateMode.AllowCertificate);
                    });
            })
            .UseSerilogCustom(s_appName);
    }

    private static void RecordExceptionOnActivity(object sender, UnhandledExceptionEventArgs e)
    {
        if (Activity.Current is { } activity && e.ExceptionObject is Exception ex)
        {
            activity.RecordException(ex);
        }
    }
}
