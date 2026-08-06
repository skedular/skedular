import path from 'node:path';
import { createRequire } from 'node:module';
import { defineConfig } from 'vitest/config';

const require = createRequire(import.meta.url);
const muiPackagePath = require.resolve('@mui/material/package.json');
const muiRequire = createRequire(path.join(path.dirname(muiPackagePath), 'internal/Transition.mjs'));
const transitionGroupContextPath = muiRequire.resolve('react-transition-group/cjs/TransitionGroupContext.js');

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
  resolve: {
    alias: [
      {
        find: 'react-transition-group/TransitionGroupContext',
        replacement: transitionGroupContextPath,
      },
    ],
  },
  test: {
    environment: 'jsdom',
    setupFiles: ['./src/test/setup.ts'],
    include: ['src/**/*.test.{ts,tsx}'],
    server: {
      deps: {
        inline: [/@mui[+/]material/, /react-transition-group/],
      },
    },
  },
});
