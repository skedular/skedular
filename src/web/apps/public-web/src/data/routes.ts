export const routeFamilies = {
  home: "/",
  teams: "/teams",
  spaces: "/spaces",
  host: "/host",
  pricing: "/pricing",
  company: "/about",
  blog: "/blog",
  resources: "/resources/",
  documentation: "/docs",
  compare: "/compare",
  termsOfService: "/terms-of-service",
  privacyPolicy: "/privacy-policy",
} as const;

export const primaryRoutes = [
  { id: "home", label: "Home", path: routeFamilies.home },
  { id: "teams", label: "Teams", path: routeFamilies.teams },
  { id: "spaces", label: "Spaces", path: routeFamilies.spaces },
  { id: "host", label: "Host", path: routeFamilies.host },
  { id: "pricing", label: "Pricing", path: routeFamilies.pricing },
  { id: "blog", label: "Blog", path: routeFamilies.blog },
  {
    id: "documentation",
    label: "Documentation",
    path: routeFamilies.documentation,
  },
  { id: "company", label: "Company", path: routeFamilies.company },
] as const;

export const utilityRoutes = [
  { id: "compare", label: "Compare", path: routeFamilies.compare },
  {
    id: "terms-of-service",
    label: "Terms of Service",
    path: routeFamilies.termsOfService,
  },
  {
    id: "privacy-policy",
    label: "Privacy Policy",
    path: routeFamilies.privacyPolicy,
  },
] as const;
