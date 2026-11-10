import { api, download, PickedFile, upload } from "../../core/services";
import type { Row } from "../../shared/models/record";
import { filesPayload, parseStoredFile, StoredFile } from "./model";
export const filesRepository = {
  list: async () =>
    ((await api("files")) as Record<string, unknown>[]).map(parseStoredFile),
  get: async (id: string) =>
    parseStoredFile(await api("files/" + encodeURIComponent(id))),
  remove: async (id: string) => {
    await api("files/" + encodeURIComponent(id), "DELETE");
  },
  save: async (value: StoredFile, file: PickedFile | null) => {
    if (!value.id) {
      if (!file) throw new Error("Choose a file.");
      await upload("files", file, filesPayload(value));
      return;
    }
    await api(
      "files" + (value.id ? "/" + encodeURIComponent(value.id) : ""),
      value.id ? "PUT" : "POST",
      filesPayload(value),
    );
  },
  history: async (id: string): Promise<Row[]> =>
    await api("files/" + encodeURIComponent(id) + "/auditlogs"),
};
export const downloadFile = (value: StoredFile) =>
  download(
    "files/" + encodeURIComponent(value.id) + "/download",
    value.fileName || "download.bin",
  );
