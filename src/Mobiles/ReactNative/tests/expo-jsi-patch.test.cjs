const assert = require('node:assert/strict');
const fs = require('node:fs');
const path = require('node:path');
const test = require('node:test');
const { patchSource } = require('../scripts/patch-expo-modules-jsi.cjs');

const packageRoot = path.dirname(require.resolve('expo-modules-jsi/package.json'));
const installed = fs.readFileSync(path.join(packageRoot, 'apple/Sources/ExpoModulesJSI/Runtime/Values/JavaScriptObject.swift'), 'utf8').replace(/\r\n/g, '\n');
// Recover the original source when npm's postinstall has already applied the patch.
const original = installed
  .replace(`    let slot = buffer.baseAddress!
    // The isolated closure runs synchronously within this buffer's lifetime.
    // Capture a Sendable address instead of the non-Sendable pointer (expo/expo#52754).
    let slotAddress = UInt(bitPattern: slot)`, `    // The isolated closure below runs synchronously on this thread.
    nonisolated(unsafe) let slot = buffer.baseAddress!`)
  .replace('        let slot = UnsafeMutablePointer<facebook.jsi.Value>(bitPattern: slotAddress)!\n', '');

test('Swift workaround changes the capture without changing buffer initialization or cleanup', () => {
  const patched = patchSource(original, '58.0.22');
  assert.notEqual(patched, original);
  assert.match(patched, /let slotAddress = UInt\(bitPattern: slot\)/);
  assert.match(patched, /assumeIsolated \{\n        let slot = UnsafeMutablePointer/);
  for (const statement of ['expo.emplaceUndefined(slot)', 'expo.destroyValues(slot, 1)', 'body(slot.pointee)']) {
    assert.ok(patched.includes(statement));
  }
  assert.equal(patchSource(patched, '58.0.22'), patched);
});

test('Swift workaround preserves CRLF sources', () => {
  const patched = patchSource(original.replace(/\n/g, '\r\n'), '58.0.22');
  assert.ok(patched.includes('\r\n'));
  assert.equal(patched.replace(/\r\n/g, '').includes('\n'), false);
});

test('Swift workaround rejects dependency upgrades and unexpected source changes', () => {
  assert.throws(() => patchSource(original, '58.0.23'), /Review/);
  assert.throws(() => patchSource(original.replace('nonisolated(unsafe) let slot', 'let slot'), '58.0.22'), /source changed/);
  assert.throws(() => patchSource(original + original, '58.0.22'), /source changed/);
});
