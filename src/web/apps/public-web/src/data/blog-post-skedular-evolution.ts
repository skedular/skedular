import type { ResourceArticle } from "./content-types";

export const skedularEvolutionArticle: ResourceArticle = {
  id: "skedular-evolution-2026",
  slug: "skedular-evolution-2026-whats-new",
  sourceUrl: "https://getskedular.com/skedular-evolution-2026-whats-new/",
  destinationPath: "/blog/skedular-evolution-2026-whats-new",
  title: "How Skedular Evolved for Coworking and Workplace Booking",
  summary:
    "A founder-led update on how Skedular has evolved from workplace booking software into separate products for hybrid workplace management and coworking operations.",
  publishedDate: "2026-06-21",
  topicTags: ["product updates", "Skedular Spaces", "marketplace"],
  migrationDecision: "publish",
  contentStatus: "published",
  claimReviewStatus: "pending",
  body: [
    "Skedular started as workplace booking software for private teams: desk booking, room booking, parking, attendance visibility, and hybrid workplace management. As more workspace operators started using the product, it became clear that coworking operations needed a different kind of system.",
    "A private office is usually trying to coordinate employees. A coworking space is managing availability, customer bookings, memberships, billing, public discovery, brand presentation, cancellation rules, and day-to-day commercial operations. Trying to serve both use cases through one generic workflow was making the product harder to explain and harder to use.",
    "This update explains how Skedular has evolved, why Teams and Spaces are now separate products, what changed inside the platform, and where the roadmap is heading next. Much of this work came from customer conversations, support questions, and the operational problems we saw operators handling manually outside the product.",
  ],
  sections: [
    {
      heading: "Two Products for Different Needs",
      body: [
        "The most important product decision was to separate Skedular into two clear offerings. The reason was practical: private organizations and coworking operators were asking for different workflows, different terminology, and different levels of commercial control.",
        '<a href="/teams">Skedular Teams</a> is for private organizations managing internal offices, hybrid attendance, workplace booking, and team coordination. <a href="/spaces">Skedular Spaces</a> is for coworking operators, shared office providers, meeting room businesses, event venues, and flexible workspace networks that need coworking management software for public bookings and commercial operations.',
        "Keeping these products separate lets each experience stay focused. Teams customers do not need the complexity of memberships and public marketplace settings. Spaces operators do need those capabilities, and they need them to feel like part of the core product rather than an add-on.",
      ],
    },
    {
      heading: "Skedular Teams: Internal Office Coordination",
      body: [
        '<a href="/teams">Skedular Teams</a> remains focused on private companies and internal workplace coordination. It covers desk booking, room booking, parking booking, workplace attendance tracking, hybrid work coordination, floor plans, and workplace analytics.',
        "The everyday problem is simple: employees need to know where they can work, who else will be in, and whether the right spaces are available before they commute. Teams gives organizations a shared view of office availability and helps employees plan their week with less back-and-forth.",
        "Slack and Microsoft Teams integrations keep scheduling close to where people already communicate. Recurring bookings also reduce a small but common frustration: employees should not have to reserve the same desk or room one day at a time when their work pattern is predictable.",
      ],
      items: [
        "Desk, room, and parking booking on interactive floor plans.",
        "Slack and Microsoft Teams integrations for easy scheduling.",
        "Workplace attendance coordination and presence tracking.",
        "Workplace analytics to guide space management decisions.",
      ],
    },
    {
      heading: "Say Goodbye to Manual Daily Bookings",
      body: [
        "Recurring bookings became one of the clearest themes in conversations with Teams customers. Many employees do not use the office randomly. They come in on specific weekdays, reserve the same kind of desk, or need the same room for a repeating team session.",
        "Before this change, reserving a desk every Tuesday and Thursday meant creating each booking separately. That was manageable once or twice, but over time it created booking fatigue and made workplace booking feel more administrative than it needed to be.",
        "Teams now handles daily bookings, weekly bookings, selected weekdays, and reservations with or without end dates. A regular office pattern can be set up once, which makes desk booking and room booking easier for employees and more predictable for workplace administrators.",
      ],
    },
    {
      heading: "Skedular Spaces: Built for Flexible Workspace Operators",
      body: [
        "Most of the recent product work has gone into Skedular Spaces. Coworking and flexible workspace operators are not only assigning desks or meeting rooms. They are selling access to physical space, managing customers, handling invoicing, protecting margins, and keeping daily operations organized.",
        "Once we started mapping those workflows properly, the product requirements looked very different. Operators needed public workspace discovery, brand control, flexible product packaging, billing cycles, subscription management, coworking memberships, cancellation rules, resource availability, and a booking experience that works for both staff and customers.",
        '<a href="/spaces">Spaces</a> is designed to stop operators from stitching those operational pieces together across booking tools, spreadsheets, accounting systems, and website updates. Instead of managing bookings in one place, invoices in another, member records somewhere else, and public information manually, operators can manage the full commercial workflow from one place.',
      ],
    },
    {
      heading: "Complete UX Redesign",
      body: [
        "As the product expanded, the old interface started carrying too much weight. More features are only useful if operators and customers can understand them without extra training.",
        "The admin dashboard and member portal have been reworked around the jobs people are trying to complete: configuring locations, mapping resources, publishing products, reviewing bookings, managing invoices, and helping customers find the right space.",
        "Setup flows are clearer, and the customer booking process is more direct. Someone booking a desk from a phone needs a fast path to availability and payment. An operator reviewing monthly revenue on a desktop needs enough detail to understand what happened without digging through disconnected screens.",
      ],
    },
    {
      heading: "Marketplace Foundation and Discovery",
      body: [
        "Internal workplace booking is only one part of the broader workspace problem. Flexible workspace operators also need to be found by people looking for desks, rooms, day passes, memberships, and event spaces.",
        "The marketplace foundation allows operators to publish spaces, locations, and products so customers can discover and book workspace directly. This matters because workspace discovery is often fragmented: customers search in one place, ask questions in another, and complete booking through a manual process.",
        "Operators can use their own custom domain, the Skedular workspace marketplace, or both at the same time. That gives them a path to marketplace discovery without forcing them to give up their own brand or customer relationship.",
      ],
    },
    {
      heading: "Custom Domains and Brand Control",
      body: [
        "Brand control came up repeatedly while building Spaces. Operators want software to make their business easier to run, but they do not want every customer touchpoint to feel like it belongs to the software provider.",
        "Workspace operators can use Skedular hosted URLs or fully custom domains. They can upload their own branding, use their own images, add custom content, and configure details for locations, resources, and products.",
        "The customer-facing booking experience can be shaped around the operator's brand while the administration dashboard stays focused on management tasks. That separation is important for coworking operators who rely on trust, presentation, and repeat customer relationships.",
      ],
    },
    {
      heading: "The Product Model",
      body: [
        "One of the biggest lessons from building for coworking operations is that the physical resource and the commercial product are not the same thing.",
        "In a private office, a desk is usually just a desk. In a coworking space, that same desk might be sold as an hourly booking, a day pass, part of a monthly membership, or a recurring subscription. A meeting room might be bookable by the hour, included in a membership allowance, or reserved as part of an event package.",
        "Spaces separates resources from products for this reason. A resource is the physical asset, such as a desk, room, office, parking space, event area, or equipment. A product is how access to that resource is packaged and sold. This model gives operators more flexibility without duplicating the same physical space in multiple systems.",
      ],
    },
    {
      heading: "New Booking Models",
      body: [
        "Spaces now has three booking models because not every reservation behaves the same way. One-time bookings cover standard single reservations. Recurring bookings handle regular access over time. Event bookings allow an operator to reserve a whole space and the related resources together.",
        "A two-hour meeting room booking, a recurring weekday desk, and an evening event reservation all behave differently in practice. Treating all of those as the same booking type would make the system harder to reason about for operators, customers, and staff.",
        "Operators can define desks, meeting rooms, private offices, event spaces, parking, equipment, and other resource types across as many locations and resources as they need. The goal is to reflect how the space is actually sold, not force every operator into one fixed model.",
      ],
    },
    {
      heading: "Resource Availability and Workspace Navigation",
      body: [
        "Busy spaces need clear availability. Without it, staff spend time answering manual questions, customers hesitate before booking, and operators lose confidence that the calendar reflects reality.",
        "Operators can now look at one view and understand what has happened, what is active, and what is already committed. That helps staff answer booking questions with more confidence and helps customers choose from what is actually available.",
        "Floor plans now play a larger role in workspace navigation. Visual resource mapping, customer-facing floor plans, and a clearer booking flow make it easier for someone to choose a desk near a window, find a room close to the entrance, or understand the layout before they arrive.",
      ],
    },
    {
      heading: "Amenities and Private Metadata",
      body: [
        "Amenities matter during workspace discovery because customers are often comparing practical details, not just location and price. Internet, coffee, kitchen access, parking, printing, and meeting facilities can all influence whether a booking is suitable.",
        "Operators can publish those amenities during discovery and booking so customers know what to expect before they commit. That reduces avoidable questions and helps the customer choose the right space.",
        "Private metadata handles the information that should not be public. Wi-Fi passwords, building access instructions, check-in notes, and internal staff details can be stored against the relevant location, resource, or booking. Sensitive information stays hidden until it needs to be shared with someone who has a confirmed booking.",
      ],
    },
    {
      heading: "A Complete Billing and Commercial Engine",
      body: [
        "Billing became one of the largest areas of investment because coworking operations are commercial from the first booking. Operators were often handling payments, invoice numbering, recurring member charges, and accounting updates outside the booking system.",
        "Skedular now includes native invoice generation, allowing operators to create customer and commercial invoices directly with proper numbering. For smaller or newer operators, that means they can start without immediately depending on a separate accounting workflow.",
        "For operators who already use Xero, Spaces connects booking and billing data into their existing accounting process. Invoices can be generated in Skedular or in Xero, depending on how the operator wants to run finance.",
        "Many operators were manually managing recurring member payments outside the platform. Recurring invoicing now allows memberships, long-term customers, and subscriptions to be billed automatically. Operators can configure weekly, fortnightly, or monthly billing periods, choose pay-upfront or pay-in-arrears models, and accept bank transfers alongside credit cards.",
      ],
    },
    {
      heading: "Subscription Management and Cancellations",
      body: [
        "Subscription management is central to coworking memberships. A coworking operator may sell casual bookings, but a healthy business often depends on recurring members, fixed-term plans, and clear rules around renewal and cancellation.",
        "Spaces now handles auto-renewing subscriptions, fixed-term subscriptions, and membership subscriptions. Operators can review active and historical subscriptions, related invoices, and payment status without piecing the story together from separate systems.",
        "Customers can view their subscriptions, review invoices, and download invoices from their dashboard. Cancellation policies are also more flexible, with operator-defined rules, multiple cancellation tiers, and different policies for different products. That matters because a day pass, monthly membership, and long-term office agreement should not always follow the same rules.",
      ],
    },
    {
      heading: "Pricing Redesigned for Coworking",
      body: [
        'Pricing needed to match how coworking operators actually grow: setup first, then booking volume. Operators can evaluate Spaces through a 14-day trial under the existing Free plan limit of 100 booking instances per month, then choose a paid plan that scales with usage. <a href="/pricing/spaces">Spaces pricing</a> is built around that path.',
        "Charging primarily around booking activity fits the way coworking operators think about value. A space with many active bookings is getting more operational benefit from the platform than one still setting up locations, products, or resources.",
        "There are no artificial limits on the number of locations, resources, or products an operator can configure. That gives new spaces room to model their business properly before volume grows, while larger operators can expand without running into arbitrary setup restrictions.",
      ],
    },
    {
      heading: "Public Website Redesign",
      body: [
        "The public website had to change because the product had changed. When Skedular was mostly focused on internal workplace booking, the message was easier to explain. Once Teams and Spaces became separate products, one broad explanation started creating confusion.",
        "A private company looking for desk booking, room booking, and hybrid workplace management has a different question than a coworking operator looking for memberships, subscriptions, billing, and public booking workflows. Trying to answer both on the same page made it harder for visitors to understand which product was right for them.",
        "The new site separates those paths more clearly. Teams is explained around internal workplace coordination, while Spaces is explained around coworking operations and commercial workspace management. That clarity should help customers make better decisions faster. It also gives search engines and AI systems a cleaner understanding of what each product does, but the main reason for the redesign was customer clarity.",
      ],
    },
    {
      heading: "Driven by Customer Feedback",
      body: [
        "Many of these changes came directly from customer conversations and early operator feedback. The same themes kept appearing: recurring bookings were too manual, billing lived outside the booking workflow, memberships needed clearer lifecycle management, and operators wanted more control over how their spaces appeared to customers.",
        "Those conversations pushed some work higher on the roadmap than we originally expected. It is one thing to design a booking form. It is another to understand what happens after the booking: invoice generation, access instructions, customer communication, cancellation rules, recurring payments, and operational reporting.",
        "The product is better when it reflects real workflows rather than assumptions. We will keep listening closely as operators use Spaces in more scenarios and as Teams customers continue refining how hybrid work functions inside their organizations.",
      ],
    },
    {
      heading: "The Road Ahead: Connecting Teams and Spaces",
      body: [
        "The next step is to connect Skedular Teams and Skedular Spaces in a way that makes sense for both sides. Teams and Spaces are separate products because their day-to-day workflows are different, but the long-term vision is for them to work together where the overlap is useful.",
        "A private organization using Skedular Teams should eventually be able to book coworking spaces through Skedular Spaces on behalf of its employees. That would give hybrid teams access to external workspaces without leaving the internal workplace management flow they already use.",
        "For operators, this creates a clearer path for their spaces to be discovered by organizations that need flexible capacity. For Teams customers, it extends workplace booking beyond the company office and into a broader workspace marketplace.",
        "These changes matter because workplace management and coworking operations are becoming more connected. Private teams need flexibility beyond their own offices, and operators need better tools to publish, sell, manage, and bill for that flexibility. Teams will stay focused on internal coordination. Spaces will stay focused on commercial workspace operations. The bridge between them has to preserve those separate workflows rather than merging everything back into one confusing experience.",
        "Thank you to everyone who has shared feedback, tested early workflows, and helped us understand the practical details behind these product decisions.",
      ],
    },
    {
      heading: "Author",
      body: [
        "Written by Morteza Alizadeh, Co-founder of Skedular.",
        "This article reflects ongoing product development, customer feedback, and future roadmap planning for Skedular Teams and Skedular Spaces.",
      ],
    },
  ],
};
