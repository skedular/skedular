---
id: CustomerUpserted
name: Customer Upserted
version: 0.0.1
summary: |
  Indicates an customer has been changed
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

The CustomerUpserted event is triggered whenever an existing customer is modified. This event ensures that all relevant services are notified of changes to a customer,

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

