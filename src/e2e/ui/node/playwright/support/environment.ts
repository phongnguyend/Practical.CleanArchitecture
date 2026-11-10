import { existsSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

export const root = path.resolve(path.dirname(fileURLToPath(import.meta.url)), '..');
if (existsSync(path.join(root, '.env'))) process.loadEnvFile(path.join(root, '.env'));
const definitions = [
  { name: 'angular', url: process.env.E2E_ANGULAR_URL || 'http://localhost:4200', script: 'start' },
  { name: 'reactjs', url: process.env.E2E_REACTJS_URL || 'http://localhost:3000', script: 'dev' },
  { name: 'vuejs', url: process.env.E2E_VUEJS_URL || 'http://localhost:8080', script: 'dev' },
  { name: 'bff-reactjs', url: process.env.E2E_BFF_REACTJS_URL || 'https://localhost:44348', script: 'dev' },
  { name: 'nextjs', url: process.env.E2E_NEXTJS_URL || 'http://localhost:3000', script: 'dev' },
];
// Next.js and React share the registered default OIDC origin. Run them separately.
const selected = (process.env.E2E_APPS || 'angular,reactjs,vuejs').split(',').map(name => name.trim());
for (const name of selected) {
  if (!definitions.some(app => app.name === name)) throw new Error(`Unknown E2E_APPS entry: ${name}`);
}
export const apps = definitions.filter(app => selected.includes(app.name)).map(app => {
  const serverURL = process.env[`E2E_${app.name.toUpperCase().replaceAll('-', '_')}_SERVER_URL`] || app.url;
  const port = new URL(serverURL).port;
  if (app.name === 'bff-reactjs') {
    if (serverURL !== app.url) throw new Error('BFF tests must use the real YARP origin; origin forwarding is not supported.');
    return { ...app, serverURL, directory: 'bff/reactjs',
      devServerURL: process.env.E2E_BFF_REACTJS_DEV_URL || 'http://localhost:3000',
      authMode: 'cookie', notificationHub: true,
      csrfCookie: 'PHONG-XSRF-TOKEN', csrfHeader: 'X-XSRF-TOKEN',
      command: `npm run dev -- --host localhost --port ${new URL(process.env.E2E_BFF_REACTJS_DEV_URL || 'http://localhost:3000').port} --strictPort` };
  }
  if (app.name === 'nextjs' && serverURL !== app.url) throw new Error('Next.js requires its real server URL; origin forwarding is not supported for cookie authentication.');
  if (app.name === 'nextjs') return { ...app, serverURL, directory: app.name, devServerURL: serverURL, csrfCookie: undefined, csrfHeader: undefined, authMode: 'cookie', notificationHub: false, command: `npm run dev -- --hostname localhost --port ${port}` };
  return { ...app, serverURL, directory: app.name, devServerURL: serverURL, csrfCookie: undefined, csrfHeader: undefined, authMode: 'bearer', notificationHub: true, command: `npm run ${app.script} -- --host localhost --port ${port}${app.script === 'dev' ? ' --strictPort' : ''}` };
});
export const apiUrl = (process.env.E2E_API_URL || 'https://localhost:44312/api/').replace(/\/?$/, '/');
export const authFile = (app: string) => path.join(root, '.auth', `${app}.json`);
export const ignoreHTTPSErrors = process.env.E2E_IGNORE_HTTPS_ERRORS === '1';

if (new Set(apps.map(app => new URL(app.url).origin)).size !== apps.length) {
  throw new Error('Selected apps have duplicate origins. Run React and Next.js separately with E2E_APPS, or configure distinct registered URLs.');
}

if (new Set(apps.map(app => new URL(app.devServerURL).origin)).size !== apps.length) {
  throw new Error('Selected apps share a development server origin. Run them separately with E2E_APPS.');
}
