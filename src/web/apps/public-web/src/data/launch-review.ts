export const launchReviewChecklist = {
  draftCoverage: { status: "pending", evidence: "draft-coverage.ts", requiredPassRate: 1 },
  pricingApproval: { status: "pending", evidence: "claim-review.ts", requiredPassRate: 1 },
  claimsReview: { status: "pending", evidence: "claim-review.ts", requiredPassRate: 1 },
  competitorReview: { status: "pending", evidence: "claim-review.ts", requiredPassRate: 1 },
  accessibilityReview: { status: "pending", evidence: "public-site-content.test.ts and manual mobile review", requiredPassRate: 1 },
  seoReview: { status: "pending", evidence: "public page metadata tests", requiredPassRate: 1 },
  humanQualityTone: { status: "pending", evidence: "manual review protocol", requiredPassRate: 0.9 },
};

export const manualReviewProtocol = {
  participantCount: 10,
  successThreshold: 0.9,
  prompts: [
    "After 10 seconds on the home page, what does Skedular help you do?",
    "After viewing navigation and Teams, who is Teams for?",
    "After viewing navigation and Spaces, who is Spaces for?",
    "Does the copy sound clear, friendly, professional, and natural?",
  ],
  evidenceFields: ["participantId", "scenario", "response", "passed", "notes", "reviewedAt"],
  resultStatus: "not-yet-run",
};

export const manualReviewFindings = [
  {
    id: "human-quality-copy",
    status: "pending",
    notes: "Review all generated public pages in context and rewrite anything that sounds generic, stiff, repetitive, or AI-generated.",
  },
  {
    id: "accessibility-mobile",
    status: "pending",
    notes: "Check keyboard navigation, mobile layout, focus order, heading structure, link text, contrast, and critical axe results.",
  },
];
