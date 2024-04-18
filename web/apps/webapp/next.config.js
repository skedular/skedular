// @ts-check
const relay = require("./relay.config");

/** @type {import('next').NextConfig} */
const nextConfig = {
  compiler: {
    relay,
  },
  async rewrites() {
    return [
      {
        source: "/api/gateway/:path*",
        destination: `${process.env.GATEWAY_ENDPOINT}/gateway/api/:path*`,
      },
      {
        source: "/api/customer/:path*",
        destination: `${process.env.CUSTOMER_ENDPOINT}/customer/api/:path*`,
      },
      {
        source: "/api/location/:path*",
        destination: `${process.env.LOCATION_ENDPOINT}/location/api/:path*`,
      },
      {
        source: "/api/notification/:path*",
        destination: `${process.env.NOTIFICATION_ENDPOINT}/notification/api/:path*`,
      },
      {
        source: "/api/organization/:path*",
        destination: `${process.env.ORGANIZATION_ENDPOINT}/organization/api/:path*`,
      },
      {
        source: "/api/slack/:path*",
        destination: `${process.env.SLACK_ENDPOINT}/slack/api/:path*`,
      },
      {
        source: "/api/team/:path*",
        destination: `${process.env.TEAM_ENDPOINT}/team/api/:path*`,
      },
      {
        source: "/api/booking/:path*",
        destination: `${process.env.BOOKING_ENDPOINT}/booking/api/:path*`,
      },
      {
        source: "/api/payment/:path*",
        destination: `${process.env.PAYMENT_ENDPOINT}/payment/api/:path*`,
      },
      {
        source: "/api/billing/:path*",
        destination: `${process.env.BILLING_ENDPOINT}/billing/api/:path*`,
      },
      {
        source: "/api/msteams/:path*",
        destination: `${process.env.MSTEAMS_ENDPOINT}/msteams/api/:path*`,
      },
    ];
  },
};

module.exports = nextConfig;
