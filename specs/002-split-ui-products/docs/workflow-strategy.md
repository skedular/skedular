# Workflow Strategy

CI/CD is managed via repository-level workflows under `.github/workflows`.

## Principles

- Keep product deployments independent.
- Reuse shared workflow templates where possible.
- Validate infra and app build separately before deploy stages.
