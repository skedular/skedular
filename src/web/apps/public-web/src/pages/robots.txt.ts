import { siteUrl } from "../data/seo";

export function GET() {
  return new Response(
    [
      "User-agent: *",
      "Allow: /",
      `Sitemap: ${new URL("/sitemap.xml", siteUrl).toString()}`,
      `Host: ${new URL(siteUrl).host}`,
    ].join("\n"),
    {
      headers: {
        "Content-Type": "text/plain; charset=utf-8",
      },
    },
  );
}
