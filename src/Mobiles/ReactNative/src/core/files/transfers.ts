import * as DocumentPicker from "expo-document-picker";
import { Directory, File, Paths } from "expo-file-system";
import * as Sharing from "expo-sharing";
import type { Row } from "../../shared/models/record";
import { api, response } from "../network/api";

export async function pickFile() {
  const result = await DocumentPicker.getDocumentAsync({
    copyToCacheDirectory: true,
  });
  return result.canceled ? null : result.assets[0];
}
export type PickedFile = NonNullable<Awaited<ReturnType<typeof pickFile>>>;
export async function upload(path: string, file: PickedFile, values: Row = {}) {
  const form = new FormData();
  form.append("formFile", {
    uri: file.uri,
    name: file.name,
    type: file.mimeType || "application/octet-stream",
  } as any);
  for (const [key, value] of Object.entries(values))
    if (value != null) form.append(key, String(value));
  return api(path, "POST", form);
}
export async function download(path: string, name: string) {
  const bytes = new Uint8Array(await (await response(path)).arrayBuffer());
  const folder = new Directory(Paths.cache, "exports");
  folder.create({ idempotent: true });
  for (const item of folder.list())
    if (
      item instanceof File &&
      item.modificationTime != null &&
      item.modificationTime < Date.now() - 86400000
    )
      item.delete();
  const file = new File(folder, name.replace(/[^a-zA-Z0-9._-]/g, "_"));
  file.write(bytes);
  if (await Sharing.isAvailableAsync()) await Sharing.shareAsync(file.uri);
  else throw new Error("Sharing is unavailable on this device.");
}
