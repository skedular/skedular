import type { PublicPage } from "./content-types";

export const aiEducationalContent: Array<PublicPage & { educationalType: string }> = [
  {
    id: "what-is-workspace-management",
    path: "/resources/what-is-workspace-management-software",
    title: "What Is Workspace Management Software?",
    description:
      "Workspace management software helps teams coordinate who works where, when. Learn how it supports desk booking, meeting room reservations, attendance tracking, and hybrid work.",
    audience: "new visitors, researchers",
    pageType: "resource",
    sourceRefs: ["educational:workspace-management"],
    educationalType: "definition",
    primaryCtaId: "book-demo",
    canonicalPath: "/resources/what-is-workspace-management-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["Article", "BreadcrumbList"],
  },
  {
    id: "what-is-coworking-management",
    path: "/resources/what-is-coworking-management-software",
    title: "What Is Coworking Management Software?",
    description:
      "Coworking management software helps workspace operators sell desk space, manage memberships, automate billing, and support customers. Learn how it replaces spreadsheets.",
    audience: "workspace operators",
    pageType: "resource",
    sourceRefs: ["educational:coworking-management"],
    educationalType: "definition",
    primaryCtaId: "contact-sales",
    canonicalPath: "/resources/what-is-coworking-management-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["Article", "BreadcrumbList"],
  },
  {
    id: "how-workspace-memberships-work",
    path: "/resources/how-workspace-memberships-work",
    title: "How Workspace Memberships Work?",
    description:
      "Workspace memberships allow customers to have guaranteed access to your space. Learn about membership tiers, billing cycles, and automatic renewal.",
    audience: "workspace operators",
    pageType: "resource",
    sourceRefs: ["educational:memberships"],
    educationalType: "workflow",
    primaryCtaId: "contact-sales",
    canonicalPath: "/resources/how-workspace-memberships-work",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["Article", "BreadcrumbList"],
  },
  {
    id: "how-workspace-billing-works",
    path: "/resources/how-workspace-billing-works",
    title: "How Workspace Billing Works?",
    description:
      "Workspace billing software handles one-time bookings, recurring subscriptions, and automated invoicing. Learn about Stripe integration and Xero accounting.",
    audience: "workspace operators",
    pageType: "resource",
    sourceRefs: ["educational:billing"],
    educationalType: "workflow",
    primaryCtaId: "contact-sales",
    canonicalPath: "/resources/how-workspace-billing-works",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["Article", "BreadcrumbList"],
  },
];

// Generate educational content with specific variations
export const generateEducationalContent = (basePages: typeof aiEducationalContent) => {
  return basePages.map((page) => ({
    ...page,
    id: `${page.id}-ai`,
    path: `/resources/${page.path}`,
    title: page.title,
    description: page.description,
  }));
};
