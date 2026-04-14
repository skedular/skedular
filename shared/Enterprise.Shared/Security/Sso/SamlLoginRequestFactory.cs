using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;
using Flurl;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlLoginRequestFactory
{
    string GenerateSamlLoginRequest(string id, string redirectUrl, string entityId, string loginUrl);
}

public class SamlLoginRequestFactory(TimeProvider timeProvider, ILogger<SamlLoginRequestFactory> logger) : ISamlLoginRequestFactory
{
    public string GenerateSamlLoginRequest(string id, string redirectUrl, string entityId, string loginUrl)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(redirectUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId);
        ArgumentException.ThrowIfNullOrWhiteSpace(loginUrl);

        logger.LogDebug(
            "Generating SAML login request. RedirectUrlConfigured={RedirectUrlConfigured}, EntityIdLength={EntityIdLength}, LoginUrlConfigured={LoginUrlConfigured}",
            !string.IsNullOrWhiteSpace(redirectUrl),
            entityId.Length,
            !string.IsNullOrWhiteSpace(loginUrl));

        var issueInstant = timeProvider.GetUtcNow().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        var authnRequestXml = BuildAuthnRequestXml(id, entityId, issueInstant);

        var compressedSamlRequest = DeflateCompress(Encoding.UTF8.GetBytes(authnRequestXml));
        var samlRequestBase64 = Convert.ToBase64String(compressedSamlRequest);

        var result = loginUrl
            .SetQueryParam("SAMLRequest", samlRequestBase64)
            .SetQueryParam("RelayState", redirectUrl);

        logger.LogInformation("Generated SAML login request successfully");
        return result;
    }

    private static string BuildAuthnRequestXml(string id, string entityId, string issueInstant)
    {
        using var stringWriter = new StringWriter(CultureInfo.InvariantCulture);
        using (var xmlWriter = XmlWriter.Create(
                   stringWriter,
                   new XmlWriterSettings { Encoding = Encoding.UTF8, OmitXmlDeclaration = true, NewLineHandling = NewLineHandling.None }))
        {
            xmlWriter.WriteStartElement("samlp", "AuthnRequest", "urn:oasis:names:tc:SAML:2.0:protocol");
            xmlWriter.WriteAttributeString("xmlns", string.Empty, null, "urn:oasis:names:tc:SAML:2.0:metadata");
            xmlWriter.WriteAttributeString("ID", $"{Models.Constants.SamlIdPrefix}{id}");
            xmlWriter.WriteAttributeString("Version", "2.0");
            xmlWriter.WriteAttributeString("IssueInstant", issueInstant);

            xmlWriter.WriteStartElement("Issuer", "urn:oasis:names:tc:SAML:2.0:assertion");
            xmlWriter.WriteString(entityId);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
        }

        return stringWriter.ToString();
    }

    private static byte[] DeflateCompress(byte[] data)
    {
        using var output = new MemoryStream();
        using (var deflateStream = new DeflateStream(output, CompressionMode.Compress, true))
        {
            deflateStream.Write(data, 0, data.Length);
            deflateStream.Flush();
        }

        return output.ToArray();
    }
}
