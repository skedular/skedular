import { describe, expect, it } from "vitest";
import {
  pricingCatalog,
  toPricingPageModels,
} from "../src/data/pricing-catalog/pricing-catalog";

describe("pricing catalog adapter", () => {
  it("renders Teams pricing plans from catalog-shaped data", () => {
    const models = toPricingPageModels();
    const teams = models.find((model) => model.id === "teams");

    expect(pricingCatalog.version).toBe("TEAMS_V1_SPACES_V1");
    expect(teams?.tiers.map((tier) => tier.name)).toEqual([
      "Free",
      "Pay As You Go",
      "Enterprise",
    ]);
    expect(teams?.tiers[0]?.price).toBe("Free");
    expect(teams?.tiers[1]?.price).toBe("$3 USD per active user/month");
    expect(teams?.tiers[2]?.price).toBe("Contact Us");
  });

  it("renders Spaces pricing plans from the merged Spaces catalog", () => {
    const models = toPricingPageModels();
    const spaces = models.find((model) => model.id === "spaces");
    const teams = models.find((model) => model.id === "teams");

    expect(spaces?.tiers.map((tier) => tier.name)).toEqual([
      "14-day free trial",
      "Growth",
      "Business",
      "Contact Us",
    ]);
    expect(spaces?.tiers[0]?.price).toBe("Free");
    expect(spaces?.tiers[0]?.highlights).toContain(
      "Up to 100 booking instances per month",
    );
    expect(spaces?.tiers[1]?.price).toBe("$49 USD per month");
    expect(spaces?.tiers[2]?.price).toBe("$149 USD per month");
    expect(spaces?.tiers[3]?.price).toBe("Contact Us");
    expect(spaces?.tiers[1]?.highlights).toContain(
      "Up to 500 booking instances per month",
    );
    expect(teams?.tiers.map((tier) => tier.name)).toEqual([
      "Free",
      "Pay As You Go",
      "Enterprise",
    ]);
  });
});
