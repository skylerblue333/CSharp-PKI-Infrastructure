using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Skycoin.X509Lab;

public sealed record CertificateRequestModel(string Subject, int ValidDays = 30, int KeySize = 2048);
public sealed record CertificateArtifact(
    string Id,
    string Subject,
    string SerialNumber,
    string ThumbprintSha256,
    int KeySize,
    DateTimeOffset NotBefore,
    DateTimeOffset NotAfter,
    string CertificatePem);

public static class X509CertificateFactory
{
    public static CertificateArtifact Create(CertificateRequestModel input, DateTimeOffset? now = null)
    {
        if (string.IsNullOrWhiteSpace(input.Subject) || input.Subject.Length > 256)
            throw new ArgumentException("subject must contain 1-256 characters", nameof(input));
        if (input.ValidDays is < 1 or > 397)
            throw new ArgumentOutOfRangeException(nameof(input), "valid_days must be between 1 and 397");
        if (input.KeySize is not (2048 or 3072 or 4096))
            throw new ArgumentOutOfRangeException(nameof(input), "key_size must be 2048, 3072, or 4096");

        var instant = now ?? DateTimeOffset.UtcNow;
        using var rsa = RSA.Create(input.KeySize);
        var distinguishedName = new X500DistinguishedName(input.Subject.Trim());
        var request = new CertificateRequest(
            distinguishedName,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
        request.CertificateExtensions.Add(new X509KeyUsageExtension(
            X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
            true));
        request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

        using var certificate = request.CreateSelfSigned(instant.AddMinutes(-1), instant.AddDays(input.ValidDays));
        var der = certificate.Export(X509ContentType.Cert);
        var fingerprint = Convert.ToHexString(SHA256.HashData(der));

        return new CertificateArtifact(
            Convert.ToHexString(RandomNumberGenerator.GetBytes(12)),
            certificate.Subject,
            certificate.SerialNumber,
            fingerprint,
            input.KeySize,
            certificate.NotBefore.ToUniversalTime(),
            certificate.NotAfter.ToUniversalTime(),
            certificate.ExportCertificatePem());
    }
}
