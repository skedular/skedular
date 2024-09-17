---
id: OrganizationUpserted
name: Organization Upserted
version: 0.0.1
summary: |
  Indicates an organization has been changed
owners:
    - malizadeh
badges:
    - content: Recently updated!
      backgroundColor: green
      textColor: green
    - content: Channel:Apache Kafka
      backgroundColor: yellow
      textColor: yellow
schemaPath: schema.proto
---

## Overview

The OrganizationUpserted event is triggered whenever an existing organization is modified. This event ensures that all relevant services are notified of changes to an organization,

## Example payload

```json title="Example Payload"
{
  "will be provided",
}
```

## Schema (Proto)

<Schema file="schema.proto" />

## Schema (JSON)

<Schema file="schema.json" />

