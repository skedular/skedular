export type ContentStatus =
  "inventory" | "drafted" | "reviewed" | "approved" | "published";
export type ReviewStatus =
  "pending" | "approved" | "rewritten" | "blocked" | "not-required";
export type MigrationDecision =
  | "publish"
  | "rewrite"
  | "merge"
  | "redirect"
  | "technical-planning"
  | "future-planning"
  | "exclude";
export type CtaPurpose =
  | "search"
  | "book"
  | "demo"
  | "login"
  | "sign-up"
  | "contact"
  | "learn-more"
  | "support"
  | "community"
  | "explore";

export interface Cta {
  id: string;
  label: string;
  purpose: CtaPurpose;
  destinationType:
    "internal-route" | "public-url" | "email" | "external-community";
  destinationRef: string;
  audience: string;
}

export interface PublicPage {
  id: string;
  path: string;
  title: string;
  description: string;
  audience: string;
  pageType:
    | "home"
    | "product"
    | "pricing"
    | "resource"
    | "support"
    | "feature"
    | "comparison"
    | "category"
    | "industry"
    | "utility";
  sourceRefs: string[];
  primaryCtaId: string;
  secondaryCtaIds?: string[];
  canonicalPath: string;
  metadataStatus: ContentStatus;
  contentStatus: ContentStatus;
  structuredDataTypes: Array<
    "Organization" | "Product" | "FAQPage" | "BreadcrumbList" | "Article"
  >;
  requiresClaimReview?: boolean;
  requiresCompetitorReview?: boolean;
  requiresPricingReview?: boolean;
}

export interface ProductPageContent {
  id: "teams" | "spaces" | "host";
  eyebrow: string;
  title: string;
  summary: string;
  audience: string;

  // Hero section
  heroHeading?: string;
  heroDescription?: string;
  heroCTAPrimary?: { label: string; href: string };
  heroCTASecondary?: { label: string; href: string };

  // Why Organizations Need More Than Desk Booking
  whyOrganizationsNeedMore?: {
    heading: string;
    description: string;
    cards: Array<{ title: string; description: string }>;
  };

  // Typical Workplace Journey
  typicalJourney?: {
    heading: string;
    steps: Array<{ title: string; description: string }>;
  };

  // Features sections
  features: Array<{
    title: string;
    body: string;
    featureBlocks: Array<{
      title: string;
      description: string;
      items: string[];
      accent?: "emerald" | "aqua" | "violet" | "sunbeam";
    }>;
  }>;

  // Why Organizations Choose Us
  whyChooseUs?: {
    heading: string;
    cards: Array<{ title: string; description: string }>;
  };

  // Differentiation Section
  differentiation?: {
    heading: string;
    description: string;
    withoutSkedular: string[];
    withSkedular: string[];
  };

  // Operator Context Section
  operatorContext?: {
    heading: string;
    content: string;
  };

  // Screenshot sections
  screenshotSections?: Array<{
    id: string;
    heading: string;
    subheading: string;
    imageSrc?: string;
    placeholderText: string;
  }>;

  // Integrations section
  integrations?: {
    heading: string;
    body: string;
    integrations: string[];
  };

  // Comms Integration (Slack/Teams)
  commsIntegration?: {
    heading: string;
    description: string;
  };

  // Built for section
  builtFor?: {
    heading: string;
    body?: string;
    audiences: string[];
  };

  // Trust section
  trust?: {
    heading: string;
    body?: string;
    logos?: Array<{ name: string; src: string; width: number; height: number }>;
    testimonials?: Array<{
      quote: string;
      author: string;
      company: string;
    }>;
  };

  // FAQ section
  faq: Array<{ question: string; answer: string }>;

  // AI Summary Section
  aiSummary?: {
    heading: string;
    description: string;
    operatorsUse: string[];
    keyCapabilities: string[];
  };

  // Final CTA section
  finalCTA?: {
    heading: string;
    description: string;
    primaryCTA?: { label: string; href: string };
    secondaryCTA?: { label: string; href: string };
  };

  integrationActions?: Array<{ type: "slack" }>;
}

export interface ResourceArticle {
  id: string;
  slug: string;
  sourceUrl: string;
  destinationPath: string;
  title: string;
  summary: string;
  seoTitle?: string;
  seoDescription?: string;
  publishedDate: string;
  topicTags: string[];
  migrationDecision: MigrationDecision;
  contentStatus: ContentStatus;
  claimReviewStatus: ReviewStatus;
  lastModified?: string;
  author?: { name: string; role: string; description: string };
  featureImage?: string;
  featureImageAlt?: string;
  body: string[];
  sections?: Array<{
    heading: string;
    body: string[];
    items?: string[];
    listType?: "ul" | "ol";
  }>;
  faq?: Array<{ question: string; answer: string }>;
  cta?: {
    heading: string;
    body: string;
    links: Array<{ label: string; href: string; primary?: boolean }>;
  };
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
  claimType:
    | "product-capability"
    | "integration"
    | "pricing"
    | "security"
    | "competitor"
    | "performance"
    | "roadmap";
  sourceRefs: string[];
  reviewStatus: ReviewStatus;
  publishedPageIds: string[];
}

// ========================================
// Comparison Hub Types (Phase 2+)
// ========================================

export type ProductKind = "skedular" | "competitor";
export type ProductCategory =
  | "workplace-management"
  | "coworking-management"
  | "hybrid-workplace"
  | "workplace-operations"
  | "marketplace-workspace-network";
export type PublicationStatus = "draft" | "reviewed" | "blocked" | "published";
export type SupportState =
  | "supported"
  | "partially-supported"
  | "not-supported"
  | "unknown"
  | "not-applicable";
export type SourceFreshness =
  "current" | "needs-review" | "outdated" | "blocked";
export type ClaimType =
  | "capability"
  | "strength"
  | "limitation"
  | "pricing"
  | "integration"
  | "best-for"
  | "faq";
export type PageType =
  "competitor-comparison" | "best-software" | "alternatives";

export interface ComparisonProduct {
  id: string;
  name: string;
  slug: string;
  productKind: ProductKind;
  category: ProductCategory;
  publicationStatus: PublicationStatus;
  reviewStatus: ReviewStatus;
  summary: string;
  bestFor: string;
  strengths: string[];
  limitations: string[];
  pricingNotes: string;
  integrationNotes: string;
}

export interface CompetitorClaim {
  id: string;
  competitorId: string;
  claimType: ClaimType;
  claimText: string;
  evidenceNote: string;
  reviewStatus: ReviewStatus;
  publishedPageIds: string[];
}

export interface SkedularCapabilityEvidence {
  id: string;
  featureId: string;
  capabilityName: string;
  category: string;
  supportState: SupportState;
  sourceRefs: string[];
  sourceFreshness: SourceFreshness;
  reviewStatus: ReviewStatus;
  notes?: string;
}

export interface FeatureCategory {
  id: string;
  name: string;
  description: string;
  displayOrder: number;
  features: string[];
}

export interface NormalizedFeature {
  id: string;
  categoryId: string;
  name: string;
  description?: string;
  displayOrder: number;
  requiredBySpec: boolean;
}

export interface FeatureSupport {
  productId: string;
  featureId: string;
  state: SupportState;
  note?: string;
  evidenceRefs: string[];
  reviewStatus: ReviewStatus;
}

export interface ComparisonPageTarget {
  id: string;
  slug: string;
  path: string;
  pageType: "competitor-comparison";
  competitorId: string;
  title: string;
  description: string;
  overview: string;
  pricingComparison: string;
  integrationComparison: string;
  bestFor: string;
  limitations: string;
  whySkedular: string;
  faqIds: string[];
  primaryCtaId: string;
  relatedPageIds: string[];
  publicationStatus: PublicationStatus;
}

export interface SupportingPageTarget {
  id: string;
  slug: string;
  path: string;
  pageType: PageType;
  focusCategoryIds: string[];
  includedProductIds: string[];
  title: string;
  description: string;
  intro: string;
  selectionCriteria: string;
  faqIds: string[];
  relatedPageIds: string[];
  publicationStatus: PublicationStatus;
}

export interface FAQEntry {
  id: string;
  question: string;
  answer: string;
  relatedPageIds: string[];
  claimRefs: string[];
  schemaEligible: boolean;
  reviewStatus: ReviewStatus;
}

export interface StructuredDataDefinition {
  pageId: string;
  types: Array<
    | "SoftwareApplication"
    | "FAQPage"
    | "BreadcrumbList"
    | "ItemList"
    | "WebPage"
  >;
  graph: Record<string, unknown>;
  sourceRefs: string[];
}

export interface ContentInventoryEntry {
  id: string;
  pageId: string;
  sourceDataRefs: string[];
  metadataStatus: ContentStatus;
  contentStatus: ContentStatus;
  reviewNotes?: string;
  validationStatus: "pending" | "passing" | "blocked";
}
