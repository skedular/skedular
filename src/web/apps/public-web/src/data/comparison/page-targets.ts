import type {
  ComparisonPageTarget,
  ComparisonProduct,
  CompetitorClaim,
} from "../content-types";
import { competitorClaims } from "./competitor-claims";
import { competitors } from "./competitors";
import { comparisonFAQs } from "./faqs";
import { featureSupport } from "./feature-support";

// Page target generation utilities
// These combine products, claims, feature support, FAQs, and CTAs to generate page targets

// Generate individual comparison page targets for a competitor
export function generateComparisonPageTarget(
  competitorId: string,
): ComparisonPageTarget | null {
  const competitor = competitors.find((c) => c.id === competitorId);
  if (!competitor || competitor.productKind !== "competitor") {
    return null;
  }

  const relevantClaims = competitorClaims.filter(
    (c) => c.competitorId === competitorId,
  );
  const competitorFeatureSupport = featureSupport.filter(
    (fs) => fs.productId === competitorId,
  );

  const title =
    competitor.id === "skedda"
      ? "Skedular vs Skedda: Workplace Booking, Coworking, and Workspace Operations Compared"
      : competitor.id === "officernd"
        ? "Skedular vs OfficeRnD: Workplace Management, Hybrid Work, and Coworking Operations Compared"
        : competitor.id === "nexudus"
          ? "Skedular vs Nexudus: Coworking Management, Workplace Booking, and Workspace Operations Compared"
          : competitor.id === "gable"
            ? "Skedular vs Gable: Hybrid Workplace, Workspace Marketplace, and Flexible Workspace Compared"
            : competitor.id === "robin"
              ? "Skedular vs Robin: Hybrid Workplace Management and Workplace Booking Compared"
              : competitor.id === "officely"
                ? "Skedular vs Officely: Hybrid Workplace Management and Slack-Based Desk Booking Compared"
                : competitor.id === "envoy"
                  ? "Skedular vs Envoy: Workplace Management, Visitor Management, and Workplace Experience Compared"
                  : competitor.id === "kadence"
                    ? "Skedular vs Kadence: Hybrid Workplace Management and Workplace Analytics Compared"
                    : competitor.id === "archie"
                      ? "Skedular vs Archie: Coworking Management and Flexible Workspace Software Compared"
                      : competitor.id === "deskbird"
                        ? "Skedular vs deskbird: Hybrid Workplace Management and Workplace Booking Compared"
                        : `Skedular vs ${competitor.name} | Workspace booking and operations`;

  return {
    id: `skedular-vs-${competitor.slug}`,
    slug: `skedular-vs-${competitor.slug}`,
    path: `/compare/skedular-vs-${competitor.slug}`,
    pageType: "competitor-comparison",
    competitorId: competitor.id,
    title,
    description: `Compare Skedular's workspace discovery, Teams, Spaces, payments, and operator workflows with ${competitor.name} alternatives.`,
    overview: `Compare Skedular with ${competitor.name} to understand the differences in workspace booking, management, and operations capabilities.`,
    pricingComparison: generatePricingComparison(competitor, relevantClaims),
    integrationComparison: generateIntegrationComparison(
      competitor,
      relevantClaims,
    ),
    bestFor: competitor.bestFor,
    limitations: competitor.limitations.join(". "),
    whySkedular: generateWhySkedular(competitor, relevantClaims),
    faqIds: getRelevantFAQIds(competitorId),
    primaryCtaId: "book-demo",
    relatedPageIds: ["/compare"],
    publicationStatus: competitor.publicationStatus,
  };
}

// Generate pricing comparison text
function generatePricingComparison(
  competitor: ComparisonProduct,
  claims: CompetitorClaim[],
): string {
  const pricingClaims = claims.filter((c) => c.claimType === "pricing");
  if (pricingClaims.length === 0) {
    return `Pricing comparison for ${competitor.name} will be added after review.`;
  }
  return `Pricing notes: ${competitor.pricingNotes}`;
}

// Generate integration comparison text
function generateIntegrationComparison(
  competitor: ComparisonProduct,
  claims: CompetitorClaim[],
): string {
  const integrationClaims = claims.filter((c) => c.claimType === "integration");
  if (integrationClaims.length === 0) {
    return `Integration comparison for ${competitor.name} will be added after review.`;
  }
  return `Integration notes: ${competitor.integrationNotes}`;
}

// Generate "Why Teams Choose Skedular" text
function generateWhySkedular(
  competitor: ComparisonProduct,
  claims: CompetitorClaim[],
): string {
  const strengths = competitor.strengths;
  return `Skedular advantages: ${strengths.join(". ")}.`;
}

// Get relevant FAQ IDs for a competitor comparison
function getRelevantFAQIds(competitorId: string): string[] {
  // Return general comparison FAQs that apply to all comparisons
  return comparisonFAQs
    .filter((faq) => faq.schemaEligible && faq.reviewStatus === "approved")
    .slice(0, 5)
    .map((faq) => faq.id);
}

// Generate all comparison page targets
export function generateAllComparisonPageTargets(): ComparisonPageTarget[] {
  const targets: ComparisonPageTarget[] = [];

  for (const competitor of competitors) {
    if (competitor.productKind === "competitor") {
      const target = generateComparisonPageTarget(competitor.id);
      if (target) {
        targets.push(target);
      }
    }
  }

  return targets;
}
