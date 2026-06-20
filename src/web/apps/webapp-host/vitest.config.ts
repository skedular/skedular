import { createRequire } from 'node:module';
import path from 'node:path';
import { defineConfig } from 'vitest/config';

const require = createRequire(import.meta.url);
const muiPackagePath = require.resolve('@mui/material/package.json');
const muiRequire = createRequire(path.join(path.dirname(muiPackagePath), 'internal/Transition.mjs'));
const transitionGroupContextPath = muiRequire.resolve('react-transition-group/cjs/TransitionGroupContext.js');
const reactFinalFormPath = require.resolve('react-final-form/dist/react-final-form.cjs.js');

export default defineConfig({
  plugins: [
    {
      name: 'resolve-react-transition-group-context',
      resolveId(id) {
        if (id === 'react-transition-group/TransitionGroupContext') {
          return transitionGroupContextPath;
        }

        return null;
      },
    },
  ],
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
    server: {
      deps: {
        inline: true,
      },
    },
  },
  resolve: {
    alias: [
      {
        find: '@',
        replacement: path.resolve(__dirname, './src'),
      },
      {
        find: /^@skedular\/ui\/(.*)$/,
        replacement: path.resolve(__dirname, '../../packages/ui/src/$1'),
      },
      {
        find: '@skedular/ui',
        replacement: path.resolve(__dirname, '../../packages/ui/src/index.ts'),
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
        find: 'react-transition-group/TransitionGroupContext',
        replacement: transitionGroupContextPath,
      },
      {
        find: 'react-final-form',
        replacement: reactFinalFormPath,
      },
    ],
  },
});
