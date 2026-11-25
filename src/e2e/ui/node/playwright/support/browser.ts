import { readFile } from 'node:fs/promises';
import { authFile } from './environment.js';
import { test as base, expect } from '@playwright/test';

export const test = base.extend({
  page: async ({ page, headless, browserName }, use) => {
    if (!headless && browserName === 'chromium') {
      const session = await page.context().newCDPSession(page);
      const { windowId } = await session.send('Browser.getWindowForTarget');
      await session.send('Browser.setWindowBounds', { windowId, bounds: { windowState: 'maximized' } });
      await session.send('Emulation.clearDeviceMetricsOverride');
      await expect.poll(async () => {
        const { bounds } = await session.send('Browser.getWindowBounds', { windowId });
        return bounds.windowState;
      }, { message: 'Headed Chromium window must be maximized' }).toBe('maximized');
      await session.detach();
    }
    await use(page);
  },
  context: async ({ context, baseURL }, use, info) => {
    if (info.project.name === 'blazor-wasm' && baseURL) {
      const session = JSON.parse(await readFile(authFile('blazor-wasm').replace(/\.json$/, '.session.json'), 'utf8'));
      await context.addInitScript(({ origin, values }) => {
        if (location.origin === origin) {
          for (const [key, value] of Object.entries(values)) {
            sessionStorage.setItem(key, String(value));
          }
        }
      }, { origin: new URL(baseURL).origin, values: session });
    }
    const serverURL = String(info.project.metadata.serverURL);
    if (baseURL && serverURL && new URL(baseURL).origin !== new URL(serverURL).origin) {
      // Optional local port forwarding. Keep the app's registered OIDC origin while
      // serving its real HTML/assets from a free port. API/Identity traffic is untouched.
      const origin = new URL(baseURL).origin;
      await context.grantPermissions(['local-network-access'], { origin });
      await context.route(url => url.origin === origin, async route => {
        const original = new URL(route.request().url());
        const upstream = new URL(original.pathname + original.search, serverURL);
        const response = await route.fetch({ url: upstream.href });
        await route.fulfill({ response });
      });
    }
    await use(context);
  },
});
