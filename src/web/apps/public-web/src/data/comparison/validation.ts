import type {
  ComparisonProduct,
  CompetitorClaim,
  SkedularCapabilityEvidence,
  FeatureSupport,
  ComparisonPageTarget,
  SupportingPageTarget,
  FAQEntry,
} from "../content-types";
import { REQUIRED_COMPARISON_PATHS, COMPARE_HUB_PATH } from "./page-paths";

// Validation result interface
export interface ValidationResult {
  isValid: boolean;
  errors: string[];
  warnings: string[];
}

// Validate duplicate IDs
export const validateDuplicateIds = (
  items: Array<{ id: string }>,
  itemType: string,
): ValidationResult => {
  const ids = items.map((item) => item.id);
  const duplicates = ids.filter((id, index) => ids.indexOf(id) !== index);
  const uniqueDuplicates = [...new Set(duplicates)];

  if (uniqueDuplicates.length === 0) {
    return { isValid: true, errors: [], warnings: [] };
  }

  return {
    isValid: false,
    errors: [`Duplicate ${itemType} IDs found: ${uniqueDuplicates.join(", ")}`],
    warnings: [],
  };
};

// Validate duplicate slugs
export const validateDuplicateSlugs = (
  items: Array<{ slug: string }>,
  itemType: string,
): ValidationResult => {
  const slugs = items.map((item) => item.slug);
  const duplicates = slugs.filter(
    (slug, index) => slugs.indexOf(slug) !== index,
  );
  const uniqueDuplicates = [...new Set(duplicates)];

  if (uniqueDuplicates.length === 0) {
    return { isValid: true, errors: [], warnings: [] };
  }

  return {
    isValid: false,
    errors: [
      `Duplicate ${itemType} slugs found: ${uniqueDuplicates.join(", ")}`,
    ],
    warnings: [],
  };
};

// Validate duplicate paths
export const validateDuplicatePaths = (
  items: Array<{ path: string }>,
  itemType: string,
): ValidationResult => {
  const paths = items.map((item) => item.path);
  const duplicates = paths.filter(
    (path, index) => paths.indexOf(path) !== index,
  );
  const uniqueDuplicates = [...new Set(duplicates)];

  if (uniqueDuplicates.length === 0) {
    return { isValid: true, errors: [], warnings: [] };
  }

  return {
    isValid: false,
    errors: [
      `Duplicate ${itemType} paths found: ${uniqueDuplicates.join(", ")}`,
    ],
    warnings: [],
  };
};

// Validate required routes exist
export const validateRequiredRoutes = (
  pageTargets: Array<{ path: string }>,
): ValidationResult => {
  const existingPaths = new Set(pageTargets.map((p) => p.path));
  const missingPaths = REQUIRED_COMPARISON_PATHS.filter(
    (path) => path !== COMPARE_HUB_PATH && !existingPaths.has(path),
  );

  if (missingPaths.length === 0) {
    return { isValid: true, errors: [], warnings: [] };
  }

  return {
    isValid: false,
    errors: [`Missing required comparison routes: ${missingPaths.join(", ")}`],
    warnings: [],
  };
};

// Validate Skedular evidence requirements
export const validateSkedularEvidence = (
  evidence: SkedularCapabilityEvidence[],
): ValidationResult => {
  const errors: string[] = [];
  const warnings: string[] = [];

  for (const ev of evidence) {
    // Check that supported/partially-supported states have current source references
    if (
      (ev.supportState === "supported" ||
        ev.supportState === "partially-supported") &&
      ev.sourceFreshness !== "current"
    ) {
      errors.push(
        `Skedular evidence ${ev.id} has support state "${ev.supportState}" but source freshness is "${ev.sourceFreshness}" (must be "current")`,
      );
    }

    // Check that supported/partially-supported states have at least one source reference
    if (
      (ev.supportState === "supported" ||
        ev.supportState === "partially-supported") &&
      ev.sourceRefs.length === 0
    ) {
      errors.push(
        `Skedular evidence ${ev.id} has support state "${ev.supportState}" but no source references`,
      );
    }

    // Warn about outdated evidence
    if (ev.sourceFreshness === "outdated") {
      warnings.push(
        `Skedular evidence ${ev.id} has outdated source references`,
      );
    }
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings,
  };
};

// Validate competitor evidence/review status requirements
export const validateCompetitorEvidence = (
  claims: CompetitorClaim[],
): ValidationResult => {
  const errors: string[] = [];
  const warnings: string[] = [];

  for (const claim of claims) {
    // Published claims require evidence note or approved review status
    if (
      claim.publishedPageIds.length > 0 &&
      claim.evidenceNote === "" &&
      claim.reviewStatus !== "approved"
    ) {
      errors.push(
        `Competitor claim ${claim.id} is published but has no evidence note and review status is "${claim.reviewStatus}" (must have evidence or be approved)`,
      );
    }

    // Blocked claims must not be published
    if (claim.reviewStatus === "blocked" && claim.publishedPageIds.length > 0) {
      errors.push(
        `Competitor claim ${claim.id} is blocked but is published to pages: ${claim.publishedPageIds.join(", ")}`,
      );
    }
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings,
  };
};

// Validate blocked claims are not rendered
export const validateBlockedClaims = (
  claims: CompetitorClaim[],
): ValidationResult => {
  const blockedPublishedClaims = claims.filter(
    (claim) =>
      claim.reviewStatus === "blocked" && claim.publishedPageIds.length > 0,
  );

  if (blockedPublishedClaims.length === 0) {
    return { isValid: true, errors: [], warnings: [] };
  }

  return {
    isValid: false,
    errors: [
      `Blocked claims are published: ${blockedPublishedClaims.map((c) => c.id).join(", ")}`,
    ],
    warnings: [],
  };
};

// Validate incomplete publication
export const validateIncompletePublication = (
  products: ComparisonProduct[],
  pageTargets: Array<{ publicationStatus: string }>,
): ValidationResult => {
  const errors: string[] = [];
  const warnings: string[] = [];

  // Check that published competitors have approved review status
  for (const product of products) {
    if (
      product.publicationStatus === "published" &&
      product.reviewStatus !== "approved"
    ) {
      errors.push(
        `Product ${product.id} is published but review status is "${product.reviewStatus}" (must be "approved")`,
      );
    }
  }

  // Check that all required pages are published (not draft or blocked)
  const unpublishedPages = pageTargets.filter(
    (p) => p.publicationStatus !== "published",
  );
  if (unpublishedPages.length > 0) {
    warnings.push(
      `Some comparison pages are not published: ${unpublishedPages.length} pages have status other than "published"`,
    );
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings,
  };
};

// Validate feature support completeness
export const validateFeatureSupportCompleteness = (
  featureSupport: FeatureSupport[],
  productIds: string[],
  featureIds: string[],
): ValidationResult => {
  const errors: string[] = [];
  const warnings: string[] = [];

  // Check that every product-feature combination has a support entry
  for (const productId of productIds) {
    for (const featureId of featureIds) {
      const hasEntry = featureSupport.some(
        (fs) => fs.productId === productId && fs.featureId === featureId,
      );
      if (!hasEntry) {
        warnings.push(
          `Missing feature support entry for product ${productId} and feature ${featureId}`,
        );
      }
    }
  }

  return {
    isValid: errors.length === 0,
    errors,
    warnings,
  };
};

// Run all comparison data validations
export const validateComparisonData = (
  products: ComparisonProduct[],
  claims: CompetitorClaim[],
  evidence: SkedularCapabilityEvidence[],
  featureSupport: FeatureSupport[],
  pageTargets: (ComparisonPageTarget | SupportingPageTarget)[],
): ValidationResult => {
  const allErrors: string[] = [];
  const allWarnings: string[] = [];

  // Validate duplicate IDs
  const productIdsResult = validateDuplicateIds(products, "product");
  allErrors.push(...productIdsResult.errors);
  allWarnings.push(...productIdsResult.warnings);

  const claimIdsResult = validateDuplicateIds(claims, "claim");
  allErrors.push(...claimIdsResult.errors);
  allWarnings.push(...claimIdsResult.warnings);

  // Validate duplicate slugs
  const productSlugsResult = validateDuplicateSlugs(products, "product");
  allErrors.push(...productSlugsResult.errors);
  allWarnings.push(...productSlugsResult.warnings);

  // Validate duplicate paths
  const pagePathsResult = validateDuplicatePaths(pageTargets, "page");
  allErrors.push(...pagePathsResult.errors);
  allWarnings.push(...pagePathsResult.warnings);

  // Validate required routes
  const routesResult = validateRequiredRoutes(pageTargets);
  allErrors.push(...routesResult.errors);
  allWarnings.push(...routesResult.warnings);

  // Validate Skedular evidence
  const evidenceResult = validateSkedularEvidence(evidence);
  allErrors.push(...evidenceResult.errors);
  allWarnings.push(...evidenceResult.warnings);

  // Validate competitor evidence
  const competitorEvidenceResult = validateCompetitorEvidence(claims);
  allErrors.push(...competitorEvidenceResult.errors);
  allWarnings.push(...competitorEvidenceResult.warnings);

  // Validate blocked claims
  const blockedClaimsResult = validateBlockedClaims(claims);
  allErrors.push(...blockedClaimsResult.errors);
  allWarnings.push(...blockedClaimsResult.warnings);

  // Validate incomplete publication
  const publicationResult = validateIncompletePublication(
    products,
    pageTargets,
  );
  allErrors.push(...publicationResult.errors);
  allWarnings.push(...publicationResult.warnings);

  return {
    isValid: allErrors.length === 0,
    errors: allErrors,
    warnings: allWarnings,
  };
};
