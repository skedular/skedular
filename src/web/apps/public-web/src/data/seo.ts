import { publicPages } from "./content-inventory";
import { resourceArticles } from "./current-public-content";
import { legalPages } from "./legal-pages";
import { documentationArticles, getDocumentationPath } from "./documentation";

export const siteUrl =
  import.meta.env.PUBLIC_WEB_SITE_URL?.trim() ||
  process.env.PUBLIC_WEB_SITE_URL?.trim() ||
  "https://getskedular.com";

export const defaultRobots = "index, follow";

export const noIndexRobots = "noindex, follow";

export const defaultSocialImagePath = "/images/skedular-logo-primary.svg";

export const defaultSocialImageUrl = new URL(
  defaultSocialImagePath,
  siteUrl,
).toString();

const defaultLastModified = "2026-06-21";

const getLastModifiedForPath = (canonicalPath: string) => {
  const article = resourceArticles.find(
    (candidate) => candidate.destinationPath === canonicalPath,
  );

  if (article) {
    return article.publishedDate;
  }

  const legalPage = legalPages.find(
    (candidate) => candidate.path === canonicalPath,
  );

  const documentationArticle = documentationArticles.find(
    (candidate) => getDocumentationPath(candidate) === canonicalPath,
  );

  if (documentationArticle) {
    return documentationArticle.updatedAt;
  }

  if (legalPage) {
    return legalPage.lastSourceReview;
  }

  return defaultLastModified;
};

export const sitemapPages = publicPages
  .filter(
    (page) =>
      page.metadataStatus === "published" && page.contentStatus === "published",
  )
  .map((page) => ({
    path: page.canonicalPath,
    lastModified: getLastModifiedForPath(page.canonicalPath),
  }))
  .filter(
    (page, index, pages) =>
      pages.findIndex((candidate) => candidate.path === page.path) === index,
  )
  .sort((left, right) => left.path.localeCompare(right.path));

export const getCanonicalUrl = (canonicalPath: string) =>
  new URL(canonicalPath, siteUrl).toString();

export const getRobotsForPath = (canonicalPath: string) => {
  const page = publicPages.find(
    (candidate) => candidate.canonicalPath === canonicalPath,
  );

  if (!page) {
    return defaultRobots;
  }

  return page.metadataStatus === "published" &&
    page.contentStatus === "published"
    ? defaultRobots
    : noIndexRobots;
};

export const getArticleMetadataForPath = (canonicalPath: string) =>
  resourceArticles.find(
    (article) => article.destinationPath === canonicalPath,
  ) ??
  (() => {
    const article = documentationArticles.find(
      (candidate) => getDocumentationPath(candidate) === canonicalPath,
    );
    return article
      ? { publishedDate: article.updatedAt, destinationPath: canonicalPath }
      : null;
  })();

export const llmsPages = [
  ...publicPages.filter((page) =>
    ["home", "product", "pricing", "resource", "support"].includes(
      page.pageType,
    ),
  ),
]
  .filter(
    (page) =>
      page.metadataStatus === "published" && page.contentStatus === "published",
  )
  .filter(
    (page, index, pages) =>
      pages.findIndex(
        (candidate) => candidate.canonicalPath === page.canonicalPath,
      ) === index,
  )
  .sort((left, right) => left.canonicalPath.localeCompare(right.canonicalPath));
