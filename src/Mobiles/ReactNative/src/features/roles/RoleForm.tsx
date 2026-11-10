import { Input } from "../../shared/ui/fields";
import { EditorProps } from "../../shared/ui/RecordPage";
import { Role } from "./model";
export function RoleForm({ value, change, busy }: EditorProps<Role>) {
  return (
    <>
      <Input
        label="Name"
        value={String(value.name)}
        onChange={(text) => change({ ...value, name: text })}
        disabled={busy}
      />
    </>
  );
}
