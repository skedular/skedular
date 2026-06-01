# Content Gap Register

This register lists flows that should not be guessed in public help. Each gap has enough context for product, engineering, or support review to fill in later.

## Customer

| Gap | Why it is a gap | First-version treatment |
| --- | --- | --- |
| Customer-facing subdomain fallback behavior | The route resolver has tests and code, but public copy should be confirmed against live domain behavior. | Explain that users may enter through an organization's public marketplace link and mark deeper fallback detail as pending. |
| Exact booking, subscription, cancellation, and refund status labels | Status names may vary by product policy and payment state. | Explain the meaning in plain language and avoid listing unsupported exact labels. |
| Auth/provider error copy | Sign-in and callback behavior depends on identity provider responses. | Explain common next steps without exposing provider internals. |

## Teams

| Gap | Why it is a gap | First-version treatment |
| --- | --- | --- |
| Exact permission matrix | Routes show teams, users, admin, and settings, but the full role matrix is not safe to infer from route names. | Explain that visible actions depend on organization access. |
| Every private resource field | Resource creation exists, but field-level behavior needs product review. | Explain the high-level resource workflow and leave field-by-field details for screenshots/product review. |
| Microsoft Teams install failure states | Integration pages exist, but provider-specific failure messages need live review. | Explain the normal install flow and where to retry or contact an admin. |
| Analytics metric definitions | Analytics and availability routes exist, but exact metric definitions should be reviewed with product. | Describe the dashboard purpose without promising exact formulas. |

## Spaces

| Gap | Why it is a gap | First-version treatment |
| --- | --- | --- |
| Direct refund route availability | Refund root page exists, but direct app route coverage needs confirmation. | Explain refund work as operator-owned and mark exact route/status labels as pending. |
| Marketplace-public route behavior in Microsoft Teams | Microsoft Teams marketplace-public surface exists, but public state transitions need review. | Explain that Microsoft Teams can support operator workflows and leave route-specific states as pending. |
| Payment setup failure states | Bank account and Stripe Connect routes exist, but provider-specific failures are sensitive and need careful review. | Explain setup purpose and public-safe troubleshooting only. |
| Exact subscription renewal and refund status labels | Operator state depends on booking/payment/refund lifecycle. | Explain what the operator should look for without exposing internal accounting details. |
