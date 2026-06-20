// Prioritized 30/60/90 day roadmap for SEO and AI optimization

export interface RoadmapItem {
  id: string;
  title: string;
  description: string;
  category:
    | "category"
    | "industry"
    | "comparison"
    | "ai-knowledge"
    | "structured-data"
    | "eet";
  effort: "low" | "medium" | "high";
  seoimpact: "high" | "medium" | "low";
  aiimpact: "high" | "medium" | "low";
}

export interface Roadmap {
  days30: RoadmapItem[];
  days60: RoadmapItem[];
  days90: RoadmapItem[];
}

// High priority items for 30 day roadmap
const days30Items: RoadmapItem[] = [
  {
    id: "1",
    title:
      "Add missing category pages for workspace management software, desk booking software, coworking management software",
    description:
      "Create category pages targeting high-volume search terms with proper schema markup",
    category: "category",
    effort: "medium",
    seoimpact: "high",
    aiimpact: "high",
  },
  {
    id: "2",
    title: "Expand FAQ sections on all product pages with long-tail queries",
    description:
      "Add 5-8 additional FAQ entries per page targeting specific search intents",
    category: "structured-data",
    effort: "low",
    seoimpact: "high",
    aiimpact: "high",
  },
  {
    id: "3",
    title: "Create comparison pages for top competitors (Skedda, Robin, Envoy)",
    description:
      "Build comparison pages targeting competitor search traffic with neutral language",
    category: "comparison",
    effort: "medium",
    seoimpact: "high",
    aiimpact: "medium",
  },
];

// Medium priority items for 60 day roadmap
const days60Items: RoadmapItem[] = [
  {
    id: "4",
    title:
      "Create industry-specific pages (coworking spaces, shared offices, enterprise)",
    description:
      "Build tailored content for each industry vertical with specific use cases",
    category: "industry",
    effort: "medium",
    seoimpact: "high",
    aiimpact: "high",
  },
  {
    id: "5",
    title: "Expand AI Knowledge Hub with educational articles",
    description:
      "Create definition and workflow content for AI retrieval optimization",
    category: "ai-knowledge",
    effort: "medium",
    seoimpact: "medium",
    aiimpact: "high",
  },
  {
    id: "6",
    title: "Add structured data to all new pages (Article, Product schemas)",
    description: "Implement schema markup on all category and resource pages",
    category: "structured-data",
    effort: "low",
    seoimpact: "medium",
    aiimpact: "high",
  },
];

// Longer-term items for 90 day roadmap
const days90Items: RoadmapItem[] = [
  {
    id: "7",
    title: "Create integration documentation pages",
    description:
      "Build detailed integration guides for Slack, Teams, Xero, Stripe",
    category: "eet",
    effort: "medium",
    seoimpact: "high",
    aiimpact: "high",
  },
  {
    id: "8",
    title:
      "Add comparison pages for remaining competitors (Archie, OfficeSpace, Condeco)",
    description:
      "Complete competitor coverage with neutral, factual comparisons",
    category: "comparison",
    effort: "medium",
    seoimpact: "medium",
    aiimpact: "medium",
  },
  {
    id: "9",
    title: "Implement workflow documentation sections across site",
    description:
      "Add reusable workflow patterns for AI understanding of business processes",
    category: "ai-knowledge",
    effort: "high",
    seoimpact: "medium",
    aiimpact: "high",
  },
];

export const roadmap: Roadmap = {
  days30: days30Items,
  days60: days60Items,
  days90: days90Items,
};
