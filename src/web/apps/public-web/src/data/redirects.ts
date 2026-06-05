import { resourceArticles, supportArticles } from "./current-public-content";

export const redirects = Object.fromEntries(
  [...resourceArticles, ...supportArticles].map((article) => [new URL(article.sourceUrl).pathname, article.destinationPath]),
);
