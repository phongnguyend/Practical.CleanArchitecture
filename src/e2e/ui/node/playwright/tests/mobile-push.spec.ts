import { test, expect } from '../support/fixtures.js';

// Skip before API fixtures initialize when queue writes are disabled.
const queueTest = process.env.E2E_ALLOW_WRITES === '1' ? test : test.skip;

test.describe('Admin / Playgrounds / Mobile Push', () => {
  test('navigation and required fields', async ({ app, page }) => {
    await page.getByRole('navigation', { name: 'Main navigation' }).getByRole('link', { name: 'Admin', exact: true }).click();
    await page.getByRole('link', { name: 'Playgrounds', exact: true }).last().click();
    await page.getByRole('link', { name: 'Mobile Push', exact: true }).click();
    await expect(page.getByRole('heading', { name: 'Mobile Push', exact: true })).toBeVisible();
    await expect(page.getByLabel('Title', { exact: true })).toHaveAttribute('maxlength', '100');
    await expect(page.getByLabel('Message', { exact: true })).toHaveAttribute('maxlength', '500');
    await page.getByRole('button', { name: 'Queue message', exact: true }).click();
    await expect(page.getByText('Title and message are required.', { exact: true })).toBeVisible();
    await expect(page.getByRole('heading', { name: 'Recent messages', exact: true })).toBeVisible();
  });

  queueTest('queues a message and displays it after refresh', async ({ app, page }) => {
    await page.goto('/admin/playgrounds/mobile-push');
    const provider = page.getByRole('status').filter({ hasText: 'Provider:' });
    await expect(provider).toBeVisible();
    test.skip(!(await provider.innerText()).includes('Provider: Fake'), 'Do not send real device notifications.');
    const title = `Playwright push ${Date.now()}`;
    await page.getByLabel('Title', { exact: true }).fill(title);
    await page.getByLabel('Message', { exact: true }).fill('Automated mobile push playground verification.');
    await page.getByLabel('Destination', { exact: true }).selectOption('notifications');
    await page.getByRole('button', { name: 'Queue message', exact: true }).click();
    await expect(page.getByRole('status').filter({ hasText: 'Message queued. ID:' })).toBeVisible();
    await page.getByRole('button', { name: 'Refresh queue', exact: true }).click();
    await expect(page.getByText(title, { exact: true })).toBeVisible();
  });
});
