using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using Enterprise.Shared.Cookie;
using Enterprise.Shared.Security.Sso.Models;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Enterprise.Shared.Security.Sso;

public interface ISamlAssertionConsumerService
{
    Task<bool> ValidateSamlResponseSignatureAsync(string samlResponse, string appFederationMetadataUrl, CancellationToken cancellationToken);
    string VerifyAndDecodeSamlResponse(string rawSamlData);
    SamlResponse ExtractSamlResponse(string saml);
    void StoreSamlResponseInCookie(HttpResponse response, string organizationId, SamlResponse samlResponse);
    SamlResponse RetrieveSamlResponseFromCookie(string rawResponse);
    Task<bool> ValidateMetadataAsync(string metadataUrl, CancellationToken cancellationToken);
    Task<bool> ValidateCertificateAsync(string metadataUrl, CancellationToken cancellationToken);
}

public class SamlAssertionConsumerService(
    IMemoryCache memoryCache,
    TimeProvider timeProvider,
    ICookieEncryptionService cookieEncryptionService,
    ILogger<SamlAssertionConsumerService> logger)
    : ISamlAssertionConsumerService
{
    public async Task<bool> ValidateSamlResponseSignatureAsync(
        string samlResponse,
        string appFederationMetadataUrl,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Validating SAML response signature");

        var certificate = await GetSigningCertificateFromMetadataAsync(appFederationMetadataUrl, cancellationToken);
        var xmlDoc = new XmlDocument
        {
            PreserveWhitespace = true,
        };
        xmlDoc.LoadXml(samlResponse);

        var xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
        xmlNamespaceManager.AddNamespace("ds", "http://www.w3.org/2000/09/xmldsig#");
        xmlNamespaceManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");
        xmlNamespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");

        var signatureNode = xmlDoc.SelectSingleNode("/samlp:Response/saml:Assertion/ds:Signature", xmlNamespaceManager);
        if (signatureNode?.ParentNode == null)
        {
            logger.LogWarning("SAML response signature node is missing");
            return false;
        }

        var signedXml = new SignedXml((XmlElement)signatureNode.ParentNode);
        signedXml.LoadXml((XmlElement)signatureNode);

        var isValid = signedXml.CheckSignature(certificate, true);
        logger.LogInformation("SAML response signature validation completed. IsValid={IsValid}", isValid);
        return isValid;
    }

    public string VerifyAndDecodeSamlResponse(string samlResponse)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(samlResponse);

        logger.LogDebug("Decoding SAML response. EncodedLength={EncodedLength}", samlResponse.Length);

        if (samlResponse.Contains('%'))
        {
            samlResponse = HttpUtility.UrlDecode(samlResponse);
        }

        var samlData = Convert.FromBase64String(samlResponse);
        logger.LogDebug("Decoded SAML response successfully. DecodedLength={DecodedLength}", samlData.Length);
        return Encoding.UTF8.GetString(samlData);
    }

    // Ref : https://learn.microsoft.com/en-us/entra/identity-platform/single-sign-on-saml-protocol
    public SamlResponse ExtractSamlResponse(string saml)
    {
        logger.LogDebug("Extracting SAML response. PayloadLength={PayloadLength}", saml.Length);

        var samlResponse = new XmlDocument();
        samlResponse.LoadXml(saml);

        var namespaceManager = new XmlNamespaceManager(samlResponse.NameTable);
        namespaceManager.AddNamespace("saml", "urn:oasis:names:tc:SAML:2.0:assertion");
        namespaceManager.AddNamespace("samlp", "urn:oasis:names:tc:SAML:2.0:protocol");

        var response = new SamlResponse();

        var nameIdNode = samlResponse.SelectSingleNode("//saml:Subject/saml:NameID", namespaceManager);
        response.NameId = nameIdNode?.InnerText;

        var sessionIndexNode = samlResponse.SelectSingleNode("//saml:AuthnStatement/@SessionIndex", namespaceManager);
        response.SessionIndex = sessionIndexNode?.Value;

        var notOnOrAfterNode = samlResponse.SelectSingleNode(
            "//saml:Subject/saml:SubjectConfirmation/saml:SubjectConfirmationData/@NotOnOrAfter",
            namespaceManager);
        response.SessionNotOnOrAfter =
            DateTimeOffset.TryParse(notOnOrAfterNode?.Value, out var notOnOrAfter) ? notOnOrAfter : DateTimeOffset.MaxValue;

        var attributes = samlResponse.SelectNodes("//saml:AttributeStatement/saml:Attribute", namespaceManager);
        ArgumentNullException.ThrowIfNull(attributes);

        foreach (XmlNode attribute in attributes)
        {
            var name = attribute.Attributes?["Name"]?.Value;
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var value = attribute.SelectSingleNode("saml:AttributeValue", namespaceManager)?.InnerText;
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            response.Roles[name] = value;
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
        if (DateTimeOffset.TryParse(authnInstantNode?.Value, out var authnInstant))
        {
            response.AuthnInstant = authnInstant;
        }

        var authnContextNode = samlResponse.SelectSingleNode("//saml:AuthnStatement/saml:AuthnContext/saml:AuthnContextClassRef", namespaceManager);
        response.AuthnContext = authnContextNode?.InnerText;

        var statusNode = samlResponse.SelectSingleNode("//samlp:Status", namespaceManager);
        ArgumentNullException.ThrowIfNull(statusNode);

        var primaryStatusCodeNode = statusNode.SelectSingleNode("samlp:StatusCode/@Value", namespaceManager);
        ArgumentException.ThrowIfNullOrWhiteSpace(primaryStatusCodeNode?.Value);
        response.StatusCode = primaryStatusCodeNode.Value;
        if (!response.StatusCode.Contains("Success"))
        {
            logger.LogWarning("SAML response status indicates failure");
            throw new InvalidOperationException("Operation Failed.");
        }

        var nestedStatusCodeNode = statusNode.SelectSingleNode("samlp:StatusCode/samlp:StatusCode/@Value", namespaceManager);
        response.NestedStatusCode = nestedStatusCodeNode?.Value;

        var statusMessageNode = statusNode.SelectSingleNode("samlp:StatusMessage", namespaceManager);
        response.StatusMessage = statusMessageNode?.InnerText;

        logger.LogInformation("Extracted SAML response successfully. RoleCount={RoleCount}", response.Roles.Count);
        return response;
    }

    public void StoreSamlResponseInCookie(HttpResponse response, string organizationId, SamlResponse samlResponse)
    {
        logger.LogDebug(
            "Storing SAML response in cookie. SessionHasExpiry={SessionHasExpiry}",
            samlResponse.SessionNotOnOrAfter != DateTimeOffset.MaxValue);

        response.Cookies.Append(
            $"{Constants.OrganizationSsoCookiePrefix}-{organizationId}",
            cookieEncryptionService.Encrypt(JsonSerializer.Serialize(samlResponse)),
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                MaxAge = samlResponse.SessionNotOnOrAfter - timeProvider.GetUtcNow(),
                Expires = samlResponse.SessionNotOnOrAfter,
            });
    }

    public SamlResponse RetrieveSamlResponseFromCookie(string rawResponse)
    {
        logger.LogDebug("Retrieving SAML response from cookie. PayloadLength={PayloadLength}", rawResponse.Length);
        return JsonSerializer.Deserialize<SamlResponse>(cookieEncryptionService.Decrypt(rawResponse))!;
    }

    public async Task<bool> ValidateMetadataAsync(string metadataUrl, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Validating SAML metadata document");
            var metadata = await metadataUrl.GetStringAsync(cancellationToken: cancellationToken);
            var document = XDocument.Parse(metadata);

            // Verify required SAML metadata elements
            var descriptor = document.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "IDPSSODescriptor");
            if (descriptor == null)
            {
                return false;
            }

            // Verify SSO service endpoint
            var ssoService = descriptor.Descendants()
                .FirstOrDefault(x => x.Name.LocalName == "SingleSignOnService");
            if (ssoService?.Attribute("Location") == null)
            {
                logger.LogWarning("SAML metadata validation failed because SSO service location is missing");
                return false;
            }

            logger.LogInformation("SAML metadata validated successfully");
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning("SAML metadata validation failed. ExceptionType={ExceptionType}", ex.GetType().Name);
            return false;
        }
    }

    public async Task<bool> ValidateCertificateAsync(string metadataUrl, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogDebug("Validating SAML signing certificate");
            var certificate = await GetSigningCertificateFromMetadataAsync(metadataUrl, cancellationToken);

            // Check if certificate is expired
            if (certificate.NotAfter < timeProvider.GetUtcNow())
            {
                logger.LogWarning("SAML signing certificate is expired");
                return false;
            }

            // Basic certificate validation
            if (!certificate.HasPrivateKey && certificate.GetRSAPublicKey() != null)
            {
                logger.LogInformation("SAML signing certificate validated successfully using public key");
                return true;
            }

            // Verify certificate has valid key usage
            var keyUsages = certificate.Extensions["2.5.29.15"] as X509KeyUsageExtension;
            var isValid = keyUsages == null || keyUsages.KeyUsages.HasFlag(X509KeyUsageFlags.DigitalSignature);
            logger.LogInformation("SAML signing certificate validation completed. IsValid={IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            logger.LogWarning("SAML certificate validation failed. ExceptionType={ExceptionType}", ex.GetType().Name);
            return false;
        }
    }

    private async Task<X509Certificate2> GetSigningCertificateFromMetadataAsync(string metadataUrl, CancellationToken cancellationToken) =>
        (await memoryCache.GetOrCreateAsync(
            $"organization-sso-settings-{metadataUrl}",
            async cacheEntry =>
            {
                logger.LogDebug("Loading SAML signing certificate from metadata source");
                cacheEntry.AbsoluteExpiration = timeProvider.GetUtcNow().AddHours(1);

                var metadata = await metadataUrl.GetStringAsync(cancellationToken: cancellationToken);
                var document = XDocument.Parse(metadata);
                var certNode = document.Descendants().FirstOrDefault(x => x.Name.LocalName == "X509Certificate");
                if (certNode == null)
                {
                    logger.LogWarning("SAML metadata did not contain an X509Certificate element");
                    throw new SamlMetadataException();
                }

                var certificateRawData = Convert.FromBase64String(certNode.Value);
                return X509CertificateLoader.LoadCertificate(certificateRawData);
            }))!;
}
