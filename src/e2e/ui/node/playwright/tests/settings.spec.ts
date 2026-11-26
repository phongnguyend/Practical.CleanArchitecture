import { test, expect, writes } from '../support/fixtures.js';

test('Settings: required field and keyboard modal dismissal', async ({ app, page }) => {
  await app.open('Settings', 'configurationentries');
  await app.action('Add').click();
  await expect(app.dialog).toBeVisible();
  await app.action('Save', app.dialog).click();
  await expect(app.dialog).toBeVisible();
  await expect(app.dialog.getByLabel('Key', { exact: true })).toHaveValue('');
  await expect(app.dialog.locator('input.is-invalid, input:invalid').first()).toBeVisible();
  await page.keyboard.press('Escape');
  await expect(app.dialog).toBeHidden();
  await expect(app.action('Add')).toBeFocused();
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

// The import endpoints parse files without persisting records, matching the web UIs.
test('Excel import uploads the selected file and resets when reopened', async ({ app, page }) => {
  await app.open('Settings', 'configurationentries');
  const download = page.waitForEvent('download');
  await app.action('Export as Excel').click();
  const file = await download;
  expect(await file.failure()).toBeNull();
  const stream = await file.createReadStream();
  const chunks: Buffer[] = [];
  for await (const chunk of stream!) {
    chunks.push(Buffer.from(chunk));
  }
  await app.action('Import Excel').click();
  await app.dialog.locator('input[type=file]').setInputFiles({
    name: 'settings.xlsx',
    mimeType: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    buffer: Buffer.concat(chunks),
  });
  await app.action('Import', app.dialog).click();
  await expect(app.dialog).toBeHidden();
  await app.action('Import Excel').click();
  await expect(app.dialog.locator('input[type=file]')).toHaveValue('');
  await app.action('Cancel', app.dialog).click();
  await expect(app.dialog).toBeHidden();
});
