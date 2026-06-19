using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var certificates = new ConcurrentDictionary<string, object>();

app.MapPost("/api/v1/certificates", async (HttpContext context) => {
    using var doc = await JsonDocument.ParseAsync(context.Request.Body);
    var root = doc.RootElement;
    var subject = root.TryGetProperty("subject", out var s) ? s.GetString() ?? "CN=Unknown" : "CN=Unknown";
    
    using var rsa = RSA.Create(2048);
    var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
    var certId = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));
    
    var cert = new {
        id = certId,
        subject,
        public_key = publicKey[..64] + "...",
        key_size = 2048,
        algorithm = "RSA",
        issued_at = DateTimeOffset.UtcNow,
        expires_at = DateTimeOffset.UtcNow.AddYears(1)
    };
    
    certificates[certId] = cert;
    await context.Response.WriteAsJsonAsync(cert);
});

app.MapGet("/api/v1/certificates", () => certificates.Values.ToArray());
app.MapGet("/health", () => new { status = "healthy" });

app.Run("http://0.0.0.0:8080");
