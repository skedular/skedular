import { describe, expect, it } from "vitest";
import { toPricingPageModels } from "../src/data/pricing-catalog/pricing-catalog";

describe("pricing product chooser", () => {
  it("renders product chooser entries from catalog-shaped data", () => {
    const models = toPricingPageModels();

    expect(models.map((model) => model.id)).toEqual([
      "teams",
      "spaces",
      "host",
    ]);
    expect(
      models
        .find((model) => model.id === "spaces")
        ?.tiers.map((tier) => tier.name),
    ).toEqual(["14-day free trial", "Growth", "Business", "Contact Us"]);
    expect(models.find((model) => model.id === "host")?.tiers).toEqual([
      expect.objectContaining({ name: "Host", price: "5% per paid booking" }),
    ]);
  });
});
