import { useState } from "react";
import { Text } from "react-native";
import { PickedFile, pickFile } from "../../core/services";
import { Button, Sheet, useStyles } from "./ui";
export function ImportButton({
  suffix,
  submit,
  done,
}: {
  suffix: string;
  submit: (file: PickedFile) => Promise<void>;
  done: () => Promise<void>;
}) {
  const s = useStyles();
  const [open, setOpen] = useState(false),
    [file, setFile] = useState<PickedFile | null>(null),
    [busy, setBusy] = useState(false),
    [error, setError] = useState("");
  return (
    <>
      <Button
        title="Import"
        onPress={() => {
          setFile(null);
          setError("");
          setOpen(true);
        }}
      />
      {open && (
        <Sheet
          title={`Import ${suffix}`}
          close={() => {
            if (!busy) setOpen(false);
          }}
          footer={
            <>
              <Button
                title="Cancel"
                disabled={busy}
                onPress={() => setOpen(false)}
              />
              <Button
                title="Import"
                disabled={busy}
                onPress={() => {
                  if (!file || !file.name.toLowerCase().endsWith(suffix)) {
                    setError(`Choose a ${suffix} file.`);
                    return;
                  }
                  setBusy(true);
                  void submit(file)
                    .then(async () => {
                      await done();
                      setOpen(false);
                    })
                    .catch((e) => setError(String(e)))
                    .finally(() => setBusy(false));
                }}
              />
            </>
          }
        >
          <Button
            title="Choose file"
            disabled={busy}
            onPress={() =>
              void pickFile()
                .then(setFile)
                .catch((e) => setError(String(e)))
            }
          />
          <Text style={s.text}>{file?.name || "No file selected"}</Text>
          {!!error && <Text style={s.error}>{error}</Text>}
        </Sheet>
      )}
    </>
  );
}
