export const documentationProducts = ["teams", "spaces", "host"] as const;
export type DocumentationProduct =
  (typeof documentationProducts)[number] | "shared";
export type PublicationState =
  "published" | "draft" | "future" | "content-gap" | "withdrawn";

export interface DocumentationCategory {
  id: string;
  label: string;
  description: string;
  product: DocumentationProduct;
  order: number;
  emptyState?: string;
}

export interface DocumentationArticle {
  id: string;
  title: string;
  description: string;
  product: DocumentationProduct;
  category: string;
  slug: string;
  articleKind:
    "landing" | "guide" | "reference" | "faq" | "best-practice" | "placeholder";
  publicationState: PublicationState;
  evidenceRefs: string[];
  terminologyRefs: string[];
  relatedArticleIds: string[];
  updatedAt: string;
  replacementArticleId?: string;
}

const categorySeeds = [
  [
    "getting-started",
    "Getting Started",
    "Set up the essentials and make your first booking.",
  ],
  [
    "core-features",
    "Core Features",
    "Learn the building blocks of the product.",
  ],
  [
    "your-place",
    "Your place",
    "Keep the details and availability of your place up to date.",
  ],
  [
    "pricing-and-availability",
    "Pricing and availability",
    "Set prices, booking times, duration limits, and cancellation terms for your place.",
  ],
  [
    "bookings-and-renters",
    "Bookings and renters",
    "View bookings for your place, understand who booked, and manage available booking actions.",
  ],
  [
    "payments-and-refunds",
    "Payments and refunds",
    "Set up payments, understand payment status, and manage refunds for your place.",
  ],
  [
    "managing-your-listing",
    "Managing your listing",
    "Keep your place information, pricing, availability, and renter-facing listing current.",
  ],
  ["bookings", "Bookings", "Understand booking workflows for this product."],
  [
    "settings",
    "Settings",
    "Manage organization setup, access, and configuration.",
  ],
  [
    "integrations",
    "Integrations",
    "Connect supported tools with public-safe guidance.",
  ],
  ["faqs", "FAQs", "Find concise answers to common questions."],
  [
    "best-practices",
    "Best Practices",
    "Use practical patterns for a reliable rollout.",
  ],
] as const;

/** Product information architecture. Do not assume every product supports every category. */
export const documentationProductCategories: Record<
  Exclude<DocumentationProduct, "shared">,
  string[]
> = {
  teams: [
    "getting-started",
    "workplace-setup",
    "bookings",
    "integrations",
    "faqs",
    "best-practices",
  ],
  spaces: [
    "getting-started",
    "workspace-setup",
    "bookings",
    "products-and-marketplace",
    "customers",
    "faqs",
    "billing-and-payments",
    "best-practices",
    "analytics",
  ],
  host: [
    "getting-started",
    "your-place",
    "core-features",
    "bookings",
    "payments-and-refunds",
    "managing-your-listing",
    "settings",
    "faqs",
    "best-practices",
  ],
};

export const getProductCategories = (
  product: Exclude<DocumentationProduct, "shared">,
) =>
  documentationCategories
    .filter(
      (category) =>
        category.product === product &&
        documentationProductCategories[product].includes(category.id),
    )
    .sort(
      (a, b) =>
        documentationProductCategories[product].indexOf(a.id) -
        documentationProductCategories[product].indexOf(b.id),
    );

export const documentationCategories: DocumentationCategory[] =
  documentationProducts
    .flatMap((product) =>
      categorySeeds.map(([id, label, description], index) => ({
        id,
        label:
          product === "spaces" && id === "core-features"
            ? "Products and marketplace"
            : product === "spaces" && id === "settings"
              ? "Billing and payments"
              : label,
        description:
          product === "spaces"
            ? id === "getting-started"
              ? "Prepare your Organization and workspace before offering it to customers."
              : id === "core-features"
                ? "Create Products, configure pricing, and make offerings available through the marketplace."
                : id === "bookings"
                  ? "Manage one-time Bookings and longer-term Subscriptions for your customers."
                  : id === "settings"
                    ? "Configure payment methods, refunds, bank accounts, and Xero accounting."
                    : id === "faqs"
                      ? "Find concise answers to common Skedular Spaces operator questions."
                      : id === "best-practices"
                        ? "Apply practical guidance for running workspace operations reliably."
                        : description
            : description,
        product,
        order: index + 1,
      })),
    )
    .concat([
      {
        id: "workplace-setup",
        label: "Workplace Setup",
        description:
          "Configure a private workplace by combining Locations, Resources, people, and workspace organization.",
        product: "teams",
        order: 2,
      },
      {
        id: "workspace-setup",
        label: "Set up your workspace",
        description:
          "Prepare Locations, Resources, Zones, and Floor Plans for customer bookings.",
        product: "spaces",
        order: 2,
      },
      {
        id: "products-and-marketplace",
        label: "Products and marketplace",
        description:
          "Create Products, configure pricing, and publish offerings for customers.",
        product: "spaces",
        order: 3,
      },
      {
        id: "billing-and-payments",
        label: "Billing and payments",
        description:
          "Configure payment methods, manage refunds, and connect accounting workflows.",
        product: "spaces",
        order: 6,
      },
      {
        id: "analytics",
        label: "Operator analytics",
        description: "Review workspace activity and operational patterns.",
        product: "spaces",
        order: 7,
      },
      {
        id: "core-concepts",
        label: "Core Concepts",
        description:
          "The shared Skedular domain model and canonical definitions.",
        product: "shared",
        order: 1,
      },
      {
        id: "marketplace",
        label: "Marketplace",
        description: "Products, offers, customers, and subscriptions.",
        product: "shared",
        order: 2,
      },
      {
        id: "commerce",
        label: "Commerce",
        description: "Payments, billing, payouts, and accounting connections.",
        product: "shared",
        order: 3,
      },
      {
        id: "insights",
        label: "Insights",
        description: "Analytics and operational booking insights.",
        product: "shared",
        order: 4,
      },
      {
        id: "administration",
        label: "Administration",
        description: "Opening hours, amenities, and organization settings.",
        product: "shared",
        order: 5,
      },
    ]);

const source = ["spec:033-documentation-center", "code:current-public-web"];
const terms = ["docs-glossary:v1"];
const article = (
  id: string,
  product: DocumentationProduct,
  category: string,
  slug: string,
  title: string,
  description: string,
  articleKind: DocumentationArticle["articleKind"] = "placeholder",
  relatedArticleIds?: string[],
): DocumentationArticle => ({
  id,
  product,
  category,
  slug,
  title,
  description,
  articleKind,
  publicationState: "published",
  evidenceRefs: source,
  terminologyRefs: terms,
  relatedArticleIds:
    relatedArticleIds ??
    (product === "shared"
      ? ["teams-get-started", "spaces-get-started", "host-get-started"]
      : [`${product}-get-started`]),
  updatedAt: "2026-07-14",
});

export const documentationArticles: DocumentationArticle[] = [
  article(
    "teams-get-started",
    "teams",
    "getting-started",
    "getting-started",
    "Get started with Skedular Teams",
    "Set up a private workplace, organize people, and make your first Booking.",
    "guide",
    ["teams-set-up-workplace", "teams-private-bookings"],
  ),
  article(
    "spaces-get-started",
    "spaces",
    "getting-started",
    "getting-started",
    "Getting Started",
    "Set up a commercial workspace operation, create Products, and prepare to publish.",
    "guide",
    ["spaces-products-pricing"],
  ),
  article(
    "host-get-started",
    "host",
    "getting-started",
    "get-started-with-host",
    "Getting Started",
    "Set up your place, choose when it can be booked, configure pricing, and prepare your listing for renters.",
    "guide",
    ["host-pricing"],
  ),
  article(
    "shared-concepts",
    "shared",
    "core-concepts",
    "skedular-concepts",
    "Understanding Skedular",
    "Understand the shared concepts and domain model behind Skedular Teams, Skedular Spaces, and Skedular Host.",
    "reference",
    [
      "shared-organizations",
      "shared-locations",
      "shared-resources",
      "shared-bookings",
      "shared-products",
    ],
  ),
  article(
    "shared-organizations",
    "shared",
    "core-concepts",
    "organizations",
    "Organizations",
    "The ownership boundary for people, locations, resources, settings, and reporting.",
    "reference",
    ["shared-locations", "shared-users"],
  ),
  article(
    "shared-locations",
    "shared",
    "core-concepts",
    "locations",
    "Locations",
    "Physical places that contain resources, opening hours, and location context.",
    "reference",
    ["shared-resources", "shared-floor-plans"],
  ),
  article(
    "shared-resources",
    "shared",
    "core-concepts",
    "resources",
    "Resources",
    "Bookable inventory such as desks, rooms, and other spaces.",
    "reference",
    ["shared-availability", "shared-tags", "shared-zones"],
  ),
  article(
    "shared-users",
    "shared",
    "core-concepts",
    "users",
    "Users",
    "People who access an organization or participate in booking workflows.",
    "reference",
    ["shared-organizations", "shared-bookings"],
  ),
  article(
    "shared-teams",
    "shared",
    "core-concepts",
    "teams",
    "Teams",
    "Groups of users used for private workplace coordination in Skedular Teams.",
    "reference",
    ["shared-users", "shared-bookings"],
  ),
  article(
    "shared-bookings",
    "shared",
    "core-concepts",
    "bookings",
    "Bookings",
    "Reservations of resources for a defined period.",
    "reference",
    ["shared-resources", "shared-availability"],
  ),
  article(
    "shared-floor-plans",
    "shared",
    "core-concepts",
    "floor-plans",
    "Floor Plans",
    "Visual layouts that place resources within a location.",
    "reference",
    ["shared-locations", "shared-resources"],
  ),
  article(
    "shared-availability",
    "shared",
    "core-concepts",
    "availability",
    "Availability",
    "The rules and current state that determine when a resource can be booked.",
    "reference",
    ["shared-resources", "shared-bookings"],
  ),
  article(
    "shared-tags",
    "shared",
    "core-concepts",
    "tags",
    "Tags",
    "Labels used to classify and find resources.",
    "reference",
    ["shared-resources", "shared-zones"],
  ),
  article(
    "shared-zones",
    "shared",
    "core-concepts",
    "zones",
    "Zones",
    "Organization-level groupings used to organize related resources by area, function, or purpose.",
    "reference",
    ["shared-locations", "shared-tags"],
  ),
  article(
    "shared-products",
    "shared",
    "marketplace",
    "products",
    "Products",
    "Create customer-facing offers that combine eligible Resources with pricing, booking rules, and listing details.",
    "reference",
    ["shared-resources", "shared-bookings"],
  ),
  article(
    "shared-customers",
    "shared",
    "marketplace",
    "customers",
    "Customers",
    "Understand the customer records connected to Products, Bookings, and Subscriptions in Skedular Spaces.",
    "reference",
    ["shared-products", "shared-bookings"],
  ),
  article(
    "shared-subscriptions",
    "shared",
    "marketplace",
    "subscriptions",
    "Subscriptions",
    "Manage recurring customer access created from marketplace Products and fulfilled through a series of Bookings.",
    "reference",
    ["shared-products", "shared-bookings"],
  ),
  article(
    "shared-payments",
    "shared",
    "commerce",
    "payments",
    "Payments",
    "Understand how Skedular tracks payment methods, payment status, confirmation, and financial activity for commercial access.",
    "reference",
    ["shared-products", "shared-subscriptions"],
  ),
  article(
    "shared-billing-payouts",
    "shared",
    "commerce",
    "billing-and-payouts",
    "Billing and Payouts",
    "Understand when commercial charges become due, how recurring billing is scheduled, and how operators receive funds through supported payment paths.",
    "reference",
    ["shared-payments"],
  ),
  article(
    "shared-analytics",
    "shared",
    "insights",
    "analytics",
    "Analytics",
    "Understand Booking activity, desk and room occupancy, and Resource availability across your Locations.",
    "reference",
    ["shared-bookings", "shared-availability"],
  ),
  article(
    "shared-organization-settings",
    "shared",
    "administration",
    "organization-settings",
    "Organization Settings",
    "Manage the settings that control how your Organization is configured and presented across Skedular.",
    "reference",
    ["shared-organizations", "shared-locations", "shared-availability"],
  ),
  article(
    "teams-set-up-workplace",
    "teams",
    "workplace-setup",
    "set-up-your-workplace",
    "Set up your workplace",
    "Create a Location, add bookable Resources, and verify that your private workplace is ready for Bookings.",
    "guide",
    [],
  ),
  article(
    "teams-organize-workplace",
    "teams",
    "workplace-setup",
    "organize-your-workplace",
    "Organize your workplace",
    "Use Resource settings, Tags, Zones, and Floor Plans to help people find the right Resources.",
    "guide",
    [],
  ),
  article(
    "teams-organize-people",
    "teams",
    "workplace-setup",
    "organize-your-people",
    "Organize your people",
    "Invite Organization members and organize them into Teams where useful for your workplace.",
    "guide",
    [],
  ),
  article(
    "teams-private-bookings",
    "teams",
    "bookings",
    "bookings",
    "Bookings",
    "Create, view, and manage Bookings in a private workplace.",
    "guide",
    [],
  ),
  article(
    "teams-slack",
    "teams",
    "integrations",
    "slack",
    "Slack integration",
    "Use the Skedular Teams Slack app to manage workplace Bookings and Locations and receive daily updates about who's coming in.",
    "guide",
    [],
  ),
  article(
    "teams-microsoft-teams",
    "teams",
    "integrations",
    "microsoft-teams",
    "Microsoft Teams integration",
    "Documentation for the Skedular Teams Microsoft Teams integration is currently in progress.",
    "guide",
    [],
  ),
  article(
    "teams-sso",
    "teams",
    "integrations",
    "enterprise-sign-in",
    "Enterprise sign-in",
    "Enable Single Sign-On for a Skedular Teams Organization and let members sign in with its enterprise identity provider.",
    "guide",
    [],
  ),
  article(
    "teams-faq",
    "teams",
    "faqs",
    "faqs",
    "FAQs",
    "Quick answers about setting up and using Skedular Teams.",
    "faq",
  ),
  article(
    "teams-best-practices",
    "teams",
    "best-practices",
    "best-practices",
    "Best Practices",
    "Practical guidance for setting up, rolling out, and maintaining Skedular Teams effectively.",
    "best-practice",
  ),
  article(
    "spaces-marketplace-setup",
    "spaces",
    "products-and-marketplace",
    "marketplace-setup",
    "Marketplace setup",
    "Configure the marketplace presence and customer-facing listing details for your workspace.",
  ),
  article(
    "spaces-locations-resources",
    "spaces",
    "workspace-setup",
    "locations-and-resources",
    "Locations and resources",
    "Prepare the physical Locations and bookable Resources that form the foundation of your workspace offering.",
  ),
  article(
    "spaces-zones-floor-plans",
    "spaces",
    "workspace-setup",
    "zones-and-floor-plans",
    "Zones and floor plans",
    "Organize and visually map Resources when that helps you manage the workspace.",
  ),
  article(
    "spaces-products-pricing",
    "spaces",
    "products-and-marketplace",
    "products-and-pricing",
    "Products and pricing",
    "Create Products, select the workspace they can offer, and configure how customers purchase or book them.",
  ),
  article(
    "spaces-analytics",
    "spaces",
    "analytics",
    "analytics",
    "Operator analytics",
    "Understand Booking activity, occupancy, and Resource availability across your Organization and Locations.",
  ),
  article(
    "spaces-bookings",
    "spaces",
    "bookings",
    "bookings",
    "Bookings",
    "Manage marketplace and operator-created bookings.",
  ),
  article(
    "spaces-credit-entitlements",
    "spaces",
    "bookings",
    "credit-entitlements",
    "Credit-based booking entitlements",
    "Configure prepaid booking credits, validity periods, and unused-credit refund handling.",
  ),
  article(
    "spaces-subscriptions",
    "spaces",
    "bookings",
    "subscriptions",
    "Subscriptions",
    "Manage recurring customer arrangements, scheduled Bookings, renewals, and billing.",
  ),
  article(
    "spaces-refunds",
    "spaces",
    "bookings",
    "refunds",
    "Refunds",
    "Understand when refunds apply and how to manage them across supported payment workflows.",
  ),
  article(
    "spaces-bank-payments",
    "spaces",
    "settings",
    "bank-accounts-and-payment-connection",
    "Payment methods",
    "Configure how customers pay for Products and how operators manage each payment flow.",
  ),
  article(
    "spaces-xero",
    "spaces",
    "settings",
    "xero-accounting",
    "Xero accounting",
    "Connect Skedular Spaces with Xero to manage invoices, accounting records, payments, and refund-related credit notes.",
  ),
  article(
    "spaces-faq",
    "spaces",
    "faqs",
    "spaces-faq",
    "FAQs",
    "Quick answers to common questions about setting up, selling, booking, and managing workspace with Skedular Spaces.",
    "faq",
  ),
  article(
    "spaces-operations",
    "spaces",
    "best-practices",
    "operator-operations",
    "Best Practices",
    "Practical guidance for setting up, launching, and operating your workspace effectively with Skedular Spaces.",
    "best-practice",
  ),
  article(
    "host-places",
    "host",
    "your-place",
    "places-and-listings",
    "Your place",
    "Keep the details, location, images, amenities, and other information renters see about your place accurate and up to date.",
  ),
  article(
    "host-pricing",
    "host",
    "pricing-and-availability",
    "pricing-and-availability",
    "Pricing and availability",
    "Set how much your place costs, when renters can book it, and the conditions that apply to their booking.",
    "guide",
    [],
  ),
  article(
    "host-media",
    "host",
    "managing-your-listing",
    "managing-your-listing",
    "Managing your listing",
    "Keep your place information, pricing, availability, and renter-facing listing current.",
    "guide",
    [],
  ),
  article(
    "host-bookings",
    "host",
    "bookings-and-renters",
    "bookings-and-renters",
    "Bookings and renters",
    "View bookings for your place, understand who booked, and manage the actions available for each booking.",
    "guide",
    [],
  ),
  article(
    "host-credit-entitlements",
    "host",
    "bookings-and-renters",
    "credit-entitlements",
    "Credit-based booking entitlements",
    "Offer prepaid booking credits and define expiry and unused-credit refund handling.",
    "guide",
    [],
  ),
  article(
    "host-payments",
    "host",
    "payments-and-refunds",
    "payments-and-refunds",
    "Payments and refunds",
    "Set up how you receive payments, understand payment status, and manage refunds when they are required.",
    "guide",
    [],
  ),
  article(
    "host-faq",
    "host",
    "faqs",
    "host-faq",
    "FAQs",
    "Answers to common questions about setting up, renting out, and managing your place with Skedular Host.",
    "faq",
  ),
  article(
    "host-operations",
    "host",
    "best-practices",
    "best-practices",
    "Best Practices",
    "Practical guidance for keeping your place accurate, bookable, and easy for renters to understand.",
    "best-practice",
  ),
];

export const getDocumentationPath = (
  article: Pick<DocumentationArticle, "product" | "category" | "slug">,
) =>
  article.product === "host" && article.slug === "get-started-with-host"
    ? "/docs/host/getting-started"
    : article.product === "host" && article.slug === "places-and-listings"
      ? "/docs/host/your-place"
      : article.product === "host" &&
          [
            "pricing",
            "availability-and-booking-rules",
            "cancellation-policies",
          ].includes(article.slug)
        ? "/docs/host/pricing-and-availability"
        : article.product === "host" && article.slug === "bookings-and-renters"
          ? "/docs/host/bookings-and-renters"
          : article.product === "host" &&
              [
                "payments-cancellations-and-refunds",
                "payment-connection",
                "payments-and-refunds",
              ].includes(article.slug)
            ? "/docs/host/payments-and-refunds"
            : article.product === "host" &&
                [
                  "organization-settings",
                  "media-and-amenities",
                  "managing-your-listing",
                ].includes(article.slug)
              ? "/docs/host/managing-your-listing"
              : article.product === "host" && article.slug === "host-faq"
                ? "/docs/host/faqs"
                : article.product === "host" &&
                    ["listing-operations", "best-practices"].includes(
                      article.slug,
                    )
                  ? "/docs/host/best-practices"
                  : article.product === "teams" &&
                      article.category === "getting-started"
                    ? "/docs/teams/getting-started"
                    : article.product === "teams" &&
                        article.category === "bookings"
                      ? "/docs/teams/bookings"
                      : article.product === "spaces" &&
                          article.category === "getting-started"
                        ? "/docs/spaces/getting-started"
                        : article.product === "teams" &&
                            article.category === "faqs"
                          ? "/docs/teams/faqs"
                          : article.product === "spaces" &&
                              article.category === "bookings" &&
                              article.slug === "bookings"
                            ? "/docs/spaces/bookings"
                            : article.product === "spaces" &&
                                article.slug === "locations-and-resources"
                              ? "/docs/spaces/workspace-setup/locations-and-resources"
                              : article.product === "spaces" &&
                                  article.slug === "zones-and-floor-plans"
                                ? "/docs/spaces/workspace-setup/zones-and-floor-plans"
                                : article.product === "spaces" &&
                                    article.slug === "marketplace-setup"
                                  ? "/docs/spaces/products-and-marketplace/marketplace-setup"
                                  : article.product === "spaces" &&
                                      article.slug === "products-and-pricing"
                                    ? "/docs/spaces/products-and-marketplace/products-and-pricing"
                                    : article.product === "spaces" &&
                                        article.slug ===
                                          "marketplace-publishing"
                                      ? "/docs/spaces/products-and-marketplace/marketplace-publishing"
                                      : article.product === "spaces" &&
                                          article.slug === "analytics"
                                        ? "/docs/spaces/analytics"
                                        : article.product === "spaces" &&
                                            article.slug ===
                                              "bank-accounts-and-payment-connection"
                                          ? "/docs/spaces/billing-and-payments/payment-methods"
                                          : article.product === "spaces" &&
                                              article.slug === "refunds"
                                            ? "/docs/spaces/billing-and-payments/refunds"
                                            : article.product === "spaces" &&
                                                article.slug ===
                                                  "xero-accounting"
                                              ? "/docs/spaces/billing-and-payments/xero-accounting"
                                              : article.product === "spaces" &&
                                                  article.category === "faqs"
                                                ? "/docs/spaces/faqs"
                                                : article.product ===
                                                      "spaces" &&
                                                    article.category ===
                                                      "best-practices"
                                                  ? "/docs/spaces/best-practices"
                                                  : article.product ===
                                                        "teams" &&
                                                      article.category ===
                                                        "best-practices"
                                                    ? "/docs/teams/best-practices"
                                                    : `/docs/${article.product}/${article.category}/${article.slug}`;
export const getProductPath = (
  product: Exclude<DocumentationProduct, "shared">,
) => `/docs/${product}`;
export const getCategoryPath = (
  product: DocumentationProduct,
  category: string,
) =>
  product === "spaces" && category === "settings"
    ? "/docs/spaces/billing-and-payments"
    : `/docs/${product}/${category}`;
export const publishedDocumentationArticles = documentationArticles.filter(
  (article) => article.publicationState === "published",
);
export const withdrawnDocumentationArticles = documentationArticles.filter(
  (article) => article.publicationState === "withdrawn",
);

export const validateDocumentationCatalog = () => {
  const errors: string[] = [];
  const ids = new Set<string>();
  const paths = new Set<string>();
  for (const item of documentationArticles) {
    const path = getDocumentationPath(item);
    if (ids.has(item.id)) errors.push(`[duplicate-id] ${item.id}`);
    if (paths.has(path)) errors.push(`[duplicate-path] ${path}`);
    if (!item.evidenceRefs.length) errors.push(`[missing-evidence] ${item.id}`);
    if (!item.terminologyRefs.length)
      errors.push(`[missing-terminology] ${item.id}`);
    if (
      !documentationCategories.some(
        (category) =>
          category.product === item.product && category.id === item.category,
      )
    )
      errors.push(`[invalid-category] ${item.id}`);
    ids.add(item.id);
    paths.add(path);
  }
  for (const item of documentationArticles) {
    for (const relatedId of item.relatedArticleIds)
      if (!ids.has(relatedId))
        errors.push(`[invalid-related-link] ${item.id}:${relatedId}`);
    if (item.replacementArticleId && !ids.has(item.replacementArticleId))
      errors.push(
        `[invalid-replacement] ${item.id}:${item.replacementArticleId}`,
      );
  }
  return errors;
};

export const capabilityCoverage = documentationArticles.map((article) => ({
  capability: article.id,
  product: article.product,
  articleId: article.id,
  coverageDecision:
    article.articleKind === "placeholder" ? "placeholder" : "article",
}));
