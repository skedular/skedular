import type { NextConfig } from "next";
import relayConfig from "./relay.config";

const nextConfig: NextConfig = {
  experimental: {
    reactCompiler: true,
  },
  compiler: {
    relay: {
      ...relayConfig,
      language: relayConfig.language as "typescript" | "javascript" | "flow",
    },
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
