# Naming Conventions for Cloud Resources

Consistent naming helps with resource management, automation, and clarity across environments and cloud providers.

---

## **General Mask**

```
<cloud-provider>-<location>-<environment>-<number>
```

---

## **Environment Codes**

| Environment   | Code |
|---------------|------|
| Production    | prd  |
| Staging       | stg  |
| Development   | dev  |
| QA            | qa   |
| Operations    | ops  |

---

## **Location Codes**

| Location      | Code |
|---------------|------|
| Australia     | au   |
| USA           | us   |

---

## **Cloud Provider Codes**

| Provider      | Code |
|---------------|------|
| Azure         | azr  |
| AWS           | aws  |
| Google Cloud  | gcp  |

---

## **Example**

```
azr-au-prd-001
aws-us-dev-002
gcp-au-qa-003
```

Use these conventions for all resource names to ensure consistency and clarity.