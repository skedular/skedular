import { aboutPage } from "./about-page";
import { aiEducationalContent } from "./ai-educational-content";
import { comparisonPages } from "./comparison-pages";
import type { PublicPage } from "./content-types";
import { resourceArticles } from "./current-public-content";
import { industryPages } from "./industry-pages";
import { legalPages } from "./legal-pages";
import { pricingPage } from "./pricing";
import {
  publishedDocumentationArticles,
  getCategoryPath,
  getDocumentationPath,
  getProductPath,
} from "./documentation";

export const publicPages: PublicPage[] = [
  {
    id: "documentation",
    path: "/docs",
    title: "Skedular Documentation | Product guides and practical help",
    description:
      "Practical Skedular documentation for Teams, Spaces, and Host.",
    audience: "Skedular users",
    pageType: "support",
    sourceRefs: ["docs:catalog"],
    primaryCtaId: "book-demo",
    canonicalPath: "/docs",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["BreadcrumbList"],
  },
  ...(["teams", "spaces", "host"] as const).flatMap((product) => [
    {
      id: `documentation-${product}`,
      path: getProductPath(product),
      title: `Skedular ${product[0].toUpperCase() + product.slice(1)} documentation`,
      description: `Guides for Skedular ${product[0].toUpperCase() + product.slice(1)}.`,
      audience: "Skedular users",
      pageType: "support" as const,
      sourceRefs: ["docs:catalog"],
      primaryCtaId: "book-demo",
      canonicalPath: getProductPath(product),
      metadataStatus: "published" as const,
      contentStatus: "published" as const,
      structuredDataTypes: ["BreadcrumbList"] as const,
    },
    ...publishedDocumentationArticles
      .filter((article) => article.product === product)
      .map((article) => ({
        id: `documentation-${article.id}`,
        path: getDocumentationPath(article),
        title:
          article.product === "spaces" &&
          [
            "spaces-bookings",
            "spaces-credit-entitlements",
            "spaces-faq",
            "spaces-operations",
          ].includes(article.id)
            ? `Skedular Spaces ${article.title}`
            : article.id === "teams-best-practices"
              ? "Skedular Teams Best Practices"
              : article.id === "host-faq"
                ? "Skedular Host FAQs"
                : article.id === "host-get-started"
                  ? "Skedular Host Getting Started"
                  : article.id === "spaces-get-started"
                    ? "Skedular Spaces Getting Started"
                    : article.id === "host-operations"
                      ? "Skedular Host Best Practices"
                      : article.id === "host-credit-entitlements"
                        ? "Skedular Host Credit-Based Booking Entitlements"
                        : article.title,
        description:
          article.id === "host-operations"
            ? "Practical guidance for keeping a Skedular Host place accurate, bookable, and easy for renters to understand."
            : article.description,
        audience: "Skedular users",
        pageType: "support" as const,
        sourceRefs: article.evidenceRefs,
        primaryCtaId: "book-demo",
        canonicalPath: getDocumentationPath(article),
        metadataStatus: "published" as const,
        contentStatus: "published" as const,
        structuredDataTypes: ["Article", "BreadcrumbList"] as const,
      })),
    ...Array.from(
      new Set(
        publishedDocumentationArticles
          .filter((article) => article.product === product)
          .map((article) => article.category),
      ),
    ).map((category) => ({
      id: `documentation-${product}-${category}`,
      path: getCategoryPath(product, category),
      title: `${category} | Skedular ${product} documentation`,
      description: `${category.replaceAll("-", " ")} guidance for Skedular ${product}.`,
      audience: "Skedular users",
      pageType: "support" as const,
      sourceRefs: ["docs:catalog"],
      primaryCtaId: "book-demo",
      canonicalPath: getCategoryPath(product, category),
      metadataStatus: "published" as const,
      contentStatus: "published" as const,
      structuredDataTypes: ["BreadcrumbList"] as const,
    })),
  ]),
  ...publishedDocumentationArticles
    .filter((article) => article.product === "shared")
    .map((article) => ({
      id: `documentation-${article.id}`,
      path: getDocumentationPath(article),
      title:
        article.id === "shared-concepts"
          ? "Understanding Skedular | Core Concepts | Skedular Documentation"
          : `Skedular concepts: ${article.title}`,
      description: `Canonical Skedular concept reference for ${article.title.toLowerCase()}.`,
      audience: "Skedular users",
      pageType: "support" as const,
      sourceRefs: article.evidenceRefs,
      primaryCtaId: "book-demo",
      canonicalPath: getDocumentationPath(article),
      metadataStatus: "published" as const,
      contentStatus: "published" as const,
      structuredDataTypes: ["Article", "BreadcrumbList"] as const,
    })),
  {
    id: "home",
    path: "/",
    title: "Skedular | Find, book, manage, and monetize workspace",
    description:
      "Find workspace, book desks and meeting rooms, manage hybrid workplaces, and run coworking operations with Skedular.",
    audience: "buyers and operators",
    pageType: "home",
    sourceRefs: ["draft:homepage", "draft:vision"],
    primaryCtaId: "book-demo",
    secondaryCtaIds: [
      "become-host",
      "learn-teams",
      "learn-spaces",
      "learn-host",
    ],
    canonicalPath: "/",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Organization", "Product", "BreadcrumbList"],
  },
  {
    id: "teams",
    path: "/teams",
    title: "Skedular Teams | Private workplace management",
    description:
      "Desk booking, room booking, parking, attendance, floor plans, analytics, Slack, Microsoft Teams, and enterprise identity for private workplaces.",
    audience: "organization buyers",
    pageType: "product",
    sourceRefs: ["draft:teams"],
    primaryCtaId: "book-demo",
    canonicalPath: "/teams",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "BreadcrumbList"],
    requiresClaimReview: true,
  },
  {
    id: "spaces",
    path: "/spaces",
    title: "Skedular Spaces | Workspace operator management",
    description:
      "Manage workspace inventory, products, payments, billing, invoicing, tax, marketplace publishing, and branding.",
    audience: "workspace operators",
    pageType: "product",
    sourceRefs: ["draft:spaces"],
    primaryCtaId: "contact-sales",
    canonicalPath: "/spaces",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "BreadcrumbList"],
    requiresClaimReview: true,
  },
  {
    id: "host",
    path: "/host",
    title: "Skedular Host | Simple space rental management",
    description:
      "List a property, room, desk, studio, venue, or other place. Set flexible pricing and cancellation policies, accept card payments, and manage renters.",
    audience: "independent hosts",
    pageType: "product",
    sourceRefs: ["spec:026-scheduler-host-app"],
    primaryCtaId: "try-host",
    canonicalPath: "/host",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "FAQPage", "BreadcrumbList"],
    requiresClaimReview: true,
  },
  {
    id: "pricing",
    path: "/pricing",
    title: pricingPage.title,
    description: pricingPage.description,
    audience: "prospects",
    pageType: "pricing",
    sourceRefs: ["draft:pricing"],
    primaryCtaId: "book-demo",
    canonicalPath: "/pricing",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "FAQPage", "BreadcrumbList"],
    requiresPricingReview: true,
  },
  {
    id: "blog",
    path: "/blog",
    title:
      "Skedular Blog | Workspace planning, booking, payments, and operations",
    description:
      "Read Skedular blog posts about hybrid work, workspace planning, payments, invoicing, Slack, Microsoft Teams, and product decisions.",
    audience: "resource readers",
    pageType: "resource",
    sourceRefs: ["blog:index"],
    primaryCtaId: "book-demo",
    canonicalPath: "/blog",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["BreadcrumbList"],
  },
  {
    id: "resources",
    path: "/resources",
    title: "Skedular Resources | Workspace planning and operations",
    description:
      "Browse Skedular resources about workspace planning, hybrid work, coworking operations, booking, payments, invoicing, and workplace management.",
    audience: "resource readers",
    pageType: "resource",
    sourceRefs: ["resources:index"],
    primaryCtaId: "book-demo",
    canonicalPath: "/resources",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["BreadcrumbList"],
  },
  {
    id: "compare",
    path: "/compare",
    title: "Compare Skedular with Alternatives",
    description:
      "Compare Skedular with leading workspace booking and management software alternatives.",
    audience: "comparison search visitors",
    pageType: "comparison",
    sourceRefs: ["comparison:hub"],
    primaryCtaId: "book-demo",
    canonicalPath: "/compare",
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "BreadcrumbList"],
    requiresCompetitorReview: true,
  },
  ...pricingPage.models
    .filter(
      (model) =>
        model.id === "teams" || model.id === "spaces" || model.id === "host",
    )
    .map((model): PublicPage => ({
      id: `pricing-${model.id}`,
      path: `/pricing/${model.id}`,
      title: model.seoTitle,
      description: model.seoDescription,
      audience: model.audience,
      pageType: "pricing",
      sourceRefs: ["draft:pricing"],
      primaryCtaId: model.ctaId,
      canonicalPath: `/pricing/${model.id}`,
      metadataStatus: "published",
      contentStatus: "published",
      structuredDataTypes: ["Product", "FAQPage", "BreadcrumbList"],
      requiresPricingReview: true,
    })),
  {
    id: aboutPage.id,
    path: aboutPage.path,
    title: aboutPage.title,
    description: aboutPage.description,
    audience: "company researchers and prospects",
    pageType: "utility",
    sourceRefs: [aboutPage.sourceUrl],
    primaryCtaId: "book-demo",
    secondaryCtaIds: ["contact-sales"],
    canonicalPath: aboutPage.path,
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Organization", "BreadcrumbList"],
  },
  ...legalPages.map((page): PublicPage => ({
    id: page.id,
    path: page.path,
    title: page.title,
    description: page.description,
    audience: "legal and procurement readers",
    pageType: "utility",
    sourceRefs: [page.sourceUrl],
    primaryCtaId: "book-demo",
    canonicalPath: page.path,
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Organization", "BreadcrumbList"],
    requiresClaimReview: page.reviewStatus === "pending",
  })),
  ...resourceArticles.map((article): PublicPage => ({
    id: article.id,
    path: article.destinationPath,
    title: article.title,
    description: article.summary,
    audience: "resource readers",
    pageType: "resource",
    sourceRefs: [article.sourceUrl],
    primaryCtaId: "book-demo",
    canonicalPath: article.destinationPath,
    metadataStatus: "published",
    contentStatus: article.contentStatus,
    structuredDataTypes: ["Article", "BreadcrumbList"],
    requiresClaimReview: article.claimReviewStatus === "pending",
  })),
  ...aiEducationalContent.map((page): PublicPage => ({
    id: page.id,
    path: page.path,
    title: page.title,
    description: page.description,
    audience: page.audience,
    pageType: "resource",
    sourceRefs: page.sourceRefs,
    primaryCtaId: page.primaryCtaId,
    canonicalPath: page.canonicalPath,
    metadataStatus: page.metadataStatus,
    contentStatus: page.contentStatus,
    structuredDataTypes: ["Article", "BreadcrumbList"],
  })),
  ...industryPages.map((page): PublicPage => ({
    id: page.id,
    path: page.path,
    title: page.title,
    description: page.description,
    audience: page.audience,
    pageType: "industry",
    sourceRefs: page.sourceRefs,
    primaryCtaId: page.primaryCtaId,
    canonicalPath: page.canonicalPath,
    metadataStatus: page.metadataStatus,
    contentStatus: page.contentStatus,
    structuredDataTypes: ["Product", "BreadcrumbList"],
  })),
  ...comparisonPages.map((page): PublicPage => ({
    id: page.id,
    path: page.path,
    title: page.title,
    description: page.description,
    audience: "comparison search visitors",
    pageType: "comparison",
    sourceRefs: ["comparison:generated"],
    primaryCtaId: page.primaryCtaId,
    canonicalPath: page.path,
    metadataStatus: "published",
    contentStatus: "published",
    structuredDataTypes: ["Product", "BreadcrumbList"],
    requiresCompetitorReview: true,
  })),
];

export const pricingReview = {
  status: "pending",
  source: "public-website-content-draft.md",
  centralizedData: "pricing.ts",
};
