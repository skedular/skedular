using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Sensitive;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Compact;

namespace Enterprise.Shared.Logging;

public static class SerilogExtensions
{
    public static IHostBuilder UseSerilogCustom(this IHostBuilder hostBuilder, string appName)
    {
        hostBuilder.UseSerilog((hostingContext, _, loggerConfiguration) =>
        {
            if (hostingContext.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console();
            }

            loggerConfiguration
                .Enrich.WithProperty("ApplicationContext", appName)
                .Enrich.WithSpan()
                .Enrich.FromLogContext()
                .Enrich.With(new GitHashEnvironmentVariableEnricher())
                .Enrich.WithSensitiveDataMasking(new SensitiveDataEnricherOptions
                {
                    Mode = MaskingMode.Globally,
                    MaskingOperators =
                    [
                        new EmailAddressMaskingOperator(), new IbanMaskingOperator(),
                        new CreditCardMaskingOperator()
                    ],
                    MaskValue = "***REDACTED***"
                })
                .ReadFrom.Configuration(hostingContext.Configuration)
                .WriteTo.Console(new RenderedCompactJsonFormatter());
        }, false, true);

        return hostBuilder;
    }
}
