# Slack API Agent Notes

This file covers `slack/apis/`.

## Agent Rule

- Keep API endpoints thin.
- Fix shared Slack behavior in the shared layer rather than burying logic in transport code.
