import type { ComparisonProduct } from "../content-types";

// Competitor product seed records
// These provide the baseline competitor data for comparison pages
// Publication status should be "draft" until evidence/review is complete

export const competitors: ComparisonProduct[] = [
  {
    id: "skedular",
    name: "Skedular",
    slug: "skedular",
    productKind: "skedular",
    category: "workplace-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workspace operating system that helps teams and operators find, book, manage, and monetize workspace.",
    bestFor:
      "Organizations managing private workplaces and workspace operators running commercial spaces",
    strengths: [
      "Unified platform for discovery, booking, operations, billing, and subscriptions",
      "Built for both teams and operators without changing platforms",
      "Flexible business models including hourly, daily, recurring, and subscription bookings",
      "Enterprise-ready with SSO, Slack, Teams, and Xero integrations",
    ],
    limitations: [
      "Newer platform compared to some established competitors",
      "Some advanced features still in development",
    ],
    pricingNotes:
      "Flexible pricing with Teams (MAU-based) and Spaces (booking instance-based) models",
    integrationNotes:
      "Slack, Microsoft Teams, SSO via WorkOS, Xero accounting, Stripe payments",
  },
  {
    id: "skedda",
    name: "Skedda",
    slug: "skedda",
    productKind: "competitor",
    category: "workplace-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace booking platform focused on desk booking, meeting room booking, interactive floor plans, resource scheduling, visitor management, and workplace coordination.",
    bestFor:
      "Organizations that need strong workplace booking, floor plans, rules, permissions, and desk or room scheduling",
    strengths: [
      "Interactive floor plans",
      "Booking governance",
      "Rules and permissions",
      "Desk booking",
      "Meeting room booking",
      "Workplace scheduling",
      "Resource reservation",
      "Utilization reporting",
      "Visitor management",
      "Microsoft 365 integration",
      "Google Workspace integration",
      "Microsoft Teams integration",
      "Slack integration",
      "SSO and enterprise identity",
    ],
    limitations: [
      "Billing, invoicing, and subscriptions are more booking/payment oriented than a broader workspace operations billing model",
      "Not a public workspace marketplace or discovery network",
    ],
    pricingNotes: "Pricing based on number of desks and rooms",
    integrationNotes: "Basic integrations available, enterprise SSO limited",
  },
  {
    id: "officernd",
    name: "OfficeRnD",
    slug: "officernd",
    productKind: "competitor",
    category: "workplace-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform focused on hybrid work and office space optimization.",
    bestFor:
      "Enterprises focused on hybrid work optimization and space utilization",
    strengths: [
      "Strong focus on hybrid work and space utilization analytics",
      "Good enterprise features and integrations",
      "Comprehensive workplace analytics",
    ],
    limitations: [
      "Less focused on commercial workspace operators",
      "Marketplace and discovery capabilities limited",
      "Pricing can be complex for smaller organizations",
    ],
    pricingNotes:
      "Enterprise-focused pricing with per-user and location-based models",
    integrationNotes:
      "Good enterprise integrations, including major SSO providers",
  },
  {
    id: "nexudus",
    name: "Nexudus",
    slug: "nexudus",
    productKind: "competitor",
    category: "coworking-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A coworking space management platform with comprehensive member and billing features.",
    bestFor:
      "Coworking spaces needing detailed member management and billing automation",
    strengths: [
      "Comprehensive coworking space management features",
      "Strong member and membership plan management",
      "Detailed billing and invoicing capabilities",
    ],
    limitations: [
      "Less focused on team/organization workplace management",
      "Marketplace and discovery features limited",
      "Interface can be complex for simpler use cases",
    ],
    pricingNotes: "Pricing based on number of members and locations",
    integrationNotes: "Good payment gateway integrations, SSO available",
  },
  {
    id: "gable",
    name: "Gable",
    slug: "gable",
    productKind: "competitor",
    category: "hybrid-workplace",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management and hybrid work platform focused on office operations, visitor workflows, distributed workspace access, workspace discovery, and flexible workplace coordination.",
    bestFor:
      "Organizations supporting distributed teams that need flexible access to third-party workspaces",
    strengths: [
      "Workspace discovery and distributed workspace access",
      "Hybrid workplace coordination",
      "Office operations and visitor management workflows",
    ],
    limitations: [
      "Not designed around full coworking operator administration",
      "Memberships, invoicing, contracts, and operator workflows are not primary capabilities",
      "Custom-domain workspace storefronts and community management are not primary capabilities",
    ],
    pricingNotes: "Company workspace spending and purchasing workflows",
    integrationNotes:
      "Slack, Microsoft Teams, calendar, SSO, HRIS, and access control integrations are positioned as part of the platform",
  },
  {
    id: "robin",
    name: "Robin",
    slug: "robin",
    productKind: "competitor",
    category: "hybrid-workplace",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform focused on hybrid work, office coordination, desk booking, meeting room scheduling, workplace analytics, and employee workplace experience.",
    bestFor:
      "Enterprises focused on hybrid workplace management, workplace experience, office attendance, and workplace analytics",
    strengths: [
      "Hybrid workplace management",
      "Workplace analytics and utilization reporting",
      "Employee workplace experience",
    ],
    limitations: [
      "Not designed as a coworking operations platform",
      "Marketplace publishing and workspace discovery are not primary capabilities",
      "Billing, invoicing, subscriptions, and operator workflows are not primary capabilities",
    ],
    pricingNotes: "Per-user and location-based pricing",
    integrationNotes:
      "Enterprise integrations including Slack, Microsoft Teams, SSO, calendar integrations, APIs, and workplace workflows",
  },
  {
    id: "officely",
    name: "Officely",
    slug: "officely",
    productKind: "competitor",
    category: "hybrid-workplace",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform designed for organizations running hybrid work environments, built around Slack for desk booking, meeting room booking, workplace coordination, and office attendance visibility.",
    bestFor:
      "Organizations that already rely heavily on Slack and want workplace booking and office coordination workflows to happen directly inside the tools employees already use",
    strengths: [
      "Slack-native workflows",
      "Desk booking",
      "Workplace coordination",
      "Office attendance visibility",
      "Lightweight hybrid workplace management",
    ],
    limitations: [
      "Not designed around coworking memberships, tenant management, recurring member billing, or customer contracts",
      "Not positioned as a billing platform",
      "Not positioned as an invoicing platform",
      "Not designed around recurring subscriptions, memberships, or customer billing workflows",
      "Not positioned as a public workspace marketplace",
    ],
    pricingNotes: "Per-user pricing with Slack-based plans",
    integrationNotes: "Excellent Slack integration, other integrations limited",
  },
  {
    id: "envoy",
    name: "Envoy",
    slug: "envoy",
    productKind: "competitor",
    category: "workplace-operations",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform focused on visitor management, workplace coordination, workplace security, desk booking, room booking, and workplace experience.",
    bestFor:
      "Enterprises that want strong visitor management capabilities alongside workplace booking and workplace coordination tools",
    strengths: [
      "Strong visitor management",
      "Workplace experience focus",
      "Workplace security",
      "Delivery management",
      "Enterprise-focused",
    ],
    limitations: [
      "Not designed around coworking memberships, contracts, or recurring member billing",
      "Not positioned as a billing platform",
      "Not positioned as an invoicing platform",
      "Not designed around recurring subscriptions or coworking memberships",
      "Not positioned as a public workspace marketplace",
    ],
    pricingNotes: "Per-location and per-user pricing",
    integrationNotes:
      "Good enterprise integrations and visitor management tools",
  },
  {
    id: "kadence",
    name: "Kadence",
    slug: "kadence",
    productKind: "competitor",
    category: "workplace-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform focused on hybrid work, workplace coordination, desk booking, room booking, office attendance planning, workplace analytics, and space utilization.",
    bestFor:
      "Organizations adopting hybrid work models that need workplace booking, attendance coordination, workplace analytics, and office utilization reporting",
    strengths: [
      "Hybrid workplace management",
      "Office attendance coordination",
      "Workplace analytics",
      "Space utilization reporting",
      "Enterprise-focused",
    ],
    limitations: [
      "Not designed around coworking memberships or recurring member billing",
      "Not positioned as a billing platform",
      "Not positioned as an invoicing platform",
      "Not designed around recurring subscriptions or coworking memberships",
      "Not positioned as a public workspace marketplace",
    ],
    pricingNotes: "Enterprise pricing with location and user-based models",
    integrationNotes: "Standard enterprise integrations available",
  },
  {
    id: "archie",
    name: "Archie",
    slug: "archie",
    productKind: "competitor",
    category: "coworking-management",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A coworking and flexible workspace management platform designed for coworking operators, shared offices, and flexible workspace providers.",
    bestFor:
      "Coworking operators, shared offices, and flexible workspace providers that need memberships, recurring billing, and workspace reservations",
    strengths: [
      "Coworking management",
      "Member management",
      "Billing and invoicing",
      "Subscriptions and payments",
      "Workspace operations",
    ],
    limitations: [
      "Not positioned as a public workspace marketplace",
      "Not positioned as a workspace discovery platform",
      "Not designed around broader workplace-management deployments",
      "Marketplace publishing is not a primary capability",
    ],
    pricingNotes: "Simple per-location pricing",
    integrationNotes: "Basic integrations available",
  },
  {
    id: "deskbird",
    name: "deskbird",
    slug: "deskbird",
    productKind: "competitor",
    category: "hybrid-workplace",
    publicationStatus: "published",
    reviewStatus: "approved",
    summary:
      "A workplace management platform focused on hybrid work, desk booking, room booking, office attendance coordination, workplace visibility, and workplace analytics.",
    bestFor:
      "Organizations adopting hybrid work models that need workplace booking, attendance coordination, workplace analytics, and employee workplace experience capabilities",
    strengths: [
      "Hybrid workplace management",
      "Workplace analytics",
      "Workplace visibility",
      "Office attendance coordination",
      "Employee workplace experience",
    ],
    limitations: [
      "Not designed around coworking memberships",
      "Not designed around tenant administration",
      "Not designed around recurring member billing",
      "Not positioned as a billing platform",
      "Not positioned as an invoicing platform",
    ],
    pricingNotes: "Enterprise pricing with location and user-based models",
    integrationNotes: "Standard enterprise integrations available",
  },
];
