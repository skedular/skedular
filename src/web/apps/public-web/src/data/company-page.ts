export const companyPage = {
  id: "company",
  path: "/about",
  title: "About Skedular | Workspace software from Auckland",
  description:
    "Learn about Skedular, the Auckland-built workspace platform helping teams and workspace operators make flexible work easier to manage.",
  sourceUrl: "https://getskedular.com/company/",
  hero: {
    eyebrow: "Company",
    title: "About Skedular",
    summary:
      "Skedular is built in Auckland, New Zealand for teams and workspace operators who need practical tools for flexible work, bookings, and day-to-day workspace operations.",
  },
  facts: [
    { label: "Founded", value: "2023", detail: "Incorporated after the first UnityHub product work began in 2022." },
    { label: "Home base", value: "Auckland", detail: "Built from New Zealand with a product focus on modern hybrid work." },
    { label: "Product family", value: "2 products", detail: "Skedular Teams and Skedular Spaces." },
  ],
  timeline: [
    {
      year: "2022",
      title: "The idea takes shape",
      body: "Leila Alavi and Morteza Alizadeh began shaping a platform for the new workplace reality: offices still mattered, but teams needed more flexibility and clearer booking tools.",
    },
    {
      year: "2023",
      title: "UnityHub launches",
      body: "The company was incorporated and the first product, UnityHub, reached early users. Their feedback helped improve the booking experience, performance, and reliability.",
    },
    {
      year: "2024",
      title: "UnityHub becomes Skedular",
      body: "The product evolved into Skedular with a stronger brand, a refreshed interface, and a sharper focus on flexibility, simplicity, and collaboration.",
    },
    {
      year: "Now",
      title: "A broader workspace platform",
      body: "Skedular is moving beyond private team scheduling into a clearer product family for workplace teams and workspace operators.",
    },
  ],
  principles: [
    "Make workspace booking simple enough for everyday use.",
    "Give teams practical tools without adding unnecessary administration.",
    "Help operators publish, sell, and manage workspace with less manual work.",
  ],
  contactEmail: "support@getskedular.com",
} as const;
