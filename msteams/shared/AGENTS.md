# MsTeams Shared Agent Notes

This file covers `msteams/shared/`.

## Agent Rule

- External contract changes here can break production integrations quickly, so keep edits conservative.
- Replicated organization, location, and team entities are part of Azure-tenant and Teams-channel routing and update targeting.
- Do not remove those replicas unless the Microsoft Teams routing model is redesigned.
