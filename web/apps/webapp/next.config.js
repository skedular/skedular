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
  images: {
    remotePatterns: [
      // TODO: 20250607 - Morteza: Add these below addresses to environment variables
      new URL("http://localhost:9000/**"),
      new URL("https://cloudflarecdnstaging.skedular.app/**"),
      new URL("https://cloudflarecdn.skedular.app/**"),
      new URL("https://awscdnstaging.skedular.app/**"),
      new URL("https://awscdn.skedular.app/**"),
    ],
  },
};

module.exports = nextConfig;
