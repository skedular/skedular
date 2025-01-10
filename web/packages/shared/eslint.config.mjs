import skedular from "@repo/eslint-config/react-internal.mjs";

export default [
  {
    ignores: ["src/clients/openapi/skedular/"],
  },
  ...skedular,
];
