export const pricingPage = {
  title: "Skedular Pricing | Teams and Spaces",
  description: "Choose Skedular Teams or Skedular Spaces pricing based on how your organization manages workspace.",
  reviewStatus: "pending-business-approval",
  models: [
    {
      id: "teams",
      name: "Teams",
      audience: "Private organizations managing employees and workplace resources",
      basis: "Active-user pricing",
      tiers: [
        { name: "Starter", price: "$3 per active user / month", summary: "Desk and room booking for smaller teams." },
        { name: "Business", price: "$6 per active user / month", summary: "Hybrid coordination, integrations, and reporting." },
        { name: "Enterprise", price: "Custom", summary: "Advanced identity, security, procurement, and support needs." },
      ],
      ctaId: "book-demo",
    },
    {
      id: "spaces",
      name: "Spaces",
      audience: "Workspace operators and flexible workspace providers",
      basis: "Location subscription",
      tiers: [
        { name: "Launch", price: "$79 per location / month", summary: "Publish and manage a small workspace catalog." },
        { name: "Grow", price: "$199 per location / month", summary: "Payments, billing, invoices, and richer operations." },
        { name: "Scale", price: "Custom", summary: "Multi-location operations, custom domains, and advanced workflows." },
      ],
      ctaId: "contact-sales",
    },
    {
      id: "hosts",
      name: "Hosts",
      audience: "Operators publishing inventory for marketplace bookings",
      basis: "Commission where marketplace bookings apply",
      tiers: [
        { name: "Marketplace host", price: "10% to 15% commission range", summary: "Commission applies to eligible public marketplace bookings." },
      ],
      ctaId: "book-demo",
    },
  ],
};
