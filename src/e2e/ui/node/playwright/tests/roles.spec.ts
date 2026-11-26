import { test, expect, writes } from '../support/fixtures.js';

test('Roles: required field and keyboard modal dismissal', async ({ app, page }) => {
  await app.open('Roles', 'roles');
  await app.action('Add Role').click();
  await expect(app.dialog).toBeVisible();
  await app.action('Save', app.dialog).click();
  await expect(app.dialog).toBeVisible();
  await expect(app.dialog.getByLabel('Name', { exact: true })).toHaveValue('');
  await expect(app.dialog.locator('input.is-invalid, input:invalid').first()).toBeVisible();
  await page.keyboard.press('Escape');
  await expect(app.dialog).toBeHidden();
  await expect(app.action('Add Role')).toBeFocused();
});

test('role create, view, edit and delete @writes', async ({ app, page, owned }) => {
  writes();
  const name = `${owned.prefix}-role`;
  await app.open('Roles', 'roles');
  await app.action('Add Role').click();
  await app.dialog.getByLabel('Name', { exact: true }).fill(name);
  const row = owned.track('roles', await app.save('roles', 'POST'));
  await expect(app.dialog).toBeHidden();
  await app.row(name).getByRole('link', { name, exact: true }).click();
  await expect(page).toHaveURL(new RegExp(`/roles/${row.id}$`));
  await expect(page.getByText(name.toUpperCase(), { exact: true })).toBeVisible();
  await app.action('Back').click();
  await app.action('Edit', app.row(name)).click();
  const renamed = `${name}-edited`;
  await app.dialog.getByLabel('Name', { exact: true }).fill(renamed);
  await app.save(`roles/${row.id}`, 'PUT');
  await expect(app.dialog).toBeHidden();
  await app.deleteRow(renamed, 'roles', row.id);
});

