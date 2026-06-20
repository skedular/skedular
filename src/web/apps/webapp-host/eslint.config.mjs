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
  // Floor plan canvases require pixel-exact overlay positioning. next/image injects
  // responsive CSS (max-width: 100%; height: auto) that breaks coordinate alignment.
  // Plain <img style={{ width: '100%', height: '100%' }}> inside an aspect-ratio
  // container is the only safe approach here.
  {
    files: ['src/components/floorPlan/**/*.tsx'],
    rules: {
      '@next/next/no-img-element': 'off',
    },
  },
]);

export default eslintConfig;
