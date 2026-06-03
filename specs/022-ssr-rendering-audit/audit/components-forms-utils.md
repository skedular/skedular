# Component Audit — Forms, Utilities, and Generic Components

**Generated**: 2026-06-03  
**Task**: T019  
**Scope**: `forms`, `datePickers`, `sorting`, `listGridToggle`, `listingMetadata`, `address`, `contactEmail`, `contactPeople`, `contactPhone`, `icons`, `avatars`, `links`, `slackButtons`, `closedOpenAllDayCustomToggle`, `weekOpeningHours`

---

## forms (2 components — 0 with `'use client'`)

| Component              | `'use client'` | Notes                                                  |
| ---------------------- | -------------- | ------------------------------------------------------ |
| Form field wrapper     | no             | MUI form field — React Hook Form or MUI-RFF controlled |
| Generic form container | no             | Form shell                                             |

**Assessment**: Form display shells are SSR-compatible. Form submission logic (event handlers) will need `'use client'` at the point of interactivity. Using React Server Actions for form submission could eliminate the need for `'use client'` on entire form pages.

---

## datePickers (3 components — 0 with `'use client'`)

| Component         | `'use client'`        | Notes                                 |
| ----------------- | --------------------- | ------------------------------------- |
| Date picker       | no (but needs client) | MUI date pickers require client state |
| Time picker       | no                    | Same                                  |
| Date range picker | no                    | Same                                  |

**Finding**: These components lack `'use client'` but REQUIRE client-side rendering (MUI date pickers use browser state, event handlers, and potentially `useEffect` for localization). The absence of `'use client'` here means they'll work in a client context (pages declare it) but would break if their parent became a Server Component.

**Action**: Add `'use client'` to date picker components to make the boundary explicit. This prevents accidental SSR breakage if parent pages are later converted to server components.

---

## address, contactEmail, contactPeople, contactPhone (utility components)

All display-layer. No `'use client'`. SSR-compatible.  
These render contact information — pure data display with no interactivity.

---

## icons (via `@skedular/ui` commons)

The icon components in `@skedular/ui` currently include `CreditCard` in the commons barrel export.  
See `lazy-load-candidates.md` → LL-001 for the barrel contamination fix.

Other icon components are SVG wrappers — SSR-compatible, no browser API needed.

---

## avatars, links, listGridToggle, listingMetadata, sorting

| Component         | `'use client'`    | Notes                                                  |
| ----------------- | ----------------- | ------------------------------------------------------ |
| Avatars           | no                | Display only; SSR-compatible                           |
| Links             | no                | Anchor wrappers; SSR-compatible                        |
| `listGridToggle`  | no (needs client) | View mode toggle — needs state for grid/list switching |
| `listingMetadata` | no                | Display; SSR-compatible                                |
| `sorting`         | no (needs client) | Sort direction — needs state                           |

**Finding**: `listGridToggle` and `sorting` components manage UI state (grid vs list view, sort order) — they need `'use client'` to function correctly. Should be tagged explicitly.

---

## slackButtons (webapp only)

| Component            | `'use client'` | Notes                               |
| -------------------- | -------------- | ----------------------------------- |
| Slack install button | no             | External link; SSR-compatible       |
| Slack auth button    | no             | OAuth redirect link; SSR-compatible |

---

## closedOpenAllDayCustomToggle, weekOpeningHours

| Component                      | `'use client'`    | Notes                                   |
| ------------------------------ | ----------------- | --------------------------------------- |
| `closedOpenAllDayCustomToggle` | no (needs client) | Toggle UI state — needs state           |
| `weekOpeningHours`             | no (needs client) | Multi-day schedule editor — needs state |

These are clearly interactive components that need `'use client'`. They're currently missing the directive.

---

## Summary for Forms/Utils/Generic

| Category                     | Total | Has `'use client'` | Needs `'use client'`          | SSR-compatible     |
| ---------------------------- | ----- | ------------------ | ----------------------------- | ------------------ |
| forms                        | 2     | 0                  | 0 (submit via Server Action?) | Yes                |
| datePickers                  | 3     | 0                  | **3**                         | No — add directive |
| contact/address utils        | ~6    | 0                  | 0                             | Yes                |
| icons                        | many  | 0                  | 0                             | Yes                |
| avatars, links               | ~4    | 0                  | 0                             | Yes                |
| listGridToggle               | 1     | 0                  | **1**                         | No — add directive |
| sorting                      | 1     | 0                  | **1**                         | No — add directive |
| weekOpeningHours             | 1     | 0                  | **1**                         | No — add directive |
| closedOpenAllDayCustomToggle | 1     | 0                  | **1**                         | No — add directive |
| slackButtons                 | 2     | 0                  | 0                             | Yes                |

**Hidden risk**: 7 components are missing `'use client'` but require client-side rendering (state, event handlers, MUI interactive components). While they work correctly today (parent pages declare `'use client'`), they would silently break if parent pages are converted to Server Components during Phase 2.

**Recommendation**: Add `'use client'` to the 7 interactive components before beginning any page-level SSR conversion. This makes the client/server boundary explicit and prevents regression.
