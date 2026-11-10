import { expect, request, type APIRequestContext } from '@playwright/test';
import { test as base } from './browser.js';
import { randomUUID } from 'node:crypto';
import { App } from './app.js';
import { apiUrl, ignoreHTTPSErrors } from './environment.js';

type Row = { id: string; name?: string; key?: string; userName?: string; [key: string]: unknown };
export class OwnedData {
  readonly prefix = `pw-${randomUUID().slice(0, 8)}`;
  private rows: { resource: string; id: string }[] = [];
  constructor(readonly api: APIRequestContext) {}
  track(resource: string, row: Row): Row {
    if (!row.id || ![row.name, row.key, row.userName].some(v => v?.startsWith(this.prefix))) {
      throw new Error(`Refusing to track ${resource}: missing ID or fixture prefix`);
    }
    this.rows.push({ resource, id: row.id });
    return row;
  }
  async create(resource: string, values: Record<string, unknown>): Promise<Row> {
    if (process.env.E2E_ALLOW_WRITES !== '1') throw new Error('Set E2E_ALLOW_WRITES=1 against a disposable backend for write scenarios.');
    const response = await this.api.post(resource, { data: values });
    expect(response.ok(), `Create fixture ${resource}: ${response.status()}`).toBeTruthy();
    return this.track(resource, await response.json());
  }
  async cleanup() {
    const errors: string[] = [];
    for (const { resource, id } of this.rows.reverse()) {
      const response = await this.api.get(`${resource}/${id}`);
      if ([204, 404].includes(response.status())) continue;
      if (!response.ok()) { errors.push(`Read ${resource}/${id}: ${response.status()}`); continue; }
      const body = await response.text();
      // Some API variants return 200 with no body for a deleted role.
      if (!body.trim()) continue;
      const row: Row = JSON.parse(body);
      if (![row.name, row.key, row.userName].some(v => v?.startsWith(this.prefix))) {
        errors.push(`Ownership check failed: ${resource}/${id}`); continue;
      }
      const removed = await this.api.delete(`${resource}/${id}`);
      if (!removed.ok() && removed.status() !== 404) errors.push(`Delete ${resource}/${id}: ${removed.status()}`);
    }
    expect(errors, 'Fixture cleanup must succeed').toEqual([]);
  }
}

export const test = base.extend<{ app: App; api: APIRequestContext; owned: OwnedData }>({
  app: async ({ page }, use) => {
    await page.goto('/');
    await expect(page.getByRole('navigation', { name: 'Main navigation' }).getByRole('link', { name: 'Logout', exact: true })).toBeVisible();
    await use(new App(page));
  },
  api: async ({ app, baseURL }, use, info) => {
    if (info.project.metadata.authMode === 'cookie') {
      // Exercise the real BFF with its HTTP-only cookies; never expose/decrypt tokens.
      const headers: Record<string, string> = {};
      const { csrfCookie, csrfHeader } = info.project.metadata;
      if (csrfCookie && csrfHeader) {
        // Refresh the BFF's session-bound anti-forgery token before seeding/cleanup.
        const response = await app.page.request.get(new URL('/userinfor', baseURL!).href);
        expect(response.ok(), 'BFF user information must load').toBeTruthy();
        const cookies = await app.page.context().cookies(baseURL!);
        const token = cookies.find(cookie => cookie.name === csrfCookie)?.value;
        if (!token) throw new Error('BFF anti-forgery cookie was not issued');
        headers[String(csrfHeader)] = token;
      }
      const api = await request.newContext({
        baseURL: new URL('/api/', baseURL!).href,
        storageState: await app.page.context().storageState(),
        extraHTTPHeaders: headers,
        ignoreHTTPSErrors,
      });
      try { await use(api); } finally { await api.dispose(); }
      return;
    }
    const token = await app.page.evaluate(() => {
      for (const key of Object.keys(localStorage)) {
        if (key.startsWith('oidc.user:')) {
          const user = JSON.parse(localStorage.getItem(key)!);
          if (user.access_token) return user.access_token as string;
        }
      }
      throw new Error('Authenticated OIDC session not found');
    });
    const api = await request.newContext({ baseURL: apiUrl, ignoreHTTPSErrors, extraHTTPHeaders: { Authorization: `Bearer ${token}` } });
    await use(api);
    await api.dispose();
  },
  owned: async ({ api }, use) => {
    const owned = new OwnedData(api);
    try { await use(owned); } finally { await owned.cleanup(); }
  },
});
export { expect };
export const writes = () => test.skip(process.env.E2E_ALLOW_WRITES !== '1', 'Requires E2E_ALLOW_WRITES=1 and a disposable backend');
