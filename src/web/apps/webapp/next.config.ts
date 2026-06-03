import withBundleAnalyzer from "@next/bundle-analyzer";
import type { NextConfig } from "next";
import relayConfig from "./relay.config";

const isVercel = process.env.VERCEL === "1";
const withAnalyzer = withBundleAnalyzer({ enabled: process.env.ANALYZE === "true" });

const nextConfig: NextConfig = {
  allowedDevOrigins: ["mapp.skedular.app"],
  transpilePackages: ["@skedular/ui", "@skedular/shared"],
  compiler: {
    relay: {
      ...relayConfig,
      language: relayConfig.language as "typescript" | "javascript" | "flow",
    },
  },
  typescript: {
    ignoreBuildErrors: isVercel,
  },
  images: {
    remotePatterns: [
      // TODO: 20250607 - Morteza: Add these below addresses to environment variables
      { protocol: 'http', hostname: 'localhost', port: '9000', pathname: '/**' },
      { protocol: 'https', hostname: 'cloudflarecdnstaging.skedular.app', pathname: '/**' },
      { protocol: 'https', hostname: 'cloudflarecdn.skedular.app', pathname: '/**' },
      { protocol: 'https', hostname: 'awscdnstaging.skedular.app', pathname: '/**' },
      { protocol: 'https', hostname: 'awscdn.skedular.app', pathname: '/**' },
    ],
  },
};

export default withAnalyzer(nextConfig);
