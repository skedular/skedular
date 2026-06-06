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
  },
  heroActions: {
    primary: "book-demo",
    secondary: "get-started",
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
        body: "Manage desks, rooms, parking, attendance, floor plans, and hybrid work.",
        ctaLabel: "Explore Teams",
        href: "/teams",
        accent: "aqua",
        features: ["Private workplace booking", "Attendance and utilization", "Slack, Microsoft Teams, and SSO"],
      },
      {
        title: "I run a workspace business",
        body: "Manage resources, products, subscriptions, invoices, payments, and customers.",
        ctaLabel: "Explore Spaces",
        href: "/spaces",
        accent: "violet",
        features: ["Resource commerce", "Billing and invoicing", "Marketplace publishing"],
      },
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
        body: "Visual workspace selection for desks, rooms, zones, and bookable resources.",
      },
      {
        title: "Workspace maps",
        body: "Location discovery and navigation for people looking for the right place to work.",
      },
      {
        title: "Payments",
        body: "Stripe-powered payment flows for paid bookings and operator revenue.",
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
        body: "Accounting workflows that keep billing, invoicing, and finance operations aligned.",
      },
      {
        title: "Analytics",
        body: "Utilization, attendance, booking, and revenue reporting for better workspace decisions.",
      },
      {
        title: "Integrations",
        body: "Slack, Microsoft Teams, calendar, identity, payments, accounting, and map workflows.",
      },
      {
        title: "Security",
        body: "WorkOS-backed enterprise SSO, identity, permissions, and access controls.",
      },
    ],
  },
  customerTrust: {
    title: "Trusted by teams and workspace operators.",
    body: "Skedular is built to support the visitor searching for space, the workplace team coordinating hybrid work, and the operator running the business behind the workspace.",
    logoHeading: "Our clients",
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
    body: "Connect workspace activity to collaboration, identity, payments, accounting, maps, and calendars. Slack and Microsoft Teams support is available for private organizations using Skedular Teams.",
    items: ["Slack", "Microsoft Teams", "Xero", "Stripe", "Google Maps", "Calendar integrations"],
  },
  finalCta: {
    title: "Ready to simplify workspace management?",
    body: "Whether you're looking for workspace, managing a workplace, or running a workspace business, Skedular has you covered.",
    actions: [
      { label: "Book demo", href: "book-demo", style: "inverse" },
      { label: "Explore Teams", href: "/teams", style: "text" },
      { label: "Explore Spaces", href: "/spaces", style: "text" },
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
      question: "How is Skedular different from workplace or coworking tools?",
      answer:
        "Skedular combines public workspace discovery, private workplace management, coworking operations, payments, billing, invoicing, subscriptions, resource management, and team collaboration in one platform.",
    },
  ],
};
