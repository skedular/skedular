---
id: NotificationUpserted
name: NotificationUpserted
version: 0.0.1
summary: |
  Indicates a notification has been changed.
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

The NotificationUpserted event is triggered whenever a notification raised in th system. This event ensures that all relevant services are notified of changes to a notification,

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

