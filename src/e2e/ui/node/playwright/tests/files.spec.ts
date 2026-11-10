import { test, expect, writes } from '../support/fixtures.js';

test('file upload rejects missing required fields', async ({ app, page }) => {
  await app.open('Files', 'files');
  await app.action('Upload File').click();
  await app.action('Save').click();
  await expect(page).toHaveURL(/\/files\/upload$/);
  await expect(page.locator('.is-invalid, input:invalid, [aria-invalid="true"]').first()).toBeVisible();
});

for (const encrypted of [false, true]) {
  test(`file upload, edit, download, history and delete (encrypted=${encrypted}) @writes`, async ({ app, page, owned }) => {
    writes();
    const name = `${owned.prefix}-file`;
    const content = Buffer.concat([Buffer.from(`Playwright fixture ${name}\n`), Buffer.from([0, 128, 255, 13, 10])]);
    await app.open('Files', 'files');
    await app.action('Upload File').click();
    await page.getByLabel('Name', { exact: true }).fill(name);
    await page.getByLabel('Description', { exact: true }).fill('Playwright upload');
    await page.locator('input[type="file"]').setInputFiles({ name: `${name}.bin`, mimeType: 'application/octet-stream', buffer: content });
    await page.getByLabel('Encrypted', { exact: true }).setChecked(encrypted);
    const row = owned.track('files', await app.save('files', 'POST'));
    await expect(page).toHaveURL(new RegExp(`/files/edit/${row.id}$`));
    await expect(page.getByLabel('Name', { exact: true })).toHaveValue(name);
    await expect(page.getByLabel('Description', { exact: true })).toHaveValue('Playwright upload');
    await page.getByLabel('Description', { exact: true }).fill('Playwright updated upload');
    await app.save(`files/${row.id}`, 'PUT');
    await expect(page).toHaveURL(/\/files$/);
    await expect(app.row(name)).toContainText('Playwright updated upload');
    const download = page.waitForEvent('download');
    await app.action('Download', app.row(name)).click();
    const file = await download;
    expect(file.suggestedFilename()).toBe(`${name}.bin`);
    expect(await file.failure()).toBeNull();
    const stream = await file.createReadStream();
    const chunks: Buffer[] = []; for await (const chunk of stream!) chunks.push(Buffer.from(chunk));
    expect(Buffer.concat(chunks)).toEqual(content);
    await app.history(name, 'files', row.id);
    await app.deleteRow(name, 'files', row.id);
  });
}
