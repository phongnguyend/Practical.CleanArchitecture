import { test, expect } from '../support/fixtures.js';

// Skip before API fixtures initialize when queue writes are disabled.
const queueTest = process.env.E2E_ALLOW_WRITES === '1' ? test : test.skip;

for (const channel of ['email', 'sms'] as const) {
  const name = channel === 'email' ? 'Email' : 'SMS';
  test.describe(`Admin / Playgrounds / ${name}`, () => {
    test('navigation, required fields, and recipient validation', async ({ app, page }) => {
      await page.getByRole('navigation', { name: 'Main navigation' }).getByRole('link', { name: 'Admin', exact: true }).click();
      await page.getByRole('link', { name: 'Playgrounds', exact: true }).last().click();
      await page.getByRole('link', { name, exact: true }).click();
      await expect(page.getByRole('heading', { name, exact: true })).toBeVisible();
      await expect(page.getByLabel('Message', { exact: true })).toHaveAttribute('maxlength', channel === 'email' ? '10000' : '1600');
      await page.getByRole('button', { name: 'Queue message', exact: true }).click();
      await expect(page.getByText('Complete all required fields.', { exact: true })).toBeVisible();
      if (channel === 'email') {
        await page.getByLabel('From', { exact: true }).fill('sender@example.com');
        await page.getByLabel('Subject', { exact: true }).fill('Validation only');
      }
      await page.getByLabel(channel === 'email' ? 'To' : 'Phone number', { exact: true }).fill('invalid');
      await page.getByLabel('Message', { exact: true }).fill('Do not send this invalid request.');
      await page.getByRole('button', { name: 'Queue message', exact: true }).click();
      const recipientError = channel === 'email' ? 'Enter valid From and To email addresses.' : 'Enter a phone number in international format';
      await expect(page.getByRole('alert').filter({ hasText: recipientError })).toBeVisible();
      await expect(page.getByRole('heading', { name: 'Recent messages', exact: true })).toBeVisible();
    });

    queueTest('queues through the Fake provider and displays the saved message', async ({ app, page }) => {
      await page.goto(`/admin/playgrounds/${channel}`);
      const provider = page.getByRole('status').filter({ hasText: 'Provider:' });
      await expect(provider).toBeVisible();
      test.skip(!(await provider.innerText()).includes('Provider: Fake'), 'Avoid real email or SMS delivery.');
      const body = `Playwright ${channel} ${Date.now()}`;
      if (channel === 'email') {
        await page.getByLabel('From', { exact: true }).fill('sender@example.com');
        await page.getByLabel('Subject', { exact: true }).fill('Playground verification');
      }
      await page.getByLabel(channel === 'email' ? 'To' : 'Phone number', { exact: true }).fill(channel === 'email' ? 'recipient@example.com' : '+15551234567');
      await page.getByLabel('Message', { exact: true }).fill(body);
      await page.getByRole('button', { name: 'Queue message', exact: true }).click();
      await expect(page.getByRole('status').filter({ hasText: 'Message queued. ID:' })).toBeVisible();
      await page.getByRole('button', { name: 'Refresh queue', exact: true }).click();
      await expect(page.locator('article').filter({ hasText: body })).toBeVisible();
    });
  });
}
