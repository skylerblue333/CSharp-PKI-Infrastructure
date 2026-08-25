using System.Security.Cryptography.X509Certificates;
using Skycoin.X509Lab;
using Xunit;

namespace Skycoin.X509Lab.Tests;

public sealed class X509CertificateFactoryTests
{
    [Fact]
    public void CreatesParseableSelfSignedCertificate()
    {
        var now = DateTimeOffset.Parse("2026-08-24T12:00:00Z");
        var artifact = X509CertificateFactory.Create(new CertificateRequestModel("CN=sky.test", 30, 2048), now);

        Assert.Equal("CN=sky.test", artifact.Subject);
        Assert.Equal(2048, artifact.KeySize);
        Assert.Equal(64, artifact.ThumbprintSha256.Length);
        Assert.Contains("BEGIN CERTIFICATE", artifact.CertificatePem);

        using var certificate = X509Certificate2.CreateFromPem(artifact.CertificatePem);
        Assert.Equal("CN=sky.test", certificate.Subject);
        Assert.True(certificate.NotAfter > certificate.NotBefore);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(398)]
    public void RejectsInvalidLifetime(int days)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            X509CertificateFactory.Create(new CertificateRequestModel("CN=sky.test", days, 2048)));
    }

    [Fact]
    public void RejectsUnsupportedKeySize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            X509CertificateFactory.Create(new CertificateRequestModel("CN=sky.test", 30, 1024)));
    }
}
