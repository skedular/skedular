# Booking Runtime Flows

This document is the visual companion
to [booking-domain-architecture.md](/Users/morteza/projects/github.com/unityhubio/unityhubio/booking/domain/docs/architecture/booking-domain-architecture.md).

It is intentionally diagram-first and focuses on how the booking domain behaves at runtime.

## 1. Booking Type Routing

This is the first split in the domain.

```mermaid
flowchart TD
    A["User action"] --> B{"Booking type?"}

    B -->|"Private one-time"| C["PrivateBookingService"]
    B -->|"Private recurring"| D["PrivateRecurringBookingService"]
    B -->|"Marketplace one-time"| E["MarketplaceBookingService"]
    B -->|"Marketplace subscription"| F["MarketplaceBookingSubscriptionService"]

    C --> C1["Persist booking + slots"]

    D --> D1["Persist recurring template"]
    D1 --> D2["Start BookPrivateRecurringResources workflow"]

    E --> E1["Persist booking + marketplace payload"]
    E1 --> E2{"Payment method?"}
    E2 -->|"Card"| E3["Start PayBookingViaCard workflow"]
    E2 -->|"Bank transfer"| E4["Start PayBookingViaBankTransfer workflow"]
    E2 -->|"In arrears"| E5["Start initial arrears invoice flow"]

    F --> F1["Persist subscription + template marketplace booking"]
    F1 --> F2["Start BookMarketplaceBookingSubscriptionResources workflow"]
```

## 2. Marketplace One-Time Booking: Card

```mermaid
sequenceDiagram
    participant User
    participant API as Booking API
    participant Shared as MarketplaceBookingService
    participant Temporal as PayBookingViaCard
    participant Invoice as InvoiceIntegrations
    participant Stripe as StripeIntegrations
    participant StripeSvc as Stripe
    participant Xero as Xero

    User->>API: Book marketplace product
    API->>Shared: validate auth, pricing, resources
    Shared->>Shared: allocate resources and persist booking
    Shared->>Temporal: start PayBookingViaCard

    Temporal->>Invoice: calculate booking amounts
    Temporal->>Invoice: generate initial invoice (unpaid)

    alt Xero-managed standard invoice
        Invoice->>Xero: export standard invoice
    else Local invoice delivery
        Invoice->>Invoice: render PDF, upload, email
    end

    Temporal->>Stripe: upsert product + pricing
    Temporal->>Stripe: upsert customer
    Temporal->>StripeSvc: create checkout session
    StripeSvc-->>Temporal: payment status signal later

    alt confirmed / no payment required
        Temporal->>Invoice: generate fully-paid invoice
    else expired / rejected / deleted
        Temporal->>API: release resources + cancel accounting state
    end
```

## 3. Marketplace One-Time Booking: Bank Transfer

```mermaid
sequenceDiagram
    participant User
    participant API as Booking API
    participant Shared as MarketplaceBookingService
    participant Temporal as PayBookingViaBankTransfer
    participant Invoice as InvoiceIntegrations
    participant Xero as Xero

    User->>API: Book marketplace product
    API->>Shared: validate auth, pricing, resources
    Shared->>Shared: allocate resources and persist booking
    Shared->>Temporal: start PayBookingViaBankTransfer

    Temporal->>Invoice: calculate booking amounts
    Temporal->>Invoice: generate initial invoice (awaiting payment)

    alt Xero-managed standard invoice
        Invoice->>Xero: export standard invoice
    else Local invoice delivery
        Invoice->>Invoice: render PDF, upload, email
    end

    Note over Temporal: Wait until expiry or a payment-status signal

    alt confirmed / payment not required
        Temporal->>Invoice: generate fully-paid invoice
    else expired / rejected / deleted
        Temporal->>API: release resources + cancel accounting state
    end
```

## 4. Private Recurring Booking Loop

This is the non-marketplace recurring path.

```mermaid
flowchart TD
    A["Private recurring booking created"] --> B["Start BookPrivateRecurringResources"]
    B --> C{"Deleted?"}
    C -->|Yes| D["Release future private recurring resources"]
    C -->|No| E["Adjust required resources for private recurring booking"]
    E --> F{"Ended or deleted?"}
    F -->|Yes| D
    F -->|No| G["Wait for update signal or 1 day"]
    G --> C
```

## 5. Marketplace Subscription: Daily Maintenance Loop

This is the core auto-renewal and reconciliation loop.

```mermaid
flowchart TD
    A["Subscription created"] --> B["Start BookMarketplaceBookingSubscriptionResources"]

    B --> C{"Subscription deleted signal?"}
    C -->|Yes| Z["Release subscription resources + stop future billing"]
    C -->|No| D["Load subscription"]
    D --> E["Update subscription state"]
    E --> F{"Subscription status"}

    F -->|"Paused"| G["Keep workflow alive"]
    F -->|"Cancelled / ended / renewal failed"| H["Finish current cycle processing"]
    F -->|"Active"| I["Ensure current cycle recurring booking exists"]

    I --> J["For each recurring booking in subscription"]
    J --> K["Build reconciliation plan"]
    K --> L["Remove obsolete future bookings"]
    K --> M["Repair resource assignment for existing future bookings"]
    K --> N["Create missing future booking days"]

    N --> O{"Need payment workflow for this cycle?"}
    O -->|Yes| P["Start recurring card/bank-transfer workflow"]
    O -->|No| Q["Keep booking instances only"]

    P --> R{"Current cycle still active?"}
    Q --> R
    R -->|Yes| S["Wait 1 day"]
    R -->|No| H
    S --> C
    G --> S
```

## 6. Marketplace Subscription Renewal Decision

This shows how the subscription transitions from one cycle to the next.

```mermaid
flowchart TD
    A["Subscription daily reconciliation"] --> B{"Current date >= NextRenewalAt?"}
    B -->|No| C["Stay on current cycle"]
    B -->|Yes| D{"AutoRenew enabled?"}
    D -->|No| E["Mark current subscription lifecycle as ending"]
    D -->|Yes| F{"CancelAtPeriodEnd?"}
    F -->|Yes| G["Do not create next cycle"]
    F -->|No| H["Load current product version + pricing"]

    H --> I{"Matching pricing still exists and supports renewal?"}
    I -->|No| J["Set status = RenewalFailed"]
    I -->|Yes| K["Advance NextRenewalAt by purchase cadence"]
    K --> L["Create or ensure next-cycle recurring booking"]
    L --> M["Start recurring payment/invoice path for that cycle"]
```

## 7. Recurring Cycle Billing Decision

This is the booking-owned decision that sits before invoice delivery.

```mermaid
flowchart TD
    A["Recurring booking cycle"] --> B["RecurringInvoiceBillingScheduleService"]
    B --> C{"Purchase cadence longer than org billing cycle?"}

    C -->|No| D["Use purchase cadence as invoice cadence"]
    C -->|Yes| E["Split by organization billing cycle"]

    D --> F["Invoice amount = full recurring charge"]
    E --> G["Invoice amount = installment amount"]

    F --> H["Resulting provider-agnostic billing definition"]
    G --> H
```

## 8. Recurring Invoice Delivery Decision

```mermaid
flowchart TD
    A["Recurring cycle needs invoice"] --> B["InvoiceIntegrations.GenerateAndSendRecurringInvoiceAsync"]
    B --> C["Load org Xero connection"]
    C --> D["Build provider-agnostic billing definition"]
    D --> E["Create first concrete invoice immediately"]
    E --> F["Build desired Xero repeating schedule if later cycles need Xero automation"]
    F --> G["Only keep template when auto-renew is on or billing-cycle splitting is required"]
    G --> H["When used, start repeating template from the next billing boundary"]
    H --> I["XeroRecurringInvoiceTransitionService"]

    I --> J{"Freeze existing repeating invoice?"}
    J -->|Yes| K["Stop here; keep local state transition-required"]
    J -->|No| L{"Org uses Xero for recurring invoicing?"}

    L -->|No| M["Generate local invoice PDF/email"]
    L -->|Yes| N{"Can export as Xero repeating template?"}
    N -->|Yes| O["Create or reuse Xero repeating invoice template for later cycles"]
    N -->|No| P["Only keep the immediate concrete invoice"]

    O --> Q{"SendInvoicesViaXero?"}
    P --> Q
    Q -->|Yes| R["Only publish state change locally"]
    Q -->|No| S["Also render/upload/send local invoice copy"]
```

## 9. Organization In-Arrears Billing

```mermaid
flowchart TD
    A["RunOrganizationArrearsBilling workflow"] --> B["Get next scheduled run time"]
    B --> C["Resolve billing period"]
    C --> D["Load in-arrears marketplace bookings in billing period"]
    D --> E["Build grouped invoice drafts per customer"]
    E --> F{"Any drafts?"}
    F -->|No| G["Stop this cycle"]
    F -->|Yes| H["Persist organization arrears invoice"]
    H --> I{"Xero-managed arrears?"}
    I -->|Yes| J["Export Xero invoice"]
    I -->|No| K["Render/upload/email local PDF"]
    J --> L["Start accounting-state maintenance workflow"]
    K --> M["Publish booking GraphQL updates"]
    L --> M
```

## 10. Cancellation Modes

```mermaid
flowchart TD
    A["User cancels subscription"] --> B{"Cancellation mode"}

    B -->|"Immediate"| C["Validate immediate cancellation policy"]
    B -->|"At period end"| D["Skip immediate cutoff check"]

    C --> E["Set status = Cancelled"]
    E --> F["AutoRenew = false"]
    E --> G["Signal subscription maintenance workflow delete/release path"]
    G --> H["Free future resources only"]
    G --> I["Cancel future billing and accounting state"]

    D --> J["CancelAtPeriodEnd = true"]
    J --> K["AutoRenew = false"]
    J --> L["Keep current cycle intact"]
    L --> M["Daily workflow reaches renewal boundary"]
    M --> N["Do not materialize next cycle"]
```

## 11. Booking / Invoice Cancellation Side Effects

```mermaid
sequenceDiagram
    participant Delete as Delete/release flow
    participant Local as Local booking state
    participant Cancel as AccountingInvoiceCancellationService
    participant Xero as Xero

    Delete->>Local: persist local cancellation / resource release
    Local-->>Delete: commit
    Delete->>Cancel: cancel accounting link

    alt no external invoice exists
        Cancel->>Cancel: mark local accounting state cancelled
    else Xero standard invoice exists
        Cancel->>Xero: void invoice
        Xero-->>Cancel: cancelled
    else Xero repeating template exists
        Cancel->>Xero: delete repeating template
        Xero-->>Cancel: cancelled
    else Xero unavailable / lookup fails
        Cancel->>Cancel: mark transition required
    end
```

## 12. Resource Repair and Known Gap

```mermaid
flowchart TD
    A["Daily reconciliation finds existing future booking"] --> B["AdjustRequiredResourcesAsync"]
    B --> C["Remove stale resource slots"]
    C --> D["Try requested resources / preference-based reassignment"]
    D --> E{"Enough replacement resources found?"}
    E -->|Yes| F["Persist repaired resource slots"]
    E -->|No| G["Current behavior is best-effort"]
```

Current known gap:

- resource repair still does not hard-fail when the full required resource count cannot be reassigned

## 13. Runtime Ownership Summary

```mermaid
flowchart LR
    UI["Web / Teams UI"] --> API["Booking API"]
    API --> Shared["Booking.Shared services"]
    Shared --> Outbox["Temporal outbox"]
    Outbox --> Workflows["Temporal workflows"]
    Workflows --> Activities["Temporal activities"]

    Activities --> Stripe["Stripe"]
    Activities --> Xero["Xero"]
    Activities --> Org["Organization API"]
    Activities --> Core["Core API"]

    Shared --> DB["Booking DB"]
    Activities --> DB
```
