const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const { test, mock } = require("node:test");
const { execFileSync } = require("node:child_process");
const configure = require("../app.config.js");

test("native Firebase linking keeps Messaging and its App dependency consistent", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "firebase-linking-"));
  const android = path.join(directory, "google-services.json");
  const ios = path.join(directory, "GoogleService-Info.plist");
  const linking = (crashlytics, push) => JSON.parse(execFileSync(process.execPath,
    ["-e", "console.log(JSON.stringify(require('./react-native.config.js').dependencies))"], {
      cwd: path.resolve(__dirname, ".."),
      env: { ...process.env, FIREBASE_ANDROID_CONFIG: android, FIREBASE_IOS_CONFIG: ios,
        CRASHLYTICS_ENABLED: String(crashlytics), PUSH_NOTIFICATIONS_ENABLED: String(push) },
      encoding: "utf8",
    }));
  try {
    const missing = linking(true, true);
    for (const dependency of Object.values(missing)) {
      assert.deepEqual(dependency.platforms, { android: null, ios: null });
    }
    fs.writeFileSync(android, "test fixture");
    const pushOnly = linking(false, true);
    assert.deepEqual(pushOnly["@react-native-firebase/app"].platforms, { ios: null });
    assert.deepEqual(pushOnly["@react-native-firebase/messaging"].platforms, { ios: null });
    assert.deepEqual(pushOnly["@react-native-firebase/crashlytics"].platforms, { android: null, ios: null });
    const crashOnly = linking(true, false);
    assert.deepEqual(crashOnly["@react-native-firebase/messaging"].platforms, { android: null, ios: null });
    assert.deepEqual(crashOnly["@react-native-firebase/app"].platforms, { ios: null });
  } finally {
    if (fs.existsSync(android)) {
      fs.unlinkSync(android);
    }
    fs.rmdirSync(directory);
  }
});

// Environment changes are isolated to this test process; no native SDK is loaded.
test("Firebase is opt-in and each configured platform gets its native plugins", () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), "firebase-config-"));
  const android = path.join(directory, "google-services.json");
  const ios = path.join(directory, "GoogleService-Info.plist");
  process.env.FIREBASE_ANDROID_CONFIG = android;
  process.env.FIREBASE_IOS_CONFIG = ios;
  delete process.env.CRASHLYTICS_ENABLED;
  const config = { plugins: ["expo-router"], extra: { eas: { projectId: "preserved" } } };
  try {
    let result = configure({ config });
    assert.deepEqual(result.extra.crashlytics, { android: false, ios: false });
    assert.deepEqual(result.plugins, ["expo-router"]);
    process.env.CRASHLYTICS_ENABLED = "true";
    assert.throws(() => configure({ config }), /configuration is missing/);
    delete process.env.CRASHLYTICS_ENABLED;
    fs.writeFileSync(android, "test fixture");
    result = configure({ config });
    assert.deepEqual(result.extra.crashlytics, { android: true, ios: false });
    assert.equal(result.android.googleServicesFile, android);
    assert.ok(result.plugins.includes("@react-native-firebase/crashlytics"));
    assert.equal(result.extra.eas.projectId, "preserved");
    fs.writeFileSync(ios, "test fixture");
    result = configure({ config });
    assert.equal(result.ios.googleServicesFile, ios);
    assert.deepEqual(result.plugins.at(-1), ["expo-build-properties", { ios: { useFrameworks: "dynamic" } }]);
    process.env.CRASHLYTICS_ENABLED = "false";
    assert.deepEqual(configure({ config }).extra.crashlytics, { android: false, ios: false });
  } finally {
    for (const file of [android, ios]) {
      if (fs.existsSync(file)) {
        fs.unlinkSync(file);
      }
    }
    fs.rmdirSync(directory);
    delete process.env.CRASHLYTICS_ENABLED;
    delete process.env.FIREBASE_ANDROID_CONFIG;
    delete process.env.FIREBASE_IOS_CONFIG;
  }
});

test("Sentry config excludes upload tokens and missing credentials disable build uploads", () => {
  const originalExists = fs.existsSync;
  const originalRead = fs.readFileSync;
  const originalToken = process.env.SENTRY_AUTH_TOKEN;
  const sentryPath = path.resolve(__dirname, "../sentry.config.json");
  const exists = mock.method(fs, "existsSync", (file) => file === sentryPath || originalExists(file));
  const read = mock.method(fs, "readFileSync", (file, ...args) => {
    if (file === sentryPath) {
      return JSON.stringify({ SENTRY_ENABLED: true, SENTRY_DSN: "https://public@example.com/123", SENTRY_AUTH_TOKEN: "private" });
    }
    return originalRead(file, ...args);
  });
  try {
    delete process.env.SENTRY_AUTH_TOKEN;
    const result = configure({ config: {} });
    assert.equal(result.extra.sentry.SENTRY_ENABLED, true);
    assert.equal(result.extra.sentry.SENTRY_AUTH_TOKEN, undefined);
    const plugin = result.plugins.find((value) => Array.isArray(value) && value[0] === "@sentry/react-native/expo");
    assert.equal(plugin[1].disableAutoUpload, true);
    assert.ok(!JSON.stringify(result).includes("private"));
  } finally {
    exists.mock.restore();
    read.mock.restore();
    if (originalToken === undefined) {
      delete process.env.SENTRY_AUTH_TOKEN;
    } else {
      process.env.SENTRY_AUTH_TOKEN = originalToken;
    }
  }
});
