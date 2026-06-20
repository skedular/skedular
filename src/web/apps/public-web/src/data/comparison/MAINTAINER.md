# Comparison Hub Maintainer Guide

This guide explains how to extend and maintain the Skedular Competitor Comparison Hub data.

## Overview

The comparison hub is data-driven using static TypeScript modules under `src/data/comparison/`. All comparison pages are generated from shared data, ensuring consistency and making it easy to add new competitors or update existing information.

## Data Structure

### Core Data Modules

- `competitors.ts` - Product definitions for Skedular and competitors
- `competitor-claims.ts` - Claims about competitor capabilities
- `skedular-evidence.ts` - Evidence backing Skedular capabilities
- `feature-support.ts` - Feature support matrix across products
- `faqs.ts` - Frequently asked questions
- `feature-matrix.ts` - Feature categories and normalized feature IDs

### Generation Modules

- `page-targets.ts` - Generates comparison page targets from shared data
- `content-inventory.ts` - Generates content inventory entries
- `validation.ts` - Validation helpers for data integrity

## Adding a New Competitor

### Step 1: Add Competitor Definition

Edit `competitors.ts` and add a new entry to the `competitors` array:

```typescript
{
  id: "new-competitor",
  name: "New Competitor",
  productKind: "competitor",
  website: "https://example.com",
  reviewStatus: "pending", // or "approved" after review
  summary: "Brief summary of the competitor",
  bestFor: "Target audience or use case",
  strengths: ["Strength 1", "Strength 2"],
  limitations: ["Limitation 1", "Limitation 2"],
  pricingNotes: "Pricing model information",
  integrationNotes: "Integration capabilities",
}
```

### Step 2: Add Competitor Claims

Edit `competitor-claims.ts` and add claims about the competitor's capabilities:

```typescript
{
  id: "new-competitor-feature-claim",
  competitorId: "new-competitor",
  featureId: "feature-id-from-feature-matrix",
  state: "supported", // or "partially-supported", "not-supported", "unknown"
  evidenceNote: "Source or justification for this claim",
  reviewStatus: "pending", // or "approved" after review
  publishedPageIds: [], // Leave empty until approved
}
```

### Step 3: Add Feature Support

Edit `feature-support.ts` and add feature support entries:

```typescript
{
  productId: "new-competitor",
  featureId: "feature-id",
  state: "supported",
  notes: "Optional implementation notes",
}
```

### Step 4: Validate Data

Run the validation function to check for errors:

```typescript
import { validateComparisonData } from "./comparison";

const result = validateComparisonData();
console.log(result);
```

### Step 5: Generate Pages

The comparison page will be automatically generated from the shared data. No manual page creation is required.

## Updating Existing Competitor Data

### Updating Claims

To update a claim, modify the relevant entry in `competitor-claims.ts`:

1. Update the `state` if capability support has changed
2. Add or update `evidenceNote` with new sources
3. Update `reviewStatus` after review
4. Add to `publishedPageIds` if approved for publication

### Updating Evidence

To update Skedular evidence, modify `skedular-evidence.ts`:

```typescript
{
  capabilityId: "capability-id",
  supported: true,
  sourceRef: "spec-id-or-help-doc-id",
  sourceLineStart: 1,
  sourceLineEnd: 10,
  contentType: "feature",
  decision: "implemented",
  verificationStatus: "verified",
}
```

## Publication Process

### Evidence Requirements

- **Skedular capabilities**: Must have evidence with a valid `sourceRef` pointing to specs, help docs, or implemented code
- **Competitor claims**: Must have `evidenceNote` OR `reviewStatus: "approved"` before publication

### Publication Status

Products and pages have a `publicationStatus` field:

- `draft` - Not ready for publication
- `reviewed` - Reviewed and ready
- `blocked` - Blocked from publication
- `published` - Published and visible

The "all-or-nothing" publication gate means:

- No comparison pages are published until all required data is complete and validated
- All competitors must have approved review status before their comparison pages can be published

### Review Status

Claims and products have a `reviewStatus` field:

- `pending` - Awaiting review
- `approved` - Approved for publication
- `blocked` - Blocked from publication (e.g., legal concerns)

## Validation

### Running Validation

The `validation.ts` module provides validation helpers:

```typescript
import {
  validateDuplicateIds,
  validateSkedularEvidence,
  validateCompetitorEvidence,
  validateBlockedClaims,
  validateIncompletePublication,
  validateComparisonData,
} from "./comparison";

// Validate specific aspects
const duplicateCheck = validateDuplicateIds(competitors, "competitors");
const evidenceCheck = validateSkedularEvidence(skedularEvidence);

// Validate everything
const fullValidation = validateComparisonData();
```

### Common Validation Errors

- **Duplicate IDs**: Ensure all `id` fields are unique across their respective arrays
- **Missing evidence**: Skedular capabilities must have valid source references
- **Blocked claims published**: Claims with `reviewStatus: "blocked"` must not be in `publishedPageIds`
- **Incomplete publication**: Published products must have `reviewStatus: "approved"`

## Testing

Run the comparison data tests:

```bash
pnpm test public-site-content.test.ts
```

The tests validate:

- Data structure integrity
- Evidence requirements
- Review status enforcement
- Page generation
- Hub linking
- Supporting page generation

## File Organization

```
src/data/comparison/
├── index.ts                    # Barrel export
├── feature-matrix.ts           # Feature categories and normalized features
├── page-paths.ts               # Route path constants
├── support-states.ts           # Support state constants
├── validation.ts               # Validation helpers
├── skedular-evidence.ts        # Skedular capability evidence
├── competitors.ts              # Product definitions
├── competitor-claims.ts        # Competitor capability claims
├── feature-support.ts         # Feature support matrix
├── faqs.ts                     # FAQ entries
├── page-targets.ts             # Comparison page generation
├── content-inventory.ts       # Content inventory generation
└── MAINTAINER.md               # This file
```

## Best Practices

1. **Evidence-based**: Always provide evidence for claims about competitor capabilities
2. **Review process**: Use the review status workflow to ensure accuracy
3. **Validation**: Run validation checks before committing changes
4. **Consistency**: Use the same feature IDs from `feature-matrix.ts` across all modules
5. **Documentation**: Update this guide when adding new data structures or workflows

## Troubleshooting

### Page Not Generating

If a comparison page is not generating:

1. Check that the competitor exists in `competitors.ts`
2. Verify the competitor has `productKind: "competitor"`
3. Ensure the competitor has at least one claim in `competitor-claims.ts`
4. Run validation to check for data integrity issues

### Validation Errors

If validation fails:

1. Check the error message for specific issues
2. Verify all IDs are unique
3. Ensure Skedular evidence has valid source references
4. Check that blocked claims are not published
5. Verify published products have approved review status

### Content Not Appearing

If content is not appearing on generated pages:

1. Check that the relevant data modules are exported from `index.ts`
2. Verify the page target generation logic includes the data
3. Ensure the component renders the data correctly
4. Check the publication status of the relevant products and claims
