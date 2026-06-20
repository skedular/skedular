import { resourceArticles } from "./current-public-content";

export const redirects = Object.fromEntries(
  resourceArticles.map((article) => [
    new URL(article.sourceUrl).pathname,
    article.destinationPath,
  ]),
);
