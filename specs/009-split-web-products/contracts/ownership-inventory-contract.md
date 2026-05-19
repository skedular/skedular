# Contract: Ownership Inventory

The ownership inventory is the reviewable record for what moves where.

## Required Fields

| Field | Required | Description |
|-------|----------|-------------|
| `item_path` | Yes | Repository path, route, or config key being classified |
| `item_type` | Yes | Route, page, component, hook, utility, provider, query, generated artefact, config, or documentation |
| `current_owner` | Yes | Current app/package location |
| `target_owner` | Yes | WebApp, WebApp Spaces, WebApp Teams, `@skedular/ui`, `@skedular/shared`, or transition |
| `reason` | Yes | Why the target owner is correct |
| `backend_return_url_risk` | Yes | `yes`, `no`, or `unknown` |
| `relay_impact` | Yes | `yes`, `no`, or `unknown` |
| `tests_required` | Yes | Verification scope for this item |
| `transition_condition` | Conditional | Required when target owner is transition |

## Validation Rules

- Every targeted item has one target owner or one transition owner.
- Transition entries include owner, reason, affected apps, and removal condition.
- Items marked `backend_return_url_risk = yes` must be included in a route retirement review before deletion.
- Items marked `relay_impact = yes` require Relay generation/checks in the migration slice.
- Shared target owners are allowed only for neutral foundations.

## Example

```markdown
| item_path | item_type | current_owner | target_owner | reason | backend_return_url_risk | relay_impact | tests_required | transition_condition |
|-----------|-----------|---------------|--------------|--------|--------------------------|--------------|----------------|----------------------|
| web/apps/webapp/src/components/example | component | webapp | webapp-teams | Private organisation-only workflow | no | no | webapp-teams test/build | |
```
