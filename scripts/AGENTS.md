# Scripts Notes

This file applies to `scripts`.

## Event Metadata Script

- `generate-event-metadata.sh` is the repo-owned entry point for regenerating checked-in event metadata companions.
- Keep its path-handling style aligned with other repo generation scripts.
- It should generate metadata files under `shared/Api.Shared.Clients/Events`, while protobuf event message classes stay
  as transient build outputs in `obj`.

## Regeneration Discipline

- If the script output shape changes, update the matching local `AGENTS.md` guidance in:
  - `api-definitions/events/`
  - `shared/Api.Shared.Clients/`
  - `shared/Skedularctl/`
