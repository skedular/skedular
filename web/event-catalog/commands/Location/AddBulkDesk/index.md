---
id: AddBulkDesk
name: Add Bulk Desk
version: 0.0.1
summary: |
  Command that will add desk in bulk
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

The AddBulkDesk command is issued to add desks in bulk in a location.

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
