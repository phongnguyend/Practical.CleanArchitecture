const fs = require('node:fs');
const path = require('node:path');

// Temporary workaround: https://github.com/expo/expo/issues/52754
// Remove after upgrading to an ExpoModulesJSI release that fixes Swift 6.2 compilation.
const original = `    // The isolated closure below runs synchronously on this thread.
    nonisolated(unsafe) let slot = buffer.baseAddress!`;
const replacement = `    let slot = buffer.baseAddress!
    // The isolated closure runs synchronously within this buffer's lifetime.
    // Capture a Sendable address instead of the non-Sendable pointer (expo/expo#52754).
    let slotAddress = UInt(bitPattern: slot)`;
const originalEncode = `      try JavaScriptActor.assumeIsolated {
        try encode(JavaScriptValue.Slot(slot), runtime)`;
const replacementEncode = `      try JavaScriptActor.assumeIsolated {
        let slot = UnsafeMutablePointer<facebook.jsi.Value>(bitPattern: slotAddress)!
        try encode(JavaScriptValue.Slot(slot), runtime)`;

function patchSource(source, version) {
  if (version !== '58.0.22') {
    throw new Error(`Review expo/expo#52754 workaround for expo-modules-jsi ${version}; expected 58.0.22.`);
  }
  const normalized = source.replace(/\r\n/g, '\n');
  const count = fragment => normalized.split(fragment).length - 1;
  if (count(replacement) === 1 && count(replacementEncode) === 1 && count(original) === 0) {
    return source;
  }
  if (count(original) !== 1 || count(originalEncode) !== 1 || count(replacement) !== 0) {
    throw new Error('ExpoModulesJSI Swift source changed; review the workaround instead of applying it blindly.');
  }
  const patched = normalized.replace(original, replacement).replace(originalEncode, replacementEncode);
  return source.includes('\r\n') ? patched.replace(/\n/g, '\r\n') : patched;
}

if (require.main === module) {
  const packageFile = require.resolve('expo-modules-jsi/package.json');
  const { version } = JSON.parse(fs.readFileSync(packageFile, 'utf8'));
  const sourceFile = path.join(path.dirname(packageFile), 'apple/Sources/ExpoModulesJSI/Runtime/Values/JavaScriptObject.swift');
  const source = fs.readFileSync(sourceFile, 'utf8');
  const patched = patchSource(source, version);
  if (patched !== source) fs.writeFileSync(sourceFile, patched);
  console.log(`ExpoModulesJSI ${version}: Swift 6.2 compatibility patch verified.`);
}

module.exports = { patchSource };
