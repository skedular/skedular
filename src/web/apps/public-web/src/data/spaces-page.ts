import type { ProductPageContent } from "./content-types";

export const spacesPage: ProductPageContent = {
  id: "spaces",
  eyebrow: "Skedular Spaces",
  title: "Run, sell, and manage flexible workspace",
  summary:
    "Skedular Spaces helps co-working operators and shared-office providers manage inventory, package products, accept bookings, bill customers, issue invoices, and publish spaces.",
  audience: "Co-working operators, flexible workspace providers, shared-office teams, venue owners, and hosts.",
  features: [
    {
      title: "Core workspace management",
      body: "Everything you need to run your flexible workspace operation.",
      featureBlocks: [
        {
          title: "Resource management",
          description: "Model the spaces and resources customers can book.",
          items: ["Desks", "Rooms", "Event spaces", "Private offices", "Equipment", "Zones and tags"],
          accent: "emerald",
        },
        {
          title: "Product management",
          description: "Package resources into products that fit how customers actually buy workspace.",
          items: ["Product catalog", "Dynamic product matching", "Images", "Amenities", "Visibility controls"],
          accent: "emerald",
        },
        {
          title: "Payments and billing",
          description: "Support commercial workflows without forcing operators into a single billing model.",
          items: ["Card payments", "Tax handling", "Billing cadence", "Invoicing", "Subscriptions", "Cancellation policies"],
          accent: "emerald",
        },
        {
          title: "Publishing and brand",
          description: "Help the right audience discover the right space while keeping operator context intact.",
          items: ["Marketplace publishing", "Host model", "Custom domains", "Branded listings", "Opening hours", "Maps"],
          accent: "emerald",
        },
      ],
    },
  ],
  faq: [
    {
      question: "What types of spaces can I manage with Skedular Spaces?",
      answer:
        "Skedular Spaces supports all types of flexible workspace including hot desks, dedicated desks, private offices, meeting rooms, event spaces, and equipment bookings. You can model any resource type that customers need to book.",
    },
    {
      question: "How does billing and invoicing work?",
      answer:
        "Skedular Spaces supports multiple billing models including one-time bookings, recurring subscriptions, and custom billing cadences. We handle tax calculations, invoice generation, and payment processing so you can focus on running your space.",
    },
    {
      question: "Can I publish my spaces to a marketplace?",
      answer:
        "Yes, Skedular Spaces includes marketplace publishing capabilities. You can list your spaces on our marketplace, use your own custom domain, or both. The platform maintains your brand context while helping customers discover your spaces.",
    },
    {
      question: "What payment methods do you support?",
      answer:
        "We support major credit and debit cards through our payment processing integration. The platform handles payment collection, refunds, and reconciliation automatically.",
    },
  ],
};
