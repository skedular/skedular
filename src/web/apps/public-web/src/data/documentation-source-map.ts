export type DocumentationConceptScope = "shared" | "teams" | "spaces" | "host";

export interface DocumentationSourceEntry {
  id: string;
  sourcePath: string;
  concept: string;
  scope: DocumentationConceptScope;
  summary: string;
  articleIds: string[];
}

/**
 * Source-of-truth map for the reviewed Markdown in doc-resources.
 * Keep this map explicit so an article cannot quietly drift away from its evidence.
 */
export const documentationSourceMap: DocumentationSourceEntry[] = [
  {
    id: "analytics",
    sourcePath: "doc-resources/analytics.md",
    concept: "Analytics",
    scope: "shared",
    summary:
      "Organization and location reporting built from booking and resource activity.",
    articleIds: ["spaces-analytics"],
  },
  {
    id: "availability",
    sourcePath: "doc-resources/availability.md",
    concept: "Availability",
    scope: "shared",
    summary:
      "Date-based visibility of available, booked, and unavailable resources.",
    articleIds: [
      "teams-set-up-workplace",
      "spaces-locations-resources",
      "host-availability",
    ],
  },
  {
    id: "bank-accounts",
    sourcePath: "doc-resources/bank-accounts.md",
    concept: "Bank accounts",
    scope: "spaces",
    summary:
      "Manual bank-transfer payment configuration for Spaces organizations.",
    articleIds: ["spaces-bank-payments"],
  },
  {
    id: "billing-and-payouts",
    sourcePath: "doc-resources/billing-and-payouts.md",
    concept: "Billing and payouts",
    scope: "spaces",
    summary:
      "Organization-level billing cadence, payment methods, and financial integrations.",
    articleIds: ["spaces-bank-payments", "spaces-xero"],
  },
  {
    id: "booking",
    sourcePath: "doc-resources/booking.md",
    concept: "Bookings",
    scope: "shared",
    summary:
      "A reservation of one or more resources for a defined period, with product-specific visibility and rules.",
    articleIds: ["teams-private-bookings", "spaces-bookings", "host-bookings"],
  },
  {
    id: "floor-plans",
    sourcePath: "doc-resources/floor-paln.md",
    concept: "Floor plans",
    scope: "shared",
    summary:
      "A visual location layout that places resources and can connect availability to a map.",
    articleIds: ["teams-organize-workplace", "spaces-zones-floor-plans"],
  },
  {
    id: "location",
    sourcePath: "doc-resources/location.md",
    concept: "Locations",
    scope: "shared",
    summary:
      "The physical context that owns resources, opening hours, address, and optional floor-plan information.",
    articleIds: [
      "teams-set-up-workplace",
      "spaces-locations-resources",
      "host-places",
    ],
  },
  {
    id: "organization",
    sourcePath: "doc-resources/organization.md",
    concept: "Organizations",
    scope: "shared",
    summary:
      "The ownership boundary for users, locations, resources, settings, booking rules, and reporting.",
    articleIds: [
      "teams-set-up-workplace",
      "spaces-marketplace-setup",
      "host-places",
    ],
  },
  {
    id: "product-tag",
    sourcePath: "doc-resources/product-tag.md",
    concept: "Product tags",
    scope: "shared",
    summary:
      "Classification used to connect products and resources and support dynamic allocation.",
    articleIds: ["spaces-products-pricing"],
  },
  {
    id: "product",
    sourcePath: "doc-resources/product.md",
    concept: "Products",
    scope: "shared",
    summary:
      "An offer and booking configuration that combines resources, pricing, rules, payment, cancellation, and activation.",
    articleIds: ["spaces-products-pricing", "host-pricing"],
  },
  {
    id: "resource",
    sourcePath: "doc-resources/resource.md",
    concept: "Resources",
    scope: "shared",
    summary:
      "The bookable inventory inside a location, with capacity, tags, zones, hours, and availability.",
    articleIds: [
      "teams-set-up-workplace",
      "spaces-locations-resources",
      "host-places",
    ],
  },
  {
    id: "stripe-connect",
    sourcePath: "doc-resources/stripe-connect.md",
    concept: "Stripe Connect",
    scope: "spaces",
    summary:
      "The reviewed Spaces payment connection for online card payments and booking confirmation.",
    articleIds: ["spaces-bank-payments"],
  },
  {
    id: "subscriptions",
    sourcePath: "doc-resources/subscriptions.md",
    concept: "Subscriptions",
    scope: "spaces",
    summary:
      "Recurring customer access that can create recurring bookings and follow billing and renewal states.",
    articleIds: ["spaces-subscriptions"],
  },
  {
    id: "tag",
    sourcePath: "doc-resources/tag.md",
    concept: "Tags",
    scope: "shared",
    summary:
      "Organization-wide labels for searching and classifying resources, distinct from zones.",
    articleIds: ["teams-organize-workplace", "spaces-locations-resources"],
  },
  {
    id: "team",
    sourcePath: "doc-resources/team.md",
    concept: "Teams",
    scope: "teams",
    summary:
      "Groups of users in a private organization that simplify group bookings and workplace coordination.",
    articleIds: ["teams-organize-people"],
  },
  {
    id: "user",
    sourcePath: "doc-resources/user.md",
    concept: "Users and roles",
    scope: "shared",
    summary:
      "People who belong to organizations, with role and booking visibility varying by product.",
    articleIds: ["teams-organize-people", "spaces-customers", "host-bookings"],
  },
  {
    id: "xero-integration",
    sourcePath: "doc-resources/xero-integration.md",
    concept: "Xero integration",
    scope: "spaces",
    summary:
      "The reviewed Spaces accounting connection for invoice export and payment synchronization.",
    articleIds: ["spaces-xero"],
  },
  {
    id: "zone",
    sourcePath: "doc-resources/zone.md",
    concept: "Zones",
    scope: "shared",
    summary:
      "Reusable organization-level groupings that can classify one resource in multiple ways.",
    articleIds: ["teams-organize-workplace", "spaces-zones-floor-plans"],
  },
];

export const sharedDocumentationSources = documentationSourceMap.filter(
  (entry) => entry.scope === "shared",
);
export const productDocumentationSources = (
  scope: Exclude<DocumentationConceptScope, "shared">,
) => documentationSourceMap.filter((entry) => entry.scope === scope);

export const validateDocumentationSourceMap = () => {
  const errors: string[] = [];
  const ids = new Set<string>();
  for (const entry of documentationSourceMap) {
    if (ids.has(entry.id)) errors.push(`[duplicate-source-id] ${entry.id}`);
    if (!entry.sourcePath.startsWith("doc-resources/"))
      errors.push(`[invalid-source-path] ${entry.id}`);
    if (!entry.articleIds.length) errors.push(`[unmapped-source] ${entry.id}`);
    ids.add(entry.id);
  }
  return errors;
};
