# Encryption Module — Agent Notes

## Purpose

Owns reusable low-level encryption primitives shared across modules, without taking ownership of the
higher-level use cases that depend on them.

## What Lives Here

- `IStringEncryptionAlgorithm`
- `StringEncryptionAlgorithm`
- Shared encryption-key configuration types in `Encryption/Configurations/`

## Ownership Boundary

- `Encryption/` owns the cipher primitive.
- `Cookie/` owns cookie-specific encryption behaviour.
- `Accounting/` owns Xero token-at-rest encryption behaviour.

## Rules

- Keep this module generic and reusable; do not add cookie-specific, SSO-specific, or Xero-specific
  policy here.
- If a service only wraps `IStringEncryptionAlgorithm` for a specific product concern, keep that
  wrapper in the owning module rather than moving it into `Encryption/`.
