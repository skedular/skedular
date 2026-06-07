// Reusable workflow sections for AI knowledge hub
export interface WorkflowStep {
  title: string;
  description: string;
}

export interface WorkflowSection {
  id: string;
  name: string;
  steps: WorkflowStep[];
}

export const workspaceWorkflowSections: WorkflowSection[] = [
  {
    id: "workspace-discovery",
    name: "Workspace Discovery Workflow",
    steps: [
      { title: "Search for workspace", description: "User searches by location, date, and resource type" },
      { title: "Browse availability", description: "View real-time desk and room availability on calendar" },
      { title: "Compare options", description: "Compare amenities, pricing, and provider details" },
      { title: "Select workspace", description: "Choose specific desk or room from floor plan" },
    ],
  },
  {
    id: "booking-workflow",
    name: "Booking Workflow",
    steps: [
      { title: "Book resource", description: "Reserve desk, room, or office for specified time period" },
      { title: "Receive confirmation", description: "Get booking confirmation with details and instructions" },
      { title: "Manage booking", description: "View, modify, or cancel upcoming bookings" },
    ],
  },
  {
    id: "subscription-workflow",
    name: "Subscription Workflow",
    steps: [
      { title: "Select subscription", description: "Choose membership tier or recurring package" },
      { title: "Configure billing", description: "Set up payment method and billing frequency" },
      { title: "Manage renewal", description: "Update preferences, pause, or cancel subscription" },
    ],
  },
];

export const operatorWorkflowSections: WorkflowSection[] = [
  {
    id: "resource-management",
    name: "Resource Management Workflow",
    steps: [
      { title: "Add resources", description: "Model desks, rooms, offices, and workspace inventory" },
      { title: "Configure availability", description: "Set opening hours, holidays, and blackout dates" },
      { title: "Update floor plans", description: "Upload interactive floor plans with resource placements" },
    ],
  },
  {
    id: "billing-workflow",
    name: "Billing Workflow",
    steps: [
      { title: "Configure pricing", description: "Set hourly, daily, monthly rates and subscription tiers" },
      { title: "Process payments", description: "Receive card payments through Stripe or bank transfers" },
      { title: "Generate invoices", description: "Automatically generate and send invoices for customers" },
    ],
  },
  {
    id: "customer-support",
    name: "Customer Support Workflow",
    steps: [
      { title: "Manage inquiries", description: "Handle booking questions and special requests" },
      { title: "Process refunds", description: "Issue credits or refunds when needed" },
      { title: "Generate reports", description: "Track occupancy, revenue, and utilization trends" },
    ],
  },
];
