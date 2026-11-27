import { test, expect, writes } from '../support/fixtures.js';

test('audit action badges use consistent colors in light and dark appearance', async ({ app, page }) => {
  await app.open('Audit Logs', 'auditlogentries/paged');
  const badges = page.locator('table tbody tr td:nth-child(3) [data-action]');
  await expect(badges.first(), 'Requires at least one existing audit event').toBeVisible();
  for (const theme of ['Light', 'Dark']) {
    await page.getByLabel(/Choose appearance/).click();
    await page.getByRole('button', { name: theme, exact: true }).click();
    await expect(page.locator('html')).toHaveAttribute('data-bs-theme', theme.toLowerCase());
    for (const badge of await badges.all()) {
      const label = (await badge.innerText()).trim();
      const action = label.toUpperCase().split(/[_\s-]/)[0];
      await expect(badge).toHaveAttribute('data-action', action);
      const [foreground, background] = ['DELETE', 'DELETED'].includes(action)
        ? ['rgb(185, 28, 28)', 'rgb(254, 226, 226)']
        : ['UPDATE', 'UPDATED'].includes(action)
          ? ['rgb(154, 52, 18)', 'rgb(255, 237, 213)']
          : ['ADD', 'ADDED', 'CREATE', 'CREATED'].includes(action)
            ? ['rgb(22, 101, 52)', 'rgb(220, 252, 231)']
            : ['rgb(51, 65, 85)', 'rgb(226, 232, 240)'];
      await expect(badge, label).toHaveCSS('color', foreground);
      await expect(badge, label).toHaveCSS('background-color', background);
    }
    await page.screenshot({ path: test.info().outputPath(`audit-${theme.toLowerCase()}.png`), fullPage: true });
  }
});

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

test('audit controls stay aligned and JSON actions remain usable', async ({ app, page }) => {
  await app.open('Audit Logs', 'auditlogentries/paged');
  const pagination = page.locator('.pagination').first();
  await expect(pagination).toBeVisible();
  const heights = await pagination.locator('.page-link').evaluateAll(elements => elements.map(element => element.getBoundingClientRect().height));
  expect(heights.length).toBeGreaterThan(0);
  expect(Math.max(...heights) - Math.min(...heights)).toBeLessThan(1);
  for (const toolbar of await page.locator('.audit-data-actions').all()) {
    const preview = toolbar.locator('..').locator('.audit-data-preview');
    const toolbarBox = await toolbar.boundingBox();
    const previewBox = await preview.boundingBox();
    expect(toolbarBox!.y + toolbarBox!.height).toBeLessThanOrEqual(previewBox!.y);
  }
  const copy = page.locator('.app-copy-trigger').first();
  if (['angular', 'reactjs', 'vuejs', 'nextjs', 'blazor-server', 'blazor-wasm'].includes(test.info().project.name)) {
    await expect(copy, 'Audit data must provide a copy action').toBeVisible();
  }
  if (await copy.count()) {
    const expectedText = await copy.locator('xpath=ancestor::td').locator('.audit-data-preview').textContent();
    const before = await copy.boundingBox();
    await copy.click();
    await expect(copy.locator('..').getByRole('status')).toHaveText('Copied to clipboard');
    expect(await page.evaluate(() => navigator.clipboard.readText())).toBe(expectedText);
    expect(await copy.boundingBox()).toEqual(before);
  }
  const json = page.getByRole('button', { name: 'View JSON', exact: true }).first();
  await expect(json, 'Audit data must provide a JSON viewer').toBeVisible();
  if (await json.count()) {
    const columns = page.locator('table').first().locator('thead th');
    const widthsBefore = await columns.evaluateAll(elements => elements.map(element => element.getBoundingClientRect().width));
    await json.click();
    await expect(app.dialog).toContainText('JSON Data');
    await page.screenshot({ path: test.info().outputPath('audit-json-open.png'), fullPage: true });
    const widthsAfter = await columns.evaluateAll(elements => elements.map(element => element.getBoundingClientRect().width));
    for (let index = 0; index < widthsBefore.length; index++) {
      expect(Math.abs(widthsAfter[index] - widthsBefore[index]), 'Opening JSON must preserve audit column widths').toBeLessThan(2);
    }
    await app.dialog.getByRole('button', { name: 'Formatted', exact: true }).click();
    await expect(app.dialog.getByLabel('JSON content')).toContainText('{');
    await app.dialog.getByRole('button', { name: 'Copy JSON', exact: true }).click();
    await expect(app.dialog.getByRole('status')).toHaveText('Copied to clipboard');
    const copiedJson = await page.evaluate(() => navigator.clipboard.readText());
    expect(() => JSON.parse(copiedJson)).not.toThrow();
    await app.dialog.getByRole('button', { name: 'Tree', exact: true }).click();
    await app.dialog.getByRole('button', { name: 'Collapse all', exact: true }).click();
    await expect(app.dialog.locator('details[open]')).toHaveCount(0);
    await page.keyboard.press('Escape');
    await expect(app.dialog).toBeHidden();
    await expect(json).toBeFocused();
  }
  await page.screenshot({ path: test.info().outputPath('audit-controls.png'), fullPage: true });
});
