import { describe, expect, it } from "vitest";
import { toPricingPageModels } from "../src/data/pricing-catalog/pricing-catalog";

describe("pricing product chooser", () => {
  it("renders product chooser entries from catalog-shaped data", () => {
    const models = toPricingPageModels();

    expect(models.map((model) => model.id)).toEqual(["teams", "spaces", "hosts"]);
    expect(models.find((model) => model.id === "spaces")?.tiers[0]?.price).toBe("Contact Us");
  });
});
