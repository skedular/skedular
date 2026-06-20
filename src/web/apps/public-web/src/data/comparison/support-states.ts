import type { SupportState } from "../content-types";

// Support state labels for display
export const supportStateLabels: Record<SupportState, string> = {
  supported: "Supported",
  "partially-supported": "Partially Supported",
  "not-supported": "Not Supported",
  unknown: "Unknown",
  "not-applicable": "Not Applicable",
};

// Support state descriptions for tooltips or help text
export const supportStateDescriptions: Record<SupportState, string> = {
  supported: "This feature is fully supported",
  "partially-supported": "This feature is partially supported with limitations",
  "not-supported": "This feature is not supported",
  unknown: "Support status is unknown or not specified",
  "not-applicable": "This feature is not applicable to this product",
};

// Support state icons (emoji or icon names)
export const supportStateIcons: Record<SupportState, string> = {
  supported: "✓",
  "partially-supported": "~",
  "not-supported": "✗",
  unknown: "?",
  "not-applicable": "—",
};

// Helper to get display label for support state
export const getSupportStateLabel = (state: SupportState): string => {
  return supportStateLabels[state];
};

// Helper to get description for support state
export const getSupportStateDescription = (state: SupportState): string => {
  return supportStateDescriptions[state];
};

// Helper to get icon for support state
export const getSupportStateIcon = (state: SupportState): string => {
  return supportStateIcons[state];
};

// Helper to check if a state is considered positive (supported or partially-supported)
export const isPositiveState = (state: SupportState): boolean => {
  return state === "supported" || state === "partially-supported";
};

// Helper to check if a state is considered negative (not-supported)
export const isNegativeState = (state: SupportState): boolean => {
  return state === "not-supported";
};

// Helper to check if a state is considered neutral (unknown or not-applicable)
export const isNeutralState = (state: SupportState): boolean => {
  return state === "unknown" || state === "not-applicable";
};
