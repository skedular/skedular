import type { PublicPage } from "./content-types";

export const industryPages: Array<PublicPage & { industryType: string }> = [
  {
    id: "coworking-spaces",
    path: "/industry/coworking-spaces",
    title: "Workspace Software for Coworking Spaces | Skedular",
    description:
      "Workspace software for coworking spaces that need to manage desks, rooms, memberships, billing, invoicing, and customer bookings from one platform.",
    audience: "coworking space operators",
    pageType: "industry",
    sourceRefs: ["industry:coworking"],
    industryType: "coworking spaces",
    primaryCtaId: "contact-sales",
    canonicalPath: "/industry/coworking-spaces",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "shared-offices",
    path: "/industry/shared-offices",
    title: "Workspace Software for Shared Offices | Skedular",
    description:
      "Workspace management software for shared office operators. Desk booking, meeting room reservations, billing automation, and customer support.",
    audience: "shared office operators",
    pageType: "industry",
    sourceRefs: ["industry:shared-offices"],
    industryType: "shared offices",
    primaryCtaId: "contact-sales",
    canonicalPath: "/industry/shared-offices",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "enterprise-offices",
    path: "/industry/enterprise",
    title: "Workspace Software for Enterprise Offices | Skedular",
    description:
      "Workplace management software for enterprise teams. Hybrid coordination, desk booking, attendance visibility, and SSO integration.",
    audience: "enterprise organizations",
    pageType: "industry",
    sourceRefs: ["industry:enterprise"],
    industryType: "enterprise offices",
    primaryCtaId: "book-demo",
    canonicalPath: "/industry/enterprise",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
];

// Generate industry pages with specific variations
export const generateIndustryPages = (basePages: typeof industryPages) => {
  return basePages.map((page) => ({
    ...page,
    id: `${page.id}-spaces`,
    path: `/pricing/${page.path.replace("/", "")}`,
    title: `Skedular Spaces | ${page.title}`,
    description: `Coworking management software for workspace operators. ${page.description}`,
  }));
};
