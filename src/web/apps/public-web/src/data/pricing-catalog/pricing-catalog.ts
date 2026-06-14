type CatalogPlan = {
  code: "free" | "pay-as-you-go" | "enterprise-capacity";
  name: string;
  description: string;
  prices: { amount: number; currency: string; cadence: string }[];
  capacityOptions: { label: string; amount?: number; currency?: string; cadence?: string; availability: "self-service" | "contact-us" }[];
};

type CatalogProduct = {
  id: "teams" | "spaces" | "hosts";
  name: string;
  audience: string;
  basis: string;
  ctaId: "book-demo" | "contact-sales";
  plans: CatalogPlan[];
};

export const pricingCatalog = {
  version: "TEAMS_V1",
  products: [
    {
      id: "teams",
      name: "Teams",
      audience: "Private organizations managing employees and workplace resources",
      basis: "Active-user pricing",
      ctaId: "book-demo",
      plans: [
        {
          code: "free",
          name: "Free",
          description: "Core workplace booking for one team, one location, and up to 10 monthly active users.",
          prices: [{ amount: 0, currency: "USD", cadence: "month" }],
          capacityOptions: [],
        },
        {
          code: "pay-as-you-go",
          name: "Pay As You Go",
          description: "Unlimited teams and locations with usage-based monthly active-user billing.",
          prices: [{ amount: 3, currency: "USD", cadence: "active user / month" }],
          capacityOptions: [],
        },
        {
          code: "enterprise-capacity",
          name: "Enterprise",
          description: "Negotiated active-user pricing and monthly capacity for larger Teams organizations.",
          prices: [],
          capacityOptions: [{ label: "Contact Us", availability: "contact-us" }],
        },
      ],
    },
    {
      id: "spaces",
      name: "Spaces",
      audience: "Workspace operators and flexible workspace providers",
      basis: "Location subscription",
      ctaId: "contact-sales",
      plans: [
        {
          code: "enterprise-capacity",
          name: "Spaces framework",
          description: "Spaces pricing is represented in the catalog framework and remains sales-led for this slice.",
          prices: [],
          capacityOptions: [{ label: "Contact Us", availability: "contact-us" }],
        },
      ],
    },
    {
      id: "hosts",
      name: "Hosts",
      audience: "Operators publishing inventory for marketplace bookings",
      basis: "Commission where marketplace bookings apply",
      ctaId: "book-demo",
      plans: [
        {
          code: "pay-as-you-go",
          name: "Marketplace host",
          description: "Commission applies to eligible public marketplace bookings.",
          prices: [{ amount: 10, currency: "PERCENT_RANGE", cadence: "to 15% commission range" }],
          capacityOptions: [],
        },
      ],
    },
  ] satisfies CatalogProduct[],
};

export function formatCatalogPlanPrice(plan: CatalogPlan) {
  const price = plan.prices[0];
  if (price) {
    if (price.currency === "PERCENT_RANGE") {
      return `${price.amount}% ${price.cadence}`;
    }

    if (price.amount === 0) {
      return "Free";
    }

    return `$${price.amount} per ${price.cadence}`;
  }

  const contactUs = plan.capacityOptions.some((option) => option.availability === "contact-us");
  return contactUs ? "Contact Us" : "";
}

export function toPricingPageModels() {
  return pricingCatalog.products.map((product) => ({
    id: product.id,
    name: product.name,
    audience: product.audience,
    basis: product.basis,
    tiers: product.plans.map((plan) => ({
      name: plan.name,
      price: formatCatalogPlanPrice(plan),
      summary: plan.description,
    })),
    ctaId: product.ctaId,
  }));
}
