import { Input, Toggle } from "../../shared/ui/fields";
import { EditorProps } from "../../shared/ui/RecordPage";
import { ConfigurationEntry } from "./model";
export function ConfigurationEntryForm({
  value,
  change,
  busy,
}: EditorProps<ConfigurationEntry>) {
  return (
    <>
      <Input
        label="Key"
        value={String(value.key)}
        onChange={(text) => change({ ...value, key: text })}
        disabled={busy}
      />
      <Input
        label="Value"
        value={String(value.value)}
        onChange={(text) => change({ ...value, value: text })}
        disabled={busy}
        secret={value.isSensitive}
      />
      <Input
        label="Description"
        value={String(value.description)}
        onChange={(text) => change({ ...value, description: text })}
        disabled={busy}
        multiline
      />
      <Toggle
        label="Sensitive"
        value={value.isSensitive}
        onChange={(text) => change({ ...value, isSensitive: text })}
        disabled={busy}
      />
    </>
  );
}
