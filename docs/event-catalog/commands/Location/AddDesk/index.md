---
id: AddDesk
name: Add Desk
version: 0.0.1
summary: |
  Command that will add desk
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

The AddDesk command is issued to add new desk in a location.

## Architecture diagram

<NodeGraph/>

## Payload example

```json title="Payload example"
{
  "Id": "789e1234-b56c-78d9-e012-3456789fghij",
  "Name": "New Desk"
}

```

## Schema

<Schema file="schema.json"/>
