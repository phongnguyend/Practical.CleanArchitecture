import { test, expect, writes } from '../support/fixtures.js';

test('product required fields reject an empty form', async ({ app, page }) => {
  await app.open('Products', 'products');
  await app.action('Add Product').click();
  await app.action('Save').click();
  await expect(page.locator('input.is-invalid, input:invalid').first()).toBeVisible();
  await expect(page).toHaveURL(/\/products\/add$/);
});

test('product CRUD, details, filter, images, delete cancellation and audit history @writes', async ({ app, page, owned }) => {
  writes();
  const name = `${owned.prefix}-product`;
  await app.open('Products', 'products');
  await app.action('Add Product').click();
  await page.getByLabel('Name', { exact: true }).fill(name);
  await page.getByLabel('Code', { exact: true }).fill('PW123');
  await page.getByLabel('Description', { exact: true }).fill('Created by Playwright');
  const row = owned.track('products', await app.save('products', 'POST'));
  await expect(page).toHaveURL(new RegExp(`/products/${row.id}$`));
  await app.open('Products', 'products');
  await page.getByLabel('Filter records').fill(name);
  await expect(app.row(name)).toHaveCount(1);
  await app.action('Show Image').click();
  await expect(app.row(name).locator('img')).toBeVisible();
  await app.action('Hide Image').click();
  await expect(app.row(name).locator('img')).toHaveCount(0);
  await app.row(name).getByRole('link', { name, exact: true }).click();
  await expect(page).toHaveURL(new RegExp(`/products/${row.id}$`));
  await expect(page.getByText('Created by Playwright', { exact: true })).toBeVisible();
  await app.action('Back').click();
  await app.action('Edit', app.row(name)).click();
  await expect(page.getByLabel('Name', { exact: true })).toHaveValue(name);
  await page.getByLabel('Description', { exact: true }).fill('Updated by Playwright');
  await app.save(`products/${row.id}`, 'PUT');
  await expect(page).toHaveURL(new RegExp(`/products/${row.id}$`));
  await app.open('Products', 'products');
  await expect(app.row(name)).toContainText('Updated by Playwright');
  await app.history(name, 'products', row.id);
  await app.action('Delete', app.row(name)).click();
  await expect(app.dialog).toContainText(name);
  await app.action('No', app.dialog).click();
  await expect(app.row(name)).toBeVisible();
  await page.getByLabel('Filter records').fill(`${owned.prefix}-missing`);
  await expect(page.locator('table tbody tr')).toHaveCount(0);
  await page.getByLabel('Filter records').fill(name);
  await app.deleteRow(name, 'products', row.id);
});

test('CSV import requires a file and can be cancelled', async ({ app, page }) => {
  await app.open('Products', 'products');
  await app.action('Import Csv').click();
  await app.action('Import', app.dialog).click();
  await expect(app.dialog).toBeVisible();
  await expect(app.dialog.locator('.is-invalid, [aria-invalid="true"]').first()).toBeVisible();
  await app.action('Cancel', app.dialog).click();
  await expect(app.dialog).toBeHidden();
});

test('CSV export downloads a nonempty file', async ({ app, page }) => {
  await app.open('Products', 'products');
  const download = page.waitForEvent('download');
  await app.action('Export as Csv').click();
  const file = await download;
  expect(file.suggestedFilename()).toMatch(/\.csv$/i);
  expect(await file.failure()).toBeNull();
  const stream = await file.createReadStream();
  const parts: Buffer[] = []; for await (const part of stream!) parts.push(Buffer.from(part));
  expect(Buffer.concat(parts).length).toBeGreaterThan(0);
});

test('CSV import uploads and parses the selected file @writes', async ({ app, owned }) => {
  writes();
  const name = `${owned.prefix}-import`;
  await app.open('Products', 'products');
  await app.action('Import Csv').click();
  await app.dialog.locator('input[type="file"]').setInputFiles({
    name: 'products.csv', mimeType: 'text/csv',
    buffer: Buffer.from(`Code,Name,Description\nPWCSV,${name},Imported by Playwright\n`),
  });
  const response = app.response('products/importcsv', 'POST');
  try {
    await app.action('Import', app.dialog).click();
    const imported = await (await response).json();
    expect(imported).toEqual(expect.arrayContaining([expect.objectContaining({ name, code: 'PWCSV' })]));
    await expect(app.dialog).toBeHidden();
  } finally {
    // Some backend variants currently parse only; others may persist imported rows.
    const result = await owned.api.get('products');
    expect(result.ok()).toBeTruthy();
    for (const row of await result.json()) if (row.name === name) owned.track('products', row);
  }
});

test('PDF export downloads a PDF @pdf', async ({ app, page }) => {
  test.skip(process.env.E2E_PDF !== '1', 'Requires a configured HTML-to-PDF conversion server');
  await app.open('Products', 'products');
  const [, file] = await Promise.all([
    app.response('products/ExportAsPdf', 'GET'),
    page.waitForEvent('download'),
    app.action('Export as Pdf').click(),
  ]);
  expect(file.suggestedFilename()).toMatch(/\.pdf$/i);
  expect(await file.failure()).toBeNull();
});
