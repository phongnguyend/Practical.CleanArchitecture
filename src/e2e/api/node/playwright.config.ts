import { defineConfig } from '@playwright/test';
import './support/environment.js';

export default defineConfig({
  testDir: './tests', timeout: 90_000, expect: { timeout: 15_000 },
  workers: 1, fullyParallel: false, retries: 0, forbidOnly: !!process.env.CI,
  reporter: [['list'], ['html', { open: 'never' }], ['junit', { outputFile: 'test-results/junit.xml' }]],
  // API authentication and password requests must not appear in traces.
  use: { trace: 'off', screenshot: 'off', video: 'off' },
});
