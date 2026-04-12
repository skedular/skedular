# Api.Shared.Services Agent Notes

This file covers `shared/Api.Shared.Services`.

## Shared Contract Rule

- Models and enum/string conversions here are cross-domain contracts.
- If a new enum value is added here, update all string constants, both conversion directions, and any display-name
  helper
  in the same change.
- When the model is consumed by multiple domains, add unit tests here so the shared contract is locked down before the
  domain-specific behavior changes.

## Copy Localisation Rule

- When this layer owns exception messages, display names, or other text that is surfaced to users or operators, write
  it in British spelling and grammar rather than American English.
- Keep that localisation limited to surfaced copy. Do not rename shared model types, enum members, namespaces, method
  names, or other technical contracts solely to make them British English.

## Xero Billing Mode Rule

- `OrganizationXeroBillingMode` is the shared contract for org-facing Xero billing settings.
- Organization owns exposing and validating the allowed billing-mode list.
- Booking owns the invoice-export behavior behind those billing modes.
- `RepeatingInvoices` is for recurring booking export behavior only; it does not mean every Xero invoice path becomes a
  repeating invoice template.
- The shared billing-mode contract does not encode recurring cadence rules by itself. The billing-cycle versus purchase-
  cadence split belongs in booking-owned recurring invoice behavior.
- If an org-facing Xero billing mode is exposed through GraphQL to the website, regenerate the web Relay artifacts after
  the schema/export change so the UI enum types stay in sync.

## Unit Test Shape

- Keep one test class/file per public method or extension-method name under `Api.Shared.Services.UnitTests`.
- Prefer direct assertions on shared enum/string conversion behavior rather than re-testing the same contract only
  through downstream domain services.
