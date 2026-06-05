import type { ProductPageContent } from "./content-types";

export const spacesPage: ProductPageContent = {
  id: "spaces",
  eyebrow: "Skedular Spaces",
  title: "Run, sell, and manage flexible workspace",
  summary:
    "Skedular Spaces helps co-working operators and shared-office providers manage inventory, package products, accept bookings, bill customers, issue invoices, and publish spaces.",
  audience: "Co-working operators, flexible workspace providers, shared-office teams, venue owners, and hosts.",
  sections: [
    {
      title: "Resource management",
      body: "Model the spaces and resources customers can book.",
      items: ["Desks", "Rooms", "Event spaces", "Private offices", "Equipment", "Zones and tags"],
    },
    {
      title: "Product management",
      body: "Package resources into products that fit how customers actually buy workspace.",
      items: ["Product catalog", "Dynamic product matching", "Images", "Amenities", "Visibility controls"],
    },
    {
      title: "Payments and billing",
      body: "Support commercial workflows without forcing operators into a single billing model.",
      items: ["Card payments", "Tax handling", "Billing cadence", "Invoicing", "Subscriptions", "Cancellation policies"],
    },
    {
      title: "Publishing and brand",
      body: "Help the right audience discover the right space while keeping operator context intact.",
      items: ["Marketplace publishing", "Host model", "Custom domains", "Branded listings", "Opening hours", "Maps"],
    },
  ],
};
