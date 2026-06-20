export const aboutPage = {
  id: "company",
  path: "/about",
  title: "About Skedular | Desk booking & workspace management software",
  description:
    "Skedular is workspace management software built in Auckland. We help teams coordinate hybrid work and operators manage coworking spaces with practical desk booking, meeting room scheduling, and billing tools.",
  sourceUrl: "https://getskedular.com/company/",
  hero: {
    eyebrow: "Company",
    title: "About Skedular",
    summary:
      "We build software for the real world of shared workspaces. Founded in Auckland by people who spent years working with offices, desks, and bookings, our tools start with the actual work teams do every day.",
  },
  founders: [
    {
      name: "Leila Alavi",
      title: "Founder",
      insight:
        "I worked for several years managing shared office spaces. I saw how a simple mix-up in desk bookings could cascade into days of confusion for everyone involved. The tools we had either did too little or made things more complicated than they needed to be.",
    },
    {
      name: "Morteza Alizadeh",
      title: "Founder",
      insight:
        "I built software for teams who booked desks and rooms every day. I learned that if a system is not easier than the current way of doing things, people will keep using what they know even if it means spending hours each week trying to find a free meeting room or coordinating who works where.",
    },
  ],
  facts: [
    {
      label: "Founded",
      value: "2023",
      detail:
        "We incorporated after working on the first version of UnityHub. Feedback from early users helped us understand what actually matters for desk scheduling, especially when teams split time between offices and remote work.",
    },
    {
      label: "Home base",
      value: "Auckland",
      detail:
        "Built in New Zealand, designed for how people actually work today.",
    },
    {
      label: "Product family",
      value: "2 products",
      detail:
        "Skedular Teams helps companies manage workplace coordination. Skedular Spaces helps coworking operators handle memberships and billing.",
    },
  ],
  timeline: [
    {
      year: "Before 2023",
      title: "What we saw in real offices",
      body: "We spent time in coworking spaces, corporate offices, and small shared workspaces. The patterns were similar everywhere: desks booked but not used, meeting rooms double-booked, operators spending hours on email coordination instead of running their business.",
    },
    {
      year: "2023",
      title: "UnityHub launches",
      body: "We launched UnityHub with one goal: make desk and room booking so straightforward that people would actually use it. Early users in Auckland gave us feedback every week. Their input shaped the no-frills approach we still follow.",
    },
    {
      year: "2024",
      title: "UnityHub becomes Skedular",
      body: "We learned that teams needed more than just a booking system. They needed to see who was in the office, and operators needed practical ways to manage memberships and invoices. We refined our focus and rebranded to Skedular.",
    },
    {
      year: "Now",
      title: "Two products for different needs",
      body: "Skedular Teams helps companies coordinate hybrid work. Desk booking, meeting rooms, team presence are all part of what teams need. Skedular Spaces helps coworking operators manage memberships, billing, and workspace operations. Both products stay true to our original lesson: practical tools that people actually use.",
    },
  ],
  principles: [
    {
      title: "Practical over clever",
      body: "If someone needs training to book a desk we have failed. People should be able to use the system without thinking about it. It should feel like checking a calendar they already open every day.",
    },
    {
      title: "Operators need more than reservations",
      body: "Booking a room is one part of the job. The rest is managing memberships, generating invoices, handling cancellations, and answering questions all day. We build tools for that ongoing work not just the reservation moment.",
    },
    {
      title: "Simplicity through focus",
      body: "We have seen teams walk away from powerful platforms because they could not find the book button. We start with what people actually do then add only what they consistently need to get their work done.",
    },
  ],
  contactEmail: "support@getskedular.com",
} as const;
