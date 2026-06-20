import { chromium } from '@playwright/test';
import { spawnSync } from 'child_process';

// Start the dev server with env vars
spawnSync('pnpm', ['dev', '--hostname', '127.0.0.1'], {
  cwd: process.cwd(),
  env: { ...process.env, SKEDULAR_UI_TEST_BYPASS_AUTH: 'true' },
  stdio: 'inherit',
  shell: false,
});

// Give it time to start
await new Promise(r => setTimeout(r, 3000));

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
await page.goto('http://localhost:15002');

const url = await page.url();
console.log('URL:', url);

const htmlDataAttr = await page.evaluate(() => document.documentElement.getAttribute('data-product-app'));
console.log('html data-product-app:', htmlDataAttr);

const divAttr = await page.evaluate(() => {
  const el = document.querySelector('[data-product-app]');
  return el ? el.getAttribute('data-product-app') : null;
});
console.log('div[data-product-app]:', divAttr);

await browser.close();
process.exit(0);
