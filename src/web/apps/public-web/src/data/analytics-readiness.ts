export const analyticsReadiness = {
  trackingEnabled: false,
  vendor: "none",
  privacyRule: "No tracking scripts or vendor-specific identifiers are added in this feature.",
  pageCategories: ["home", "product", "pricing", "resource", "support", "feature", "comparison"],
  ctaIdentifiers: ["search-workspace", "book-workspace", "book-demo", "login", "get-started", "contact-sales", "learn-teams", "learn-spaces"],
  routeFamilies: ["/", "/teams", "/spaces", "/pricing", "/resources", "/support", "/features", "/compare"],
  futureMeasurementNotes: [
    "A future analytics provider can use page category and CTA identifiers without changing page meaning.",
    "Consent, regional privacy rules, and vendor configuration must be reviewed before enabling tracking.",
    "Build diagnostics must never print full public destination URL values.",
  ],
};
