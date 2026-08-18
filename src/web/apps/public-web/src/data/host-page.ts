import type { ProductPageContent } from "./content-types";

export const hostPage: ProductPageContent = {
  id: "host",
  eyebrow: "Skedular Host",
  title: "Simple Space Rental Software for Independent Hosts",
  summary:
    "List a place, set flexible prices and cancellation rules, accept card bookings, and manage renters without learning coworking software.",
  audience:
    "Individuals and small independent hosts renting a room, desk, studio, property, event space, parking space, or other place.",
  heroHeading: "Rent out your place without the marketplace admin",
  heroDescription:
    "Create a location and Skedular Host prepares the private listing behind the scenes. Add hourly, daily, weekly, monthly, or longer-term prices, choose your cancellation policy, connect Stripe, and publish only when you are ready.",
  heroCTAPrimary: {
    label: "Try Host",
    href: "PUBLIC_SKEDULAR_HOST_APP_URL",
  },
  heroCTASecondary: { label: "View Host pricing", href: "/pricing/host" },
  whyOrganizationsNeedMore: {
    heading: "Hosting should not feel like running a coworking business",
    description:
      "Independent hosts need a clear way to publish one place, control its price and availability, and manage bookings. Skedular Host keeps resources, product tags, and marketplace configuration out of sight.",
    cards: [
      {
        title: "Start with the place",
        description:
          "Add the room, desk, studio, property, or venue you want to rent. The booking setup is prepared automatically.",
      },
      {
        title: "Choose how you charge",
        description:
          "Offer different prices for an hour, day, week, month, or longer stay from one listing.",
      },
      {
        title: "Stay in control",
        description:
          "Keep the listing private while you finish it, then activate it when the details and policies are ready.",
      },
      {
        title: "Manage bookings simply",
        description:
          "See bookings, renters, payments, cancellations, and refunds from the same owner-facing app.",
      },
    ],
  },
  typicalJourney: {
    heading: "How independent hosts use Skedular Host",
    steps: [
      {
        title: "Create your Host organization",
        description:
          "Set up your profile and ownership details in the dedicated Host app.",
      },
      {
        title: "Add the place you want to rent",
        description:
          "Describe the location, address, opening hours, amenities, and customer-facing information.",
      },
      {
        title: "Complete the private draft",
        description:
          "Skedular automatically creates the hidden booking setup and a private product draft for the location.",
      },
      {
        title: "Set prices and policies",
        description:
          "Add one or more pricing tiers and choose the cancellation and refund rules customers will see.",
      },
      {
        title: "Connect Stripe",
        description:
          "Accept card payments and receive Host proceeds through your connected Stripe account.",
      },
      {
        title: "Activate the listing",
        description:
          "Publish explicitly when the listing is complete and your Host organization is verified.",
      },
      {
        title: "Manage renters and bookings",
        description:
          "Track upcoming bookings, payment activity, cancellations, refunds, and performance.",
      },
    ],
  },
  features: [
    {
      title:
        "Everything an independent host needs, without resource management",
      body: "Skedular Host uses the same dependable booking platform as Skedular Spaces while presenting a much simpler place-first workflow.",
      featureBlocks: [
        {
          title: "Place-first listing setup",
          description:
            "Create the place once and let Skedular prepare its private booking structure automatically.",
          items: [
            "Location details",
            "Images and amenities",
            "Opening hours",
            "Private draft listing",
            "Explicit activation",
          ],
          accent: "emerald",
        },
        {
          title: "Flexible pricing",
          description:
            "Match the way people rent your place with clear purchase options, including booking credits for customers who want to schedule later.",
          items: [
            "Hourly pricing",
            "Half-day and daily pricing",
            "Weekly and monthly pricing",
            "Longer-term pricing",
            "Prepaid booking credits",
            "Tax-inclusive options",
          ],
          accent: "aqua",
        },
        {
          title: "Card payments and payouts",
          description:
            "Use Stripe Connect for card payments, Host proceeds, Skedular commission, and policy-approved refunds.",
          items: [
            "Card payment only",
            "Stripe Connect payouts",
            "5% Skedular commission",
            "Payment history",
            "Refund processing",
          ],
          accent: "violet",
        },
        {
          title: "Booking administration",
          description:
            "Run the rental from an owner-only application designed around the whole place being booked.",
          items: [
            "Booking calendar",
            "Renter management",
            "Cancellation policies",
            "Availability protection",
            "Performance insights",
          ],
          accent: "sunbeam",
        },
        {
          title: "Prepaid booking credits",
          description:
            "Give customers the flexibility to buy now and choose an eligible date later. Credits become available after payment is confirmed, and you set the allowed days, validity period, and unused-credit refund policy.",
          items: [
            "Buy now, schedule later",
            "Eligible booking days",
            "Confirmed payment before use",
            "Expiry, refunds, and auto-renewal",
          ],
          accent: "emerald",
        },
      ],
    },
  ],
  whyChooseUs: {
    heading: "Why independent hosts choose Skedular Host",
    cards: [
      {
        title: "No resource setup",
        description:
          "You manage the place customers see. Skedular manages the hidden booking resource and product mapping.",
      },
      {
        title: "No monthly software subscription",
        description:
          "Host is free to set up. Skedular earns a fixed 5% commission when a paid booking succeeds.",
      },
      {
        title: "Publish deliberately",
        description:
          "A new place starts as a private draft and cannot become public until you activate it.",
      },
      {
        title: "Built on the existing booking engine",
        description:
          "Availability and conflict protection use the same resource-based booking system as Skedular Spaces.",
      },
    ],
  },
  differentiation: {
    heading: "Host simplicity without a separate booking system",
    description:
      "Skedular Host removes coworking-specific setup from the owner experience while keeping the established booking, availability, payment, and refund infrastructure underneath.",
    withoutSkedular: [
      "Manual calendars",
      "Payment links",
      "Pricing spreadsheets",
      "Separate renter records",
      "Manual refund tracking",
      "Marketplace configuration",
    ],
    withSkedular: [
      "One place-first listing",
      "Flexible pricing tiers",
      "Prepaid booking credits",
      "Card payments",
      "Connected payouts",
      "Booking administration",
      "Policy-driven cancellations and refunds",
    ],
  },
  operatorContext: {
    heading: "Designed for hosts, not workspace operators",
    content:
      "Skedular Spaces is built for operators managing locations with many explicit resources. Skedular Host is for someone with a place to rent. Each Host location has one system-managed booking resource, so the owner can focus on the listing, pricing, policies, renters, and bookings.",
  },
  integrations: {
    heading: "Reach customers through Skedular discovery",
    body: "Once verified and activated, Host listings can participate in the same discovery and booking experience as other marketplace locations while remaining clearly identified as Host listings.",
    integrations: [
      "Marketplace discovery",
      "Map visibility",
      "Host verification",
      "Card checkout",
      "Stripe Connect",
    ],
  },
  commsIntegration: {
    heading: "A clear financial boundary",
    description:
      "Booking policy decides whether a cancellation qualifies for a refund. Skedular initiates the approved refund, and Stripe handles the money movement against the connected Host account.",
  },
  builtFor: {
    heading: "Built for places people want to rent",
    body: "Treat the thing being rented as the location, whether it is an entire property, one room, a desk, a studio, a venue, or another independently bookable place.",
    audiences: [
      "Property hosts",
      "Room hosts",
      "Desk owners",
      "Studio owners",
      "Event-space hosts",
      "Parking-space hosts",
      "Small venue owners",
      "Independent space providers",
    ],
  },
  aiSummary: {
    heading: "Skedular Host at a glance",
    description:
      "Skedular Host is simple rental-management software for individuals and small independent hosts listing one whole bookable place per location.",
    operatorsUse: [
      "Create a place",
      "Complete a private listing draft",
      "Set pricing tiers",
      "Choose cancellation policies",
      "Connect Stripe",
      "Publish a listing",
      "Manage bookings and renters",
    ],
    keyCapabilities: [
      "Automatic booking setup",
      "Flexible pricing",
      "Prepaid booking credits",
      "Card payments",
      "Stripe Connect payouts",
      "Cancellation and refund policies",
      "Marketplace discovery",
    ],
  },
  faq: [
    {
      question: "What can I rent with Skedular Host?",
      answer:
        "You can list a property, room, desk, studio, event space, parking space, or another place that should be booked as one whole unit. Each thing you rent is represented as its own Host location.",
    },
    {
      question: "Do I need to create resources or product tags?",
      answer:
        "No. When you create a Host location, Skedular prepares the hidden booking resource, product tag, and private product draft automatically.",
    },
    {
      question: "Can I set different prices for the same place?",
      answer:
        "Yes. A location can have pricing options for different durations, including hourly, daily, weekly, monthly, and longer-term bookings.",
    },
    {
      question: "When does my listing become public?",
      answer:
        "It does not publish automatically. Complete the listing and pricing, connect the required payment setup, pass Host verification, and explicitly activate the product when you are ready.",
    },
    {
      question: "How much does Skedular Host cost?",
      answer:
        "There is no monthly Host software subscription for the current offering. Skedular retains a fixed 5% commission from successful paid bookings.",
    },
    {
      question: "Which payment methods are supported?",
      answer:
        "Skedular Host currently supports card payments through Stripe only. Bank transfer is not part of the Host offering.",
    },
    {
      question: "Who decides whether a cancelled booking is refunded?",
      answer:
        "The cancellation policy configured for the pricing option determines refund eligibility and amount. Skedular applies that policy and asks Stripe to process an approved refund.",
    },
  ],
  finalCTA: {
    heading: "Turn your place into a bookable listing",
    description:
      "Add the place, choose the prices and policies, connect Stripe, and publish when you are ready. Skedular handles the booking setup behind the scenes.",
    primaryCTA: { label: "Try Host", href: "PUBLIC_SKEDULAR_HOST_APP_URL" },
    secondaryCTA: { label: "View Host pricing", href: "/pricing/host" },
  },
};
