using System.Security.Cryptography.X509Certificates;
using Enterprise.Shared.Security.Sso;
using FluentAssertions;
using Flurl.Http.Testing;
using Testing.Shared;
using Xunit;

namespace Enterprise.Shared.UnitTests.Security.Sso.SamlAssertionConsumerServiceTests;

public class GetSigningCertificateFromMetadataAsyncShould
{
    [Theory]
    [AutoFakeItEasyData]
    public async Task Return_Certificate(SamlAssertionConsumerService sut, CancellationToken cancellationToken)
    {
        // Arrange
        const string MetaDataUrl = "https://example.com/metadata.xml";
        var certBase64FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sso-sample-cert.cer");
        var metadataFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "sso-sample-metadata.xml");

        var metadataXml = await File.ReadAllTextAsync(metadataFilePath, cancellationToken);
        var expectedCertificate = X509CertificateLoader.LoadCertificateFromFile(certBase64FilePath);

        using var httpTest = new HttpTest();
        httpTest.RespondWith(metadataXml);

        // Act
        var certificate = await sut.GetSigningCertificateFromMetadataAsync(MetaDataUrl, cancellationToken);

        // Assert
        certificate.Should().NotBeNull();
        certificate.Should().Be(expectedCertificate);
    }
}
