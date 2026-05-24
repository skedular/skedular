import nextVitals from "eslint-config-next/core-web-vitals";
import nextTs from "eslint-config-next/typescript";
import { defineConfig, globalIgnores } from "eslint/config";

const eslintConfig = defineConfig([
  ...nextVitals,
  ...nextTs,
  // Override default ignores of eslint-config-next.
  globalIgnores([
    // Default ignores of eslint-config-next:
    ".next/**",
    "node_modules/**",
    "out/**",
    "dist/**",
    "build/**",
    "coverage/**",
    "**/*.min.js",
    "next-env.d.ts",
    "**/__generated__/",
    "**/src/clients/openapi/nominatim/**",
    "**/src/clients/openapi/skedular/**",
  ]),
]);

export default eslintConfig;
