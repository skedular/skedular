#!/usr/bin/env node
import { spawnSync } from 'node:child_process';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

const scriptDir = path.dirname(fileURLToPath(import.meta.url));
const webRoot = path.resolve(scriptDir, '..');
const appsDir = path.join(webRoot, 'apps');
const validApps = new Set(['webapp', 'webapp-spaces', 'webapp-teams']);

const log = (event, context = {}) => {
  console.log(JSON.stringify({ event, timestamp: new Date().toISOString(), ...context }));
};

const getAppPath = (appName) => {
  if (!validApps.has(appName)) {
    throw new Error(`Unknown web app "${appName}". Expected one of: ${Array.from(validApps).join(', ')}.`);
  }

  const appPath = path.join(appsDir, appName);
  if (!fs.existsSync(appPath)) {
    throw new Error(`App directory not found: ${appPath}`);
  }

  return appPath;
};

const run = (command, args, options = {}) => {
  const result = spawnSync(command, args, {
    stdio: 'inherit',
    shell: false,
    ...options,
  });

  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
};

const runTests = (appName, args) => {
  const appPath = getAppPath(appName);
  const headed = args.includes('--headed');
  const recordVideo = args.includes('--record-video') || process.env.PLAYWRIGHT_RECORD_VIDEO === 'true';
  const captureScreenshots = args.includes('--screenshots') || process.env.PLAYWRIGHT_CAPTURE_SCREENSHOTS === 'true';
  const runArgs = ['exec', 'playwright', 'test', 'tests/e2e', `--project=${headed ? 'chromium-headed' : 'chromium'}`];

  log('ui_test_suite_started', { appId: appName, headed, recordVideo, captureScreenshots });
  run('pnpm', runArgs, {
    cwd: appPath,
    env: {
      ...process.env,
      PLAYWRIGHT_RECORD_VIDEO: recordVideo ? 'true' : 'false',
      PLAYWRIGHT_CAPTURE_SCREENSHOTS: captureScreenshots ? 'true' : 'false',
    },
  });
  log('ui_test_suite_completed', { appId: appName });
};

const main = () => {
  const [commandOrApp, maybeApp, ...rest] = process.argv.slice(2);

  if (!commandOrApp) {
    console.error('Usage: pnpm test:e2e [app-name] [--run] [--headed] [--record-video] [--screenshots]');
    process.exit(1);
  }

  if (validApps.has(commandOrApp)) {
    runTests(commandOrApp, [maybeApp, ...rest].filter(Boolean));
    return;
  }

  if ((commandOrApp === 'run' || commandOrApp === 'test') && maybeApp) {
    runTests(maybeApp, rest);
    return;
  }

  if (commandOrApp === 'all') {
    const allArgs = [maybeApp, ...rest].filter(Boolean);
    for (const appName of validApps) {
      run('pnpm', ['test:e2e', appName, ...allArgs], { cwd: webRoot, env: process.env });
    }
    return;
  }

  throw new Error(`Unknown command "${commandOrApp}".`);
};

main();
