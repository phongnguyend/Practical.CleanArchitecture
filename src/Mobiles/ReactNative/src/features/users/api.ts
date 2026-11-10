import { api, PickedFile } from "../../core/services";
import type { Row } from "../../shared/models/record";
import { parseUser, User, usersPayload } from "./model";
export const usersRepository = {
  list: async () =>
    ((await api("users")) as Record<string, unknown>[]).map(parseUser),
  get: async (id: string) =>
    parseUser(await api("users/" + encodeURIComponent(id))),
  remove: async (id: string) => {
    await api("users/" + encodeURIComponent(id), "DELETE");
  },
  save: async (value: User, file: PickedFile | null) => {
    await api(
      "users" + (value.id ? "/" + encodeURIComponent(value.id) : ""),
      value.id ? "PUT" : "POST",
      usersPayload(value),
    );
  },
  history: async (id: string): Promise<Row[]> =>
    await api("users/" + encodeURIComponent(id) + "/auditlogs"),
};
export const setPassword = (id: string, password: string) =>
  api("users/" + encodeURIComponent(id) + "/password", "PUT", { id, password });
export const sendResetEmail = (id: string) =>
  api("users/" + encodeURIComponent(id) + "/passwordresetemail", "POST", {
    id,
  });
export const sendConfirmationEmail = (id: string) =>
  api("users/" + encodeURIComponent(id) + "/emailaddressconfirmation", "POST", {
    id,
  });
