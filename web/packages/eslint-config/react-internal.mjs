import pluginJs from "@eslint/js";
import eslintConfigPrettier from "eslint-config-prettier";
import pluginReactHook from "eslint-plugin-react-hooks";
import pluginTurbo from "eslint-plugin-turbo";
import globals from "globals";
import tseslint from "typescript-eslint";

export default [
  {
    files: ["**/*.{js,mjs,cjs,ts}"],
  },
  {
    languageOptions: {
      globals: globals.browser,
    },
  },
  {
    plugins: {
      turbo: pluginTurbo,
      reactHook: pluginReactHook,
    },
  },
  {
    ignores: ["node_modules/", "dist/"],
  },
  pluginJs.configs.recommended,
  eslintConfigPrettier,
  ...tseslint.configs.recommended,
];
