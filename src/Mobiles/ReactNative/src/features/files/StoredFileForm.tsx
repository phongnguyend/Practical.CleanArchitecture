import { useState } from "react";
import { Text } from "react-native";
import { pickFile } from "../../core/services";
import { Input, Toggle } from "../../shared/ui/fields";
import { EditorProps } from "../../shared/ui/RecordPage";
import { Button } from "../../shared/ui/ui";
import { StoredFile } from "./model";
export function StoredFileForm({
  value,
  change,
  busy,
  file,
  pick,
}: EditorProps<StoredFile>) {
  const [error, setError] = useState("");
  return (
    <>
      <Input
        label="Name"
        value={String(value.name)}
        onChange={(text) => change({ ...value, name: text })}
        disabled={busy}
      />
      <Input
        label="Description"
        value={String(value.description)}
        onChange={(text) => change({ ...value, description: text })}
        disabled={busy}
        multiline
      />
      <Toggle
        label="Encrypted"
        value={value.encrypted}
        onChange={(text) => change({ ...value, encrypted: text })}
        disabled={busy || !!value.id}
      />
      {!!value.id && (
        <Input
          label="File name"
          value={String(value.fileName)}
          onChange={(text) => change({ ...value, fileName: text })}
          disabled={busy || true}
        />
      )}
      {!value.id && (
        <>
          <Button
            title="Choose file"
            disabled={busy}
            onPress={() =>
              void pickFile()
                .then(pick)
                .catch((e) => setError(String(e)))
            }
          />
          <Text>{file?.name || "No file selected"}</Text>
        </>
      )}
      {!!error && <Text accessibilityRole="alert">{error}</Text>}
    </>
  );
}
