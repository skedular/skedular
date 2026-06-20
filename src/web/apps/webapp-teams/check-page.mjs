import { chromium } from '@playwright/test';

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();

// Set environment variable to bypass auth
await page.setExtraHTTPHeaders({
  'Cookie': 'skedular-ui-test-bypass-auth=true'
});

await page.goto('http://localhost:15002');

// Get the URL after any redirects
const url = await page.url();
console.log('URL:', url);

// Check HTML attributes
const htmlDataAttr = await page.evaluate(() => document.documentElement.getAttribute('data-product-app'));
console.log('html data-product-app:', htmlDataAttr);

await browser.close();
