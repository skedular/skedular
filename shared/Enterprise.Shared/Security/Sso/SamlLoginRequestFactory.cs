using System.IO.Compression;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using Enterprise.Shared.Security.Sso.Models;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlLoginRequestFactory
{
    string GenerateSamlLoginRequest(string organizationId, string entityId, string loginUrl);
}

public class SamlLoginRequestFactory(ILogger<SamlLoginRequestFactory> logger) : ISamlLoginRequestFactory
{
    public string GenerateSamlLoginRequest(string organizationId, string entityId, string loginUrl)
    {
        var authnRequestXml = GenerateSamlRequest(organizationId, entityId);
        var compressedSamlRequest = DeflateCompress(Encoding.UTF8.GetBytes(authnRequestXml));
        var samlRequestBase64 = Convert.ToBase64String(compressedSamlRequest);
        var encodedSamlRequestUrl = HttpUtility.UrlEncode(samlRequestBase64);

        logger.LogDebug("{loginUrl}?SAMLRequest={encodedSamlRequestUrl}", loginUrl, encodedSamlRequestUrl);
        Console.WriteLine($"{loginUrl}?SAMLRequest={encodedSamlRequestUrl}");
        return $"{loginUrl}?SAMLRequest={encodedSamlRequestUrl}";
    }

    private static string GenerateSamlRequest(string organizationId, string appUrl)
    {
        var samlId = $"{Constants.SamlIdPrefix}{organizationId}"; 
        //ref : https://learn.microsoft.com/en-us/entra/identity-platform/single-sign-on-saml-protocol
        return $@"
                    <samlp:AuthnRequest
                      xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
                      ID=""{samlId}""
                      Version=""2.0"" IssueInstant=""{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}""
                      xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
                      <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">{appUrl}</Issuer>
                    </samlp:AuthnRequest>";
    }
    private static byte[] DeflateCompress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var deflateStream = new DeflateStream(output, CompressionMode.Compress))
        {
            deflateStream.Write(data, 0, data.Length);
        }

        return output.ToArray();
    }
}
