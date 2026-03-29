# Core Shared Agent Notes

This file covers `core/shared/`.

## Agent Rule

- Treat `core/shared/` as a dependency surface for the rest of the repo.
- Preserve compatibility and avoid domain-specific shortcuts here.
- Assume replicated auth state may be intentional and required by downstream authorization paths.
