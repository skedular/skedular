# Asset Optimization Findings

**Generated**: 2026-06-03  
**Covers**: T021 (Images) + T022 (Fonts)

---

## Image Audit (T021)

### Summary

| Metric                                           | Count                        |
| ------------------------------------------------ | ---------------------------- |
| Files using `next/image`                         | 41                           |
| Files using raw `<img>`                          | 12                           |
| Images with `priority` prop                      | 1                            |
| Images missing `priority` (potential LCP impact) | ~40                          |
| Images with `sizes` attribute                    | To be verified per component |

### Images WITH `priority` prop

| File                                    | Component     | Notes                                           |
| --------------------------------------- | ------------- | ----------------------------------------------- |
| `@skedular/ui: app-shell-layout.tsx:55` | Skedular logo | `priority` correctly set on above-the-fold logo |

### Images MISSING `priority` prop — LCP Candidates

All 41 `next/image` usage locations lack `priority`. Most of these are below-the-fold or conditional (loaded after auth), so missing `priority` may be expected. However, the following are likely above-the-fold and would benefit from `priority`:

| Component                                           | Route(s)                       | Image Content   | Priority Needed?            |
| --------------------------------------------------- | ------------------------------ | --------------- | --------------------------- |
| `appBar/no-organization-app-bar.tsx`                | all                            | Logo/branding   | **Yes** — always above fold |
| `appBar/unauthenticated-app-bar.tsx`                | auth routes                    | Logo/branding   | **Yes**                     |
| `components/auth/custom-organization-auth-page.tsx` | `/auth/signin`, `/auth/signup` | Org logo        | **Yes** — LCP candidate     |
| `organization-admin-setup-section.tsx`              | admin pages                    | Org setup image | Conditional                 |
| `location-card.tsx`                                 | location lists                 | Location photo  | Below fold                  |
| `product-card.tsx`                                  | product lists                  | Product photo   | Conditional                 |
| `packages/ui: app-shell-layout.tsx`                 | all (shared)                   | App logo        | Already has `priority` ✓    |

**Estimated LCP impact**: Adding `priority` to app bar logos and auth page org images could reduce LCP by 200–500ms (eliminates render-blocking resource wait for above-fold images).

### Raw `<img>` Tags — Optimization Targets

12 files use raw `<img>` instead of `next/image`. Raw `<img>` tags:

- Do not get automatic WebP/AVIF conversion
- Do not get lazy loading
- Do not get `srcset` optimization
- May contribute to CLS if no `width`/`height` specified

| Component                                                | Raw `<img>` Usage               | Impact                 |
| -------------------------------------------------------- | ------------------------------- | ---------------------- |
| `marketplace-location.tsx`                               | Location photos in map popup    | Potentially above fold |
| `add-floor-plan.tsx` (×3 apps)                           | Floor plan image upload preview | Admin only, below fold |
| `edit-floor-plan.tsx` (×3 apps)                          | Floor plan image edit preview   | Admin only, below fold |
| `floor-plans.tsx` (×3 apps)                              | Floor plan thumbnail list       | Admin only             |
| `@skedular/shared: image-file-uploader.tsx`              | Upload preview                  | UI component           |
| `@skedular/shared: image-file-uploader-with-cropper.tsx` | Crop preview                    | UI component           |

**Note**: Image uploaders and crop previews (`@skedular/shared`) use raw `<img>` correctly — they display user-uploaded blob URLs where `next/image` doesn't apply. These can remain as-is.

**Actionable target**: `marketplace-location.tsx` uses raw `<img>` for location map popup images — these are user-generated photos, potentially the LCP element on the `/marketplace/locations/[locationId]` route. Replace with `next/image` with `width`/`height`.

### Missing `sizes` Attributes

`next/image` components without explicit `sizes` default to full viewport width for `srcset`, which generates unnecessarily large image variants. Review all 41 files; key candidates:

- `location-card.tsx` — card images (typically ~300px wide in grid)
- `product-card.tsx` — product card images
- `team-card.tsx` — team avatar images

**Estimated impact**: `sizes` optimization reduces bandwidth by ~30–60% for card images.

---

## Font Audit (T022)

### Current Font Configuration

All three apps use the same font stack via `localFont` in `layout.tsx`:

```typescript
// Inter — variable font, woff2 ✓
const inter = localFont({
  src: "./fonts/InterVariable.woff2", // 344 KB
  variable: "--font-inter",
  display: "swap", // ✓ FOUT handled
  adjustFontFallback: false, // ⚠️ CLS risk
});

// Barlow — 4 TTF files ❌
const barlow = localFont({
  src: [
    { path: "./fonts/Barlow-Regular.ttf", weight: "400" }, // 104 KB
    { path: "./fonts/Barlow-Medium.ttf", weight: "500" }, // 104 KB
    { path: "./fonts/Barlow-SemiBold.ttf", weight: "600" }, // 108 KB
    { path: "./fonts/Barlow-Bold.ttf", weight: "700" }, // 108 KB
  ],
  variable: "--font-barlow",
  display: "swap", // ✓ FOUT handled
  adjustFontFallback: false, // ⚠️ CLS risk
});
```

### Font File Summary

| Font             | Format  | Size   | Issues                                                                  |
| ---------------- | ------- | ------ | ----------------------------------------------------------------------- |
| InterVariable    | woff2   | 344 KB | Format is fine; 344 KB is large for variable font — consider subsetting |
| Barlow-Regular   | **TTF** | 104 KB | 🔴 TTF is not optimal for web; woff2 ~25% smaller (~78 KB)              |
| Barlow-Medium    | **TTF** | 104 KB | 🔴 Same                                                                 |
| Barlow-SemiBold  | **TTF** | 108 KB | 🔴 Same                                                                 |
| Barlow-Bold      | **TTF** | 108 KB | 🔴 Same                                                                 |
| **Total Barlow** | TTF     | 424 KB | → Estimated woff2: ~318 KB (save ~106 KB)                               |

### Findings

#### F-001: Barlow font in TTF format (affects all 3 apps)

**Severity**: Medium  
**Impact**: ~106 KB extra download (424 KB TTF vs ~318 KB woff2 estimated)  
**FCP impact**: TTF browsers display fallback font during download; woff2 downloads faster, reducing FOUT duration  
**Fix**: Convert `Barlow-*.ttf` to `Barlow-*.woff2` using `woff2_compress` or Fontsquirrel, update `localFont` config  
**Applies to**: All 3 apps (same fonts, same size)

#### F-002: `adjustFontFallback: false` on both fonts (affects all 3 apps)

**Severity**: Low  
**Impact**: With `adjustFontFallback: false`, Next.js does NOT generate a size-adjusted system font fallback. This means layout shifts (CLS) may occur during font swap (fallback → web font). Next.js default (`true`) would inject a CSS fallback that closely matches metrics of the target font, minimizing CLS.  
**Fix**: Remove `adjustFontFallback: false` OR manually provide a `fallback: ['system-ui']` with CSS-adjusted metrics  
**Note**: Was `false` intentionally to avoid fallback-font render issues — verify no visual regression before changing.

#### F-003: InterVariable at 344 KB — subsetting opportunity

**Severity**: Low  
**Impact**: Variable font covers the entire Unicode range. If only Latin characters are needed (likely for a SaaS platform), subsetting to Latin reduces file size significantly (estimate: 60–80% reduction → ~70–140 KB)  
**Fix**: Use `next/font/local` with `unicode-range` CSS OR pre-subset via `pyftsubset` or online tool  
**Note**: Investigate whether any non-Latin characters are displayed (org names, user names could include non-Latin).

#### F-004: `display: 'swap'` present on both fonts ✓

**Severity**: N/A (positive finding)  
Both fonts correctly use `display: 'swap'` which ensures text remains visible during font load (no invisible text = good FCP). No change needed.

#### F-005: Font loaded in Server Component (`layout.tsx`) ✓

**Severity**: N/A (positive finding)  
`localFont` is called in `layout.tsx` which is a Server Component (it has no `'use client'`). This is the correct pattern — fonts load via `<link rel="preload">` in the HTML head. No change needed.

### Font Optimization Summary

| Finding                            | Fix                 | Est. Saving     | Effort       |
| ---------------------------------- | ------------------- | --------------- | ------------ |
| F-001: Barlow TTF → woff2          | Convert 4 files     | ~106 KB         | Low          |
| F-002: `adjustFontFallback: false` | Remove flag         | CLS improvement | Low (verify) |
| F-003: Inter subsetting            | Unicode-range Latin | ~200 KB         | Medium       |

**Total estimated font saving**: ~300 KB on initial page load if all three applied.
