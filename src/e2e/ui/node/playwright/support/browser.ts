import { test as base } from '@playwright/test';

export const test = base.extend({
  context: async ({ context, baseURL }, use, info) => {
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
