import type { PublicPage } from "./content-types";

export const categoryPages: Array<PublicPage & { primaryCategory: string }> = [
  {
    id: "workspace-management-software",
    path: "/workspace-management-software",
    title: "Workspace Management Software | Skedular",
    description:
      "Workspace management software for finding desks, meeting rooms, and flexible workspace. Desk booking, room booking, hybrid team coordination, and workplace analytics.",
    audience: "organization buyers",
    pageType: "category",
    sourceRefs: ["category:workspace-management"],
    primaryCategory: "workspace management software",
    primaryCtaId: "book-demo",
    canonicalPath: "/workspace-management-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "desk-booking-software",
    path: "/desk-booking-software",
    title: "Desk Booking Software | Skedular",
    description:
      "Desk booking software for hot desking, dedicated desks, and hybrid workplace coordination. Book workspace by the hour, day, week, or subscription.",
    audience: "hybrid teams",
    pageType: "category",
    sourceRefs: ["category:desk-booking"],
    primaryCategory: "desk booking software",
    primaryCtaId: "book-demo",
    canonicalPath: "/desk-booking-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "coworking-management-software",
    path: "/coworking-management-software",
    title: "Coworking Management Software | Skedular",
    description:
      "Coworking management software for selling workspace, managing memberships, automating billing, and supporting customers from a single platform.",
    audience: "workspace operators",
    pageType: "category",
    sourceRefs: ["category:coworking-management"],
    primaryCategory: "coworking management software",
    primaryCtaId: "contact-sales",
    canonicalPath: "/coworking-management-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "hybrid-workplace-software",
    path: "/hybrid-workplace-software",
    title: "Hybrid Workplace Software | Skedular",
    description:
      "Hybrid workplace software for coordinating who comes to the office when. Attendance visibility, desk booking, meeting room reservations, and team presence.",
    audience: "enterprise teams",
    pageType: "category",
    sourceRefs: ["category:hybrid-workplace"],
    primaryCategory: "hybrid workplace software",
    primaryCtaId: "book-demo",
    canonicalPath: "/hybrid-workplace-software",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
  {
    id: "workspace-marketplace",
    path: "/workspace-marketplace",
    title: "Workspace Marketplace | Skedular",
    description:
      "Workspace marketplace for discovering and booking desks, meeting rooms, private offices, and event spaces from providers across the network.",
    audience: "workspace seekers",
    pageType: "category",
    sourceRefs: ["category:workspace-marketplace"],
    primaryCategory: "workspace marketplace",
    primaryCtaId: "search-workspace",
    canonicalPath: "/workspace-marketplace",
    metadataStatus: "pending",
    contentStatus: "drafted",
    structuredDataTypes: ["SoftwareApplication", "Product", "BreadcrumbList"],
  },
];

// Generate category pages with specific pricing variants
export const generateCategoryPages = (basePages: typeof categoryPages) => {
  return basePages.map((page) => ({
    ...page,
    id: `${page.id}-teams`,
    path: `/pricing/${page.path.replace("/", "")}`,
    title: `Skedular Teams | ${page.title}`,
    description: `Desk booking software with hybrid team coordination for teams. ${page.description}`,
  }));
};
