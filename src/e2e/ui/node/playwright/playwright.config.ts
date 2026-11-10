import { defineConfig, devices } from '@playwright/test';
import path from 'node:path';
import { apps, authFile, ignoreHTTPSErrors, root } from './support/environment.js';

const artifacts = process.env.E2E_ARTIFACTS_DIR;

export default defineConfig({
  outputDir: artifacts ? path.join(artifacts, 'test-results') : 'test-results',
  testDir: './tests',
  timeout: 60_000,
  expect: { timeout: 15_000 },
  fullyParallel: false,
  workers: Number(process.env.E2E_WORKERS || 1),
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  reporter: [['list'], ['html', { open: 'never', outputFolder: artifacts ? path.join(artifacts, 'playwright-report') : 'playwright-report' }], ['junit', { outputFile: artifacts ? path.join(artifacts, 'junit.xml') : 'test-results/junit.xml' }]],
  use: {
    ...devices['Desktop Chrome'], ignoreHTTPSErrors,
    trace: 'retain-on-failure', screenshot: 'only-on-failure', video: 'retain-on-failure',
    permissions: ['clipboard-read', 'clipboard-write'],
  },
  webServer: process.env.E2E_START_SERVERS === '1' ? apps.map(app => ({
    command: app.command, cwd: path.resolve(root, '../../../../UIs', app.directory),
    url: app.devServerURL, reuseExistingServer: !process.env.CI, timeout: 180_000,
    // Development servers can forward application console output containing OIDC state.
    stdout: 'ignore', stderr: 'ignore',
  })) : undefined,
  projects: apps.flatMap(app => [
    { name: `${app.name}-setup`, testMatch: /auth\.setup\.ts/, metadata: { app: app.name, serverURL: app.serverURL, authMode: app.authMode, notificationHub: app.notificationHub, csrfCookie: app.csrfCookie, csrfHeader: app.csrfHeader },
      use: { baseURL: app.url, trace: 'off' as const, video: 'off' as const, screenshot: 'off' as const } },
    { name: app.name, testMatch: /.*\.spec\.ts/, dependencies: [`${app.name}-setup`],
      metadata: { app: app.name, serverURL: app.serverURL, authMode: app.authMode, notificationHub: app.notificationHub, csrfCookie: app.csrfCookie, csrfHeader: app.csrfHeader }, use: { baseURL: app.url, storageState: authFile(app.name) } },
  ]),
});
