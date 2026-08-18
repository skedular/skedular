import { describe, expect, it } from "vitest";
import { GET as getLlms } from "../src/pages/llms.txt";
import { GET as getLlmsFull } from "../src/pages/llms-full.txt";

describe("machine-readable Spaces pricing", () => {
  it("describes the Free plan as a 14-day trial with its existing booking limit", async () => {
    const [summary, full] = await Promise.all([
      getLlms().text(),
      getLlmsFull().text(),
    ]);

    expect(summary).toContain("14-day free trial");
    expect(summary).toContain("not a permanent free tier");
    expect(full).toContain("100 booking instances per month");
  });

  it("describes Skedular Host and its commission model", async () => {
    const [summary, full] = await Promise.all([
      getLlms().text(),
      getLlmsFull().text(),
    ]);

    expect(summary).toContain("Skedular Host");
    expect(summary).toContain("5% commission");
    expect(summary).toContain("no setup fee");
    expect(summary).toContain("Stripe Connect");
    expect(full).toContain("private product draft");
    expect(full).toContain("paid card booking");
  });

  it("documents the Host place-first listing and publication lifecycle", async () => {
    const summary = await getLlms().text();

    expect(summary).toContain("## Host Place-First Model");
    expect(summary).toContain("## Host Platform Model");
    expect(summary).toContain("## Host Listing Lifecycle");
    expect(summary).toContain("## Host Publication Workflow");
    expect(summary).toContain("Automatic Booking Setup");
    expect(summary).toContain("Private Listing Draft");
    expect(summary).toContain("Host Verification");
    expect(summary).toContain("explicitly activates it");
  });

  it("documents shared product relationships and supported booking models", async () => {
    const summary = await getLlms().text();

    expect(summary).toContain("## Product Relationships");
    expect(summary).toContain("**Place**:");
    expect(summary).toContain("Discovery → Availability → Selection");
    expect(summary).toContain("## Booking Types");
    expect(summary).toContain("respecting availability rules");
    expect(summary).toContain("## Common Use Cases");
  });

  it("documents booking credits in both machine-readable formats", async () => {
    const [summary, full] = await Promise.all([
      getLlms().text(),
      getLlmsFull().text(),
    ]);

    expect(summary).toContain("**Booking credits**: Prepaid usage");
    expect(summary).toContain(
      "Customers buy a defined number of prepaid credits",
    );
    expect(full).toContain("## Booking Credits");
    expect(full).toContain("does not create a Booking, reserve a resource");
    expect(full).toContain(
      "A failed renewal or unavailable compatible pricing does not grant a new cycle",
    );
  });
});
