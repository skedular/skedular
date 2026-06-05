import { readFile } from "node:fs/promises";
import { existsSync } from "node:fs";
import { spawnSync } from "node:child_process";
import { JSDOM } from "jsdom";
import { beforeAll, describe, expect, it } from "vitest";
import { comparisonPages } from "../src/data/comparison-pages";
import { supportArticles, resourceArticles } from "../src/data/current-public-content";
import { featurePages } from "../src/data/feature-pages";
import { publicPages } from "../src/data/content-inventory";
import { publicUrlFixtures } from "./public-url-fixtures";
import { publicUrlEnvironment } from "./public-url-fixtures";

beforeAll(() => {
  const result = spawnSync("pnpm", ["build"], {
    cwd: process.cwd(),
    env: { ...process.env, ...publicUrlEnvironment },
    encoding: "utf8",
  });

  expect(result.status, `${result.stdout}\n${result.stderr}`).toBe(0);
});

async function loadDistPage(path: string) {
  const filePath = path === "/" ? "../dist/index.html" : `../dist${path}/index.html`;
  const html = await readFile(new URL(filePath, import.meta.url), "utf8");
  return new JSDOM(html, { url: `https://www.example.test${path}` });
}

const primaryPaths = [
  "/",
  "/teams",
  "/spaces",
  "/pricing",
  "/pricing/teams",
  "/pricing/spaces",
  "/blog",
  "/resources",
  "/support",
  "/company",
  "/terms-of-service",
  "/privacy-policy",
];

describe("expanded public site content", () => {
  it.each(primaryPaths)("publishes %s with one h1, metadata, canonical URL, landmarks, and CTA links", async (path) => {
    const dom = await loadDistPage(path);
    const document = dom.window.document;

    expect(document.querySelectorAll("h1")).toHaveLength(1);
    expect(document.querySelector("title")?.textContent?.trim()).not.toEqual("");
    expect(document.querySelector('meta[name="description"]')?.getAttribute("content")).toBeTruthy();
    expect(document.querySelector('link[rel="canonical"]')?.getAttribute("href")).toContain(path);
    expect(document.querySelector("header")).toBeTruthy();
    expect(document.querySelector("main")).toBeTruthy();
    expect(document.querySelector("footer")).toBeTruthy();
    expect(document.querySelectorAll("[data-cta-id]").length).toBeGreaterThan(0);
  });

  it("publishes all resource, support, feature, comparison, company, and legal routes", () => {
    for (const article of [...resourceArticles, ...supportArticles]) {
      expect(existsSync(new URL(`../dist${article.destinationPath}/index.html`, import.meta.url))).toBe(true);
    }

    for (const page of [...featurePages, ...comparisonPages]) {
      expect(existsSync(new URL(`../dist${page.path}/index.html`, import.meta.url))).toBe(true);
    }

    for (const path of ["/company", "/terms-of-service", "/privacy-policy"]) {
      expect(existsSync(new URL(`../dist${path}/index.html`, import.meta.url))).toBe(true);
    }
  });

  it("keeps destination URLs environment-sourced and avoids hardcoded staging or production domains in source content", async () => {
    const html = await readFile(new URL("../dist/index.html", import.meta.url), "utf8");

    expect(html).toContain(publicUrlFixtures.appUrl);
    expect(html).toContain(publicUrlFixtures.signupUrl);
    expect(html).toContain(publicUrlFixtures.demoUrl);
    expect(html).not.toContain("https://skedular.app");
    expect(html).not.toContain("https://staging.skedular.app");
  });

  it("has unique public page metadata and complete comparison metadata", () => {
    const titles = new Set(publicPages.map((page) => page.title));
    const descriptions = new Set(publicPages.map((page) => page.description));

    expect(titles.size).toBe(publicPages.length);
    expect(descriptions.size).toBe(publicPages.length);
    expect(comparisonPages.every((page) => page.title && page.description && page.competitorName)).toBe(true);
  });

  it("publishes organized footer social and community links", async () => {
    const dom = await loadDistPage("/");
    const document = dom.window.document;

    expect(document.querySelectorAll(".footer-social-button")).toHaveLength(4);
    expect(document.querySelector('a[href="https://www.linkedin.com/company/getskedular/"]')).toBeTruthy();
    expect(document.querySelector('a[href="https://www.facebook.com/profile.php?id=61571588471440"]')).toBeTruthy();
    expect(document.querySelector('a[href="https://discord.gg/kBczX24y"]')).toBeTruthy();
    expect(document.querySelector('a[href^="https://betalist.com/startups/skedular"]')).toBeTruthy();
  });

  it("publishes legal pages as source-preserving legal documents", async () => {
    const termsDom = await loadDistPage("/terms-of-service");
    const termsText = termsDom.window.document.querySelector(".legal-document")?.textContent ?? "";

    expect(termsText).toContain("SKEDULAR ORDER FORM");
    expect(termsText).toContain("By accessing and using the Services, you represent");
    expect(termsText).toContain("SKEDULAR DATA PROCESSING ADDENDUM");
    expect(termsText).toContain("Technical and Organisational Security Measures");

    const privacyDom = await loadDistPage("/privacy-policy");
    const privacyText = privacyDom.window.document.querySelector(".legal-document")?.textContent ?? "";

    expect(privacyText).toContain("Welcome to our privacy policy. We respect your privacy");
    expect(privacyText).toContain("Skedular Limited, trading as Skedular, is the data controller");
    expect(privacyText).toContain("The data we collect about you");
    expect(privacyText).toContain("Your legal rights");
  });

  it("places become-a-host before login and uses its own public URL", async () => {
    const dom = await loadDistPage("/");
    const document = dom.window.document;
    const headerLinks = [...document.querySelectorAll(".header-actions a")].map((link) => ({
      ctaId: link.getAttribute("data-cta-id"),
      href: link.getAttribute("href"),
    }));

    expect(headerLinks.map((link) => link.ctaId)).toEqual(["become-host", "login", "book-demo"]);
    expect(headerLinks[0]?.href).toBe(publicUrlFixtures.becomeHostUrl);
    expect(headerLinks[1]?.href).toBe(publicUrlFixtures.signupUrl);
  });

  it("publishes pricing as a product chooser with focused product pricing pages", async () => {
    const dom = await loadDistPage("/pricing");
    const document = dom.window.document;

    expect(document.querySelectorAll(".pricing-selector-card")).toHaveLength(2);
    expect(document.querySelector('a[href="/pricing/public-booking"]')).toBeNull();
    expect(document.querySelector('a[href="/pricing/teams"]')?.textContent).toContain("Teams");
    expect(document.querySelector('a[href="/pricing/spaces"]')?.textContent).toContain("Spaces");

    const teamsDom = await loadDistPage("/pricing/teams");
    expect(teamsDom.window.document.querySelector(".pricing-card-header h2")?.textContent).toBe("Teams");
    expect(teamsDom.window.document.querySelectorAll(".tier")).toHaveLength(3);
    expect(teamsDom.window.document.querySelector(".pricing-marketplace-note")).toBeNull();

    const spacesDom = await loadDistPage("/pricing/spaces");
    expect(spacesDom.window.document.querySelector(".pricing-card-header h2")?.textContent).toBe("Spaces");
    expect(spacesDom.window.document.querySelector(".pricing-marketplace-note")?.textContent).toContain("Hosts");
  });
});
