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

  // Screenshot sections
  screenshotSections?: Array<{
    id: string;
    heading: string;
    subheading: string;
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
