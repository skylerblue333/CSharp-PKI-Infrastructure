# Security

Sky X509 Lab is an engineering-beta development utility, not a production certificate authority.

## Boundaries

- Private RSA keys are generated in memory only and are not returned or persisted.
- The service issues self-signed leaf certificates only.
- Inputs are bounded by subject length, validity period, and approved RSA key sizes.
- The API has no authentication, authorization, tenant isolation, durable audit log, revocation, HSM/KMS integration, or CA hierarchy.
- TLS termination and network access controls must be provided externally.

Do not expose this service to untrusted networks or use its certificates as production trust anchors.

Report suspected vulnerabilities through the repository's private security-reporting channel when available; otherwise open a minimal issue without secrets or exploit material.
