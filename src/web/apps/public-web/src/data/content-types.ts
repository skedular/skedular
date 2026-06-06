export type ContentStatus = "inventory" | "drafted" | "reviewed" | "approved" | "published";
export type ReviewStatus = "pending" | "approved" | "rewritten" | "blocked" | "not-required";
export type MigrationDecision = "publish" | "rewrite" | "merge" | "redirect" | "technical-planning" | "future-planning" | "exclude";
export type CtaPurpose = "search" | "book" | "demo" | "login" | "sign-up" | "contact" | "learn-more" | "support" | "community";

export interface Cta {
  id: string;
  label: string;
  purpose: CtaPurpose;
  destinationType: "internal-route" | "public-url" | "email" | "external-community";
  destinationRef: string;
  audience: string;
}

export interface PublicPage {
  id: string;
  path: string;
  title: string;
  description: string;
  audience: string;
  pageType: "home" | "product" | "pricing" | "resource" | "support" | "feature" | "comparison" | "utility";
  sourceRefs: string[];
  primaryCtaId: string;
  secondaryCtaIds?: string[];
  canonicalPath: string;
  metadataStatus: ContentStatus;
  contentStatus: ContentStatus;
  structuredDataTypes: Array<"Organization" | "Product" | "FAQPage" | "BreadcrumbList" | "Article">;
  requiresClaimReview?: boolean;
  requiresCompetitorReview?: boolean;
  requiresPricingReview?: boolean;
}

export interface ProductPageContent {
  id: "teams" | "spaces";
  eyebrow: string;
  title: string;
  summary: string;
  audience: string;
  sections: Array<{
    title: string;
    body: string;
    items: string[];
  }>;
  integrationActions?: Array<{ type: "slack" }>;
}

export interface ResourceArticle {
  id: string;
  slug: string;
  sourceUrl: string;
  destinationPath: string;
  title: string;
  summary: string;
  publishedDate: string;
  topicTags: string[];
  migrationDecision: MigrationDecision;
  contentStatus: ContentStatus;
  claimReviewStatus: ReviewStatus;
  body: string[];
}

export interface ComparisonPage {
  id: string;
  slug: string;
  path: string;
  competitorName: string;
  title: string;
  description: string;
  searchIntent: string;
  skedularPositioning: string;
  claimList: string[];
  primaryCtaId: string;
  metadataStatus: ContentStatus;
  competitorReviewStatus: ReviewStatus;
}

export interface DraftCoverageItem {
  id: string;
  heading: string;
  sourceLineStart: number;
  sourceLineEnd?: number;
  contentType:
    | "page"
    | "feature"
    | "capability"
    | "pricing"
    | "seo"
    | "accessibility"
    | "performance"
    | "technical-constraint"
    | "future-item"
    | "note";
  decision: MigrationDecision;
  destinationRef: string;
  verificationStatus: ReviewStatus;
}

export interface CapabilityClaim {
  id: string;
  claimText: string;
  claimType: "product-capability" | "integration" | "pricing" | "security" | "competitor" | "performance" | "roadmap";
  sourceRefs: string[];
  reviewStatus: ReviewStatus;
  publishedPageIds: string[];
}
