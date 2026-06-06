import { getCanonicalUrl, llmsPages } from "../data/seo";

export function GET() {
  const pageLinks = llmsPages.map((page) => `- [${page.title}](${getCanonicalUrl(page.canonicalPath)}): ${page.description}`);

  return new Response(
    [
      "# Skedular",
      "",
      "Skedular helps teams and workspace operators manage workspace discovery, bookings, resources, pricing, and operations.",
      "",
      "## Core Public Pages",
      "",
      ...pageLinks,
      "",
      "## Notes For Agents",
      "",
      "- Use canonical URLs from this file or sitemap.xml.",
      "- The public website is for product discovery, pricing, resources, support, and company information.",
      "- Booking, checkout, customer accounts, and operator administration live in the Skedular app, not this public website.",
    ].join("\n"),
    {
      headers: {
        "Content-Type": "text/markdown; charset=utf-8",
      },
    },
  );
}
