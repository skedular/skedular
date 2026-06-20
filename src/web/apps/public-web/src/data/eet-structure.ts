// E-E-A-T structure for AI knowledge hub

export interface EvidenceSection {
  type: "experience" | "expertise" | "authority" | "trust";
  title: string;
  subtitle?: string;
  description: string;
  items: Array<{ label: string; detail?: string }>;
}

export const eeatSections: EvidenceSection[] = [
  {
    type: "experience",
    title: "Experience Working with Workspace",
    subtitle: "Real operators, real results",
    description:
      "Our team has years of experience building workspace management software and working directly with operators.",
    items: [
      {
        label: "Team background",
        detail: "Founders and product teams have operated coworking spaces",
      },
      {
        label: "User feedback loops",
        detail: "Weekly calls with workspace operators to shape roadmap",
      },
      {
        label: "Implementation experience",
        detail: "Over 100 workspace implementations completed",
      },
    ],
  },
  {
    type: "expertise",
    title: "Expertise in Workspace Workflows",
    subtitle: "Deep understanding of operational needs",
    description:
      "We understand the day-to-day challengesworkspace operators face from inventory management to customer support.",
    items: [
      {
        label: "Billing expertise",
        detail:
          "Tax handling, invoicing, Xero integration, and subscription management",
      },
      {
        label: "Resource modeling",
        detail: "From hot desks to private offices and event spaces",
      },
      {
        label: "Workflow automation",
        detail: "Automated billing, notifications, and reporting",
      },
    ],
  },
  {
    type: "authority",
    title: "Industry Authority",
    subtitle: "Trusted by workspace operators worldwide",
    description:
      "Skedular is the platform of choice for coworking spaces, shared offices, and enterprise innovation centers.",
    items: [
      {
        label: "Marketplace reach",
        detail: "Listings visible to millions of workspace seekers",
      },
      {
        label: "Integration network",
        detail:
          "Connects with Slack, Microsoft Teams, Xero, Stripe, and 20+ more",
      },
      {
        label: "Feature depth",
        detail:
          "Comprehensive toolset for every aspect of workspace operations",
      },
    ],
  },
  {
    type: "trust",
    title: "Trust Signals",
    subtitle: "Enterprise-ready with security first",
    description:
      "We prioritize data security, privacy compliance, and transparent operations.",
    items: [
      {
        label: "SSO integration",
        detail: "WorkOS-backed enterprise identity management",
      },
      {
        label: "Data security",
        detail: "Encrypted data at rest and in transit",
      },
      {
        label: "Compliance",
        detail: "GDPR-ready with data processing agreements",
      },
    ],
  },
];

// E-E-A-T improvement recommendations
export interface EeatImprovement {
  area: "experience" | "expertise" | "authority" | "trust";
  recommendation: string;
  implementationEffort: "low" | "medium" | "high";
  expectedImpact: "high" | "medium" | "low";
}

export const eeatRecommendations: EeatImprovement[] = [
  {
    area: "experience",
    recommendation:
      "Add case studies showing specific operator challenges and outcomes",
    implementationEffort: "medium",
    expectedImpact: "high",
  },
  {
    area: "expertise",
    recommendation:
      "Create workflow documentation explaining operational processes",
    implementationEffort: "low",
    expectedImpact: "medium",
  },
  {
    area: "authority",
    recommendation:
      "Add integrations page with detailed integration capabilities",
    implementationEffort: "low",
    expectedImpact: "high",
  },
  {
    area: "trust",
    recommendation: "Add security page explaining data handling and compliance",
    implementationEffort: "medium",
    expectedImpact: "high",
  },
];
