export const routeFamilies = {
  home: "/",
  teams: "/teams",
  spaces: "/spaces",
  pricing: "/pricing",
  company: "/company",
  blog: "/blog",
  resources: "/resources/",
  support: "/support",
  features: "/features",
  compare: "/compare",
  termsOfService: "/terms-of-service",
  privacyPolicy: "/privacy-policy",
} as const;

export const primaryRoutes = [
  { id: "home", label: "Home", path: routeFamilies.home },
  { id: "teams", label: "Teams", path: routeFamilies.teams },
  { id: "spaces", label: "Spaces", path: routeFamilies.spaces },
  { id: "pricing", label: "Pricing", path: routeFamilies.pricing },
  { id: "blog", label: "Blog", path: routeFamilies.blog },
  { id: "company", label: "Company", path: routeFamilies.company },
] as const;

export const utilityRoutes = [
  { id: "support", label: "Support", path: routeFamilies.support },
  { id: "features", label: "Features", path: routeFamilies.features },
  { id: "compare", label: "Compare", path: routeFamilies.compare },
  { id: "terms-of-service", label: "Terms", path: routeFamilies.termsOfService },
  { id: "privacy-policy", label: "Privacy", path: routeFamilies.privacyPolicy },
] as const;
