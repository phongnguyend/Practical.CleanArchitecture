import { api, download, PickedFile, upload } from "../../core/services";
import {
  configurationentriesPayload,
  ConfigurationEntry,
  parseConfigurationEntry,
} from "./model";
export const configurationentriesRepository = {
  list: async () =>
    ((await api("configurationentries")) as Record<string, unknown>[]).map(
      parseConfigurationEntry,
    ),
  get: async (id: string) =>
    parseConfigurationEntry(
      await api("configurationentries/" + encodeURIComponent(id)),
    ),
  remove: async (id: string) => {
    await api("configurationentries/" + encodeURIComponent(id), "DELETE");
  },
  save: async (value: ConfigurationEntry, file: PickedFile | null) => {
    await api(
      "configurationentries" +
        (value.id ? "/" + encodeURIComponent(value.id) : ""),
      value.id ? "PUT" : "POST",
      configurationentriesPayload(value),
    );
  },
};
export const importRecords = (file: PickedFile) =>
  upload("configurationentries/ImportExcel", file);
export const ExportAsExcel = () =>
  download("configurationentries/ExportAsExcel", "ConfigurationEntries.xlsx");
