import type { ProductPageContent } from "./content-types";

export const teamsPage: ProductPageContent = {
  id: "teams",
  eyebrow: "Skedular Teams",
  title: "Help employees find workspace while giving workplace teams the visibility they need",
  summary:
    "Give employees a simple way to reserve desks, rooms, parking, and equipment while workplace teams keep attendance, space use, and collaboration visible.",
  audience: "Enterprises, government teams, hybrid workplaces, facilities teams, executive assistants, and corporate offices.",

  // Hero section content
  heroHeading: "Help employees find the right place to work",
  heroDescription:
    "Help employees find the right place to work while giving workplace teams the visibility they need to coordinate attendance, manage space, and support hybrid work.",
  heroCTAPrimary: { label: "Book a demo", href: "/contact" },
  heroCTASecondary: { label: "View Teams pricing", href: "/pricing/teams" },

  // Why Organizations Need More Than Desk Booking section
  whyOrganizationsNeedMore: {
    heading: "Workplace management is more than desk booking",
    description:
      "Most workplace software focuses on booking desks and rooms. Modern organizations also need to understand who is coming to the office, how teams coordinate in person, how space is being used, and how workplace policies are being adopted.",
    cards: [
      {
        title: "See who is coming to the office",
        description: "Help teams coordinate in-person work by knowing which colleagues will be onsite.",
      },
      {
        title: "Understand how office space is being used",
        description: "Track which desks and rooms are in demand and make better decisions about office layout.",
      },
      {
        title: "Support hybrid work without spreadsheets",
        description: "Manage flexible workplace policies and coordinate who comes to the office without manual tracking.",
      },
      {
        title: "Give workplace teams the tools they need",
        description: "Help facilities and workplace teams manage office resources without chasing spreadsheets.",
      },
    ],
  },

  // Typical Workplace Journey section
  typicalJourney: {
    heading: "How employees use Skedular Teams",
    steps: [
      {
        title: "Open Slack, Microsoft Teams, or Skedular",
        description: "Employees access workplace tools from the applications they use daily.",
      },
      {
        title: "View attendance and team presence",
        description: "See who is coming to the office and plan accordingly.",
      },
      {
        title: "Book workspace resources",
        description: "Reserve desks, rooms, parking spaces, or equipment in seconds.",
      },
      {
        title: "Find your space with maps",
        description: "Use interactive floor plans to locate the right space.",
      },
      {
        title: "Receive confirmation",
        description: "Get booking confirmations and reminders automatically.",
      },
      {
        title: "Arrive with confidence",
        description: "Show up at the office knowing your workspace is ready.",
      },
      {
        title: "Teams gain visibility",
        description: "Workplace managers see attendance trends and utilization insights.",
      },
    ],
  },

  // Feature sections
  features: [
    {
      title: "Everything needed to manage workplace coordination",
      body: "Skedular Teams brings together desk booking, room reservations, attendance tracking, team coordination, and workplace visibility in one place.",
      featureBlocks: [
        {
          title: "Book the resources employees need",
          description: "Make everyday workplace resources easier to find and reserve.",
          items: ["Desks", "Meeting rooms", "Parking spaces", "Equipment"],
          accent: "emerald",
        },
        {
          title: "Help employees plan better office days",
          description: "Help people plan office days with enough context to make the trip worthwhile.",
          items: ["See who is coming in", "Interactive floor plans", "Office maps", "When space is available"],
          accent: "aqua",
        },
        {
          title: "Keep teams connected",
          description: "Keep workspace activity close to the tools teams already use.",
          items: ["Slack", "Microsoft Teams", "Notifications", "Team booking views"],
          accent: "violet",
        },
        {
          title: "Built for workplace operations",
          description: "Support organizations that need control, reporting, and identity-aware access.",
          items: ["Analytics", "SSO", "Permissions", "Reporting"],
          accent: "sunbeam",
        },
      ],
    },
  ],

  // Why Organizations Choose Skedular Teams section
  whyChooseUs: {
    heading: "Why organizations choose Skedular Teams",
    cards: [
      {
        title: "One place for workplace coordination",
        description: "Manage attendance, desks, rooms, and parking from one platform instead of juggling multiple tools.",
      },
      {
        title: "Work where employees already work",
        description: "Slack and Microsoft Teams integrations mean employees can book workspace without leaving the tools they use daily.",
      },
      {
        title: "Built for hybrid work",
        description: "Support flexible workplace policies and coordinate who comes to the office without manual spreadsheets.",
      },
      {
        title: "Enterprise ready",
        description: "SSO, permissions, and reporting built in for organizations that need control and security.",
      },
    ],
  },

  // Screenshot sections (placeholder structure)
  screenshotSections: [
    {
      id: "interactive-floor-plans",
      heading: "Interactive floor plans",
      subheading: "Help employees find desks, rooms, parking spaces, and workplace resources visually.",
      placeholderText: "Interactive Floor Plans Screenshot",
    },
    {
      id: "attendance-visibility",
      heading: "Attendance and workplace visibility",
      subheading: "Understand who is coming to the office and help teams coordinate in-person work.",
      placeholderText: "Attendance Dashboard Screenshot",
    },
    {
      id: "desk-room-booking",
      heading: "Desk and room booking",
      subheading: "Give employees a fast and simple way to reserve workplace resources.",
      placeholderText: "Booking Interface Screenshot",
    },
    {
      id: "workplace-analytics",
      heading: "Workplace analytics",
      subheading: "Track attendance, utilization, and workplace trends over time.",
      placeholderText: "Analytics Dashboard Screenshot",
    },
  ],

  // Integrations section
  integrations: {
    heading: "Connect with your workplace ecosystem",
    body: "Integrate Skedular Teams with the tools your organization already relies on.",
    integrations: ["Slack", "Microsoft Teams", "Enterprise SSO"],
  },

  // Slack/Teams section
  commsIntegration: {
    heading: "Work where your teams already work",
    description:
      "Book desks, coordinate attendance, receive notifications, and stay informed without leaving Slack or Microsoft Teams. Skedular Teams integrates directly into the tools employees already use every day.",
  },

  // Built for section
  builtFor: {
    heading: "Built for modern organizations",
    body: "Whether you're managing a corporate office, supporting hybrid work, coordinating facilities, or overseeing workplace operations, Skedular Teams helps bring workplace visibility and resource management together.",
    audiences: [
      "Enterprise organizations",
      "Corporate offices",
      "Government departments",
      "Facilities teams",
      "Workplace managers",
      "Executive assistants",
      "Hybrid work programs",
      "Operations teams",
    ],
  },

  // Trust section
  trust: {
    heading: "Trusted by workplace teams",
    body: "Organizations use Skedular Teams to coordinate workplace attendance, bookings, and hybrid work.",
    logos: [
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
  },

  // FAQ section
  faq: [
    {
      question: "Can employees book desks and rooms?",
      answer:
        "Yes. Employees can easily book desks, meeting rooms, parking spaces, and other workplace resources through the Skedular Teams interface. The booking experience is intuitive and works across desktop and mobile devices.",
    },
    {
      question: "Does Skedular Teams support hybrid work?",
      answer:
        "Absolutely. Skedular Teams was built specifically for hybrid workplaces. It helps teams coordinate who comes to the office when, track attendance patterns, and manage flexible workplace policies without spreadsheets or manual coordination.",
    },
    {
      question: "Does Teams integrate with Slack?",
      answer:
        "Yes. Skedular Teams integrates directly with Slack, allowing employees to book workspace resources, view team presence, and receive notifications—all from within Slack.",
    },
    {
      question: "Does Teams integrate with Microsoft Teams?",
      answer:
        "Yes. Full integration with Microsoft Teams means your organization can coordinate workplace attendance and bookings without leaving the Microsoft ecosystem you rely on daily.",
    },
    {
      question: "Does Teams support floor plans?",
      answer:
        "Yes. Skedular Teams includes interactive floor plans that help employees navigate the office and find available desks, rooms, and resources visually.",
    },
    {
      question: "Does Teams support enterprise SSO?",
      answer:
        "Yes. Skedular Teams supports enterprise SSO, SCIM provisioning, role-based permissions, and comprehensive identity management for organizations with strict security requirements.",
    },
    {
      question: "Can employees see who is coming to the office?",
      answer:
        "Yes. Team attendance visibility is a core feature of Skedular Teams. Employees can see which colleagues are scheduled to be in the office, helping them coordinate meetings and collaboration in person.",
    },
  ],

  integrationActions: [{ type: "slack" }],
  finalCTA: {
    heading: "Ready to simplify workplace coordination?",
    description:
      "See how Skedular Teams helps organizations manage attendance, workplace visibility, desk booking, room booking, and hybrid work from a single platform.",
    primaryCTA: { label: "Book a demo", href: "/contact" },
    secondaryCTA: { label: "View Teams pricing", href: "/pricing/teams" },
  },
};
