// @ts-check
const relay = require("./relay.config");

/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: {
    reactCompiler: true,
  },
  compiler: {
    relay,
  },
  async rewrites() {
    return [
      {
        source: "/api/gateway/:path*",
        destination: new URL(":path*", process.env.GATEWAY_ENDPOINT).href,
      },
      {
        source: "/api/customer/:path*",
        destination: new URL(":path*", process.env.CUSTOMER_ENDPOINT).href,
      },
      {
        source: "/api/location/:path*",
        destination: new URL(":path*", process.env.LOCATION_ENDPOINT).href,
      },
      {
        source: "/api/notification/:path*",
        destination: new URL(":path*", process.env.NOTIFICATION_ENDPOINT).href,
      },
      {
        source: "/api/organization/:path*",
        destination: new URL(":path*", process.env.ORGANIZATION_ENDPOINT).href,
      },
      {
        source: "/api/slack/:path*",
        destination: new URL(":path*", process.env.SLACK_ENDPOINT).href,
      },
      {
        source: "/api/team/:path*",
        destination: new URL(":path*", process.env.TEAM_ENDPOINT).href,
      },
      {
        source: "/api/booking/:path*",
        destination: new URL(":path*", process.env.BOOKING_ENDPOINT).href,
      },
      {
        source: "/api/payment/:path*",
        destination: new URL(":path*", process.env.PAYMENT_ENDPOINT).href,
      },
      {
        source: "/api/billing/:path*",
        destination: new URL(":path*", process.env.BILLING_ENDPOINT).href,
      },
      {
        source: "/api/marketplace/:path*",
        destination: new URL(":path*", process.env.MARKETPLACE_ENDPOINT).href,
      },
    ];
  },
};

module.exports = nextConfig;
