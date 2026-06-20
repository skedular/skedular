type CatalogPlan = {
  code:
    | "free"
    | "pay-as-you-go"
    | "enterprise-capacity"
    | "growth"
    | "business"
    | "commission"
    | "contact-us";
  name: string;
  description: string;
  bestFor: string;
  prices: { amount: number; currency: string; cadence: string }[];
  capacityOptions: {
    label: string;
    amount?: number;
    currency?: string;
    cadence?: string;
    availability: "self-service" | "contact-us";
  }[];
  highlights: string[];
};

type CatalogProduct = {
  id: "teams" | "spaces" | "host";
  name: string;
  audience: string;
  basis: string;
  seoTitle: string;
  seoDescription: string;
  headline: string;
  intro: string;
  buyingNote: string;
  explanationTitle?: string;
  explanationBody?: string;
  includedTitle: string;
  includedItems: string[];
  faq: { question: string; answer: string }[];
  ctaId: "book-demo" | "try-host";
  plans: CatalogPlan[];
};

export const pricingCatalog = {
  version: "TEAMS_V1_SPACES_V1",
  products: [
    {
      id: "teams",
      name: "Teams",
      audience:
        "Private organizations managing employees, office attendance, and shared workplace resources",
      basis: "Monthly active-user pricing",
      seoTitle: "Skedular Teams Pricing | Private Workplace Management",
      seoDescription:
        "Compare Skedular Teams pricing for private workplaces. Start free, pay only for monthly active users, or choose enterprise capacity for larger organizations.",
      headline: "Teams pricing for private workplaces",
      intro:
        "Skedular Teams is workplace management software for private organizations that need a clear way to manage desks, meeting rooms, workplace attendance, teams, locations, and shared resources. Pricing is based on monthly active users, so you are not paying for employees who sit in the directory but do not use the product.",
      buyingNote:
        "A monthly active user is someone who actually uses Skedular Teams during the billing month. Active-user pricing keeps the cost tied to real office use instead of headcount, which matters when not every employee books space every month.",
      includedTitle: "Included in Teams",
      includedItems: [
        "Desk booking",
        "Meeting room booking",
        "Resource booking",
        "Workplace attendance",
        "Team management",
        "Location management",
        "Unlimited bookings",
        "Booking workflows in Slack",
        "Booking workflows in Microsoft Teams",
        "Interactive floor plans",
        "Workplace analytics",
        "Enterprise SSO support",
      ],
      faq: [
        {
          question: "What is a monthly active user?",
          answer:
            "A monthly active user is someone who uses Skedular Teams during the billing month. This keeps billing tied to real workplace activity instead of the number of employees stored in your directory.",
        },
        {
          question: "Do inactive employees count toward billing?",
          answer:
            "No. If an employee does not use Teams during the month, they do not count toward active-user billing. This is useful for companies where office attendance changes week to week.",
        },
        {
          question: "Can we use Teams for multiple locations?",
          answer:
            "Yes. Paid Teams plans support multiple locations, so you can manage desks, rooms, resources, attendance, and bookings across more than one office.",
        },
        {
          question: "Is Teams suitable for larger organizations?",
          answer:
            "Yes. Larger organizations can use Enterprise for predictable active-user capacity, support for multiple locations, procurement needs, priority support, workplace analytics, and enterprise identity requirements.",
        },
        {
          question: "Can employees book desks from Slack or Microsoft Teams?",
          answer:
            "Yes. Skedular Teams is built for workplace workflows that can run through Slack and Microsoft Teams, so employees can book space and stay close to the tools they already use.",
        },
      ],
      ctaId: "book-demo",
      plans: [
        {
          code: "free",
          name: "Free",
          description:
            "For small teams that need simple office booking software for one workplace.",
          bestFor:
            "Best when one team is starting with desk reservation software, room booking, and shared resources.",
          prices: [{ amount: 0, currency: "USD", cadence: "month" }],
          capacityOptions: [],
          highlights: [
            "Up to 10 monthly active users",
            "One team",
            "One location",
            "Unlimited bookings",
          ],
        },
        {
          code: "pay-as-you-go",
          name: "Pay As You Go",
          description:
            "For organizations where office use changes month to month.",
          bestFor:
            "Best when you want hybrid workplace software and workplace scheduling software that scales with active use.",
          prices: [
            { amount: 3, currency: "USD", cadence: "active user/month" },
          ],
          capacityOptions: [],
          highlights: [
            "$3 USD per monthly active user",
            "Unlimited teams",
            "Unlimited locations",
            "Unlimited bookings",
            "Desk, room, and resource booking",
            "Monthly active-user billing",
          ],
        },
        {
          code: "enterprise-capacity",
          name: "Enterprise",
          description:
            "For larger organizations that need predictable pricing across teams, locations, and workplace programs.",
          bestFor:
            "Best when you need purchased active-user capacity, enterprise identity support, priority support, and procurement support.",
          prices: [],
          capacityOptions: [
            { label: "Contact Us", availability: "contact-us" },
          ],
          highlights: [
            "Purchased active-user capacity",
            "Unlimited teams",
            "Unlimited locations",
            "Unlimited bookings",
            "Desk, room, and resource booking",
            "Procurement-ready billing",
            "Priority support",
            "Custom agreement",
          ],
        },
      ],
    },
    {
      id: "spaces",
      name: "Spaces",
      audience:
        "Coworking spaces, flexible workspace providers, venues, and operators selling bookable workspace",
      basis:
        "14-day trial, then monthly fixed-price plans by booking-instance volume",
      seoTitle:
        "Skedular Spaces Pricing | Coworking and Workspace Operator Plans",
      seoDescription:
        "Start a 14-day Skedular Spaces trial with the existing Free plan limit of 100 booking instances per month, then choose a Growth, Business, or custom monthly plan.",
      headline: "Spaces pricing for coworking and workspace operators",
      intro:
        "Skedular Spaces is coworking management software for operators running coworking spaces, flexible workspaces, serviced offices, and shared workspace businesses. It helps teams manage listings, bookings, memberships, subscriptions, payments, invoicing, and marketplace publishing without stitching together separate tools.",
      buyingNote:
        "Spaces starts with a 14-day trial under the existing Free plan limit of 100 booking instances per month. After the trial, paid pricing is based on booking-instance volume. Locations are unlimited, so operators can add sites without moving plans just because the business grows across more addresses.",
      explanationTitle: "What is a booking instance?",
      explanationBody:
        "A booking instance is a booking created within Skedular. It can represent a desk booking, meeting room booking, event booking, workspace reservation, or another resource reservation. Spaces pricing counts booking instances created during the billing period.",
      includedTitle: "Included in Spaces",
      includedItems: [
        "Workspace listings",
        "Resource booking",
        "Product management",
        "Pricing management",
        "Payments with Stripe support",
        "Subscription billing",
        "Invoicing with Xero integration",
        "Marketplace publishing",
        "Multi-location support",
        "Analytics",
        "GST and VAT support",
        "Custom domains",
      ],
      faq: [
        {
          question: "Is the Spaces Free plan permanent?",
          answer:
            "No. The Spaces Free plan is a 14-day trial that starts when the organization is created. The existing limit of 100 booking instances per month applies during the trial. A paid plan is required after it ends to keep using Spaces and accepting bookings.",
        },
        {
          question: "What is a booking instance?",
          answer:
            "A booking instance is a booking created in Skedular during the billing period. It may be a desk booking, room booking, event booking, workspace reservation, or another resource reservation.",
        },
        {
          question: "How is a location counted?",
          answer:
            "A location is a workspace site managed in Skedular Spaces. Current plans include unlimited locations, so pricing is based on booking activity rather than how many sites you operate.",
        },
        {
          question: "Can Spaces support multiple locations?",
          answer:
            "Yes. Spaces supports multi-location operators, including coworking groups, serviced offices, flexible workspace providers, and shared office software use cases.",
        },
        {
          question: "Are marketplace commissions required?",
          answer:
            "No. These plans are based on monthly pricing and booking-instance volume. Marketplace publishing can be part of the workflow, but required commission is not the basis of the current pricing model.",
        },
        {
          question:
            "Does Spaces support payments, invoicing, and subscriptions?",
          answer:
            "Yes. Spaces includes coworking billing software for payments, invoicing, subscription billing, product pricing, and marketplace publishing.",
        },
      ],
      ctaId: "book-demo",
      plans: [
        {
          code: "free",
          name: "14-day free trial",
          description:
            "Try the existing Spaces Free capabilities for 14 days, with up to 100 booking instances per month.",
          bestFor:
            "Best for evaluating listings, bookings, and daily operations before choosing a paid plan.",
          prices: [{ amount: 0, currency: "USD", cadence: "month" }],
          capacityOptions: [],
          highlights: [
            "14 days from organization creation",
            "Up to 100 booking instances per month",
            "Unlimited locations",
            "Upgrade required after the trial",
          ],
        },
        {
          code: "growth",
          name: "Growth",
          description:
            "For spaces with steady booking activity that need more room to grow.",
          bestFor:
            "Best when a workspace booking software workflow is becoming part of day-to-day operations.",
          prices: [{ amount: 49, currency: "USD", cadence: "month" }],
          capacityOptions: [],
          highlights: [
            "Up to 500 booking instances per month",
            "Unlimited locations",
            "Unlimited bookings",
            "Analytics and support",
          ],
        },
        {
          code: "business",
          name: "Business",
          description:
            "For busier operators that need higher booking capacity and stronger support.",
          bestFor:
            "Best for coworking space management across more resources, customers, and recurring activity.",
          prices: [{ amount: 149, currency: "USD", cadence: "month" }],
          capacityOptions: [],
          highlights: [
            "Up to 1,000 booking instances per month",
            "Unlimited locations",
            "Unlimited bookings",
            "Premium support",
            "Analytics and support",
          ],
        },
        {
          code: "contact-us",
          name: "Contact Us",
          description:
            "For operators whose volume, procurement process, or commercial model needs a tailored agreement.",
          bestFor:
            "Best for larger workspace management software rollouts across multiple sites or business units.",
          prices: [],
          capacityOptions: [
            { label: "Contact Us", availability: "contact-us" },
          ],
          highlights: [
            "Custom booking-instance capacity",
            "Unlimited locations",
            "Unlimited bookings",
            "Analytics and support",
            "Custom pricing",
            "Procurement-ready billing",
            "Priority support",
          ],
        },
      ],
    },
    {
      id: "host",
      name: "Host",
      audience:
        "Individuals and small independent hosts renting a whole property, room, desk, studio, venue, parking space, or similar place",
      basis: "No monthly subscription; 5% commission per paid booking",
      seoTitle: "Skedular Host Pricing | Simple Rental Management",
      seoDescription:
        "Start listing with Skedular Host without a monthly software subscription. Skedular retains a fixed 5% commission from successful paid card bookings.",
      headline: "Simple pricing for independent hosts",
      intro:
        "Skedular Host is free to set up and has no monthly software subscription under the current offering. Create your places, complete private listing drafts, set pricing and cancellation policies, and publish when ready. Skedular earns a fixed 5% commission only when a paid booking succeeds.",
      buyingNote:
        "Customers pay by card through Stripe. Skedular retains 5% of the paid booking value as its application fee and routes the remaining Host proceeds to the connected Stripe account. Cancellation and refund amounts follow the policy configured for the purchased pricing option.",
      explanationTitle: "How does the Host commission work?",
      explanationBody:
        "The 5% commission is calculated on each successful paid Host booking. There is no bank-transfer option for Host bookings and no monthly Host subscription in the current offering.",
      includedTitle: "Included in Host",
      includedItems: [
        "Multiple Host locations",
        "Automatic private product drafts",
        "Automatic hidden booking-resource setup",
        "Hourly through longer-term pricing tiers",
        "Cancellation and refund policies",
        "Card payments through Stripe",
        "Stripe Connect Host payouts",
        "Booking and renter management",
        "Marketplace and map discovery after activation",
        "Booking and payment reporting",
      ],
      faq: [
        {
          question: "Is there a monthly Skedular Host subscription?",
          answer:
            "No. The current Host offering has no monthly software subscription. Skedular charges a fixed commission when a paid booking succeeds.",
        },
        {
          question: "What commission does Skedular charge?",
          answer:
            "Skedular retains 5% of the successful paid booking value. The remaining Host proceeds are routed through Stripe Connect.",
        },
        {
          question: "Can renters pay by bank transfer?",
          answer:
            "No. Skedular Host currently supports card payment through Stripe only.",
        },
        {
          question: "Does creating a location publish it immediately?",
          answer:
            "No. A new Host location receives a private product draft. The Host must complete the listing and explicitly activate it after verification.",
        },
        {
          question: "Can each place have different prices?",
          answer:
            "Yes. Each Host location has its own product and can use different pricing tiers and cancellation policies.",
        },
      ],
      ctaId: "try-host",
      plans: [
        {
          code: "commission",
          name: "Host",
          description:
            "For independent hosts who want to rent a place without managing coworking resources or paying a monthly software subscription.",
          bestFor:
            "Best when you want a simple place-first listing, flexible prices, card payments, connected payouts, and owner-facing booking administration.",
          prices: [],
          capacityOptions: [
            {
              label: "5% per paid booking",
              availability: "self-service",
            },
          ],
          highlights: [
            "No monthly software subscription",
            "5% commission per paid booking",
            "Multiple locations",
            "Flexible pricing tiers",
            "Card payments only",
            "Stripe Connect payouts",
            "Private until explicitly activated",
          ],
        },
      ],
    },
  ] satisfies CatalogProduct[],
};

export function formatCatalogPlanPrice(plan: CatalogPlan) {
  const price = plan.prices[0];
  if (price) {
    if (price.amount === 0) {
      return "Free";
    }

    return `$${price.amount} ${price.currency} per ${price.cadence}`;
  }

  return plan.capacityOptions[0]?.label ?? "";
}

export function toPricingPageModels() {
  return pricingCatalog.products.map((product) => ({
    id: product.id,
    name: product.name,
    audience: product.audience,
    basis: product.basis,
    seoTitle: product.seoTitle,
    seoDescription: product.seoDescription,
    headline: product.headline,
    intro: product.intro,
    buyingNote: product.buyingNote,
    includedTitle: product.includedTitle,
    includedItems: product.includedItems,
    explanationTitle: product.explanationTitle,
    explanationBody: product.explanationBody,
    faq: product.faq,
    tiers: product.plans.map((plan) => ({
      name: plan.name,
      price: formatCatalogPlanPrice(plan),
      summary: plan.description,
      bestFor: plan.bestFor,
      highlights: plan.highlights,
    })),
    ctaId: product.ctaId,
  }));
}
