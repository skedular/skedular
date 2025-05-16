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
};

module.exports = nextConfig;
