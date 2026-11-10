import { Input } from "../../shared/ui/fields";
import { EditorProps } from "../../shared/ui/RecordPage";
import { Product } from "./model";
export function ProductForm({ value, change, busy }: EditorProps<Product>) {
  return (
    <>
      <Input
        label="Name"
        value={String(value.name)}
        onChange={(text) => change({ ...value, name: text })}
        disabled={busy}
      />
      <Input
        label="Code"
        value={String(value.code)}
        onChange={(text) => change({ ...value, code: text })}
        disabled={busy}
      />
      <Input
        label="Description"
        value={String(value.description)}
        onChange={(text) => change({ ...value, description: text })}
        disabled={busy}
        multiline
      />
    </>
  );
}
