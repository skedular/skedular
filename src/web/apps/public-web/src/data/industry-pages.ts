import type { PublicPage } from "./content-types";

export const industryPages: Array<PublicPage & { industryType: string }> = [
  {
    id: "coworking-spaces",
    path: "/workspace-software-for-coworking-spaces",
    title: "Workspace Software for Coworking Spaces | Skedular",
    description:
      "Coworking management software for selling workspace, managing memberships, automating billing, and supporting customers from a single platform.",
    audience: "coworking space operators",
    pageType: "industry",
    sourceRefs: ["industry:coworking"],
    industryType: "coworking spaces",
    primaryCtaId: "contact-sales",
    canonicalPath: "/workspace-software-for-coworking-spaces",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "shared-offices",
    path: "/workspace-software-for-shared-offices",
    title: "Workspace Software for Shared Offices | Skedular",
    description:
      "Workspace management software for shared office operators. Desk booking, meeting room reservations, billing automation, and customer support.",
    audience: "shared office operators",
    pageType: "industry",
    sourceRefs: ["industry:shared-offices"],
    industryType: "shared offices",
    primaryCtaId: "contact-sales",
    canonicalPath: "/workspace-software-for-shared-offices",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "enterprise-offices",
    path: "/workspace-software-for-enterprise",
    title: "Workspace Software for Enterprise Offices | Skedular",
    description: "Workplace management software for enterprise teams. Hybrid coordination, desk booking, attendance visibility, and SSO integration.",
    audience: "enterprise organizations",
    pageType: "industry",
    sourceRefs: ["industry:enterprise"],
    industryType: "enterprise offices",
    primaryCtaId: "book-demo",
    canonicalPath: "/workspace-software-for-enterprise",
    metadataStatus: "pending",
    contentStatus: "drafted",
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
