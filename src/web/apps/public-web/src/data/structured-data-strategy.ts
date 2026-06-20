// Structured data strategy for AI and SEO

export interface SchemaType {
  type: string;
  description: string;
  适用场景: string[];
  aiBenefits: string[];
}

export const schemaTypes: SchemaType[] = [
  {
    type: "Organization",
    description: "Brand identity and contact information",
    适用场景: ["Homepage", "About page"],
    aiBenefits: [
      "AI assistants can extract brand name, logo, and contact details",
      "Search engines understand organization context",
    ],
  },
  {
    type: "SoftwareApplication",
    description: "Product capabilities and features",
    适用场景: ["Category pages", "Feature pages"],
    aiBenefits: [
      "AI systems can identify product category and functionality",
      "Knowledge graphs can link to related products",
    ],
  },
  {
    type: "FAQPage",
    description: "Structured frequently asked questions",
    适用场景: ["Product pages", "Category pages"],
    aiBenefits: [
      "Google AI Overviews can extract direct answers",
      "ChatGPT, Claude, Gemini can retrieve specific information",
    ],
  },
  {
    type: "Article",
    description: "Educational content and guides",
    适用场景: ["Resources pages"],
    aiBenefits: [
      "AI systems can understand educational intent",
      "Knowledge base entries for retrieval",
    ],
  },
  {
    type: "BreadcrumbList",
    description: "Site hierarchy navigation",
    适用场景: ["All pages"],
    aiBenefits: [
      "AI understands page location in site structure",
      "Better context for content relationships",
    ],
  },
];

// Structured data implementation recommendations
export interface SchemaRecommendation {
  pageType: string;
  schemaTypes: string[];
  priority: "high" | "medium" | "low";
  implementationNotes: string;
}

export const schemaRecommendations: SchemaRecommendation[] = [
  {
    pageType: "Homepage",
    schemaTypes: [
      "Organization",
      "SoftwareApplication",
      "FAQPage",
      "BreadcrumbList",
    ],
    priority: "high",
    implementationNotes: "Core schema for brand and product identification",
  },
  {
    pageType: "Category Pages",
    schemaTypes: ["SoftwareApplication", "Product", "FAQPage"],
    priority: "high",
    implementationNotes: "Important for category-specific AI retrieval",
  },
  {
    pageType: "Comparison Pages",
    schemaTypes: ["SoftwareApplication"],
    priority: "medium",
    implementationNotes: "Compare Skedular vs competitor functionality",
  },
  {
    pageType: "Resources/AI Knowledge Hub",
    schemaTypes: ["Article", "FAQPage"],
    priority: "high",
    implementationNotes: "Educational content benefits from Article schema",
  },
];

// Implementation roadmap
export interface SchemaImplementationPhase {
  phase: number;
  name: string;
  items: Array<{
    type: string;
    pages: string[];
    estimatedEffort: "low" | "medium" | "high";
  }>;
}

export const implementationPhases: SchemaImplementationPhase[] = [
  {
    phase: 1,
    name: "Phase 1 - Core Schema",
    items: [
      { type: "Organization", pages: ["/"], estimatedEffort: "low" },
      {
        type: "SoftwareApplication",
        pages: ["/", "/teams", "/spaces", "/host"],
        estimatedEffort: "medium",
      },
      { type: "FAQPage", pages: ["/", "/pricing"], estimatedEffort: "low" },
    ],
  },
  {
    phase: 2,
    name: "Phase 2 - Educational and Product Schema",
    items: [
      { type: "Article", pages: ["/resources/*"], estimatedEffort: "low" },
      { type: "Product", pages: ["/pricing/*"], estimatedEffort: "medium" },
    ],
  },
];
