import type { Cta } from "./content-types";

export const ctas = [
  {
    id: "search-workspace",
    label: "Search workspace",
    purpose: "search",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_APP_URL",
    audience: "public bookers",
  },
  {
    id: "book-workspace",
    label: "Book workspace",
    purpose: "book",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_APP_URL",
    audience: "public bookers",
  },
  {
    id: "book-demo",
    label: "Book demo",
    purpose: "demo",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_DEMO_URL",
    audience: "buyers and operators",
  },
  {
    id: "login",
    label: "Login",
    purpose: "login",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_SIGNUP_URL",
    audience: "existing users",
  },
  {
    id: "become-host",
    label: "Become a host",
    purpose: "contact",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_BECOME_HOST_URL",
    audience: "workspace operators",
  },
  {
    id: "get-started",
    label: "Get started",
    purpose: "sign-up",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_SIGNUP_URL",
    audience: "new visitors",
  },
  {
    id: "explore-skedular",
    label: "Explore Skedular",
    purpose: "explore",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_APP_URL",
    audience: "new visitors",
  },
  {
    id: "contact-sales",
    label: "Contact sales",
    purpose: "contact",
    destinationType: "public-url",
    destinationRef: "PUBLIC_SKEDULAR_DEMO_URL",
    audience: "business buyers",
  },
  {
    id: "learn-teams",
    label: "Learn about Teams",
    purpose: "learn-more",
    destinationType: "internal-route",
    destinationRef: "/teams",
    audience: "organization buyers",
  },
  {
    id: "learn-spaces",
    label: "Learn about Spaces",
    purpose: "learn-more",
    destinationType: "internal-route",
    destinationRef: "/spaces",
    audience: "workspace operators",
  },
] satisfies Cta[];

export type CtaId = (typeof ctas)[number]["id"];

export function getCta(id: CtaId): Cta {
  const cta = ctas.find((item) => item.id === id);

  if (!cta) {
    throw new Error(`Unknown CTA: ${id}`);
  }

  return cta;
}
