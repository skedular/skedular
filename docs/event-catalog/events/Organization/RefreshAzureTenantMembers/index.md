---
id: RefreshAzureTenantMembers
name: Refresh Azure Tenant Members
version: 0.0.1
summary: |
  Indicates azure tenant members need to be refreshed. It is an internal event for Organization domain.
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

The RefreshAzureTenantMembers event is triggered in an interval to refresh Azure tenants members. 

## Example payload

```json title="Example Payload"
{
  "will be provided"
}
```

## Schema (Proto)

<Schema file="schema.proto" />

## Schema (JSON)

<Schema file="schema.json" />

