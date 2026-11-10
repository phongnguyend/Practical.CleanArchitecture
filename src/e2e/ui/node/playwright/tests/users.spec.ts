import { test, expect, writes } from '../support/fixtures.js';

test('user creation rejects an empty username', async ({ app, page }) => {
  await app.open('Users', 'users');
  await app.action('Add User').click();
  await app.action('Save').click();
  await expect(page).toHaveURL(/\/users\/add$/);
  await expect(page.locator('input.is-invalid, input:invalid').first()).toBeVisible();
});

test('user CRUD and account dialogs without sending emails @writes', async ({ app, page, owned }) => {
  writes();
  const name = `${owned.prefix}-user`;
  await app.open('Users', 'users');
  await app.action('Add User').click();
  await page.getByLabel('User Name', { exact: true }).fill(name);
  await page.getByLabel('Email', { exact: true }).fill(`${name}@example.invalid`);
  await page.getByLabel('Phone Number', { exact: true }).fill('0123456789');
  const row = owned.track('users', await app.save('users', 'POST'));
  await expect(page).toHaveURL(new RegExp(`/users/${row.id}$`));
  await app.open('Users', 'users');
  await app.action('Edit', app.row(name)).click();
  await expect(page.getByLabel('User Name', { exact: true })).toHaveValue(name);
  await page.getByLabel('Phone Number', { exact: true }).fill('0987654321');
  await app.save(`users/${row.id}`, 'PUT');
  await expect(page).toHaveURL(new RegExp(`/users/${row.id}$`));
  await expect(page.getByText('0987654321', { exact: true })).toBeVisible();
  for (const label of ['Set Password', 'Send Password Reset Email', 'Send Email Address Confirmation Email']) {
    await app.action(label).click();
    await expect(app.dialog).toBeVisible();
    await expect(app.dialog).toContainText(label);
    await page.keyboard.press('Escape');
    await expect(app.dialog).toBeHidden();
  }
  await app.action('Back').click();
  await app.deleteRow(name, 'users', row.id);
});
