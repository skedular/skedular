# Component Audit — Booking, Marketplace, Payment

**Generated**: 2026-06-03  
**Task**: T015  
**Scope**: `booking`, `marketplaceProduct`, `marketplaceProductBooking`, `marketplaceProductCard`, `marketplaceProductGuest`, `carousel`, `stripeConnectAccount`, `bankAccount`, `marketplaceProductSubscription`, `marketplaceRefund`

---

## booking (18 components — 0 with `'use client'`)

All 18 booking components are **server-compatible** (no `'use client'`). However, they render within client pages, so they currently execute client-side.

| Sub-category    | Count | Notes                                                                       |
| --------------- | ----- | --------------------------------------------------------------------------- |
| Booking display | ~8    | Booking card, detail, list — pure display                                   |
| Booking form    | ~5    | Form components — will need `'use client'` when forms become interactive    |
| Booking actions | ~5    | Cancel, modify — event handlers → need `'use client'` for interactive parts |

**Key insight**: Booking display components (lists, cards, detail views) are SSR candidates. Booking action components (cancel button, modify form) need client. The pattern to apply: render the data-display shell server-side, keep action buttons in client islands.

---

## marketplaceProduct (2 components — 0 with `'use client'`)

| Component      | `'use client'` | Notes                              |
| -------------- | -------------- | ---------------------------------- |
| Product detail | no             | Data display; SSR-compatible       |
| Product form   | no             | Form display; SSR-compatible shell |

---

## marketplaceProductBooking (9 components — 0 with `'use client'`)

All 9 components are display-layer, no browser API usage.  
**Opportunity**: Product booking flow can render confirmation, pricing, and schedule information server-side. Only the interactive calendar/date picker needs client.

---

## marketplaceProductCard (1 component — 0 with `'use client'`)

| Component          | `'use client'` | Notes                             |
| ------------------ | -------------- | --------------------------------- |
| `product-card.tsx` | no             | Image + text card; SSR-compatible |

Uses `next/image` but no `priority` prop. Missing `sizes` attribute. (See `asset-findings.md`.)

---

## marketplaceProductGuest, marketplaceProductSubscription (display components)

All display-layer. No `'use client'`. SSR-compatible.

`marketplaceProductSubscription`: Subscription pricing, renewal info display — perfect SSR candidate when paired with Relay server-side prefetch.

---

## marketplaceRefund

Display components for refund status and history. No `'use client'`. SSR-compatible.

---

## Payment Components (stripeConnectAccount, bankAccount)

**webapp-spaces only**.

| Component           | `'use client'` | Reason                     | Notes                                                         |
| ------------------- | -------------- | -------------------------- | ------------------------------------------------------------- |
| Stripe Connect form | no (check)     | Stripe.js loads via script | `@stripe/react-stripe-js` (9 KB) handles its own lazy loading |
| Bank account form   | no (check)     | —                          | Form display shell                                            |

**Stripe**: `@stripe/react-stripe-js` is already small (9 KB) and uses its own lazy-load mechanism via Stripe.js CDN script. No action needed here.

---

## carousel (webapp only)

| Component | `'use client'` | Notes                                       |
| --------- | -------------- | ------------------------------------------- |
| Carousel  | likely         | Animation/swipe interaction requires client |

Carousels require event handlers and state for navigation. Correctly needs `'use client'`.

---

## Summary for Booking/Marketplace/Payment

| Category             | Total | Client | Server-compatible | Key Opportunity                    |
| -------------------- | ----- | ------ | ----------------- | ---------------------------------- |
| booking              | 18    | 0      | 18                | Data display = SC                  |
| marketplaceProduct\* | 13    | 0      | 13                | Full SSR when product pages are SC |
| payment (Stripe)     | ~2    | 0      | ~2                | SC with Stripe.js                  |
| carousel             | 1     | 1      | 0                 | Correctly client                   |

**Total**: ~34 components. 1 correctly client. ~33 server-compatible.

**High-value observation**: The booking and marketplace display components represent the bulk of the webapp's UI surface. If product/booking pages became server-rendered (with client islands for interactive booking actions), the initial page HTML would be fully populated — eliminating the spinner/skeleton state that currently appears while React hydrates and Relay queries complete client-side.

**Dependency**: This optimization path requires architectural changes to `AuthKitProvider` scoping (see `relay-queries.md` and `client-boundary-findings.md`).
