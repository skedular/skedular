import { describe, expect, it } from "vitest";
import {
  capabilityCoverage,
  documentationArticles,
  documentationProducts,
  getProductCategories,
  getDocumentationPath,
  publishedDocumentationArticles,
  validateDocumentationCatalog,
} from "../src/data/documentation";
import {
  documentationSourceMap,
  validateDocumentationSourceMap,
} from "../src/data/documentation-source-map";
import { llmsPages, sitemapPages } from "../src/data/seo";

describe("documentation catalog", () => {
  it("validates routes, evidence, terminology, and relationships", () => {
    expect(validateDocumentationCatalog()).toEqual([]);
    expect(
      new Set(publishedDocumentationArticles.map(getDocumentationPath)).size,
    ).toBe(publishedDocumentationArticles.length);
  });

  it("provides all required categories and complete Getting Started guidance", () => {
    for (const product of documentationProducts) {
      expect(
        getProductCategories(product).map((category) => category.id),
      ).toEqual(
        product === "teams"
          ? [
              "getting-started",
              "workplace-setup",
              "bookings",
              "integrations",
              "faqs",
              "best-practices",
            ]
          : product === "spaces"
            ? [
                "getting-started",
                "workspace-setup",
                "bookings",
                "products-and-marketplace",
                "faqs",
                "billing-and-payments",
                "best-practices",
                "analytics",
              ]
            : [
                "getting-started",
                "your-place",
                "core-features",
                "bookings",
                "payments-and-refunds",
                "managing-your-listing",
                "settings",
                "faqs",
                "best-practices",
              ],
      );
      expect(
        documentationArticles.some(
          (article) =>
            article.product === product &&
            article.category === "getting-started" &&
            article.articleKind === "guide",
        ),
      ).toBe(true);
    }
  });

  it("covers every initial capability and makes only published content discoverable", () => {
    expect(capabilityCoverage).toHaveLength(documentationArticles.length);
    for (const article of publishedDocumentationArticles) {
      expect(
        sitemapPages.some(
          (page) => page.path === getDocumentationPath(article),
        ),
      ).toBe(true);
      expect(
        llmsPages.some(
          (page) => page.canonicalPath === getDocumentationPath(article),
        ),
      ).toBe(true);
      expect(article.evidenceRefs.length).toBeGreaterThan(0);
      expect(article.terminologyRefs.length).toBeGreaterThan(0);
    }
  });

  it("maps every reviewed Markdown source to an owned documentation concept", () => {
    expect(documentationSourceMap).toHaveLength(18);
    expect(validateDocumentationSourceMap()).toEqual([]);
    expect(
      documentationSourceMap.every((source) => source.articleIds.length > 0),
    ).toBe(true);
  });
});
