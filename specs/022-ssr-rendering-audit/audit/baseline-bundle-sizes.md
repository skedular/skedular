# Baseline Bundle Sizes

**Generated**: 2026-06-03  
**Method**: `ANALYZE=true pnpm build --webpack` per app (Next.js 16.2.7, Webpack mode)  
**Tool**: `@next/bundle-analyzer` (webpack-bundle-analyzer treemap) — client.html parsed  
**Note**: Next.js 16 webpack build does not emit per-route first-load sizes in CLI output;  
 package-level sizes are extracted from the bundle analyzer treemap.  
 Per-route sizes require Turbopack (`pnpm build`) with `route-bundle-stats.json` — see § Per-Route below.

---

## Summary — Client Bundle Totals

| App           | Parsed (KB) | Gzip (KB) |
| ------------- | ----------- | --------- |
| webapp        | 4,057       | 1,218     |
| webapp-teams  | 5,128       | 1,455     |
| webapp-spaces | 5,428       | 1,517     |

> All three apps share virtually identical shared-chunk contents; the size difference is primarily application code (`app-code`).

---

## webapp — Client Bundle Packages (≥ 5 KB parsed)

| Package                             | Parsed (KB) | Gzip (KB) | Notes                               |
| ----------------------------------- | ----------- | --------- | ----------------------------------- |
| react-svg-credit-card-payment-icons | **521**     | **180**   | 🔴 Critical — lazy-load candidate   |
| app-code                            | 276         | 63        | App components + Relay artifacts    |
| next (framework)                    | 195         | 61        | —                                   |
| react-dom                           | 170         | 53        | —                                   |
| leaflet                             | 144         | 41        | 🟡 Map only — lazy-load candidate   |
| next/dist                           | 122         | 43        | —                                   |
| @mui/x-data-grid                    | 119         | 33        | 🟡 Lazy-load candidate              |
| relay-runtime                       | 90          | 26        | Expected — always needed            |
| @mui/material (split)               | 68+39 = 107 | 37        | MUI core                            |
| logrocket                           | 61          | 15        | 🟡 Could defer                      |
| node-ipinfo/dist                    | 41          | 9         | 🔴 Unexpected in client bundle      |
| yup                                 | 35          | 10        | Form validation                     |
| countries-list                      | 33          | 10        | 🟡 Consider tree-shaking            |
| leaflet.markercluster               | 33          | 8         | Map — lazy-load with leaflet        |
| react-toastify                      | 32          | 9         | —                                   |
| buffer                              | 23          | 6         | 🟡 Node polyfill — needed?          |
| react-relay                         | 21          | 7         | Expected                            |
| @mui/x-date-pickers                 | 21          | 6         | —                                   |
| @mui/x-virtualizer                  | 20          | 6         | —                                   |
| final-form                          | 18          | 5         | Form lib                            |
| lru-cache                           | 16          | 4         | 🟡 Node lib — needed in browser?    |
| react-image-crop                    | 14          | 4         | —                                   |
| react-final-form                    | 10          | 3         | —                                   |
| react                               | 7           | 2         | —                                   |
| pino                                | 6           | 2         | 🟡 Node logger — needed in browser? |
| dayjs                               | 6           | 2         | —                                   |
| graphql-sse                         | 6           | 2         | —                                   |

---

## webapp-teams — Client Bundle Packages (≥ 5 KB parsed)

| Package                             | Parsed (KB) | Gzip (KB) | Notes                             |
| ----------------------------------- | ----------- | --------- | --------------------------------- |
| react-svg-credit-card-payment-icons | **521**     | **180**   | 🔴 Critical — lazy-load candidate |
| app-code                            | 460         | 109       | Larger than webapp (+184K)        |
| next (framework)                    | 195         | 61        | —                                 |
| react-dom                           | 170         | 53        | —                                 |
| next/dist                           | 122         | 43        | —                                 |
| @mui/x-data-grid                    | 119         | 33        | 🟡 Lazy-load candidate            |
| relay-runtime                       | 90          | 26        | Expected                          |
| @azure/msal-browser                 | **72**      | **18**    | 🟡 Teams SSO — needed everywhere? |
| @mui/material                       | 63+39 = 102 | 35        | MUI core                          |
| logrocket                           | 61          | 15        | 🟡 Could defer                    |
| @azure/msal-common                  | **51**      | **13**    | 🟡 Teams SSO dep                  |
| yup                                 | 35          | 10        | —                                 |
| countries-list                      | 33          | 10        | 🟡 Consider tree-shaking          |
| react-toastify                      | 32          | 9         | —                                 |
| buffer                              | 23          | 6         | —                                 |
| react-relay                         | 21          | 7         | Expected                          |
| @mui/x-virtualizer                  | 20          | 6         | —                                 |
| final-form                          | 18          | 5         | —                                 |
| @mui/x-date-pickers                 | 16          | 5         | —                                 |
| react-image-crop                    | 14          | 4         | —                                 |
| @mui/x-date-pickers-pro             | 11          | 3         | —                                 |
| react-final-form                    | 10          | 3         | —                                 |
| @stripe/react-stripe-js             | 9           | 3         | —                                 |
| react                               | 7           | 2         | —                                 |
| dayjs                               | 6           | 2         | —                                 |
| graphql-sse                         | 6           | 2         | —                                 |
| @microsoft/teamsfx                  | 5           | 1         | —                                 |

---

## webapp-spaces — Client Bundle Packages (≥ 5 KB parsed)

| Package                             | Parsed (KB) | Gzip (KB) | Notes                              |
| ----------------------------------- | ----------- | --------- | ---------------------------------- |
| app-code                            | **602**     | **138**   | Largest app-code of the three      |
| react-svg-credit-card-payment-icons | **521**     | **180**   | 🔴 Critical — lazy-load candidate  |
| next (framework)                    | 195         | 61        | —                                  |
| react-dom                           | 170         | 53        | —                                  |
| next/dist                           | 122         | 43        | —                                  |
| @mui/x-data-grid                    | 119         | 33        | 🟡 Lazy-load candidate             |
| relay-runtime                       | 90          | 26        | Expected                           |
| @azure/msal-browser                 | **72**      | **18**    | 🟡 Spaces SSO — needed everywhere? |
| @mui/material                       | 63+39 = 102 | 35        | MUI core                           |
| logrocket                           | 61          | 15        | 🟡 Could defer                     |
| @azure/msal-common                  | **51**      | **13**    | 🟡 SSO dep                         |
| yup                                 | 35          | 10        | —                                  |
| countries-list                      | 33          | 10        | 🟡 Consider tree-shaking           |
| react-toastify                      | 32          | 9         | —                                  |
| buffer                              | 23          | 6         | —                                  |
| react-relay                         | 21          | 7         | Expected                           |
| @mui/x-virtualizer                  | 20          | 6         | —                                  |
| final-form                          | 18          | 5         | —                                  |
| @mui/x-date-pickers                 | 16          | 5         | —                                  |
| react-image-crop                    | 14          | 4         | —                                  |
| @mui/x-date-pickers-pro             | 11          | 3         | —                                  |
| react-final-form                    | 10          | 3         | —                                  |
| @stripe/react-stripe-js             | 9           | 3         | —                                  |
| react                               | 7           | 2         | —                                  |
| dayjs                               | 6           | 2         | —                                  |
| graphql-sse                         | 6           | 2         | —                                  |
| @microsoft/teamsfx                  | 5           | 1         | —                                  |

---

## Per-Route First-Load JS Sizes

> **Note**: Next.js 16 webpack build does not emit per-route sizes in CLI output.  
> Per-route sizes are available from **Turbopack** builds via `.next/diagnostics/route-bundle-stats.json`.  
> A Turbopack build was run for webapp on 2026-06-03; the analytics directory was overwritten by the
> subsequent webpack `ANALYZE=true` build. Re-run `pnpm build` (Turbopack, no `--webpack`) to regenerate.
>
> **All routes are `ƒ` (Dynamic / server-rendered on demand)** — no static routes confirmed.
>
> webapp routes (43 total): `/`, `/_not-found`, `/api/*` (8), `/auth/signin`, `/auth/signup`,  
> `/callback`, `/install-slack`, `/marketplace/*` (20), `/notifications`, `/settings`, `/signin`,  
> `/signup`, `/slack-success-install`, `/welcome`
>
> webapp-teams routes (~55 total): enumerated from build output (not captured in this run)
>
> webapp-spaces routes (~60 total): enumerated from build output (not captured in this run)

---

## Lighthouse Baselines (T010b) — Pending

> Lighthouse baselines require running app servers. Run each app with `pnpm dev`, then:
>
> ```
> npx lighthouse http://localhost:3000 --preset=mobile --output=json --output-path=./audit/lighthouse-webapp.json
> npx lighthouse http://localhost:3001 --preset=mobile --output=json --output-path=./audit/lighthouse-webapp-teams.json
> npx lighthouse http://localhost:3002 --preset=mobile --output=json --output-path=./audit/lighthouse-webapp-spaces.json
> ```
>
> Record 3-run averages for: LCP, FCP, CLS, TBT, Performance Score.
> Baseline table to be added here after T010b execution.

---

## Key Findings for Downstream Audit Tasks

### 🔴 Critical (> 100 KB, clearly lazy-loadable)

| Package                             | Parsed | Affected Apps | Opportunity                                      |
| ----------------------------------- | ------ | ------------- | ------------------------------------------------ |
| react-svg-credit-card-payment-icons | 521K   | all 3         | Lazy-load — only needed on payment/booking pages |
| leaflet + leaflet.markercluster     | 177K   | webapp only   | Lazy-load — only needed on map pages             |
| @azure/msal-browser + msal-common   | 123K   | teams, spaces | Lazy-load — only needed on auth-gated pages      |

### 🟡 Medium (10–100 KB, investigate usage breadth)

| Package          | Parsed | Affected Apps | Opportunity                                          |
| ---------------- | ------ | ------------- | ---------------------------------------------------- |
| @mui/x-data-grid | 119K   | all 3         | Lazy-load — only needed on data grid pages           |
| logrocket        | 61K    | all 3         | Defer init after page load                           |
| node-ipinfo/dist | 41K    | webapp        | 🚨 Should not be in client bundle — server-side only |
| countries-list   | 33K    | all 3         | Check if full list is needed or can be tree-shaken   |
| buffer           | 23K    | all 3         | Node polyfill — verify if still needed               |
| lru-cache        | 16K    | webapp        | Node lib — should this be in client bundle?          |
| pino             | 6K     | webapp        | Node logger — should not be in client bundle         |

### ℹ️ Context

- **app-code** (276–602K): All application components. High client-boundary saturation expected based on research.md.  
  Breakdown requires route-level analysis (T011–T019).
- **relay-runtime** (90K): Expected — Relay operates client-side due to AuthKit/WorkOS client-only constraint.
- Total client bundle: 4–5.4 MB parsed / 1.2–1.5 MB gzip — in line with complex data-heavy SaaS apps but above  
  Next.js recommended 130 KB first-load target.
