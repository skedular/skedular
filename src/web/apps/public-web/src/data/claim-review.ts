import type { CapabilityClaim } from "./content-types";

export const capabilityClaims: CapabilityClaim[] = [
  {
    id: "teams-resource-booking",
    claimText:
      "Teams supports desk, room, parking, equipment, team attendance, floor plans, analytics, Slack, Microsoft Teams, and enterprise identity content.",
    claimType: "product-capability",
    sourceRefs: [
      "draft:teams-product-page",
      "draft:complete-feature-inventory",
    ],
    reviewStatus: "pending",
    publishedPageIds: ["teams", "workplace-integrations"],
  },
  {
    id: "spaces-commercial-workflows",
    claimText:
      "Spaces covers resource management, product catalog management, pricing, billing cadence, payments, invoicing, tax, marketplace publishing, and branding.",
    claimType: "product-capability",
    sourceRefs: ["draft:spaces-product-page", "draft:pricing-strategy"],
    reviewStatus: "pending",
    publishedPageIds: [
      "spaces",
      "payments-billing-invoicing",
      "operator-publishing",
    ],
  },
  {
    id: "pricing-values",
    claimText:
      "Pricing uses Teams active-user tiers and Spaces fixed monthly plans based on booking-instance volume.",
    claimType: "pricing",
    sourceRefs: ["spec:028-skedular-spaces-pricing", "draft:pricing-strategy"],
    reviewStatus: "pending",
    publishedPageIds: ["pricing"],
  },
  {
    id: "comparison-neutrality",
    claimText:
      "Comparison pages use neutral reviewed positioning and no unverified competitor claims.",
    claimType: "competitor",
    sourceRefs: ["draft:comparison-pages"],
    reviewStatus: "pending",
    publishedPageIds: [
      "skedular-vs-skedda",
      "skedular-vs-robin",
      "skedular-vs-envoy",
    ],
  },
];

export const competitorClaimReview = {
  status: "pending",
  reviewer: "Product/legal owner before launch",
  notes: [
    "Use neutral positioning when external claims cannot be verified.",
    "Do not copy competitor language or proprietary layouts.",
    "Keep comparison pages published but soften any unsupported claims.",
  ],
};
