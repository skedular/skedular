---
id: AddLocation
name: Add Location
version: 0.0.1
summary: |
  Command that will add location
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

The AddLocation command is issued to add new location to an organization.

## Architecture diagram

<NodeGraph/>

## Payload example

```json title="Payload example"
{
  "Id": "789e1234-b56c-78d9-e012-3456789fghij",
  "Name": "New Team"
}

```

## Schema

<Schema file="schema.json"/>
