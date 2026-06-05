import { describe, expect, it } from "vitest";
import { analyticsReadiness } from "../src/data/analytics-readiness";
import { capabilityClaims, competitorClaimReview } from "../src/data/claim-review";
import { comparisonPages } from "../src/data/comparison-pages";
import { supportArticles, resourceArticles } from "../src/data/current-public-content";
import { draftCoverageItems } from "../src/data/draft-coverage";
import { futureFeatures } from "../src/data/future-features";
import { launchReviewChecklist, manualReviewProtocol } from "../src/data/launch-review";
import { legalPages } from "../src/data/legal-pages";
import { redirects } from "../src/data/redirects";
import { sourceAudit } from "../src/data/source-audit";

describe("public website content inventories", () => {
  it("tracks all current public resource and support content with destinations or redirects", () => {
    for (const article of [...resourceArticles, ...supportArticles]) {
      expect(article.sourceUrl).toMatch(/^https:\/\/getascheduler\.com/);
      expect(article.destinationPath || article.redirectTargetPath).toBeTruthy();
      expect(Object.values(redirects)).toContain(article.destinationPath);
    }
  });

  it("covers every major draft heading and routes future items out of current-state copy", () => {
    expect(draftCoverageItems.length).toBeGreaterThanOrEqual(45);
    expect(draftCoverageItems.every((item) => item.heading && item.destinationRef && item.decision)).toBe(true);
    expect(futureFeatures.every((item) => item.status === "future-planning")).toBe(true);
  });

  it("keeps comparison and capability claims in reviewable inventories", () => {
    expect(comparisonPages.length).toBeGreaterThanOrEqual(3);
    expect(comparisonPages.every((page) => page.competitorReviewStatus === "pending")).toBe(true);
    expect(capabilityClaims.length).toBeGreaterThanOrEqual(4);
    expect(competitorClaimReview.notes.length).toBeGreaterThan(0);
  });

  it("tracks migrated company and legal pages with source URLs and review status", () => {
    expect(legalPages).toHaveLength(2);
    expect(legalPages.every((page) => page.sourceUrl.startsWith("https://getskedular.com/"))).toBe(true);
    expect(legalPages.every((page) => page.reviewStatus === "pending")).toBe(true);
  });

  it("defines launch review, source audit, and privacy-safe analytics readiness", () => {
    expect(manualReviewProtocol.successThreshold).toBe(0.9);
    expect(manualReviewProtocol.participantCount).toBeGreaterThanOrEqual(10);
    expect(Object.values(launchReviewChecklist).every((item) => item.status === "pending")).toBe(true);
    expect(sourceAudit.length).toBeGreaterThanOrEqual(4);
    expect(analyticsReadiness.trackingEnabled).toBe(false);
    expect(analyticsReadiness.vendor).toBe("none");
  });
});
