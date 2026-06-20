import { defineConfig } from "astro/config";

export default defineConfig({
  output: "static",
  redirects: {
    "/company": "/about",
    "/creating-a-productive-and-flexible-workplace-a-guide-for-modern-offices":
      "/blog/creating-a-productive-and-flexible-workplace-a-guide-for-modern-offices",
    "/the-smart-workplace-of-2025-rethinking-office-strategy-for-the-hybrid-era":
      "/blog/the-smart-workplace-of-2025-rethinking-office-strategy-for-the-hybrid-era",
    "/how-to-create-a-high-performance-office-a-modern-space-planning-guide":
      "/blog/how-to-create-a-high-performance-office-a-modern-space-planning-guide",
    "/how-to-determine-the-right-amount-of-office-space-for-your-team":
      "/blog/how-to-determine-the-right-amount-of-office-space-for-your-team",
    "/desk-sharing-with-skedular-the-future-of-flexible-workspaces":
      "/blog/desk-sharing-with-skedular-the-future-of-flexible-workspaces",
    "/boosting-workplace-visibility-keeping-hybrid-teams-connected-and-engaged":
      "/blog/boosting-workplace-visibility-keeping-hybrid-teams-connected-and-engaged",
    "/learn-to-manage-your-time-better":
      "/blog/learn-to-manage-your-time-better",
  },
});
