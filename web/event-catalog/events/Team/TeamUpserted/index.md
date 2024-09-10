---
id: TeamUpserted
name: Team Upserted
version: 0.0.1
summary: |
  Indicates an team has been changed
owners:
    - malizadeh
badges:
    - content: Recently updated!
      backgroundColor: green
      textColor: green
    - content: Channel:Apache Kafka
      backgroundColor: yellow
      textColor: yellow
schemaPath: schema.avro
---

## Overview

The TeamUpserted event is triggered whenever an existing team is modified. This event ensures that all relevant services are notified of changes to a team,

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

