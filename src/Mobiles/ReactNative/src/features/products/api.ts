import { api, download, PickedFile, upload } from "../../core/services";
import type { Row } from "../../shared/models/record";
import { parseProduct, Product, productsPayload } from "./model";
export const productsRepository = {
  list: async () =>
    ((await api("products")) as Record<string, unknown>[]).map(parseProduct),
  get: async (id: string) =>
    parseProduct(await api("products/" + encodeURIComponent(id))),
  remove: async (id: string) => {
    await api("products/" + encodeURIComponent(id), "DELETE");
  },
  save: async (value: Product, file: PickedFile | null) => {
    await api(
      "products" + (value.id ? "/" + encodeURIComponent(value.id) : ""),
      value.id ? "PUT" : "POST",
      productsPayload(value),
    );
  },
  history: async (id: string): Promise<Row[]> =>
    await api("products/" + encodeURIComponent(id) + "/auditlogs"),
};
export const importRecords = (file: PickedFile) =>
  upload("products/importcsv", file);
export const exportascsv = () =>
  download("products/exportascsv", "Products.csv");
export const exportaspdf = () =>
  download("products/exportaspdf", "Products.pdf");
