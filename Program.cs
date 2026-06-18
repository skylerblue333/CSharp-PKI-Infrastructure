using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PKIInfrastructure
{
    public class CertificateAuthority
    {
        private readonly RSA _caKey;
        private readonly string _caName;

        public CertificateAuthority(string name)
        {
            _caName = name;
            _caKey = RSA.Create(2048);
            Console.WriteLine($"[CA] Initialized: {_caName}");
            Console.WriteLine($"[CA] Key size: {_caKey.KeySize} bits");
        }

        public string SignData(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var signature = _caKey.SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }

        public bool VerifySignature(string data, string signature)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            var sigBytes = Convert.FromBase64String(signature);
            return _caKey.VerifyData(bytes, sigBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== PKI Infrastructure Demo ===\n");
            var ca = new CertificateAuthority("Enterprise Root CA");

            string payload = "CN=skyler.example.com,O=Enterprise Inc,C=US";
            Console.WriteLine($"\n[SIGN] Payload: {payload}");

            string sig = ca.SignData(payload);
            Console.WriteLine($"[SIG]  {sig[..40]}...");

            bool valid = ca.VerifySignature(payload, sig);
            Console.WriteLine($"[VERIFY] Signature valid: {valid}");

            bool tampered = ca.VerifySignature(payload + "TAMPERED", sig);
            Console.WriteLine($"[VERIFY] Tampered data valid: {tampered}");
        }
    }
}
