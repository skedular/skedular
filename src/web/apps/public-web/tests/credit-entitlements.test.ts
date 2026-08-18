import { describe, expect, it } from "vitest";
import { documentationArticles } from "../src/data/documentation";
import { documentationSourceMap } from "../src/data/documentation-source-map";

describe("credit entitlement documentation", () => {
  it("publishes Spaces and Host guidance", () => {
    for (const id of [
      "spaces-credit-entitlements",
      "host-credit-entitlements",
    ]) {
      const article = documentationArticles.find((item) => item.id === id);
      expect(article).toBeDefined();
      expect(article?.publicationState).toBe("published");
      expect(article?.evidenceRefs.length).toBeGreaterThan(0);
    }
  });

  it("documents the booking-free purchase and later token workflow", () => {
    for (const id of [
      "spaces-credit-entitlements",
      "host-credit-entitlements",
    ]) {
      const article = documentationArticles.find((item) => item.id === id);
      expect(article?.title).toMatch(/Credit-based booking entitlements/);
    }

    const source = documentationSourceMap.find(
      (item) => item.id === "credit-entitlements",
    );
    expect(source?.concept).toMatch(/Credit-based booking entitlements/);
    expect(source?.summary).toMatch(/validity|expiry|refund/i);
    expect(source?.summary).toMatch(
      /Stripe card payment|bank-transfer payment/i,
    );
    expect(source?.summary).toMatch(/confirmed/i);
    expect(source?.summary).toMatch(
      /unchanged reservation-based and recurring booking behavior/i,
    );
  });
});
