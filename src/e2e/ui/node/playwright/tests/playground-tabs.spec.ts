import { test, expect } from '../support/fixtures.js';

test('Admin preserves both subtab levels when switching playground channels', async ({ app, page }) => {
  await app.open('Admin');
  const admin = page.getByRole('navigation', { name: 'Admin sections', exact: true });
  await admin.getByRole('link', { name: 'Playgrounds', exact: true }).click();
  const channels = page.getByRole('navigation', { name: 'Playground channels', exact: true });
  for (const label of ['Mobile Push', 'Email', 'SMS']) {
    await channels.getByRole('link', { name: label, exact: true }).click();
    await expect(page.getByRole('heading', { name: label, exact: true })).toBeVisible();
    await expect(admin.getByRole('link', { name: 'Playgrounds', exact: true })).toHaveClass(/active/);
    await expect(channels.getByRole('link', { name: label, exact: true })).toHaveAttribute('aria-current', 'page');
    await expect(channels.locator('a.active')).toHaveCount(1);
    await expect(channels.getByRole('link')).toHaveCount(3);
  }
  await page.reload();
  await expect(channels.getByRole('link', { name: 'SMS', exact: true })).toHaveAttribute('aria-current', 'page');
});
