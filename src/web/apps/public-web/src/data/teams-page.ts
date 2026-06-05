import type { ProductPageContent } from "./content-types";

export const teamsPage: ProductPageContent = {
  id: "teams",
  eyebrow: "Skedular Teams",
  title: "Private workplace management for modern organizations",
  summary:
    "Give employees a simple way to reserve desks, rooms, parking, and equipment while workplace teams keep attendance, space use, and collaboration visible.",
  audience: "Enterprises, government teams, hybrid workplaces, facilities teams, executive assistants, and corporate offices.",
  sections: [
    {
      title: "Resource booking",
      body: "Make everyday workplace resources easier to find and reserve.",
      items: ["Desk booking", "Room booking", "Parking booking", "Equipment and shared resource booking"],
    },
    {
      title: "Workplace experience",
      body: "Help people plan office days with enough context to make the trip worthwhile.",
      items: ["Team attendance", "Floor plans", "Map-based workplace views", "Availability and opening-hour rules"],
    },
    {
      title: "Collaboration",
      body: "Keep workspace activity close to the tools teams already use.",
      items: ["Slack integration", "Microsoft Teams integration", "Clear notifications", "Team-aware booking views"],
    },
    {
      title: "Administration and security",
      body: "Support organizations that need control, reporting, and identity-aware access.",
      items: ["Analytics and reporting", "Enterprise identity", "WorkOS and SSO planning", "Private metadata controls"],
    },
  ],
};
