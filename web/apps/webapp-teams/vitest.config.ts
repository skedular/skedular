import path from 'node:path';
import { defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
  },
  resolve: {
    alias: [
      {
        find: '@',
        replacement: path.resolve(__dirname, './src'),
      },
      {
        find: '@skedular/ui',
        replacement: path.resolve(__dirname, '../../packages/ui/src/index.ts'),
      },
      {
        find: /^@skedular\/ui\/(.*)$/,
        replacement: path.resolve(__dirname, '../../packages/ui/src/$1'),
      },
      {
        find: '@skedular/shared',
        replacement: path.resolve(__dirname, '../../packages/shared/src/index.ts'),
      },
      {
        find: /^@skedular\/shared\/(.*)$/,
        replacement: path.resolve(__dirname, '../../packages/shared/src/$1'),
      },
      {
        find: /^@webapp\/(.*)$/,
        replacement: path.resolve(__dirname, '../webapp/src/$1'),
      },
    ],
  },
});
