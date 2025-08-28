using System.IO.Compression;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlLoginRequestFactory
{
    string GenerateSamlLoginRequest(string id, string redirectUrl, string entityId, string loginUrl);
}

public class SamlLoginRequestFactory(ILogger<SamlLoginRequestFactory> logger, TimeProvider timeProvider) : ISamlLoginRequestFactory
{
    public string GenerateSamlLoginRequest(string id, string redirectUrl, string entityId, string loginUrl)
    {
        //ref : https://learn.microsoft.com/en-us/entra/identity-platform/single-sign-on-saml-protocol
        var authnRequestXml = $@"
                    <samlp:AuthnRequest
                      xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
                      ID=""{Models.Constants.SamlIdPrefix}{id}""
                      Version=""2.0"" IssueInstant=""{timeProvider.GetUtcNow():yyyy-MM-ddTHH:mm:ssZ}""
                      xmlns:samlp=""urn:oasis:names:tc:SAML:2.0:protocol"">
                      <Issuer xmlns=""urn:oasis:names:tc:SAML:2.0:assertion"">{entityId}</Issuer>
                    </samlp:AuthnRequest>";

        var compressedSamlRequest = DeflateCompress(Encoding.UTF8.GetBytes(authnRequestXml));
        var samlRequestBase64 = Convert.ToBase64String(compressedSamlRequest);
        var encodedSamlRequestUrl = HttpUtility.UrlEncode(samlRequestBase64);
        var encodedRedirectUrl = HttpUtility.UrlEncode(redirectUrl);
        var url = $"{loginUrl}?SAMLRequest={encodedSamlRequestUrl}&RelayState={encodedRedirectUrl}";

        logger.LogDebug("{url}", url);

        return url;
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
