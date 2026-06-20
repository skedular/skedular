import { generateAllComparisonPageTargets } from "./comparison";
import type { ComparisonPage } from "./content-types";

// Generate comparison pages from shared data
// This replaces the legacy one-off comparison implementation with no redirect or alias
// Full comparison pages are generated from evidence-based shared data

const generatedTargets = generateAllComparisonPageTargets();

export const comparisonPages: ComparisonPage[] = generatedTargets.map(
  (target) => ({
    id: target.id,
    slug: target.slug,
    path: target.path,
    competitorName:
      target.competitorId.charAt(0).toUpperCase() +
      target.competitorId.slice(1), // Capitalize competitor ID
    title: target.title,
    description: target.description,
    searchIntent: "Visitors comparing workspace booking software.",
    skedularPositioning: target.whySkedular,
    claimList: [
      target.overview,
      target.pricingComparison,
      target.integrationComparison,
    ],
    primaryCtaId: target.primaryCtaId,
    metadataStatus:
      target.publicationStatus === "published" ? "published" : "drafted",
    competitorReviewStatus: "pending",
  }),
);
