import { toPricingPageModels } from "./pricing-catalog/pricing-catalog";

export const pricingPage = {
  title: "Skedular Pricing | Teams, Spaces, and Host",
  description:
    "Compare Skedular Teams pricing for private workplaces, Spaces pricing for workspace operators, and commission-based Host pricing for independent rentals.",
  headline: "Pricing for teams, workspace operators, and independent hosts",
  intro:
    "Skedular uses pricing that matches each operating model. Teams is based on monthly active users. Spaces starts with a 14-day trial followed by fixed monthly plans based on booking-instance volume. Host has no monthly subscription and charges 5% per successful paid booking.",
  note: "Choose Teams for private workplaces, Spaces for coworking and flexible-workspace operations, or Host for a simple place-first rental workflow.",
  models: toPricingPageModels(),
};
