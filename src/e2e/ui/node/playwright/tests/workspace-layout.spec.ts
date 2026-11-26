import { test, expect } from '../support/fixtures.js';

for (const width of [1440, 390]) {
  test(`workspace stays usable at ${width}px`, async ({ app, page }, info) => {
    await page.setViewportSize({ width, height: 900 });
    await app.open('Home');
    const main = page.locator('#main-content');
    await expect(main).toBeVisible();
    const skip = page.getByRole('link', { name: 'Skip to content', exact: true });
    await skip.focus();
    await skip.press('Enter');
    await expect(main).toBeFocused();
    for (const theme of ['Light', 'Dark']) {
      await page.getByLabel(/Choose appearance/).click();
      await page.getByRole('button', { name: theme, exact: true }).click();
      await expect(page.locator('html')).toHaveAttribute('data-bs-theme', theme.toLowerCase());
      await app.open('Home');
      await app.open('Products', 'products');
      await expect(app.nav.getByRole('link', { name: 'Admin', exact: true })).toBeVisible();
      await expect.poll(() => page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
      await page.screenshot({ path: info.outputPath(`products-${theme.toLowerCase()}-${width}.png`), fullPage: true });
      await app.action('Import Csv').click();
      await expect(app.dialog).toBeVisible();
      await expect(app.action('Cancel', app.dialog)).toBeInViewport();
      await expect(app.action('Import', app.dialog)).toBeInViewport();
      await page.screenshot({ path: info.outputPath(`import-${theme.toLowerCase()}-${width}.png`) });
      await page.keyboard.press('Escape');
      await expect(app.dialog).toBeHidden();
      await app.open('Admin');
      const sections = page.getByRole('navigation', { name: 'Admin sections', exact: true });
      await sections.getByRole('link', { name: 'Playgrounds', exact: true }).click();
      const channels = page.getByRole('navigation', { name: 'Playground channels', exact: true });
      await channels.getByRole('link', { name: 'Email', exact: true }).click();
      const primaryTab = app.nav.getByRole('link', { name: 'Admin', exact: true });
      const activeColor = await primaryTab.evaluate(element => getComputedStyle(element).backgroundColor);
      for (const tab of [primaryTab, sections.getByRole('link', { name: 'Playgrounds', exact: true }), channels.getByRole('link', { name: 'Email', exact: true })]) {
        await expect(tab).toHaveCSS('border-radius', '8px');
        await expect(tab).toHaveCSS('background-color', activeColor);
      }
      await channels.screenshot({ path: info.outputPath(`tabs-${theme.toLowerCase()}-${width}.png`) });
    }
  });
}

for (const width of [1440, 390]) {
  test(`panel actions stay right-aligned at ${width}px`, async ({ app, page }) => {
    await page.setViewportSize({ width, height: 900 });
    for (const [section, resource, action] of [
      ['Products', 'products', 'Add Product'],
      ['Files', 'files', 'Upload File'],
      ['Users', 'users', 'Add User'],
      ['Roles', 'roles', 'Add Role'],
      ['Settings', 'configurationentries', 'Add'],
    ]) {
      await app.open(section, resource);
      const button = app.action(action);
      await expect(button).toBeVisible();
      const header = button.locator('xpath=ancestor::*[contains(concat(" ", normalize-space(@class), " "), " card-header ")]');
      await expect(header).toHaveCount(1);
      const gap = await button.evaluate(element => {
        const panel = element.closest('.card-header')!;
        const style = getComputedStyle(panel);
        return panel.getBoundingClientRect().right - element.getBoundingClientRect().right
          - parseFloat(style.paddingRight) - parseFloat(style.borderRightWidth);
      });
      expect(Math.abs(gap), `${section} primary action should touch the header's right content edge`).toBeLessThanOrEqual(2);
    }
  });
}
