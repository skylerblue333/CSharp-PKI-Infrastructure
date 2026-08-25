# Sky X509 Lab

Engineering-beta ASP.NET Core 8 service for generating bounded self-signed X.509 certificates for development and integration testing.

## What it does

- `POST /api/v1/certificates` creates a real self-signed RSA X.509 certificate.
- Accepts a bounded X.500 subject, validity period of 1-397 days, and RSA key sizes of 2048/3072/4096 bits.
- Returns PEM certificate material, serial number, SHA-256 fingerprint, validity window, and metadata.
- Keeps only the latest 100 certificate artifacts in process memory.
- `GET /api/v1/certificates` lists retained artifacts.
- `GET /health` and `/ready` expose operational status.
- CI verifies Release build, tests, vulnerable packages, container build, non-root execution, and runtime health.

## Example

```bash
curl -X POST http://localhost:8080/api/v1/certificates \
  -H 'Content-Type: application/json' \
  -d '{"subject":"CN=local.skycoin4444.test","validDays":30,"keySize":2048}'
```

## Run

```bash
dotnet restore CSharp-PKI-Infrastructure.csproj
dotnet run --project CSharp-PKI-Infrastructure.csproj
```

Or:

```bash
docker build -t sky-x509-lab .
docker run --rm -p 8080:8080 sky-x509-lab
```

## Product status

**Engineering beta / security lab.** This repository does not implement a certificate authority, CSR approval workflow, CA chain, revocation/CRL/OCSP, HSM/KMS key custody, private-key persistence/export, ACME, mTLS enrollment, RBAC, durable audit history, HA, or verified production deployment. Generated private keys exist only transiently during certificate creation and are not returned or retained.

Use generated certificates for local development, test fixtures, and integration experiments—not as a production trust infrastructure.

## SKYCOIN4444 integration

Potential consumers should treat this as a development-only certificate fixture service behind a stable HTTP boundary. Production ecosystem identity/TLS infrastructure should use independently verified managed CA/KMS/HSM controls.

## License

See `LICENSE`.
