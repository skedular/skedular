import { readFile } from "node:fs/promises";
import { spawnSync } from "node:child_process";
import { JSDOM } from "jsdom";
import { getAllByRole, getAllByText, getByRole } from "@testing-library/dom";
import axe from "axe-core";
import { beforeAll, describe, expect, it } from "vitest";
import { publicUrlEnvironment } from "./public-url-fixtures";

beforeAll(() => {
  const result = spawnSync("pnpm", ["build"], {
    cwd: process.cwd(),
    env: { ...process.env, ...publicUrlEnvironment },
    encoding: "utf8",
  });

  expect(result.status, `${result.stdout}\n${result.stderr}`).toBe(0);
});

async function loadPage() {
  const html = await readFile(new URL("../dist/index.html", import.meta.url), "utf8");
  return new JSDOM(html, { url: "https://www.example.test/" });
}

describe("public website home page", () => {
  it("presents the Teams and Spaces product paths", async () => {
    const dom = await loadPage();
    const document = dom.window.document;

    expect(getByRole(document, "heading", { level: 1, name: "One workspace platform for teams and operators" })).toBeTruthy();
    expect(document.body.textContent).not.toContain("Public Booking");
    expect(getByRole(document, "heading", { level: 3, name: "Skedular Teams" })).toBeTruthy();
    expect(getByRole(document, "heading", { level: 3, name: "Skedular Spaces" })).toBeTruthy();
    expect(getAllByText(document, "Resources").length).toBeGreaterThan(0);
    expect(getAllByRole(document, "link", { name: "Blog" }).some((link) => link.getAttribute("href") === "/blog")).toBe(true);

    expect(getAllByRole(document, "link", { name: "Explore products" }).some((link) => link.getAttribute("href") === "#products")).toBe(true);
  });

  it("uses semantic landmarks, one page-level heading, descriptive links, metadata, and has no critical axe violations", async () => {
    const dom = await loadPage();
    const document = dom.window.document;

    expect(getByRole(document, "banner")).toBeTruthy();
    expect(getByRole(document, "navigation", { name: "Primary navigation" })).toBeTruthy();
    expect(getByRole(document, "main")).toBeTruthy();
    expect(getByRole(document, "contentinfo")).toBeTruthy();
    expect(document.querySelectorAll("h1")).toHaveLength(1);
    expect(document.querySelector("title")?.textContent).toContain("Find and book workspace");
    expect(document.querySelector('meta[name="description"]')?.getAttribute("content")).toMatch(/meeting rooms/i);
    expect(getAllByRole(document, "link").every((link) => link.getAttribute("aria-label") || link.textContent?.trim())).toBe(true);

    const results = await axe.run(document.documentElement, {
      rules: {
        "color-contrast": { enabled: false },
      },
    });
    expect(results.violations.filter((violation) => violation.impact === "critical")).toEqual([]);
  });
});
