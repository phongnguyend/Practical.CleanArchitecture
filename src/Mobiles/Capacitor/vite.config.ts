import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import { existsSync, readFileSync } from "node:fs";
import { sentryVitePlugin } from "@sentry/vite-plugin";
const publicConfig = existsSync("sentry.config.json") ? JSON.parse(readFileSync("sentry.config.json", "utf8")) : {};
const config = Object.fromEntries(["SENTRY_ENABLED", "SENTRY_DSN", "SENTRY_ENVIRONMENT", "SENTRY_RELEASE"].map((key) => [key, publicConfig[key]]));
const upload = !!(process.env.SENTRY_AUTH_TOKEN && process.env.SENTRY_ORG && process.env.SENTRY_PROJECT);
export default defineConfig({
  define: { __SENTRY_CONFIG__: JSON.stringify(config) },
  build: { sourcemap: upload ? "hidden" : false },
  plugins: [react(), ...(upload ? [sentryVitePlugin({
    org: process.env.SENTRY_ORG,
    project: process.env.SENTRY_PROJECT,
    authToken: process.env.SENTRY_AUTH_TOKEN,
    release: { name: config.SENTRY_RELEASE || undefined },
    sourcemaps: { filesToDeleteAfterUpload: ["./dist/**/*.map"] },
    telemetry: false,
  })] : [])],
  server: { port: 3000, strictPort: true },
});
