import { Capacitor } from "@capacitor/core";
import { Filesystem, Directory } from "@capacitor/filesystem";
import { Share } from "@capacitor/share";
import { Clipboard } from "@capacitor/clipboard";
import { Browser } from "@capacitor/browser";

export function safeFileName(value: string): string {
  const result = value
    .split(/[\\/]/)
    .pop()!
    .replace(/[\\/<>:\"|?*\x00-\x1f]/g, "_")
    .replace(/^\.+/, "");
  return result || "download";
}
export async function downloadOrShare(blob: Blob, name: string): Promise<void> {
  const filename = safeFileName(name);
  if (Capacitor.isNativePlatform()) {
    const data = await new Promise<string>((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(String(reader.result).split(",")[1]);
      reader.onerror = () =>
        reject(new Error("Unable to read downloaded file."));
      reader.readAsDataURL(blob);
    });
    const result = await Filesystem.writeFile({
      path: `exports/${Date.now()}-${filename}`,
      data,
      directory: Directory.Cache,
      recursive: true,
    });
    await Share.share({
      title: filename,
      files: [result.uri],
      dialogTitle: "Save or share file",
    });
    return;
  }
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.append(link);
  link.click();
  link.remove();
  setTimeout(() => URL.revokeObjectURL(url), 1000);
}
export async function copyText(value: string): Promise<void> {
  if (Capacitor.isNativePlatform()) {
    await Clipboard.write({ string: value });
  } else {
    await navigator.clipboard.writeText(value);
  }
}
export async function openExternal(url: string): Promise<void> {
  if (Capacitor.isNativePlatform()) {
    await Browser.open({ url });
  } else {
    window.open(url, "_blank", "noopener,noreferrer");
  }
}
