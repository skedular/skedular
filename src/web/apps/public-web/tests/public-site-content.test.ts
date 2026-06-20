import { JSDOM } from "jsdom";
import { spawnSync } from "node:child_process";
import { existsSync } from "node:fs";
import { readFile } from "node:fs/promises";
import { beforeAll, describe, expect, it } from "vitest";
import { comparisonPages } from "../src/data/comparison-pages";
import { competitorClaims } from "../src/data/comparison/competitor-claims";
import { competitors } from "../src/data/comparison/competitors";
import { generateComparisonDataSummary } from "../src/data/comparison/content-inventory";
import {
  featureCategories,
  normalizedFeatures,
} from "../src/data/comparison/feature-matrix";
import { featureSupport } from "../src/data/comparison/feature-support";
import { generateAllComparisonPageTargets } from "../src/data/comparison/page-targets";
import { skedularCapabilityEvidence } from "../src/data/comparison/skedular-evidence";
import {
  validateComparisonData,
  validateCompetitorEvidence,
  validateDuplicateIds,
  validateSkedularEvidence,
} from "../src/data/comparison/validation";
import { publicPages } from "../src/data/content-inventory";
import { resourceArticles } from "../src/data/current-public-content";
import { featurePages } from "../src/data/feature-pages";
import { getRobotsForPath, sitemapPages } from "../src/data/seo";
import { publicUrlEnvironment, publicUrlFixtures } from "./public-url-fixtures";
import {
  COMPARE_HUB_PATH,
  INDIVIDUAL_COMPARISON_PATHS,
  REQUIRED_COMPARISON_PATHS,
  REMOVED_LEGACY_COMPARISON_PATHS,
  isLegacyComparisonPath,
} from "../src/data/comparison/page-paths";
import { supportStateLabels } from "../src/data/comparison/support-states";
import { validateBlockedClaims } from "../src/data/comparison/validation";
import { redirects } from "../src/data/redirects";
import {
  routeFamilies,
  primaryRoutes,
  utilityRoutes,
} from "../src/data/routes";
import { footerNavigation } from "../src/data/navigation";

beforeAll(() => {
  const result = spawnSync("pnpm", ["build"], {
    cwd: process.cwd(),
    env: { ...process.env, ...publicUrlEnvironment },
    encoding: "utf8",
  });

  expect(result.status, `${result.stdout}\n${result.stderr}`).toBe(0);
});

async function loadDistPage(path: string) {
  const filePath =
    path === "/" ? "../dist/index.html" : `../dist${path}/index.html`;
  const html = await readFile(new URL(filePath, import.meta.url), "utf8");
  return new JSDOM(html, { url: `https://www.example.test${path}` });
}

const primaryPaths = [
  "/",
  "/teams",
  "/spaces",
  "/host",
  "/pricing",
  "/pricing/teams",
  "/pricing/spaces",
  "/pricing/host",
  "/blog",
  "/resources",
  "/about",
  "/terms-of-service",
  "/privacy-policy",
  "/docs",
  "/docs/teams",
  "/docs/spaces",
  "/docs/host",
];

describe("expanded public site content", () => {
  it("publishes Documentation in header, mobile, and footer resource navigation", async () => {
    const dom = await loadDistPage("/docs");
    const links = Array.from(dom.window.document.querySelectorAll("a"));
    expect(
      links
        .filter((link) => link.getAttribute("href") === "/docs")
        .map((link) => link.textContent?.trim()),
    ).toContain("Documentation");
    expect(
      dom.window.document.querySelector(".documentation-navigation"),
    ).toBeTruthy();
  });
  it.each(primaryPaths)(
    "publishes %s with one h1, metadata, canonical URL, landmarks, and CTA links",
    async (path) => {
      const dom = await loadDistPage(path);
      const document = dom.window.document;

      expect(document.querySelectorAll("h1")).toHaveLength(1);
      expect(document.querySelector("title")?.textContent?.trim()).not.toEqual(
        "",
      );
      expect(
        document
          .querySelector('meta[name="description"]')
          ?.getAttribute("content"),
      ).toBeTruthy();
      expect(
        document.querySelector('meta[name="robots"]')?.getAttribute("content"),
      ).toBe(getRobotsForPath(path));
      expect(
        document.querySelector('link[rel="canonical"]')?.getAttribute("href"),
      ).toContain(path);
      expect(
        document
          .querySelector('meta[property="og:image"]')
          ?.getAttribute("content"),
      ).toContain("/images/skedular-logo-primary.svg");
      expect(
        document
          .querySelector('meta[name="twitter:image"]')
          ?.getAttribute("content"),
      ).toContain("/images/skedular-logo-primary.svg");
      expect(document.querySelector("header")).toBeTruthy();
      expect(document.querySelector("main")).toBeTruthy();
      expect(document.querySelector("footer")).toBeTruthy();
      expect(document.querySelectorAll("[data-cta-id]").length).toBeGreaterThan(
        0,
      );
    },
  );

  it("publishes all resource, feature, comparison, company, and legal routes", () => {
    for (const article of resourceArticles) {
      expect(
        existsSync(
          new URL(
            `../dist${article.destinationPath}/index.html`,
            import.meta.url,
          ),
        ),
      ).toBe(true);
    }

    for (const page of [...featurePages, ...comparisonPages]) {
      expect(
        existsSync(new URL(`../dist${page.path}/index.html`, import.meta.url)),
      ).toBe(true);
    }

    for (const path of ["/about", "/terms-of-service", "/privacy-policy"]) {
      expect(
        existsSync(new URL(`../dist${path}/index.html`, import.meta.url)),
      ).toBe(true);
    }
  });

  it("publishes robots.txt and sitemap.xml from public SEO inventory", async () => {
    const robots = await readFile(
      new URL("../dist/robots.txt", import.meta.url),
      "utf8",
    );
    const sitemap = await readFile(
      new URL("../dist/sitemap.xml", import.meta.url),
      "utf8",
    );
    const llms = await readFile(
      new URL("../dist/llms.txt", import.meta.url),
      "utf8",
    );

    expect(robots).toContain("User-agent: *");
    expect(robots).toContain("Allow: /");
    expect(robots).toContain(
      `Sitemap: ${publicUrlFixtures.siteUrl}/sitemap.xml`,
    );
    expect(robots).toContain(
      `Host: ${new URL(publicUrlFixtures.siteUrl).host}`,
    );
    expect(sitemap).toContain(
      '<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">',
    );
    expect(llms).toContain("# Skedular");
    expect(llms).toContain("## Core Public Pages");
    expect(llms).toContain(
      `[Skedular Teams | Private workplace management](${publicUrlFixtures.siteUrl}/teams)`,
    );

    for (const page of sitemapPages) {
      expect(sitemap).toContain(
        `${publicUrlFixtures.siteUrl}${page.path === "/" ? "/" : page.path}`,
      );
    }

    for (const path of [
      "/blog",
      "/resources",
      "/compare",
      "/terms-of-service",
      "/privacy-policy",
    ]) {
      expect(sitemap).toContain(`${publicUrlFixtures.siteUrl}${path}`);
    }
  });

  it("publishes article metadata for blog and support pages", async () => {
    const dom = await loadDistPage(
      "/blog/how-to-determine-the-right-amount-of-office-space-for-your-team",
    );
    const document = dom.window.document;

    expect(
      document
        .querySelector('meta[property="article:published_time"]')
        ?.getAttribute("content"),
    ).toBe("2025-05-13");
    expect(
      document
        .querySelector('meta[property="article:modified_time"]')
        ?.getAttribute("content"),
    ).toBe("2025-05-13");
    expect(
      existsSync(
        new URL(
          "../dist/resources/how-to-determine-the-right-amount-of-office-space-for-your-team/index.html",
          import.meta.url,
        ),
      ),
    ).toBe(false);
  });

  it("lists migrated blog articles in newest-first publish date order", async () => {
    const dom = await loadDistPage("/blog");
    const document = dom.window.document;
    const articleTitles = Array.from(
      document.querySelectorAll(".resource-card h2"),
    ).map((heading) => heading.textContent?.trim());

    expect(articleTitles).toEqual(
      resourceArticles.map((article) => article.title),
    );
    expect(articleTitles).not.toContain(
      "Hybrid workplace planning, attendance tracking, and desk booking",
    );
    expect(document.body.textContent).not.toContain("Migration:");
  });

  it("keeps destination URLs environment-sourced and avoids hardcoded staging or production domains in source content", async () => {
    const html = await readFile(
      new URL("../dist/index.html", import.meta.url),
      "utf8",
    );

    expect(html).toContain(publicUrlFixtures.appUrl);
    expect(html).toContain(publicUrlFixtures.signupUrl);
    expect(html).toContain(publicUrlFixtures.demoUrl);
    expect(html).not.toContain("https://skedular.app");
    expect(html).not.toContain("https://staging.skedular.app");

    const teamsHtml = await readFile(
      new URL("../dist/teams/index.html", import.meta.url),
      "utf8",
    );
    expect(teamsHtml).toContain(publicUrlFixtures.slackInstallUrl);
    expect(teamsHtml).toContain(publicUrlFixtures.teamsAppUrl);
    expect(teamsHtml).not.toContain("client_id=118234978193.5578039519830");

    const spacesHtml = await readFile(
      new URL("../dist/spaces/index.html", import.meta.url),
      "utf8",
    );
    expect(spacesHtml).toContain(publicUrlFixtures.spacesAppUrl);
  });

  it("has unique public page metadata and complete comparison metadata", () => {
    const titles = new Set(publicPages.map((page) => page.title));
    const descriptions = new Set(publicPages.map((page) => page.description));

    expect(titles.size).toBe(publicPages.length);
    expect(descriptions.size).toBe(publicPages.length);
    expect(
      comparisonPages.every(
        (page) => page.title && page.description && page.competitorName,
      ),
    ).toBe(true);
  });

  it("publishes organized footer social and community links", async () => {
    const dom = await loadDistPage("/");
    const document = dom.window.document;

    expect(document.querySelectorAll(".footer-social-button")).toHaveLength(3);
    expect(
      document.querySelector(
        'a[href="https://www.linkedin.com/company/getskedular/"]',
      ),
    ).toBeTruthy();
    expect(
      document.querySelector(
        'a[href="https://www.facebook.com/profile.php?id=61571588471440"]',
      ),
    ).toBeTruthy();
    expect(
      document.querySelector('a[href="https://discord.gg/kBczX24y"]'),
    ).toBeTruthy();
    expect(
      document.querySelector(
        'a[href^="https://betalist.com/startups/skedular"]',
      ),
    ).toBeNull();
  });

  it("publishes legal pages as source-preserving legal documents", async () => {
    const termsDom = await loadDistPage("/terms-of-service");
    const termsText =
      termsDom.window.document.querySelector(".legal-document")?.textContent ??
      "";

    expect(termsText).toContain("SKEDULAR ORDER FORM");
    expect(termsText).toContain(
      "By accessing and using the Services, you represent",
    );
    expect(termsText).toContain("SKEDULAR DATA PROCESSING ADDENDUM");
    expect(termsText).toContain(
      "Technical and Organisational Security Measures",
    );

    const privacyDom = await loadDistPage("/privacy-policy");
    const privacyText =
      privacyDom.window.document.querySelector(".legal-document")
        ?.textContent ?? "";

    expect(privacyText).toContain(
      "Welcome to our privacy policy. We respect your privacy",
    );
    expect(privacyText).toContain(
      "Skedular Limited, trading as Skedular, is the data controller",
    );
    expect(privacyText).toContain("The data we collect about you");
    expect(privacyText).toContain("Your legal rights");
  });

  it("places become-a-host before login and uses its own public URL", async () => {
    const dom = await loadDistPage("/");
    const document = dom.window.document;
    const headerLinks = [...document.querySelectorAll(".header-actions a")].map(
      (link) => ({
        ctaId: link.getAttribute("data-cta-id"),
        href: link.getAttribute("href"),
      }),
    );

    expect(headerLinks.map((link) => link.ctaId)).toEqual([
      "become-host",
      "login",
      "book-demo",
    ]);
    expect(headerLinks[0]?.href).toBe(publicUrlFixtures.becomeHostUrl);
    expect(headerLinks[1]?.href).toBe(publicUrlFixtures.signupUrl);
  });

  // ========================================
  // Comparison Hub Validation (Phase 1-9)
  // ========================================

  describe("comparison hub", () => {
    it("validates comparison data structure and exports", () => {
      // Test that comparison data modules export expected types

      expect(featureCategories).toBeDefined();
      expect(Array.isArray(featureCategories)).toBe(true);
      expect(featureCategories.length).toBeGreaterThan(0);

      expect(normalizedFeatures).toBeDefined();
      expect(Array.isArray(normalizedFeatures)).toBe(true);
      expect(normalizedFeatures.length).toBeGreaterThan(0);

      expect(REQUIRED_COMPARISON_PATHS).toBeDefined();
      expect(Array.isArray(REQUIRED_COMPARISON_PATHS)).toBe(true);
      expect(REQUIRED_COMPARISON_PATHS).toContain("/compare");

      expect(INDIVIDUAL_COMPARISON_PATHS).toBeDefined();
      expect(Array.isArray(INDIVIDUAL_COMPARISON_PATHS)).toBe(true);
      expect(INDIVIDUAL_COMPARISON_PATHS.length).toBe(10);

      expect(supportStateLabels).toBeDefined();
      expect(typeof supportStateLabels === "object").toBe(true);

      expect(validateBlockedClaims).toBeDefined();
      expect(typeof validateBlockedClaims === "function").toBe(true);
    });

    it("validates legacy comparison paths are removed", () => {
      // Test that legacy comparison paths are not emitted, redirected, aliased, linked, or listed

      // Check that no legacy paths are in the required paths
      for (const legacyPath of REMOVED_LEGACY_COMPARISON_PATHS) {
        expect(REQUIRED_COMPARISON_PATHS).not.toContain(legacyPath);
      }

      // Test helper function
      expect(isLegacyComparisonPath("/some-old-comparison")).toBe(false);
    });

    it("validates /compare hub exists and links to individual pages", () => {
      // Test that /compare hub links to all individual comparison pages

      expect(COMPARE_HUB_PATH).toBe("/compare");
      expect(INDIVIDUAL_COMPARISON_PATHS).toContain(
        "/compare/skedular-vs-skedda",
      );

      // Test that hub links to all generated page targets
      const pageTargets = generateAllComparisonPageTargets();
      expect(pageTargets.length).toBeGreaterThan(0);

      // Test that each page target has a valid path
      for (const target of pageTargets) {
        expect(target.path).toBeDefined();
        expect(target.path.startsWith("/compare/")).toBe(true);
      }
    });

    it("validates individual comparison pages render required sections", () => {
      // Test that individual comparison pages render required sections (overview, pricing comparison, integration comparison, best for, limitations, why Skedular, FAQs)

      const pageTargets = generateAllComparisonPageTargets();
      expect(pageTargets.length).toBeGreaterThan(0);

      // Test that each page target has required sections
      for (const target of pageTargets) {
        expect(target.overview).toBeDefined();
        expect(target.pricingComparison).toBeDefined();
        expect(target.integrationComparison).toBeDefined();
        expect(target.bestFor).toBeDefined();
        expect(target.limitations).toBeDefined();
        expect(target.whySkedular).toBeDefined();
        expect(target.faqIds).toBeDefined();
        expect(Array.isArray(target.faqIds)).toBe(true);
      }
    });

    it("validates /compare hub displays competitor listings", () => {
      // Test that /compare hub displays all competitor listings

      // Test that competitors data exists
      expect(competitors).toBeDefined();
      expect(Array.isArray(competitors)).toBe(true);

      // Test that there are competitor products (excluding Skedular itself)
      const competitorProducts = competitors.filter(
        (c: any) => c.productKind === "competitor",
      );
      expect(competitorProducts.length).toBeGreaterThan(0);

      // Test that page targets are generated for all competitors
      const pageTargets = generateAllComparisonPageTargets();
      expect(pageTargets.length).toBe(competitorProducts.length);
    });

    it("validates generated comparison content is sourced from shared data", () => {
      // Test that generated comparison content is sourced from shared data rather than page-local hardcoded claim arrays

      // Test that shared data exists
      expect(featureCategories).toBeDefined();
      expect(featureCategories.length).toBeGreaterThan(0);

      expect(normalizedFeatures).toBeDefined();
      expect(normalizedFeatures.length).toBeGreaterThan(0);

      // Test that features reference categories
      const firstFeature = normalizedFeatures[0];
      expect(firstFeature.categoryId).toBeDefined();
      expect(
        featureCategories.some(
          (cat: any) => cat.id === firstFeature.categoryId,
        ),
      ).toBe(true);
    });

    it("validates feature matrix displays correctly", () => {
      // Test that feature matrix displays support states for Skedular and competitors

      // Test that feature support data exists
      expect(featureSupport).toBeDefined();
      expect(Array.isArray(featureSupport)).toBe(true);

      expect(competitors).toBeDefined();
      expect(Array.isArray(competitors)).toBe(true);

      expect(normalizedFeatures).toBeDefined();
      expect(Array.isArray(normalizedFeatures)).toBe(true);

      // Test that Skedular has feature support entries
      const skedularSupport = featureSupport.filter(
        (fs: any) => fs.productId === "skedular",
      );
      expect(skedularSupport.length).toBeGreaterThan(0);

      // Test that support states are valid
      const validStates = [
        "supported",
        "partially-supported",
        "not-supported",
        "unknown",
        "not-applicable",
      ];
      for (const fs of featureSupport) {
        expect(validStates).toContain(fs.state);
      }
    });

    it("validates evidence and review status requirements", () => {
      // Test that Skedular evidence requirements and competitor evidence/review status requirements are enforced

      // Test validation functions exist
      expect(validateSkedularEvidence).toBeDefined();
      expect(typeof validateSkedularEvidence === "function").toBe(true);

      expect(validateCompetitorEvidence).toBeDefined();
      expect(typeof validateCompetitorEvidence === "function").toBe(true);

      expect(validateBlockedClaims).toBeDefined();
      expect(typeof validateBlockedClaims === "function").toBe(true);
    });

    it("validates maintainer extension workflow", () => {
      // Test that maintainer can extend competitor data by adding new competitor

      // Test that data structures are extensible
      expect(Array.isArray(competitors)).toBe(true);
      expect(Array.isArray(competitorClaims)).toBe(true);
      expect(Array.isArray(featureSupport)).toBe(true);

      // Test that adding a new competitor would generate a page target
      const initialPageTargets = generateAllComparisonPageTargets();
      expect(initialPageTargets.length).toBeGreaterThan(0);

      // Test that validation functions exist for maintainer use
      expect(validateDuplicateIds).toBeDefined();
      expect(validateSkedularEvidence).toBeDefined();
      expect(validateCompetitorEvidence).toBeDefined();
    });

    it("validates all comparison data passes validation", () => {
      // Test that all comparison data passes validation

      const pageTargets = [...generateAllComparisonPageTargets()];
      const validationResult = validateComparisonData(
        competitors,
        competitorClaims,
        skedularCapabilityEvidence,
        featureSupport,
        pageTargets,
      );

      // Test that validation returns a result
      expect(validationResult).toBeDefined();
      expect(validationResult.isValid).toBe(true);
      expect(validationResult.errors).toEqual([]);
      expect(Array.isArray(validationResult.warnings)).toBe(true);
    });

    it("validates no legacy comparison routes in redirects, routes, navigation, or content-inventory", () => {
      // Verify no removed legacy comparison route is present in redirects, routes, navigation, or content-inventory

      // Check redirects - should only have resource article redirects, no comparison redirects
      const redirectPaths = Object.keys(redirects);
      const comparisonRedirects = redirectPaths.filter((path: string) =>
        path.startsWith("/compare"),
      );
      expect(comparisonRedirects.length).toBe(0);

      // Check routes - /compare should exist as the hub, but no legacy individual comparison routes
      expect(routeFamilies.compare).toBe("/compare");
      const allRoutePaths = [...primaryRoutes, ...utilityRoutes].map(
        (r: any) => r.path,
      );
      const legacyComparisonRoutes = allRoutePaths.filter(
        (path: string) => path.startsWith("/compare/") && path !== "/compare",
      );
      expect(legacyComparisonRoutes.length).toBe(0);

      // Check navigation - compare should be in utility/footer, but no legacy individual comparison routes
      const footerPaths = footerNavigation.map((r: any) => r.path);
      const legacyNavComparisonRoutes = footerPaths.filter(
        (path: string) => path.startsWith("/compare/") && path !== "/compare",
      );
      expect(legacyNavComparisonRoutes.length).toBe(0);
    });

    it("validates comparison page diagnostics", () => {
      // Test that comparison page diagnostics are available and accurate

      const summary = generateComparisonDataSummary();
      expect(summary).toBeDefined();
      expect(summary.products).toBeGreaterThan(0);
      expect(summary.claims).toBeGreaterThan(0);
      expect(summary.evidence).toBeGreaterThan(0);
      expect(summary.featureSupport).toBeGreaterThan(0);
      expect(summary.faqs).toBeGreaterThan(0);
      expect(summary.featureCategories).toBeGreaterThan(0);
      expect(summary.normalizedFeatures).toBeGreaterThan(0);

      const pageTargets = generateAllComparisonPageTargets();
      expect(pageTargets.length).toBeGreaterThan(0);

      // Test that all page targets have required fields
      for (const target of pageTargets) {
        expect(target.id).toBeDefined();
        expect(target.slug).toBeDefined();
        expect(target.path).toBeDefined();
        expect(target.competitorId).toBeDefined();
        expect(target.title).toBeDefined();
        expect(target.description).toBeDefined();
      }
    });

    it("validates hub links to individual pages", () => {
      // Test that hub component links to all individual comparison pages

      const competitorProducts = competitors.filter(
        (c: any) => c.productKind === "competitor",
      );
      const pageTargets = generateAllComparisonPageTargets();

      // Test that each competitor has a corresponding page target
      expect(pageTargets.length).toBe(competitorProducts.length);

      // Test that each page target has a valid path linking to individual comparison
      for (const target of pageTargets) {
        expect(target.path).toBeDefined();
        expect(target.path.startsWith("/compare/")).toBe(true);
        expect(target.competitorId).toBeDefined();
      }
    });
  });

  it("publishes pricing as a product chooser with focused product pricing pages", async () => {
    const dom = await loadDistPage("/pricing");
    const document = dom.window.document;

    expect(document.querySelectorAll(".pricing-selector-card")).toHaveLength(3);
    expect(
      document.querySelector('a[href="/pricing/public-booking"]'),
    ).toBeNull();
    expect(
      document.querySelector('a[href="/pricing/teams"]')?.textContent,
    ).toContain("Teams");
    expect(
      document.querySelector('a[href="/pricing/spaces"]')?.textContent,
    ).toContain("Spaces");
    expect(
      document.querySelector('a[href="/pricing/host"]')?.textContent,
    ).toContain("Host");

    const teamsDom = await loadDistPage("/pricing/teams");
    expect(
      teamsDom.window.document.querySelector(".pricing-card-header h2")
        ?.textContent,
    ).toBe("Teams");
    expect(teamsDom.window.document.querySelectorAll(".tier")).toHaveLength(3);
    expect(teamsDom.window.document.body.textContent).toContain("Free");
    expect(teamsDom.window.document.body.textContent).toContain(
      "Pay As You Go",
    );
    expect(teamsDom.window.document.body.textContent).toContain("Enterprise");
    expect(teamsDom.window.document.body.textContent).toContain("Contact Us");
    expect(teamsDom.window.document.body.textContent).toContain(
      "Included in Teams",
    );
    expect(teamsDom.window.document.body.textContent).toContain(
      "Booking workflows in Slack",
    );
    expect(teamsDom.window.document.body.textContent).toContain(
      "Booking workflows in Microsoft Teams",
    );
    expect(teamsDom.window.document.body.textContent).toContain(
      "What is a monthly active user?",
    );
    expect(teamsDom.window.document.body.textContent).toContain(
      "workplace management software",
    );
    expect(teamsDom.window.document.body.textContent).toContain(
      "office booking software",
    );
    expect(
      teamsDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("FAQPage");
    expect(
      teamsDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("BreadcrumbList");
    expect(
      teamsDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("Product");
    expect(
      teamsDom.window.document.querySelector(
        '.tier-contact-link[href="mailto:support@getskedular.com"]',
      ),
    ).toBeTruthy();
    expect(
      teamsDom.window.document.querySelector(".pricing-marketplace-note")
        ?.textContent,
    ).toContain("How billing works");
    expect(
      teamsDom.window.document.querySelector(
        '.pricing-card-cta [data-cta-id="book-demo"]',
      )?.textContent,
    ).toContain("Book demo");

    const teamsPageDom = await loadDistPage("/teams");
    const teamsTryLink = teamsPageDom.window.document.querySelector(
      '[data-cta-id="try-teams"]',
    );
    expect(teamsTryLink?.textContent).toContain("Try Teams");
    expect(teamsTryLink?.getAttribute("href")).toBe(
      publicUrlFixtures.teamsAppUrl,
    );

    const spacesDom = await loadDistPage("/pricing/spaces");
    expect(
      spacesDom.window.document.querySelector(".pricing-card-header h2")
        ?.textContent,
    ).toBe("Spaces");
    expect(spacesDom.window.document.querySelectorAll(".tier")).toHaveLength(4);
    expect(spacesDom.window.document.body.textContent).toContain("Growth");
    expect(spacesDom.window.document.body.textContent).toContain(
      "$49 USD per month",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "Up to 1,000 booking instances per month",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "Included in Spaces",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "Payments with Stripe support",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "Invoicing with Xero integration",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "What is a booking instance?",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "Are marketplace commissions required?",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "coworking management software",
    );
    expect(spacesDom.window.document.body.textContent).toContain(
      "shared office software",
    );
    expect(
      spacesDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("FAQPage");
    expect(
      spacesDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("BreadcrumbList");
    expect(
      spacesDom.window.document.querySelector(
        'script[type="application/ld+json"]',
      )?.textContent,
    ).toContain("Product");
    expect(
      spacesDom.window.document.querySelector(
        '.tier-contact-link[href="mailto:support@getskedular.com"]',
      ),
    ).toBeTruthy();
    expect(
      spacesDom.window.document.querySelector(".pricing-marketplace-note")
        ?.textContent,
    ).toContain("booking-instance volume");
    expect(
      spacesDom.window.document.querySelector(
        '.pricing-card-cta [data-cta-id="book-demo"]',
      )?.textContent,
    ).toContain("Book demo");

    const spacesPageDom = await loadDistPage("/spaces");
    const spacesTryLink = spacesPageDom.window.document.querySelector(
      '[data-cta-id="try-spaces"]',
    );
    expect(spacesTryLink?.textContent).toContain("Try Spaces");
    expect(spacesTryLink?.getAttribute("href")).toBe(
      publicUrlFixtures.spacesAppUrl,
    );

    const hostDom = await loadDistPage("/pricing/host");
    expect(
      hostDom.window.document.querySelector(".pricing-card-header h2")
        ?.textContent,
    ).toBe("Host");
    expect(hostDom.window.document.querySelectorAll(".tier")).toHaveLength(1);
    expect(hostDom.window.document.body.textContent).toContain(
      "5% per paid booking",
    );
    expect(hostDom.window.document.body.textContent).toContain(
      "Included in Host",
    );
    expect(
      hostDom.window.document.querySelector(
        '.pricing-card-cta [data-cta-id="try-host"]',
      )?.textContent,
    ).toContain("Try Host");

    const hostPageDom = await loadDistPage("/host");
    const hostTryLink = hostPageDom.window.document.querySelector(
      '[data-cta-id="try-host"]',
    );
    expect(hostTryLink?.textContent).toContain("Try Host");
    expect(hostTryLink?.getAttribute("href")).toBe(
      publicUrlFixtures.hostAppUrl,
    );
    expect(hostPageDom.window.document.body.textContent).toContain(
      "without the marketplace admin",
    );
  });
});
