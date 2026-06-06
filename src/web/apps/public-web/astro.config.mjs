import { defineConfig } from "astro/config";

export default defineConfig({
  output: "static",
  redirects: {
    "/company": "/about",
    "/blog/hybrid-workplace-planning": "/resources/hybrid-workplace-planning",
    "/blog/workspace-payments-invoicing": "/resources/workspace-payments-invoicing",
    "/blog/slack-microsoft-teams-workplace": "/resources/slack-microsoft-teams-workplace",
    "/support/getting-started": "/support/getting-started-with-skedular",
    "/support/booking-workspace": "/support/booking-workspace-from-public-site",
  },
});
