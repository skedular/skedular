import { chromium } from '@playwright/test';

const browser = await chromium.launch({ headless: true });
const page = await browser.newPage();
await page.goto('http://localhost:15002');

// Get the URL after any redirects
const url = await page.url();
console.log('URL:', url);

// Check HTML attributes
const htmlDataAttr = await page.evaluate(() => document.documentElement.getAttribute('data-product-app'));
console.log('html data-product-app:', htmlDataAttr);

// Check div attributes  
const divAttr = await page.evaluate(() => {
  const el = document.querySelector('[data-product-app]');
  return el ? el.getAttribute('data-product-app') : null;
});
console.log('div[data-product-app]:', divAttr);

await browser.close();
