using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Security.Sso.Models;
using Flurl.Http;
using Microsoft.AspNetCore.Http;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlAssertionConsumerService
{
    Task<X509Certificate2> GetSigningCertificateFromMetadataAsync(
        string metadataUrl,
        CancellationToken cancellationToken);

    bool ValidateSamlResponseSignature(string samlResponse, X509Certificate2 certificate);
    string VerifyAndDecodeSamlResponse(string rawSamlData);
    SamlResponse ExtractSamlResponse(string decodedSaml);
    void StoreSamlResponseInCookie(HttpResponse response, SamlResponse samlResponse, string organizationId);
}

public class SamlAssertionConsumerService : ISamlAssertionConsumerService
{
    public async Task<X509Certificate2> GetSigningCertificateFromMetadataAsync(
        string metadataUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            var metadata = await metadataUrl.GetStringAsync(cancellationToken: cancellationToken);
            var document = XDocument.Parse(metadata);
            var certNode = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "X509Certificate");

            if (certNode == null)
            {
                throw new SamlMetadataException();
            }

            var certificateRawData = Convert.FromBase64String(certNode.Value);
            var certificate = X509CertificateLoader.LoadCertificate(certificateRawData)
                              ?? throw new InvalidOperationException("Invalid certificate. No private key found.");

            return certificate;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to fetch metadata from {metadataUrl}", ex);
        }
    }

    public bool ValidateSamlResponseSignature(string samlResponse, X509Certificate2 certificate)
    {
        try
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(samlResponse);

            var xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
            xmlNamespaceManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
            xmlNamespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

            var signatureNode =
                xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/ds:Signature", xmlNamespaceManager);

            if (signatureNode?.ParentNode == null)
            {
                return false;
            }

            var signedXml = new SignedXml((XmlElement)signatureNode.ParentNode);
            signedXml.LoadXml((XmlElement)signatureNode);

            return signedXml.CheckSignature(certificate, true);
        }
        catch
        {
            //TODO : log exception
            return false;
        }
    }

    public string VerifyAndDecodeSamlResponse(string samlResponse)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(samlResponse))
            {
                throw new ArgumentException("SAML response is empty");
            }

            if (samlResponse.Contains('%'))
            {
                samlResponse = HttpUtility.UrlDecode(samlResponse);
            }

            var samlData = Convert.FromBase64String(samlResponse);
            return Encoding.UTF8.GetString(samlData);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Invalid SAML response format", ex);
        }
    }

    // Ref : https://learn.microsoft.com/en-us/entra/identity-platform/single-sign-on-saml-protocol
    public SamlResponse ExtractSamlResponse(string decodedSaml)
    {
        var samlResponse = new XmlDocument();
        samlResponse.LoadXml(decodedSaml);

        var namespaceManager = new XmlNamespaceManager(samlResponse.NameTable);
        namespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
        namespaceManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

        var response = new SamlResponse();

        var nameIdNode = samlResponse.SelectSingleNode("//saml:Subject/saml:NameID", namespaceManager);
        response.NameId = nameIdNode?.InnerText;

        var sessionIndexNode = samlResponse.SelectSingleNode("//saml:AuthnStatement/@SessionIndex", namespaceManager);
        response.SessionIndex = sessionIndexNode?.Value;

        var notOnOrAfterNode = samlResponse.SelectSingleNode(
            "//saml:Subject/saml:SubjectConfirmation/saml:SubjectConfirmationData/@NotOnOrAfter", namespaceManager);
        if (DateTime.TryParse(notOnOrAfterNode?.Value, out var notOnOrAfter))
        {
            response.SessionNotOnOrAfter = notOnOrAfter;
        }

        var attributes = samlResponse.SelectNodes("//saml:AttributeStatement/saml:Attribute", namespaceManager);
        ArgumentNullException.ThrowIfNull(attributes);

        foreach (XmlNode attribute in attributes)
        {
            var name = attribute.Attributes?["Name"]?.Value;
            var value = attribute.SelectSingleNode("saml:AttributeValue", namespaceManager)?.InnerText;
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            response.Roles.Add($"{name}:{value}");
        }

        var responseNode = samlResponse.SelectSingleNode("//samlp:Response", namespaceManager);
        ArgumentNullException.ThrowIfNull(responseNode);

        ArgumentException.ThrowIfNullOrWhiteSpace(responseNode.Attributes?["Destination"]?.Value);
        response.Destination = responseNode.Attributes?["Destination"]?.Value!;

        ArgumentException.ThrowIfNullOrWhiteSpace(responseNode.Attributes?["InResponseTo"]?.Value);
        response.InResponseTo = responseNode.Attributes?["InResponseTo"]?.Value!;

        var issuerNode = samlResponse.SelectSingleNode("//saml:Assertion/saml:Issuer", namespaceManager);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuerNode?.InnerText);
        response.Issuer = issuerNode.InnerText;

        var authnInstantNode = samlResponse.SelectSingleNode("//saml:AuthnStatement/@AuthnInstant", namespaceManager);
        if (DateTime.TryParse(authnInstantNode?.Value, out var authnInstant))
        {
            response.AuthnInstant = authnInstant;
        }

        var authnContextNode =
            samlResponse.SelectSingleNode("//saml:AuthnStatement/saml:AuthnContext/saml:AuthnContextClassRef",
                namespaceManager);
        response.AuthnContext = authnContextNode?.InnerText;

        var statusNode = samlResponse.SelectSingleNode("//samlp:Status", namespaceManager);
        ArgumentNullException.ThrowIfNull(statusNode);

        var primaryStatusCodeNode = statusNode.SelectSingleNode("samlp:StatusCode/@Value", namespaceManager);
        ArgumentException.ThrowIfNullOrWhiteSpace(primaryStatusCodeNode?.Value);
        response.StatusCode = primaryStatusCodeNode.Value;
        if (!response.StatusCode.Contains("Success"))
        {
            throw new InvalidOperationException("Operation Failed.");
        }

        var nestedStatusCodeNode =
            statusNode.SelectSingleNode("samlp:StatusCode/samlp:StatusCode/@Value", namespaceManager);
        response.NestedStatusCode = nestedStatusCodeNode?.Value;

        var statusMessageNode = statusNode.SelectSingleNode("samlp:StatusMessage", namespaceManager);
        response.StatusMessage = statusMessageNode?.InnerText;

        return response;
    }

    public void StoreSamlResponseInCookie(HttpResponse response, SamlResponse samlResponse, string organizationId)
    {
        //TODO : encrypt the response
        var serializerContent = JsonSerializer.Serialize(samlResponse);
        response.Cookies.Append(
            $"skedular-sso-{organizationId}",
            serializerContent,
            new CookieOptions { HttpOnly = true, Secure = true, Expires = samlResponse.SessionNotOnOrAfter });
    }
}
