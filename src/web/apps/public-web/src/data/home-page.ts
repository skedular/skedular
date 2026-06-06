type CustomerTestimonial = {
  quote: string;
  name: string;
  role: string;
  company: string;
  logo: string;
  photo: string;
};

type CustomerMetric = {
  label: string;
  value: string;
};

type CustomerCaseStudy = {
  logo: string;
  industry: string;
  locations: string;
  users: string;
  results: string[];
  href: string;
};

export const homePage = {
  title: "Skedular | Workspace booking and workspace management",
  description: "Find workspace, book desks and meeting rooms, manage hybrid workplaces, and run coworking operations with Skedular.",
  keywords: [
    "Workspace booking",
    "Desk booking",
    "Meeting room booking",
    "Workspace management",
    "Hybrid workplace software",
    "Coworking software",
    "Workspace marketplace",
    "Flexible workspace",
  ],
  hero: {
    title: "Find, book, manage, and monetize workspace.",
    summary:
      "Whether you're looking for a desk, managing a workplace, or running a coworking business, Skedular brings discovery, booking, operations, payments, billing, invoicing, and subscriptions together in one platform.",
    trustMessage: "Get started for free. Upgrade when you're ready.",
  },
  heroActions: {
    primary: "book-demo",
    secondary: "explore-skedular",
  },
  trustStatement: "Built for every side of modern workspace",
  trustSignals: ["People booking space", "Hybrid teams", "Workplace managers", "Coworking operators", "Flexible workspace providers"],
  searchFields: ["Location", "Date", "Resource type"],
  resourceCategories: ["Desks", "Meeting rooms", "Event spaces", "Private offices", "Flexible workspaces"],
  discovery: {
    title: "Find workspace that fits the way you work.",
    body: "Discover desks, meeting rooms, event spaces, private offices, and flexible workspaces from providers across the network.",
    modules: [
      {
        title: "Search by location",
        body: "Use map-based discovery to compare nearby workspaces, available resources, amenities, and host details.",
      },
      {
        title: "Choose the right resource",
        body: "Book desks, meeting rooms, event spaces, private offices, parking, and operator-defined flexible workspace.",
      },
      {
        title: "Move from discovery to booking",
        body: "Search and booking actions route visitors into the Skedular application when they are ready to reserve space.",
      },
    ],
  },
  audience: {
    title: "How do you use workspace?",
    body: "Start with the outcome you need, then follow the product path that supports it.",
    paths: [
      {
        title: "I need a workspace",
        body: "Find desks, meeting rooms, private offices, and event spaces.",
        ctaLabel: "Find workspace",
        href: "#workspace-discovery",
        accent: "emerald",
        features: ["Workspace marketplace", "Map discovery", "Desk and room booking"],
      },
      {
        title: "I manage a workplace",
        body: "Give employees a simpler workplace experience while providing administrators with the visibility and controls needed to manage hybrid work.",
        ctaLabel: "Explore Teams",
        href: "/teams",
        accent: "aqua",
        features: ["Private workplace booking", "Attendance and utilization", "Slack, Microsoft Teams, and SSO"],
      },
      {
        title: "I run a workspace business",
        body: "Generate revenue from workspace resources, sell subscriptions, simplify billing, and manage operations from one platform.",
        ctaLabel: "Explore Spaces",
        href: "/spaces",
        accent: "violet",
        features: ["Resource commerce", "Billing and invoicing", "Marketplace publishing"],
      },
    ],
  },
  builtFor: {
    title: "Built for every workspace model",
    items: [
      { label: "Hybrid workplaces" },
      { label: "Coworking spaces" },
      { label: "Flexible workspace providers" },
      { label: "Shared offices" },
      { label: "Innovation hubs" },
      { label: "Meeting room providers" },
      { label: "Event venues" },
    ],
  },
  whySkedular: {
    title: "Everything needed to run modern workspace.",
    pillars: [
      {
        title: "Workspace discovery",
        body: "Find the right space when and where you need it.",
      },
      {
        title: "Flexible booking",
        body: "Book by the hour, day, week, month, or subscription.",
      },
      {
        title: "Workplace management",
        body: "Manage attendance, resources, and workplace utilization.",
      },
      {
        title: "Workspace commerce",
        body: "Turn resources into products and generate revenue.",
      },
      {
        title: "Integrated billing",
        body: "Payments, subscriptions, invoicing, taxes, and accounting.",
      },
      {
        title: "Enterprise ready",
        body: "SSO, Slack, Microsoft Teams, permissions, and identity management.",
      },
    ],
  },
  differentiation: {
    title: "Why organizations choose Skedular",
    subtitle:
      "Most workspace software focuses on one problem. Skedular brings workspace discovery, booking, workplace management, and workspace operations together in a single platform.",
    supportingText:
      "This allows organizations and operators to manage everything from finding and booking space through to billing, subscriptions, and day-to-day operations.",
    pillars: [
      {
        title: "One platform instead of multiple tools",
        body: "Manage discovery, booking, operations, billing, subscriptions, payments, and reporting from a single system.",
      },
      {
        title: "Built for both teams and operators",
        body: "Support private workplaces and commercial workspace businesses without changing platforms.",
      },
      {
        title: "Flexible business models",
        body: "Support hourly bookings, daily bookings, recurring bookings, subscriptions, recurring billing, and invoicing.",
      },
      {
        title: "Enterprise ready",
        body: "WorkOS, SSO, Slack, Microsoft Teams, permissions, integrations, and identity management built in.",
      },
    ],
  },
  becomeAHost: {
    title: "Turn your workspace into revenue.",
    subtitle:
      "List desks, meeting rooms, private offices, event spaces, and flexible workspace products. Manage bookings, subscriptions, invoicing, billing, and payments from one platform.",
    supportingText:
      "Perfect for coworking operators, workspace providers, meeting room businesses, event venues, and organizations looking to monetize underutilized space.",
    ctaLabel: "Become a Host",
    href: "/become-host",
  },
  productShowcases: [
    {
      productName: "Skedular Teams",
      title: "Give employees a simpler way to book the workplace.",
      body: "Skedular Teams helps organizations coordinate desk booking, meeting room booking, attendance, floor plans, analytics, Slack, Microsoft Teams, and SSO in one private workplace experience.",
      href: "/teams",
      ctaLabel: "Explore Teams",
      visualTitle: "Team workplace workflow",
      visualStats: ["Desk booking", "Meeting rooms", "Floor plans", "Attendance", "Analytics", "SSO"],
      accent: "aqua",
    },
    {
      productName: "Skedular Spaces",
      title: "Run the commercial side of flexible workspace.",
      body: "Skedular Spaces helps operators manage resources, products, marketplace publishing, subscriptions, payments, invoicing, billing, and customer relationships from one operational surface.",
      href: "/spaces",
      ctaLabel: "Explore Spaces",
      visualTitle: "Operator commerce workflow",
      visualStats: ["Resources", "Products", "Marketplace", "Subscriptions", "Payments", "Invoices"],
      accent: "violet",
    },
  ],
  featureHighlights: {
    title: "Powerful features built for modern workspace.",
    groups: [
      {
        title: "Interactive floor plans",
        body: "Help people quickly find and reserve the right workspace with visual room selection.",
      },
      {
        title: "Workspace maps",
        body: "Location discovery and navigation for people looking for the right place to work.",
      },
      {
        title: "Payments",
        body: "Collect payments and automate billing without manual administration.",
      },
      {
        title: "Billing",
        body: "Flexible billing cycles for one-time bookings, recurring purchases, and operator-led terms.",
      },
      {
        title: "Invoicing",
        body: "Automated invoice workflows for workspace bookings, subscriptions, and business customers.",
      },
      {
        title: "Xero integration",
        body: "Keep invoicing and accounting in sync with Xero.",
      },
      {
        title: "Analytics",
        body: "Understand how workspace is being used and make better operational decisions.",
      },
      {
        title: "Integrations",
        body: "Connect to Slack, Microsoft Teams, calendar, identity, payments, accounting, and maps.",
      },
      {
        title: "Security",
        body: "WorkOS-backed enterprise SSO, identity, permissions, and access controls.",
      },
    ],
  },
  customerTrust: {
    title: "Trusted by organizations and workspace operators.",
    body: "Skedular is built to support the visitor searching for space, the workplace team coordinating hybrid work, and the operator running the business behind the workspace.",
    logoHeading: "Our customers",
    customerLogos: [
      {
        name: "Seequent",
        src: "/images/customer-seequent.png",
        width: 5000,
        height: 834,
      },
      {
        name: "EMD",
        src: "/images/customer-emd.svg",
        width: 924,
        height: 245,
      },
    ],
    metrics: [] as CustomerMetric[],
    testimonials: [] as CustomerTestimonial[],
    caseStudies: [] as CustomerCaseStudy[],
  },
  integrations: {
    title: "Works with the tools your teams already use.",
    body: "Connect Skedular to the tools your teams already use. Bring booking, workplace management, payments, accounting, maps, and calendars together through the integrations your business already relies on. Slack and Microsoft Teams support is available for private organizations using Skedular Teams.",
    items: ["Slack", "Microsoft Teams", "Xero", "Stripe", "Google Maps", "Calendar integrations"],
  },
  finalCta: {
    title: "Ready to simplify workspace?",
    body: "Whether you're looking for workspace, managing a workplace, or running a workspace business, Skedular helps you get started quickly.",
    actions: [
      { label: "Book demo", href: "book-demo", style: "inverse" },
      { label: "Explore Teams", href: "/teams", style: "text" },
      { label: "Explore Spaces", href: "/spaces", style: "text" },
      { label: "View Pricing", href: "/pricing", style: "text" },
    ],
  },
  faq: [
    {
      question: "What is Skedular?",
      answer:
        "Skedular is a workspace operating system for finding workspace, booking desks and meeting rooms, managing hybrid workplaces, and running coworking or flexible workspace operations.",
    },
    {
      question: "Can people use Skedular to find and book workspace?",
      answer:
        "Yes. Skedular supports public workspace discovery and booking paths for desks, meeting rooms, event spaces, private offices, and flexible workspace.",
    },
    {
      question: "Who is Skedular for?",
      answer: "Skedular is for people who need workspace, organizations that manage private workplaces, and operators that run workspace businesses.",
    },
    {
      question: "How is Skedular different from other workspace tools?",
      answer:
        "Skedular combines public workspace discovery, private workplace management, coworking operations, payments, billing, invoicing, subscriptions, resource management, and team collaboration in one platform. Most tools solve only part of the problem.",
    },
    {
      question: "Does Skedular support coworking spaces?",
      answer:
        "Yes. Skedular Spaces helps operators manage workspace resources, products, subscriptions, billing, invoicing, and customer relationships from a single platform.",
    },
    {
      question: "Does Skedular support recurring bookings?",
      answer: "Yes. Organizations and operators can create recurring booking experiences for workspace resources and subscriptions.",
    },
    {
      question: "Does Skedular integrate with Microsoft Teams?",
      answer: "Yes. Skedular Teams supports Microsoft Teams integration for workplace booking and workplace coordination.",
    },
    {
      question: "Does Skedular integrate with Slack?",
      answer: "Yes. Slack integration allows organizations to coordinate workplace attendance and booking workflows directly within Slack.",
    },
    {
      question: "Does Skedular support subscriptions?",
      answer: "Yes. Workspace operators can offer recurring subscriptions alongside one-time bookings.",
    },
    {
      question: "Does Skedular support invoicing?",
      answer: "Yes. Skedular supports invoicing, billing workflows, subscriptions, and accounting integrations like Xero.",
    },
    {
      question: "Does Skedular support floor plans?",
      answer: "Yes. Skedular supports interactive floor plans that allow users to visually browse and reserve desks, rooms, and workspace resources.",
    },
    {
      question: "Does Skedular support Xero integration?",
      answer: "Yes. Skedular integrates with Xero to help operators manage invoicing, billing, subscriptions, and accounting workflows.",
    },
    {
      question: "Can I use my own domain with Skedular?",
      answer: "Yes. Workspace operators can use a Skedular subdomain or configure a fully custom domain for a branded customer experience.",
    },
  ],
};
