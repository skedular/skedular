# Contract: Migration Slice

Each implementation slice must be independently reviewable.

## Required Slice Record

```markdown
## Slice: <slice-id>

Target app: <WebApp | WebApp Spaces | WebApp Teams | Shared>
Journey: <one user-visible journey or tightly related group>
Owner: <person or role responsible for review>

### Scope

- Included:
- Excluded:

### Ownership Moves

- App-owned code moved:
- Shared UI foundations moved:
- Shared application foundations moved:
- Transitional adapters retained:

### Route Retirement

- Old routes:
- Action: keep | redirect | block | delete | transition
- Backend-originated return URL audit: pass | blocked | not applicable

### Verification

- Lint:
- Tests:
- Build:
- Relay:
- Manual review:

### Acceptance

- Ready for user review: yes | no
- Accepted before next slice: yes | no
```

## Rules

- A slice should move one journey or one tightly related journey group.
- A slice must not depend on unreviewed future slices for basic app usability.
- A slice must stop at `Ready for user review` until the user confirms it works.
- A slice must not delete old routes with unresolved backend-originated return URL usage.
- A slice must keep Teams free of marketplace organisation/product concepts.
