# Slack Shared Agent Notes

This file covers `slack/shared/`.

## Agent Rule

- Preserve stable integration semantics and avoid unnecessary behavioral drift.
- Replicated organization, location, and team entities are part of Slack workspace/channel routing and update targeting.
- Do not remove those replicas unless the Slack routing model is redesigned.
