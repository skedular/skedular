# MembershipTerm Contracts

## Domain and persistence boundary

- Domain models, services, validators, workflows, mappers, and serializers expose `MembershipTerm`.
- The existing product/offering storage field remains unchanged in this changeset.
- Persistence reads and writes only that existing field and explicitly maps it to/from `MembershipTerm`.

## Event and API contracts

- Protobuf/event definitions expose the renamed field and generated event artifacts match them.
- GraphQL object/input fields and generated Relay operations expose `membershipTerm`.
- OpenAPI definitions and generated clients expose the renamed field.
- No active external alias is required; compatibility is limited to persistence.

## Web contract

- Customer webapp, Host, Spaces, public web, and legacy editors call the contractual period “membership term.”
- Generated Relay and API clients are regenerated from updated source contracts.
- Existing mutation payload, stable-ID, connection-update, and refetch behavior remains unchanged.

## Compatibility

- Existing records remain readable and writable through the old storage representation.
- Null, unset, cadence-free, and invalid values retain existing handling.
- The migration does not change business values or semantics.
