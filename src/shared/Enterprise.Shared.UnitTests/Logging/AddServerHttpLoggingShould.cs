using Enterprise.Shared.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enterprise.Shared.UnitTests.Logging;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddServerHttpLoggingShould
{
    [Fact]
    public void Apply_configured_aspnet_http_logging_options()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            [$"{ServerHttpLoggingOptions.Key}:Enabled"] = "true",
            [$"{ServerHttpLoggingOptions.Key}:LoggingFields"] = "Request,Response,Duration",
            [$"{ServerHttpLoggingOptions.Key}:RequestBodyLogLimit"] = "12345",
            [$"{ServerHttpLoggingOptions.Key}:ResponseBodyLogLimit"] = "23456",
            [$"{ServerHttpLoggingOptions.Key}:CombineLogs"] = "true",
            [$"{ServerHttpLoggingOptions.Key}:RequestHeaders:0"] = "Content-Type",
            [$"{ServerHttpLoggingOptions.Key}:ResponseHeaders:0"] = "Content-Type",
        });

        builder.AddServerHttpLogging();

        using var serviceProvider = builder.Services.BuildServiceProvider();
        var configured = serviceProvider.GetRequiredService<ServerHttpLoggingOptions>();
        var aspNetOptions = serviceProvider.GetRequiredService<IOptions<HttpLoggingOptions>>().Value;
        configured.Enabled.ShouldBeTrue();
        aspNetOptions.LoggingFields.ShouldBe(HttpLoggingFields.Request | HttpLoggingFields.Response | HttpLoggingFields.Duration);
        aspNetOptions.RequestBodyLogLimit.ShouldBe(12345);
        aspNetOptions.ResponseBodyLogLimit.ShouldBe(23456);
        aspNetOptions.CombineLogs.ShouldBeTrue();
        aspNetOptions.RequestHeaders.ShouldBe(["Content-Type"]);
        aspNetOptions.ResponseHeaders.ShouldBe(["Content-Type"]);
    }

    [Fact]
    public void Remain_disabled_when_configuration_is_absent()
    {
        var builder = WebApplication.CreateBuilder();
        builder.AddServerHttpLogging();
        using var serviceProvider = builder.Services.BuildServiceProvider();
        serviceProvider.GetRequiredService<ServerHttpLoggingOptions>().Enabled.ShouldBeFalse();
    }
}
