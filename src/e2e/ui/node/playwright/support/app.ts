import { expect, type Locator, type Page, type Response } from '@playwright/test';

export class App {
  constructor(readonly page: Page) {}
  get nav() { return this.page.getByRole('navigation', { name: 'Main navigation' }); }
  get dialog() { return this.page.getByRole('dialog'); }
  action(name: string | RegExp, scope: Page | Locator = this.page) {
    return scope.getByRole('button', { name, exact: typeof name === 'string' })
      .or(scope.getByRole('link', { name, exact: typeof name === 'string' }));
  }
  row(name: string) {
    const escaped = name.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    // File lists include the original filename beside the record name.
    return this.page.locator('table tbody tr').filter({ has: this.page.getByRole('cell', { name: new RegExp(`^${escaped}(?: \\([^)]*\\))?$`) }) });
  }
  async open(section: string, resource?: string) {
    const loaded = resource ? this.response(resource, 'GET') : undefined;
    await this.nav.getByRole('link', { name: section, exact: true }).click();
    if (loaded) await loaded;
  }
  async response(resource: string, method: string): Promise<Response> {
    const response = await this.page.waitForResponse(r => {
      const path = new URL(r.url()).pathname.toLowerCase().replace(/\/$/, '');
      // Next.js canonicalizes trailing slashes with a 308 before the API responds.
      if ([301, 302, 303, 307, 308].includes(r.status())) return false;
      return path.endsWith(`/api/${resource.toLowerCase()}`) && r.request().method() === method;
    });
    expect(response.ok(), `${method} ${resource}: HTTP ${response.status()}`).toBeTruthy();
    return response;
  }
  async save(resource: string, method: 'POST' | 'PUT') {
    const response = this.response(resource, method);
    await this.action('Save', await this.dialog.count() ? this.dialog : this.page).click();
    return (await response).json();
  }
  async deleteRow(name: string, resource: string, id: string) {
    await this.action('Delete', this.row(name)).click();
    await expect(this.dialog).toContainText(name);
    const removed = this.response(`${resource}/${id}`, 'DELETE');
    await this.action(/^(Yes|Delete)$/, this.dialog).click();
    await removed;
    await expect(this.dialog).toBeHidden();
    await expect(this.row(name)).toHaveCount(0);
  }
  async history(name: string, resource: string, id: string) {
    const loaded = this.response(`${resource}/${id}/auditlogs`, 'GET');
    await this.action('View Audit Logs', this.row(name)).click();
    await loaded;
    await expect(this.dialog).toBeVisible();
    await expect(this.dialog).toContainText(/Audit Logs/i);
    await this.page.keyboard.press('Escape');
    await expect(this.dialog).toBeHidden();
  }
}
