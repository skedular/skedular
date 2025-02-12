---
id: UpdateLocationTag
name: Update Location Tag
version: 0.0.1
summary: |
  Command that will update location
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

The UpdateLocation command is issued to update a location tag in an organization.

## Architecture diagram

<NodeGraph/>

## Payload example

```json title="Payload example"
{
  "Id": "789e1234-b56c-78d9-e012-3456789fghij",
  "Name": "Location Tag"
}

```

## Schema

<Schema file="schema.json"/>
