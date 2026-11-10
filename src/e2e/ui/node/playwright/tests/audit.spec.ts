import { test, expect, writes } from '../support/fixtures.js';

test('audit JSON viewer formats, copies and closes with focus restored @writes', async ({ app, page, owned }) => {
  writes();
  await owned.create('products', { name: `${owned.prefix}-audit`, code: 'PWAUDIT', description: 'Audit fixture' });
  await app.open('Audit Logs', 'auditlogentries/paged');
  const trigger = page.getByRole('button', { name: 'View JSON', exact: true }).first();
  await trigger.click();
  await expect(app.dialog).toContainText('JSON Data');
  await app.dialog.getByRole('button', { name: 'Formatted', exact: true }).click();
  await expect(app.dialog.getByLabel('JSON content')).toContainText('{');
  await app.dialog.getByRole('button', { name: 'Copy JSON', exact: true }).click();
  await expect(app.dialog.getByRole('status')).toContainText('Copied to clipboard');
  const copied = await page.evaluate(() => navigator.clipboard.readText());
  expect(() => JSON.parse(copied)).not.toThrow();
  await app.dialog.getByRole('button', { name: 'Tree', exact: true }).click();
  await app.dialog.getByRole('button', { name: 'Collapse all', exact: true }).click();
  await expect(app.dialog.locator('details[open]')).toHaveCount(0);
  await page.keyboard.press('Escape');
  await expect(app.dialog).toBeHidden();
  await expect(trigger).toBeFocused();
});

test('audit pagination requests the selected page @writes', async ({ app, page, owned }) => {
  writes();
  // Each create leaves an audit event; seed enough for the common five-item page.
  for (let i = 0; i < 6; i++) await owned.create('products', { name: `${owned.prefix}-page-${i}`, code: `PWP${i}`, description: 'Pagination fixture' });
  await app.open('Audit Logs', 'auditlogentries/paged');
  const response = page.waitForResponse(r => /auditlogentries\/paged/i.test(r.url()) && new URL(r.url()).searchParams.get('page') === '2');
  await page.locator('.pagination').first().getByText('2', { exact: true }).click();
  expect((await response).ok()).toBeTruthy();
  await expect(page.locator('.pagination').first().locator('.active')).toContainText('2');
  const back = page.waitForResponse(r => /auditlogentries\/paged/i.test(r.url()) && new URL(r.url()).searchParams.get('page') === '1');
  await page.locator('.pagination').first().getByText('1', { exact: true }).click();
  expect((await back).ok()).toBeTruthy();
});
