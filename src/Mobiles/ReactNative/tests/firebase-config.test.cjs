const assert = require("node:assert/strict");
const fs = require("node:fs");
const os = require("node:os");
const path = require("node:path");
const { test } = require("node:test");
const configure = require("../app.config.js");

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
