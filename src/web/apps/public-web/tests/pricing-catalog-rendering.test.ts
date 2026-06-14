import { describe, expect, it } from "vitest";
import { pricingCatalog, toPricingPageModels } from "../src/data/pricing-catalog/pricing-catalog";

describe("pricing catalog adapter", () => {
  it("renders Teams pricing plans from catalog-shaped data", () => {
    const models = toPricingPageModels();
    const teams = models.find((model) => model.id === "teams");

    expect(pricingCatalog.version).toBe("TEAMS_V1");
    expect(teams?.tiers.map((tier) => tier.name)).toEqual(["Free", "Pay As You Go", "Enterprise"]);
    expect(teams?.tiers[0]?.price).toBe("Free");
    expect(teams?.tiers[1]?.price).toBe("$3 per active user / month");
    expect(teams?.tiers[2]?.price).toBe("Contact Us");
  });
});
