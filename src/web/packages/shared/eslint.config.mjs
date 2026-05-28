import nextTs from 'eslint-config-next/typescript';
import { defineConfig, globalIgnores } from 'eslint/config';

const eslintConfig = defineConfig([
  ...nextTs,
  globalIgnores(['dist/**', 'build/**']),
]);

export default eslintConfig;
