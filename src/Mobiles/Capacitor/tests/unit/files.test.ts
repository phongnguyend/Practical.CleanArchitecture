import { expect, it, vi } from "vitest";
vi.mock("@capacitor/core", () => ({
  Capacitor: { isNativePlatform: () => true },
}));
const write = vi.hoisted(() =>
  vi.fn(async () => ({ uri: "file:///cache/export.txt" })),
);
const share = vi.hoisted(() => vi.fn(async () => ({})));
vi.mock("@capacitor/filesystem", () => ({
  Directory: { Cache: "CACHE" },
  Filesystem: { writeFile: write },
}));
vi.mock("@capacitor/share", () => ({ Share: { share } }));
import {
  downloadOrShare,
  safeFileName,
} from "../../src/core/files/native-files";
it("uses only a safe basename for downloaded files", () => {
  expect(safeFileName("../../report.csv")).toBe("report.csv");
  expect(safeFileName("..\\report.csv")).toBe("report.csv");
});
it("writes exports to the cache and shares the native URI", async () => {
  await downloadOrShare(
    new Blob(["hello"], { type: "text/plain" }),
    "export.txt",
  );
  expect(write).toHaveBeenCalledWith(
    expect.objectContaining({ directory: "CACHE", data: "aGVsbG8=" }),
  );
  expect(share).toHaveBeenCalledWith(
    expect.objectContaining({ files: ["file:///cache/export.txt"] }),
  );
});
