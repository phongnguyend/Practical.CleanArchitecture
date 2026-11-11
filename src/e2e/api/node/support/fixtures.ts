import { test as base, expect, request, type APIRequestContext, type APIResponse } from '@playwright/test';
import { randomUUID } from 'node:crypto';
import { settings } from './environment.js';
import type { RecordModel } from './models.js';

export function successful(response: APIResponse) {
  expect(response.ok(), `${response.status()} from API request`).toBeTruthy();
}
export async function json<T>(response: APIResponse): Promise<T> {
  successful(response);
  return response.json() as Promise<T>;
}
export async function absent(response: APIResponse) {
  if ([404, 204].includes(response.status())) return;
  successful(response);
  const body = await response.text();
  expect(body.trim() === '' || body.trim() === 'null', 'Deleted resource must be absent').toBeTruthy();
}
export class Data {
  readonly prefix = `api-e2e-${randomUUID().slice(0, 12)}`;
  private resources = new Set<string>();
  constructor(readonly api: APIRequestContext) {}
  owns(row: RecordModel) { return [row.name, row.key, row.userName].some(v => v?.startsWith(this.prefix)); }
  watch(resource: string) { this.resources.add(resource); }
  async create<T extends RecordModel>(resource: string, data: Omit<T, 'id'>): Promise<T> {
    this.watch(resource);
    const row = await json<T>(await this.api.post(resource, { data }));
    expect(row.id).toMatch(/^[0-9a-f-]{36}$/i);
    expect(row.id).not.toBe('00000000-0000-0000-0000-000000000000');
    expect(this.owns(row), 'Created record must belong to this test').toBeTruthy();
    return row;
  }
  async remove(resource: string, id: string) {
    const current = await this.api.get(`${resource}/${id}`);
    if ([204, 404].includes(current.status())) return;
    successful(current);
    const body = await current.text();
    if (!body.trim() || body.trim() === 'null') return;
    expect(this.owns(JSON.parse(body)), 'Refusing to delete an unowned record').toBeTruthy();
    successful(await this.api.delete(`${resource}/${id}`));
    await absent(await this.api.get(`${resource}/${id}`));
  }
  async cleanup() {
    const failures: string[] = [];
    // Discover this test's records even if a create returned an error after committing.
    for (const resource of this.resources) {
      try {
        const rows = await json<RecordModel[]>(await this.api.get(resource));
        for (const row of rows.filter(row => this.owns(row))) await this.remove(resource, row.id);
      } catch { failures.push(resource); }
    }
    expect(failures, 'Fixture cleanup failed for these resources').toEqual([]);
  }
}
export const test = base.extend<{ api: APIRequestContext; data: Data }, { accessToken: string }>({
  accessToken: [async ({}, use) => {
    if (process.env.API_ACCESS_TOKEN) { await use(process.env.API_ACCESS_TOKEN); return; }
    if (!settings.username || !settings.password) throw new Error('Configure API_USERNAME/API_PASSWORD and OIDC client credentials, API_DOTNET_SETTINGS, or API_ACCESS_TOKEN.');
    const auth = await request.newContext({ ignoreHTTPSErrors: settings.ignoreHTTPSErrors, timeout: 30_000 });
    try {
      const discovery = await json<{ token_endpoint: string }>(await auth.get(`${settings.authority.replace(/\/$/, '')}/.well-known/openid-configuration`));
      const endpoint = new URL(discovery.token_endpoint);
      const authority = new URL(settings.authority);
      if (endpoint.origin !== authority.origin) throw new Error('Discovery token endpoint must match the configured authority origin.');
      const response = await auth.post(endpoint.href, { form: {
        grant_type: 'password', client_id: settings.clientId, client_secret: settings.clientSecret,
        username: settings.username, password: settings.password, scope: settings.scope,
      } });
      successful(response);
      const token = await response.json();
      if (typeof token.access_token !== 'string' || !token.access_token) throw new Error('Identity Server did not return an access token');
      await use(token.access_token);
    } finally { await auth.dispose(); }
  }, { scope: 'worker' }],
  api: async ({ accessToken }, use) => {
    const api = await request.newContext({ baseURL: settings.baseURL, ignoreHTTPSErrors: settings.ignoreHTTPSErrors,
      timeout: 30_000, extraHTTPHeaders: { Authorization: `Bearer ${accessToken}` } });
    try { await use(api); } finally { await api.dispose(); }
  },
  data: async ({ api }, use) => {
    const data = new Data(api);
    try { await use(data); } finally { await data.cleanup(); }
  },
});
export { expect };
export const writes = () => test.skip(process.env.API_ALLOW_WRITES !== '1', 'Requires API_ALLOW_WRITES=1 against a disposable backend');
