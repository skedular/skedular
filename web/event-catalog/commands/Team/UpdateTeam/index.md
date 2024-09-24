---
id: UpdateTeam
name: Update Team
version: 0.0.1
summary: |
  Command that will update team
owners:
    - malizadeh
    - full-stack
badges:
    - content: Recently updated!
      backgroundColor: green
      textColor: green
schemaPath: 'schema.json'
---

## Overview

The UpdateTeam command is issued to update a team in an organization.

## Architecture diagram

<NodeGraph/>

## Payload example

```json title="Payload example"
{
  "Id": "789e1234-b56c-78d9-e012-3456789fghij",
  "Name": "New Team",
  "About": "Development Team"
}

```

## Schema

<Schema file="schema.json"/>
