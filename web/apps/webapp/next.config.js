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
    ];
  },
};

module.exports = nextConfig;
