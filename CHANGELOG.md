# Changelog

## Unreleased

- Replaced the placeholder public-key response with real self-signed X.509 certificate generation.
- Added bounded subject, validity, and RSA key-size validation.
- Added PEM output, serial metadata, and SHA-256 certificate fingerprints.
- Added isolated xUnit tests, Release/warnings-as-errors CI, vulnerable dependency reporting, non-root container verification, and runtime health smoke testing.
- Added explicit engineering-beta/security-lab boundaries.
