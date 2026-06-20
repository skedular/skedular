// Comparison page path constants
// Defines the comparison hub, canonical competitor pages, and removed legacy routes

// Hub path
export const COMPARE_HUB_PATH = "/compare";

// Individual competitor comparison paths (required)
export const INDIVIDUAL_COMPARISON_PATHS = [
  "/compare/skedular-vs-skedda",
  "/compare/skedular-vs-officernd",
  "/compare/skedular-vs-nexudus",
  "/compare/skedular-vs-gable",
  "/compare/skedular-vs-robin",
  "/compare/skedular-vs-officely",
  "/compare/skedular-vs-envoy",
  "/compare/skedular-vs-kadence",
  "/compare/skedular-vs-archie",
  "/compare/skedular-vs-deskbird",
] as const;

// All required comparison paths
export const REQUIRED_COMPARISON_PATHS = [
  COMPARE_HUB_PATH,
  ...INDIVIDUAL_COMPARISON_PATHS,
] as const;

// Legacy comparison paths that must be removed (no redirect/alias)
export const REMOVED_LEGACY_COMPARISON_PATHS = [
  // Any legacy comparison paths that existed before the hub implementation
  // These must not be emitted, redirected, aliased, linked, or listed
] as const;

// Helper to check if a path is a comparison page
export const isComparisonPath = (path: string): boolean => {
  return REQUIRED_COMPARISON_PATHS.some((p) => p === path);
};

// Helper to check if a path is a legacy comparison path
export const isLegacyComparisonPath = (path: string): boolean => {
  return REMOVED_LEGACY_COMPARISON_PATHS.some((p) => p === path);
};

// Helper to get page type from path
export const getPageTypeFromPath = (
  path: string,
): "hub" | "individual-comparison" | "unknown" => {
  if (path === COMPARE_HUB_PATH) return "hub";
  if (INDIVIDUAL_COMPARISON_PATHS.some((p) => p === path))
    return "individual-comparison";
  return "unknown";
};
