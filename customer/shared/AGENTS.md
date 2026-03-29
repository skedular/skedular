# Customer Shared Agent Notes

This file covers `customer/shared/`.

## Agent Rule

- Preserve stable customer identity semantics and replicated-state assumptions.
- Other domains intentionally replicate customer and identity data to support local authorization and membership-aware access checks.
- Do not propose removing those replicas unless the downstream authorization model is changing too.
