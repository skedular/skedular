import { readFile } from "node:fs/promises";
import { spawnSync } from "node:child_process";
import { JSDOM } from "jsdom";
import { getAllByRole, getByRole, getByText } from "@testing-library/dom";
import axe from "axe-core";
import { beforeAll, describe, expect, it } from "vitest";

const signupUrl = "https://app.example.test/sign-up?source=public-web";

beforeAll(() => {
  const result = spawnSync("pnpm", ["build"], {
    cwd: process.cwd(),
    env: { ...process.env, PUBLIC_SKEDULAR_SIGNUP_URL: signupUrl },
    encoding: "utf8",
  });

  expect(result.status, `${result.stdout}\n${result.stderr}`).toBe(0);
});

async function loadPage() {
  const html = await readFile(new URL("../dist/index.html", import.meta.url), "utf8");
  return new JSDOM(html, { url: "https://www.example.test/" });
}

describe("public website home page", () => {
  it("explains Skedular and links the primary CTA to the configured sign-up URL", async () => {
    const dom = await loadPage();
    const document = dom.window.document;

    expect(getByRole(document, "heading", { level: 1, name: "Make every workspace work better" })).toBeTruthy();
    expect(getByText(document, /desks, rooms, and hybrid teams/i)).toBeTruthy();
    expect(getAllByRole(document, "link", { name: "Get Started" }).every((link) => link.getAttribute("href") === signupUrl)).toBe(true);
  });

  it("uses semantic landmarks, one page-level heading, descriptive links, and has no critical axe violations", async () => {
    const dom = await loadPage();
    const document = dom.window.document;

    expect(getByRole(document, "banner")).toBeTruthy();
    expect(getByRole(document, "main")).toBeTruthy();
    expect(getByRole(document, "contentinfo")).toBeTruthy();
    expect(document.querySelectorAll("h1")).toHaveLength(1);
    expect(getAllByRole(document, "link").every((link) => link.getAttribute("aria-label") || link.textContent?.trim())).toBe(true);

    const results = await axe.run(document.documentElement, {
      rules: {
        "color-contrast": { enabled: false },
      },
    });
    expect(results.violations.filter((violation) => violation.impact === "critical")).toEqual([]);
  });
});
