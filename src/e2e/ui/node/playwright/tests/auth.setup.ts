import { expect } from '@playwright/test';
import { test } from '../support/browser.js';
import { mkdir } from 'node:fs/promises';
import path from 'node:path';
import { authFile } from '../support/environment.js';

test('sign in through Identity Server', async ({ page }, info) => {
  test.setTimeout(120_000);
  const username = process.env.E2E_USERNAME;
  const password = process.env.E2E_PASSWORD;
  if (!username || !password) throw new Error('Set E2E_USERNAME and E2E_PASSWORD for a disposable development account.');
  await page.goto('/');
  await expect(page.getByRole('navigation', { name: 'Main navigation' }), 'Base URL must serve the Classified Ads UI').toBeVisible();
  await page.getByRole('navigation', { name: 'Main navigation' }).getByRole('link', { name: 'Login', exact: true }).click();
  await page.getByRole('textbox', { name: 'Username', exact: true }).fill(username);
  await page.getByLabel('Password', { exact: true }).fill(password);
  await page.getByRole('button', { name: 'Login', exact: true }).click();
  await expect(page.getByRole('navigation', { name: 'Main navigation' }).getByRole('link', { name: 'Logout', exact: true })).toBeVisible({ timeout: 45_000 });
  const file = authFile(String(info.project.metadata.app));
  await mkdir(path.dirname(file), { recursive: true });
  await page.context().storageState({ path: file });
});
