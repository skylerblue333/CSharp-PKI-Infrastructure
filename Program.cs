using System.Collections.Concurrent;
using Skycoin.X509Lab;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");
var app = builder.Build();

const int maxCertificates = 100;
var certificates = new ConcurrentQueue<CertificateArtifact>();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["Cache-Control"] = "no-store";
    await next();
});

app.MapPost("/api/v1/certificates", (CertificateRequestModel request) =>
{
    try
    {
        var certificate = X509CertificateFactory.Create(request);
        certificates.Enqueue(certificate);
        while (certificates.Count > maxCertificates && certificates.TryDequeue(out _)) { }
        return Results.Created($"/api/v1/certificates/{certificate.Id}", certificate);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/v1/certificates", () => Results.Ok(certificates.ToArray()));
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "sky-x509-lab" }));
app.MapGet("/ready", () => Results.Ok(new { status = "ready" }));

app.Run();
