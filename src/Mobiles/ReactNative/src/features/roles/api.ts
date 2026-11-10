import { api, PickedFile } from "../../core/services";
import { parseRole, Role, rolesPayload } from "./model";
export const rolesRepository = {
  list: async () =>
    ((await api("roles")) as Record<string, unknown>[]).map(parseRole),
  get: async (id: string) =>
    parseRole(await api("roles/" + encodeURIComponent(id))),
  remove: async (id: string) => {
    await api("roles/" + encodeURIComponent(id), "DELETE");
  },
  save: async (value: Role, file: PickedFile | null) => {
    await api(
      "roles" + (value.id ? "/" + encodeURIComponent(value.id) : ""),
      value.id ? "PUT" : "POST",
      rolesPayload(value),
    );
  },
};
