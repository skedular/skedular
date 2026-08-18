import type { ResourceArticle } from "./content-types";

export const creditBasedBookingEntitlementsArticle: ResourceArticle = {
  id: "credit-based-booking-entitlements",
  slug: "credit-based-booking-entitlements",
  sourceUrl: "https://getskedular.com/blog/credit-based-booking-entitlements",
  destinationPath: "/blog/credit-based-booking-entitlements",
  title: "Booking Credits: Buy Now, Schedule Later",
  summary:
    "Learn how Skedular lets customers buy prepaid credits, schedule workspace use later, and manage payment, eligibility, cancellations, expiry, and refunds.",
  seoTitle: "Booking Credits: Buy Now, Schedule Later | Skedular",
  seoDescription:
    "Learn how Skedular booking credits work, including payment, eligibility, scheduling later, cancellations, expiry, refunds, and renewals.",
  publishedDate: "2026-08-17",
  lastModified: "2026-08-17",
  author: {
    name: "Skedular product and engineering team",
    role: "Product and engineering",
    type: "Organization",
    description:
      "The team building Skedular's booking, payment, entitlement, and workspace operations platform.",
  },
  topicTags: ["Credit entitlements", "Bookings", "Product notes"],
  migrationDecision: "publish",
  contentStatus: "published",
  claimReviewStatus: "approved",
  // TODO: Add a credit-flow diagram when a suitable buy-to-book visual is available.
  body: [
    "Booking credits give customers a more flexible way to plan workspace use: buy prepaid booking credits now, choose a date later, and spend them only on bookings that meet the offer's rules. The purchase does not reserve a room or resource before a date is chosen.",
    "In brief: 1) customers buy a set number of credits; 2) they return when they know the date they need; 3) a credit is used only when the booking is eligible and successfully created. That makes flexible workspace bookings easier to plan without asking customers to commit to every date upfront.",
    "This product note explains the customer journey, the safeguards behind it, and how the public website describes the feature for Skedular Spaces and Skedular Host.",
  ],
  sections: [
    {
      heading: "The important change: purchase now, choose the date later",
      body: [
        "A credit purchase is a standalone order. Starting it does not create a booking, schedule, resource allocation, reservation, or booking quota usage. The customer is buying future use, not holding a particular slot.",
        "After a Stripe payment succeeds or an authorized operator confirms a bank transfer, the credits included in the offer are added to the customer's balance. If payment is pending, rejected, or expired, no usable credits are added.",
      ],
      items: [
        "Configure a pricing option to offer booking credits.",
        "Set the number of included credits and how long they remain valid.",
        "Choose optional weekday, product, and resource restrictions.",
        "Choose how cancellation and unused-credit refunds are handled.",
        "Grant credits only after the purchase payment is confirmed.",
      ],
    },
    {
      heading: "How a customer uses a credit",
      body: [
        "When the customer later opens the booking flow, the system checks their booking credits before creating the booking. The date must fall within the period set for the offer, the weekday must be allowed, and the requested product and resource must be covered. Normal opening-hour, availability, and conflict checks still apply.",
        "If the customer has more than one eligible booking-credit balance, the one that expires soonest is used. The booking and the credit use are committed together, so a failed booking does not spend a credit and two competing requests cannot spend the same final credit.",
        "A successful booking stays linked to the purchase and its credit history. Customers and authorized operators can therefore see how the balance changed and which booking used it.",
      ],
    },
    {
      heading: "A practical example",
      body: [
        "Suppose a customer buys four booking credits that are valid for 30 days. They pay by card today, so the four credits become available after the payment is confirmed. No room, time, or resource is reserved at that point.",
        "A week later, the customer knows they need a meeting room on Tuesday afternoon. They choose the date and time in the booking flow. If Tuesday is an allowed day, the room is available, and the offer covers that resource, one credit pays for the booking and three remain.",
        "If the customer later cancels, the booking rules decide whether that credit returns to the balance or is forfeited. If the 30-day period ends, any unused balance follows the refund or expiry choice saved with the original offer.",
      ],
    },
    {
      heading: "Cancellation and changes remain understandable",
      body: [
        "A customer can change the date, time, or resource of a credit-funded booking, subject to the same eligibility and availability rules. The system checks the new booking before moving the reservation and keeps one consumed credit attached to it.",
        "Cancelling a booking is separate from cancelling unused booking credits. Depending on the offer's refund rules and how close the booking is to its start time, cancellation can restore the credit or forfeit it. The customer-facing experience explains which result applies.",
        "Cancelling booking credits stops future use. Existing bookings remain governed by the booking and cancellation rules that apply to them; cancelling unused booking credits does not silently erase booking history.",
      ],
    },
    {
      heading: "Expiry, unused credits, and refunds",
      body: [
        "At the configured end date, unused credits become unusable. The original offer decides whether they are forfeited or enter the normal refund process. Used credits are never treated as unused.",
        "When a refund is allowed, the amount is prorated across the unused balance using the refund rules saved with the original purchase. The credit history records what expired and links it to the refund, while the refund record tracks the financial outcome.",
        "The payment method affects whether money has actually been returned. Stripe refunds can follow the automatic provider path when supported. Bank-transfer refunds require manual settlement. Xero records the accounting adjustment as a credit note, while payment return remains a separate step until the integration supports it.",
        "If the original payment was never confirmed, the balance closes or expires without creating a refund. That avoids treating an unconfirmed payment as money that can be returned.",
      ],
    },
    {
      heading: "Auto-renewal uses the current pricing",
      body: [
        "Booking credits can renew when their offer enables auto-renewal. Renewal follows the existing payment and workflow behavior, but it creates a new cycle only after the new payment is confirmed.",
        "At renewal time, the system checks the current active credit-based offer. The original purchase keeps its quantity, restrictions, price, and policy; a future cycle uses the current eligible offer. If payment fails or no eligible offer remains, the current cycle ends and no unconfirmed credits are granted.",
      ],
    },
    {
      heading: "How this works on the public website",
      body: [
        "The public website explains booking credits as an additional option alongside reservation and recurring pricing. Reservation pricing chooses the date and resource during purchase; prepaid credits let the customer schedule later and create the booking when they are ready.",
        'The <a href="/spaces/bookings/credit-entitlements">Spaces guide</a> explains how workspace operators configure quantity, validity, allowed days, payment confirmation, expiry, refunds, renewal, and actions on behalf of customers.',
        'The <a href="/host/bookings/credit-entitlements">Host guide</a> presents the same rules for independent hosts. Both guides explain that credits are granted after confirmed payment, eligibility is checked when a credit is used, and existing reservation and recurring bookings remain unchanged.',
        "The blog gives the product context; the documentation gives operators the steps. Customers get flexibility without having to understand the software's internal data model.",
      ],
    },
    {
      heading: "What customers and operators can see",
      body: [
        "Customers can review active booking credits, their remaining balance, validity, restrictions, credit history, linked bookings, and any refund or payment-return status. Spaces and Host owners or administrators can perform eligible booking actions on a customer's behalf, with the acting operator preserved in the audit trail.",
        "Booking credits are most useful when customers know they will need workspace but are not ready to choose a date. They can buy now and schedule later, while operators retain control over availability, payment, cancellation, and refund rules.",
      ],
    },
  ],
  faq: [
    {
      question: "Does buying credits create a booking?",
      answer:
        "No. Buying prepaid booking credits creates a standalone purchase. After payment is confirmed, the customer receives a balance that can be used later. A booking is created only when the customer spends a credit on an eligible date and resource.",
    },
    {
      question: "When are credits granted?",
      answer:
        "Credits are granted only after payment is confirmed. Stripe card payments follow the automatic checkout path. Bank-transfer purchases remain pending until an authorized operator confirms the payment, so an unpaid order cannot be used to make a booking.",
    },
    {
      question: "Can unused credits be refunded?",
      answer:
        "Sometimes. The original offer must allow unused-balance refunds, and the purchase payment must be confirmed. When those conditions are met, only unused credits are included and the refund follows the normal payment-method rules: Stripe may process it automatically, bank transfers require manual settlement, and Xero records the accounting credit note separately.",
    },
    {
      question: "Do existing recurring and reservation bookings change?",
      answer:
        "No. Booking credits are an additional way to pay for future use. Existing reservation-based bookings still choose a date and resource during purchase, and recurring bookings continue to follow their existing schedule and payment behavior.",
    },
  ],
  finalTakeaway:
    "Booking credits give customers flexibility when they are not ready to choose a date, while operators retain control over availability, payment, cancellation, and refund rules.",
  cta: {
    heading: "Explore credit-based bookings",
    body: "See how prepaid booking credits can support flexible workspace bookings in Skedular Spaces and Skedular Host.",
    links: [
      {
        label: "Read the Spaces guide",
        href: "/spaces/bookings/credit-entitlements",
        primary: true,
      },
      {
        label: "Read the Host guide",
        href: "/host/bookings/credit-entitlements",
      },
      { label: "Explore Skedular Spaces", href: "/spaces" },
    ],
  },
};
