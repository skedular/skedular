import type { ProductPageContent } from "./content-types";

export const spacesPage: ProductPageContent = {
  id: "spaces",
  eyebrow: "Skedular Spaces",
  title: "Coworking Management Software for Workspace Operators",
  summary:
    "Coworking management software for selling workspace, managing memberships, automating billing, publishing availability, and supporting customers from a single platform.",
  audience:
    "Coworking spaces, shared offices, serviced offices, meeting room providers, event venues, innovation hubs, business centres, and flexible workspace networks.",

  heroHeading: "Coworking Management Software for Workspace Operators",
  heroDescription:
    "Coworking management software for selling workspace, managing memberships, automating billing, publishing availability, and supporting customers. Whether you operate a coworking space, serviced office, meeting room business, or flexible workspace network, Skedular Spaces helps you spend less time on administration and more time growing your business.",
  heroCTAPrimary: {
    label: "Try Spaces",
    href: "PUBLIC_SKEDULAR_SPACES_APP_URL",
  },
  heroCTASecondary: { label: "View Spaces pricing", href: "/pricing/spaces" },

  // Why Not Spreadsheets Section
  whyNotSpreadsheets: {
    heading: "Why operators outgrow spreadsheets",
    subtitle: "Spreadsheets work until they don't.",
    description:
      "You can start managing workspace with spreadsheets, but as you add locations, members, and bookings, it becomes impossible to keep track. Coworking management software automates the things that scale while giving you visibility into what's happening in real time.",
    features: [
      {
        title: "Real-time availability",
        body: "Spreadsheets show static data. Your software should show live availability so customers can book instantly.",
      },
      {
        title: "Automated billing",
        body: "Manual invoicing breaks at scale. Coworking management software handles recurring billing, tax calculations, and payment reminders automatically.",
      },
      {
        title: "Customer self-service",
        body: "Spreadsheets require phone calls to check availability. Your software should let customers book, view, and manage their own bookings.",
      },
    ],
  },

  whyOrganizationsNeedMore: {
    heading: "Running a workspace is more than booking desks",
    description:
      "Workspace operators manage much more than availability. They need to package products, publish inventory, process payments, issue invoices, manage subscriptions, support customers, and keep occupancy high. Most operators end up stitching together multiple tools to make this work. Skedular Spaces brings those workflows together into a single platform.",
    cards: [
      {
        title: "Sell workspace",
        description: "Turn resources into products customers can actually buy.",
      },
      {
        title: "Reduce administration",
        description:
          "Automate billing, invoicing, payments, and recurring subscriptions.",
      },
      {
        title: "Increase visibility",
        description:
          "Publish inventory through marketplaces, branded listings, and custom domains.",
      },
      {
        title: "Support growth",
        description: "Manage one location or many without changing platforms.",
      },
    ],
  },
  typicalJourney: {
    heading: "How workspace operators use Skedular Spaces",
    steps: [
      {
        title: "Create locations and resources",
        description: "Model desks, rooms, offices, and workspace inventory.",
      },
      {
        title: "Configure products and pricing",
        description:
          "Create hourly bookings, day passes, booking credits, memberships, and subscriptions.",
      },
      {
        title: "Publish inventory",
        description:
          "Make workspace discoverable through marketplaces and custom domains.",
      },
      {
        title: "Accept bookings",
        description: "Receive and manage customer bookings automatically.",
      },
      {
        title: "Collect payments",
        description: "Process payments and handle billing cycles.",
      },
      {
        title: "Issue invoices",
        description: "Generate and send invoices for commercial customers.",
      },
      {
        title: "Manage memberships and subscriptions",
        description: "Support recurring access and subscription workflows.",
      },
      {
        title: "Support customers",
        description: "Handle customer inquiries and support requests.",
      },
      {
        title: "Track business performance",
        description: "Monitor occupancy, revenue, and utilization trends.",
      },
    ],
  },

  features: [
    {
      title:
        "Manage every resource your customers can book from a single platform",
      body: "Model desks, meeting rooms, private offices, event spaces, parking, equipment, and shared resources in a way that reflects how your business operates.",
      featureBlocks: [
        {
          title: "Resource management",
          description: "Model the spaces and resources customers can book.",
          items: [
            "Desks",
            "Rooms",
            "Event spaces",
            "Private offices",
            "Equipment",
            "Zones and tags",
          ],
          accent: "emerald",
        },
        {
          title: "Product management",
          description:
            "Create products that match real customer demand, from hourly bookings and day passes to booking credits, memberships, subscriptions, and recurring access.",
          items: [
            "Product catalog",
            "Dynamic product matching",
            "Images",
            "Amenities",
            "Visibility controls",
          ],
          accent: "aqua",
        },
        {
          title: "Payments and billing",
          description:
            "Handle billing, invoicing, payments, and subscriptions without manual work. Manage billing cycles, invoices, tax, and cancellation policies from one place.",
          items: [
            "Card payments",
            "Tax handling",
            "Billing cadence",
            "Invoicing",
            "Subscriptions",
            "Cancellation policies",
          ],
          accent: "violet",
        },
        {
          title: "Publishing and brand",
          description:
            "Publish inventory through marketplaces, custom domains, branded listings, maps, and discovery experiences while maintaining ownership of your customer relationships.",
          items: [
            "Marketplace publishing",
            "Host model",
            "Custom domains",
            "Branded listings",
            "Opening hours",
            "Maps",
          ],
          accent: "sunbeam",
        },
        {
          title: "Prepaid booking credits",
          description:
            "Let customers pay now and choose an eligible workspace booking later. Credits become available after payment is confirmed, while you keep control of the dates, resources, validity period, and unused-credit refund policy.",
          items: [
            "Buy now, schedule later",
            "Eligible days and resources",
            "Confirmed payment before use",
            "Expiry, refunds, and auto-renewal",
          ],
          accent: "emerald",
        },
      ],
    },
  ],
  whyChooseUs: {
    heading: "Why operators choose Skedular Spaces",
    cards: [
      {
        title: "Sell more workspace",
        description: "Make inventory easier to discover and book.",
      },
      {
        title: "Reduce administration",
        description:
          "Automate billing, invoicing, subscriptions, and payments.",
      },
      {
        title: "Support multiple business models",
        description:
          "Hourly bookings, day passes, prepaid booking credits, memberships, subscriptions, and recurring bookings.",
      },
      {
        title: "Keep your brand",
        description:
          "Use custom domains and branded experiences while benefiting from marketplace visibility.",
      },
    ],
  },

  differentiation: {
    heading: "One platform instead of multiple tools",
    description:
      "Many workspace operators rely on separate systems for bookings, invoicing, payments, subscriptions, marketplace publishing, customer management, and reporting. As the business grows, those systems become increasingly difficult to manage. Skedular Spaces brings those workflows together into a single platform so operators can spend less time managing software and more time growing their business.",
    withoutSkedular: [
      "Booking software",
      "Payment software",
      "Invoicing software",
      "Marketplace software",
      "Spreadsheets",
      "Manual administration",
    ],
    withSkedular: [
      "Inventory management",
      "Product management",
      "Marketplace publishing",
      "Billing and invoicing",
      "Subscriptions",
      "Customer management",
      "Reporting",
    ],
  },

  operatorContext: {
    heading: "Most operators start with spreadsheets",
    content:
      "Many workspace businesses begin with spreadsheets, shared calendars, accounting software, and manual invoicing. That approach works for a while. As occupancy grows, memberships increase, and customers expect online booking, administration becomes harder to manage. Skedular Spaces helps operators move from manual processes to a single operational platform.",
  },

  screenshotSections: [
    {
      id: "resource-management",
      heading: "Resource management",
      subheading:
        "Show locations, desks, rooms, offices, and workspace inventory.",
      imageSrc: "/images/screenshots/skedular-spaces/resource-management.png",
      placeholderText: "Resource Management Screenshot",
    },
    {
      id: "product-configuration",
      heading: "Product configuration",
      subheading: "Show products, memberships, pricing, and subscriptions.",
      imageSrc: "/images/screenshots/skedular-spaces/product-configuration.png",
      placeholderText: "Product Configuration Screenshot",
    },
    {
      id: "billing-invoicing",
      heading: "Billing and invoicing",
      subheading:
        "Show invoices, billing schedules, subscriptions, and payment activity.",
      imageSrc: "/images/screenshots/skedular-spaces/billing-and-invoicing.png",
      placeholderText: "Billing and Invoicing Screenshot",
    },
    {
      id: "marketplace-publishing",
      heading: "Marketplace publishing",
      subheading:
        "Show listings, custom domains, and branded workspace experiences.",
      imageSrc:
        "/images/screenshots/skedular-spaces/marketplace-publishing.png",
      placeholderText: "Marketplace Publishing Screenshot",
    },
    {
      id: "operator-analytics",
      heading: "Operator analytics",
      subheading:
        "Show occupancy, utilization, bookings, revenue, and subscription trends.",
      imageSrc: "/images/screenshots/skedular-spaces/operator-analytics.png",
      placeholderText: "Operator Analytics Screenshot",
    },
  ],

  integrations: {
    heading: "Benefit from marketplace discovery without giving up your brand",
    body: "List workspace on the Skedular marketplace, publish through a custom domain, or support both approaches at the same time. You maintain ownership of your brand while making inventory discoverable.",
    integrations: [
      "Marketplace publishing",
      "Custom domains",
      "Branded listings",
      "Search visibility",
      "Maps and discovery",
    ],
  },

  commsIntegration: {
    heading: "Built for workspace commerce",
    description:
      "Support one-time bookings, prepaid booking credits, recurring subscriptions, memberships, invoicing, tax handling, billing cadences, and payment workflows. Skedular Spaces handles commercial operations so you can focus on running your workspace.",
  },

  builtFor: {
    heading: "Built for workspace operators",
    body: "Whether you operate a coworking space, shared office, serviced office, meeting room business, or flexible workspace network, Skedular Spaces helps you manage inventory, automate billing, publish availability, and grow occupancy.",
    audiences: [
      "Coworking spaces",
      "Shared offices",
      "Serviced offices",
      "Meeting room providers",
      "Event venues",
      "Innovation hubs",
      "Business centres",
      "Flexible workspace networks",
    ],
  },

  trust: {
    heading: "Trusted by workspace operators",
    body: "Workspace operators use Skedular Spaces to manage inventory, automate billing, publish availability, and support customers from a single platform.",
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
  aiSummary: {
    heading: "Skedular Spaces at a glance",
    description:
      "Skedular Spaces is workspace management software designed for coworking operators, serviced office providers, meeting room businesses, event venues, and flexible workspace networks.",
    operatorsUse: [
      "Manage inventory",
      "Create products",
      "Publish workspace",
      "Accept bookings",
      "Manage memberships",
      "Process payments",
      "Generate invoices",
      "Manage subscriptions",
      "Support customers",
    ],
    keyCapabilities: [
      "Resource management",
      "Product management",
      "Marketplace publishing",
      "Custom domains",
      "Billing and invoicing",
      "Subscription management",
      "Workspace analytics",
    ],
  },
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
        "We support credit card and debit card payments through Stripe, integrate with Xero for accounting, and support bank transfer with customer verification and confirmation through Skedular.",
    },
    {
      question: "What is coworking management software?",
      answer:
        "Coworking management software helps workspace operators sell desk space, private offices, meeting rooms, and event venues. It includes inventory management, booking workflows, billing automation, and customer support from a single platform.",
    },
    {
      question: "How does workspace membership management work?",
      answer:
        "Workspace memberships allow customers to have guaranteed access to your space. Coworking management software handles membership tiers, billing cycles, and automatic renewal with credit card or invoice payment options.",
    },
    {
      question:
        "Can I use spreadsheets instead of coworking management software?",
      answer:
        "You can start with spreadsheets for a single location, but they become difficult to manage as you add locations, members, and bookings. Coworking management software automates billing, publishing availability, and customer communication at scale.",
    },
    {
      question: "What billing cadences does Skedular Spaces support?",
      answer:
        "Skedular Spaces supports hourly, daily, weekly, monthly, and annual billing. You can set different rates for different resources and offer discounts for longer-term commitments.",
    },
  ],
  finalCTA: {
    heading:
      "Reduce administration. Automate billing. Sell workspace more easily.",
    description:
      "Support customers from one platform. See how Skedular Spaces helps operators manage inventory, publish workspace, automate billing, and grow occupancy.",
    primaryCTA: { label: "Try Spaces", href: "PUBLIC_SKEDULAR_SPACES_APP_URL" },
    secondaryCTA: { label: "View Spaces pricing", href: "/pricing/spaces" },
  },
};
