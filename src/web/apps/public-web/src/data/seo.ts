import { publicPages } from "./content-inventory";
import { resourceArticles, supportArticles } from "./current-public-content";

export const siteUrl = process.env.PUBLIC_WEB_SITE_URL ?? "https://www.getascheduler.com";

export const defaultRobots = "index, follow";

export const noIndexRobots = "noindex, follow";

export const defaultSocialImagePath = "/images/skedular-logo-primary.svg";

export const defaultSocialImageUrl = new URL(defaultSocialImagePath, siteUrl).toString();

export const sitemapPages = publicPages
  .filter((page) => page.metadataStatus === "published" && page.contentStatus === "published")
  .map((page) => ({
    path: page.canonicalPath,
    lastModified: "2026-06-05",
  }))
  .filter((page, index, pages) => pages.findIndex((candidate) => candidate.path === page.path) === index)
  .sort((left, right) => left.path.localeCompare(right.path));

export const getCanonicalUrl = (canonicalPath: string) => new URL(canonicalPath, siteUrl).toString();

export const getRobotsForPath = (canonicalPath: string) => {
  const page = publicPages.find((candidate) => candidate.canonicalPath === canonicalPath);

  if (!page) {
    return defaultRobots;
  }

  return page.metadataStatus === "published" && page.contentStatus === "published" ? defaultRobots : noIndexRobots;
};

export const getArticleMetadataForPath = (canonicalPath: string) =>
  [...resourceArticles, ...supportArticles].find((article) => article.destinationPath === canonicalPath) ?? null;

export const llmsPages = [...publicPages.filter((page) => ["home", "product", "pricing", "feature", "resource", "support"].includes(page.pageType))]
  .filter((page) => page.metadataStatus === "published" && page.contentStatus === "published")
  .filter((page, index, pages) => pages.findIndex((candidate) => candidate.canonicalPath === page.canonicalPath) === index)
  .sort((left, right) => left.canonicalPath.localeCompare(right.canonicalPath));
