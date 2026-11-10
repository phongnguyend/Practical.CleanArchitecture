import { test, expect } from '../support/fixtures.js';

for (const [section, resource, path] of [
  ['Products', 'products', '/products'], ['Files', 'files', '/files'], ['Users', 'users', '/users'],
  ['Roles', 'roles', '/roles'], ['Settings', 'configurationentries', '/settings'], ['Audit Logs', 'auditlogentries/paged', '/auditlogs'],
]) {
  test(`navigate to ${section} and reload`, async ({ app, page }) => {
    await app.open(section, resource);
    await expect(page).toHaveURL(new RegExp(`${path}$`));
    const loaded = app.response(resource, 'GET');
    await page.reload();
    await loaded;
    await expect(app.nav.getByRole('link', { name: section, exact: true })).toHaveClass(/active/);
    await app.open('Home');
    await expect(page.getByRole('heading', { name: 'Phong Nguyen', exact: true })).toBeVisible();
  });
}

test('home project link has a safe external destination', async ({ app, page }) => {
  await app.open('Home');
  await expect(page.getByRole('link', { name: /Practical.CleanArchitecture/ })).toHaveAttribute('href', 'https://github.com/phongnguyend/Practical.CleanArchitecture');
});

for (const theme of ['Dark', 'Light']) {
  test(`${theme} appearance persists after reload`, async ({ app, page }) => {
    await page.getByLabel(/Choose appearance/).click();
    await page.getByRole('button', { name: theme, exact: true }).click();
    await expect(page.locator('html')).toHaveAttribute('data-bs-theme', theme.toLowerCase());
    await page.reload();
    await expect(page.locator('html')).toHaveAttribute('data-bs-theme', theme.toLowerCase());
  });
}
test('System appearance follows the browser color scheme', async ({ app, page }) => {
  await page.getByLabel(/Choose appearance/).click();
  await page.getByRole('button', { name: 'System', exact: true }).click();
  await page.emulateMedia({ colorScheme: 'dark' });
  await expect(page.locator('html')).toHaveAttribute('data-bs-theme', 'dark');
  await page.emulateMedia({ colorScheme: 'light' });
  await expect(page.locator('html')).toHaveAttribute('data-bs-theme', 'light');
});

test('notification hub connects and shows a toast', async ({ page }, info) => {
  test.skip(info.project.metadata.notificationHub === false, 'This application has no notification-hub client yet');
  await page.goto('/');
  await expect(page.getByText('Connected to NotificationHub', { exact: true })).toBeVisible({ timeout: 30_000 });
});
