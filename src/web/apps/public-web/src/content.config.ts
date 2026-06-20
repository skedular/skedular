import { defineCollection, z } from "astro:content";
import { glob } from "astro/loaders";

const docs = defineCollection({
  loader: glob({
    base: "./src/content/docs",
    pattern: "**/*.md",
    generateId: ({ data }) => data.id,
  }),
  schema: z.object({
    id: z.string().min(1),
    title: z.string().min(1),
    description: z.string().min(1),
    product: z.enum(["teams", "spaces", "host", "shared"]),
    category: z.string().min(1),
    slug: z.string().regex(/^[a-z0-9]+(?:-[a-z0-9]+)*$/),
    articleKind: z.enum([
      "landing",
      "guide",
      "reference",
      "faq",
      "best-practice",
      "placeholder",
    ]),
    publicationState: z.enum([
      "published",
      "draft",
      "future",
      "content-gap",
      "withdrawn",
    ]),
    evidenceRefs: z.array(z.string()).min(1),
    terminologyRefs: z.array(z.string()).min(1),
    relatedArticleIds: z.array(z.string()).default([]),
    updatedAt: z.coerce.date(),
  }),
});

export const collections = { docs };
