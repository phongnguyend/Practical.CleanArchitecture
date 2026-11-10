import { test, expect, writes } from '../support/fixtures.js';

for (const [section, resource, add, field] of [
  ['Roles', 'roles', 'Add Role', 'Name'], ['Settings', 'configurationentries', 'Add', 'Key'],
]) {
  test(`${section}: required field and keyboard modal dismissal`, async ({ app, page }) => {
    await app.open(section, resource);
    await app.action(add).click();
    await expect(app.dialog).toBeVisible();
    await app.action('Save', app.dialog).click();
    await expect(app.dialog).toBeVisible();
    await expect(app.dialog.getByLabel(field, { exact: true })).toHaveValue('');
    await expect(app.dialog.locator('input.is-invalid, input:invalid').first()).toBeVisible();
    await page.keyboard.press('Escape');
    await expect(app.dialog).toBeHidden();
    await expect(app.action(add)).toBeFocused();
  });
}

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

test('configuration create, mask sensitive value, edit and delete @writes', async ({ app, owned }) => {
  writes();
  const key = `${owned.prefix}-setting`;
  await app.open('Settings', 'configurationentries');
  await app.action('Add').click();
  await app.dialog.getByLabel('Key', { exact: true }).fill(key);
  await app.dialog.getByLabel('Value', { exact: true }).fill('fixture-secret');
  await app.dialog.getByLabel('Description', { exact: true }).fill('E2E configuration');
  await app.dialog.getByLabel('Sensitive', { exact: true }).check();
  const row = owned.track('configurationentries', await app.save('configurationentries', 'POST'));
  await expect(app.dialog).toBeHidden();
  await expect(app.row(key)).toContainText('******');
  await expect(app.row(key)).not.toContainText('fixture-secret');
  await app.action('Edit', app.row(key)).click();
  await app.dialog.getByLabel('Sensitive', { exact: true }).uncheck();
  await app.dialog.getByLabel('Value', { exact: true }).fill('fixture-public');
  await app.save(`configurationentries/${row.id}`, 'PUT');
  await expect(app.row(key)).toContainText('fixture-public');
  await app.deleteRow(key, 'configurationentries', row.id);
});

test('Excel import requires a file and cancels without uploading', async ({ app }) => {
  await app.open('Settings', 'configurationentries');
  await app.action('Import Excel').click();
  await app.action('Import', app.dialog).click();
  await expect(app.dialog.locator('.is-invalid, [aria-invalid="true"]').first()).toBeVisible();
  await app.action('Cancel', app.dialog).click();
  await expect(app.dialog).toBeHidden();
});

test('configuration export downloads Excel', async ({ app, page }) => {
  await app.open('Settings', 'configurationentries');
  const download = page.waitForEvent('download');
  await app.action('Export as Excel').click();
  const file = await download;
  expect(file.suggestedFilename()).toMatch(/\.xlsx$/i);
  expect(await file.failure()).toBeNull();
});
