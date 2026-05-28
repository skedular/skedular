using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Enterprise.Shared.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Sso;

public class OrganizationSsoCookie
{
    [JsonPropertyName("name")] public required string Name { get; set; }
    [JsonPropertyName("value")] public required string Value { get; set; }
}

public class SsoContextEnricherMiddleware(
    RequestDelegate next,
    ISamlAssertionConsumerService samlAssertionConsumerService,
    ILogger<SsoContextEnricherMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext, IContext context)
    {
        if (!httpContext.Request.Headers.ContainsKey(Constants.OrganizationSsoCookieHeader))
        {
            await next(httpContext);

            return;
        }

        var encodedSsoCookie = httpContext.Request.Headers[Constants.OrganizationSsoCookieHeader].ToString();
        if (string.IsNullOrWhiteSpace(encodedSsoCookie))
        {
            await next(httpContext);

            return;
        }

        var decodedSsoCookies = Encoding.UTF8.GetString(Convert.FromBase64String(encodedSsoCookie));
        if (string.IsNullOrWhiteSpace(decodedSsoCookies))
        {
            await next(httpContext);

            return;
        }

        var ssoCookies = JsonSerializer.Deserialize<IReadOnlyList<OrganizationSsoCookie>>(decodedSsoCookies);
        ArgumentNullException.ThrowIfNull(ssoCookies);

        foreach (var cookie in ssoCookies.Where(item => item.Name.StartsWith(Constants.OrganizationSsoCookiePrefix)))
        {
            var organizationId = cookie.Name[(Constants.OrganizationSsoCookiePrefix.Length + 1)..];
            try
            {
                var samlResponse = samlAssertionConsumerService.RetrieveSamlResponseFromCookie(cookie.Value);

                if (string.IsNullOrWhiteSpace(samlResponse.NameId))
                {
                    continue;
                }

                context.AddUserSsoContext(organizationId, new UserSsoContext(samlResponse.NameId));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to retrieve cookie value");
            }
        }

        await next(httpContext);
    }
}
