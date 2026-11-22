import { expect, it, vi } from "vitest";
const record = vi.hoisted(() => vi.fn());
const available = vi.hoisted(() => ({ value: false }));
vi.mock("@capacitor/core", () => ({
  Capacitor: {
    isNativePlatform: () => true,
    isPluginAvailable: () => available.value,
  },
}));
vi.mock("@capacitor-firebase/crashlytics", () => ({
  FirebaseCrashlytics: { recordException: record },
}));
import { reportFailure } from "../../src/core/crashlytics";
it("does not initialize Firebase when its configuration is missing", async () => {
  await reportFailure("javascript");
  expect(record).not.toHaveBeenCalled();
});
it("reports a fixed error category without application data", async () => {
  available.value = true;
  await reportFailure("react");
  expect(record).toHaveBeenCalledWith({ message: "Unhandled react failure" });
});

it("preserves bundle positions without leaking exception messages or origins", async () => {
  available.value = true;
  const error = new Error("sensitive user input");
  error.stack =
    "Error: sensitive user input\n at fn (https://localhost/assets/index-abc.js:42:9)\n at https://private.example/customer?id=secret";
  await reportFailure("javascript", error);
  expect(record).toHaveBeenLastCalledWith({
    message: "Unhandled javascript failure",
    stacktrace: [{ fileName: "assets/index-abc.js", lineNumber: 42 }],
  });
});
