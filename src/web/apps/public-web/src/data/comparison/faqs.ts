import type { FAQEntry } from "../content-types";

// Shared comparison FAQ records and schema eligibility flags
// FAQ schema is emitted only for visible, approved, schema-eligible FAQs
// FAQ answers must not contain unsupported competitor or Skedular claims

export const comparisonFAQs: FAQEntry[] = [
  {
    id: "faq-why-choose-skedular",
    question: "Why choose Skedular over other workspace booking platforms?",
    answer:
      "Skedular provides a unified platform that serves both teams managing private workplaces and workspace operators running commercial spaces. Unlike competitors that focus on only one side of the equation, Skedular supports discovery, booking, operations, billing, subscriptions, and payments in one system. This means organizations don't need separate tools for different workplace needs, and workspace operators can manage their entire business without platform fragmentation.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-skedular-vs-coworking-platforms",
    question:
      "How does Skedular compare to coworking-focused platforms like Skedda or Nexudus?",
    answer:
      "While coworking-focused platforms excel at member management and billing for coworking spaces, Skedular extends beyond coworking to support private workplace management, public marketplace discovery, and a broader range of booking types including events, parking, and custom resources. Skedular also provides native marketplace capabilities for workspace operators to publish their inventory publicly.",
    relatedPageIds: [],
    claimRefs: ["skedda-coworking-focus", "nexudus-coworking-comprehensive"],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-skedular-vs-hybrid-platforms",
    question:
      "How does Skedular compare to hybrid work platforms like Robin or Gable?",
    answer:
      "Hybrid work platforms focus on workplace experience and coordination for enterprise teams. Skedular includes these capabilities but also provides comprehensive workspace operator features including marketplace publishing, subscription management, and billing automation. Organizations using Skedular can manage their internal workplace needs while also participating in or operating commercial workspace networks.",
    relatedPageIds: [],
    claimRefs: ["robin-workplace-experience", "gable-hybrid-modern"],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-pricing-model",
    question: "What is Skedular's pricing model?",
    answer:
      "Skedular offers flexible pricing models tailored to different use cases. Teams is priced on monthly active users (MAU), making it scalable for organizations of all sizes. Spaces is priced on booking instance volume, which aligns costs with actual workspace utilization. This flexible approach means organizations pay for what they use rather than fixed seat-based pricing that may not match their actual booking patterns.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-integrations",
    question: "What integrations does Skedular support?",
    answer:
      "Skedular supports key integrations for modern workplaces including Slack and Microsoft Teams for booking workflows and notifications, WorkOS for enterprise SSO, Stripe and Stripe Connect for payments, and Xero for accounting. These integrations are designed to work seamlessly with both Teams and Spaces workflows, ensuring that whether you're managing a private workplace or running a commercial space, you can connect Skedular to the tools you already use.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-marketplace",
    question: "Does Skedular include a marketplace for workspace discovery?",
    answer:
      "Yes, Skedular includes a public marketplace where workspace operators can publish their inventory for discovery by people looking for workspace. This marketplace integration means that the same inventory management system used for private workplace bookings can also power public discovery and booking, eliminating the need for separate systems or manual synchronization.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-billing-cycles",
    question: "What billing cycles does Skedular support?",
    answer:
      "Skedular supports multiple billing cycles including weekly, fortnightly (bi-weekly), and monthly billing. This flexibility allows workspace operators to choose billing cycles that match their business model and customer preferences. For example, some operators prefer weekly billing for short-term passes, while others use monthly billing for recurring memberships.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-custom-resources",
    question: "Can I define custom resource types in Skedular?",
    answer:
      "Yes, Skedular supports custom resource types beyond standard desks and rooms. This means you can define and book parking spaces, event venues, equipment, or any other bookable asset your workplace or business needs. Custom resources inherit the same booking rules, permissions, and analytics capabilities as standard resources.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-multi-location",
    question: "Does Skedular support multi-location management?",
    answer:
      "Yes, Skedular supports multi-location management for both Teams and Spaces. Organizations can manage multiple workplace locations with unified or independent configurations, and workspace operators can manage multiple venues from a single system. This multi-location support includes location-specific booking rules, pricing, and branding while maintaining centralized oversight and reporting.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-analytics",
    question: "What analytics and reporting does Skedular provide?",
    answer:
      "Skedular provides comprehensive analytics including occupancy reporting, utilization reporting, revenue reporting, and booking analytics. These analytics help organizations understand how their workspace is being used, identify booking patterns, and make data-driven decisions about space allocation and capacity planning. For workspace operators, revenue reporting helps track business performance and identify growth opportunities.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-api-webhooks",
    question: "Does Skedular provide API and webhook support?",
    answer:
      "Yes, Skedular provides a RESTful API for programmatic access and webhooks for event notifications. This allows organizations to integrate Skedular with custom systems, automate workflows, and build custom applications on top of the Skedular platform. The API and webhooks are designed to support both Teams and Spaces use cases.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: true,
    reviewStatus: "approved",
  },
  {
    id: "faq-data-driven-architecture",
    question:
      "How does Skedular ensure comparison content is accurate and evidence-based?",
    answer:
      "Skedular's comparison content is generated from a shared dataset that requires evidence or explicit review status for all claims. Skedular capabilities are only marked as supported when backed by current source references from specs, help documentation, or implemented features. Competitor claims require evidence notes or approved review status before publication. This evidence-based approach ensures that comparison content remains accurate and can be updated systematically as products evolve.",
    relatedPageIds: [],
    claimRefs: [],
    schemaEligible: false,
    reviewStatus: "approved",
  },
];
