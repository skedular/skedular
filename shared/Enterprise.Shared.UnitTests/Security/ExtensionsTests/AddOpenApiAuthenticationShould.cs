using Enterprise.Shared.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Enterprise.Shared.UnitTests.Security.ExtensionsTests;

[Trait(CategoryNames.Key, CategoryNames.Unit)]
public class AddOpenApiAuthenticationShould
{
    [Fact]
    public void Throw_when_authentication_section_is_missing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        var error = Should.Throw<InvalidOperationException>(() => services.AddOpenApiAuthentication(configuration));

        error.Message.ShouldContain("Authentication configuration");
    }

    [Fact]
    public void Throw_when_jwt_issuer_is_missing()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Authentication:Jwt:Issuer"] = string.Empty })
            .Build();

        var error = Should.Throw<InvalidOperationException>(() => services.AddOpenApiAuthentication(configuration));

        error.Message.ShouldContain("Authentication:Jwt:Issuer");
    }

    [Fact]
    public void Register_bearer_authentication_with_issuer_only_validation()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Jwt:Issuer"] = "http://identityserver", ["ASPNETCORE_ENVIRONMENT"] = "Development"
            })
            .Build();

        services.AddOpenApiAuthentication(configuration);

        using var provider = services.BuildServiceProvider();
        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
        var options = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);

        options.Authority.ShouldBe("http://identityserver");
        options.TokenValidationParameters.ValidateIssuer.ShouldBeTrue();
        options.TokenValidationParameters.ValidIssuer.ShouldBe("http://identityserver");
        options.TokenValidationParameters.ValidateAudience.ShouldBeFalse();
        options.TokenValidationParameters.ValidateLifetime.ShouldBeTrue();
        options.TokenValidationParameters.ValidateIssuerSigningKey.ShouldBeTrue();
        options.RequireHttpsMetadata.ShouldBeFalse();
        options.Events.OnChallenge.ShouldNotBeNull();
    }
}
