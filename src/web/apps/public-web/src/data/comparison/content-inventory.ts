import type { ContentInventoryEntry } from "../content-types";
import { generateAllComparisonPageTargets } from "./page-targets";
import { competitors } from "./competitors";
import { featureCategories, normalizedFeatures } from "./feature-matrix";
import { comparisonFAQs } from "./faqs";
import { skedularCapabilityEvidence } from "./skedular-evidence";
import { competitorClaims } from "./competitor-claims";
import { featureSupport } from "./feature-support";

// Content inventory generation helpers
// These generate reviewable content inventory entries for generated comparison pages

// Generate content inventory entry for a comparison page
export function generateComparisonContentInventory(
  pageId: string,
): ContentInventoryEntry {
  const pageTarget = generateAllComparisonPageTargets().find(
    (p) => p.id === pageId,
  );
  if (!pageTarget) {
    throw new Error(`Page target not found: ${pageId}`);
  }

  const competitor = competitors.find((c) => c.id === pageTarget.competitorId);
  const relevantClaims = competitorClaims.filter(
    (c) => c.competitorId === pageTarget.competitorId,
  );
  const relevantSupport = featureSupport.filter(
    (fs) => fs.productId === pageTarget.competitorId,
  );
  const relevantFAQs = comparisonFAQs.filter((faq) =>
    pageTarget.faqIds.includes(faq.id),
  );

  return {
    id: `inventory-${pageId}`,
    pageId: pageId,
    sourceDataRefs: [
      `competitor:${pageTarget.competitorId}`,
      ...relevantClaims.map((c) => `claim:${c.id}`),
      ...relevantSupport.map((fs) => `support:${fs.productId}-${fs.featureId}`),
      ...relevantFAQs.map((faq) => `faq:${faq.id}`),
    ],
    metadataStatus:
      pageTarget.publicationStatus === "published" ? "published" : "drafted",
    contentStatus:
      pageTarget.publicationStatus === "published" ? "published" : "drafted",
    reviewNotes:
      competitor?.reviewStatus === "pending"
        ? "Competitor review pending"
        : undefined,
    validationStatus: "pending",
  };
}

// Generate content inventory for all comparison pages
export function generateAllComparisonContentInventory(): ContentInventoryEntry[] {
  const pageTargets = generateAllComparisonPageTargets();
  return pageTargets.map((target) =>
    generateComparisonContentInventory(target.id),
  );
}

// Generate summary of comparison data for review
export function generateComparisonDataSummary() {
  return {
    products: competitors.length,
    claims: competitorClaims.length,
    evidence: skedularCapabilityEvidence.length,
    featureSupport: featureSupport.length,
    faqs: comparisonFAQs.length,
    featureCategories: featureCategories.length,
    normalizedFeatures: normalizedFeatures.length,
    pageTargets: generateAllComparisonPageTargets().length,
  };
}
